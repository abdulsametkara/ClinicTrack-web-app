import { useNavigate } from 'react-router-dom';
import { LogOut, User, Calendar, Clock, CheckCircle, Users, Menu, Activity, Search, X, MessageSquare, ChevronRight, Edit2, Save, Phone, UserCircle } from 'lucide-react';
import { useState, useEffect } from 'react';
import Swal from 'sweetalert2';
import { getProfile, getDoktorAppointments, getAllDoktorlar, updateRandevuDurum, addDoktorNote, updateUserPhone } from '../api';

function DoktorDashboard() {
  const navigate = useNavigate();
  const [kullaniciAd, setKullaniciAd] = useState(localStorage.getItem('kullaniciAd'));
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [activeTab, setActiveTab] = useState('Dashboard');
  const [loading, setLoading] = useState(true);

  // Data States
  const [appointments, setAppointments] = useState([]);
  const [patients, setPatients] = useState([]);
  const [stats, setStats] = useState({
    today: 0,
    pending: 0,
    completed: 0,
    totalPatients: 0
  });
  const [doktorId, setDoktorId] = useState(null);
  const [userId, setUserId] = useState(null);
  const [userProfile, setUserProfile] = useState(null);

  // Search & Filter States
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState('Tümü');
  const [patientSearchTerm, setPatientSearchTerm] = useState('');

  // Note Modal States
  const [noteModalOpen, setNoteModalOpen] = useState(false);
  const [selectedAppointment, setSelectedAppointment] = useState(null);
  const [noteText, setNoteText] = useState('');

  // Profile Edit State
  const [isEditingProfile, setIsEditingProfile] = useState(false);
  const [editPhone, setEditPhone] = useState('');

  // Fetch All Data
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        // 1. Get Profile to find current user ID
        const profileRes = await getProfile();
        if (!profileRes.isSuccess) throw new Error('Profil alınamadı');
        
        const data = profileRes.data;
        const currentUserId = data.id || data.Id;
        setUserId(currentUserId);
        setUserProfile(data);
        
        // Handle casing differences (isim vs İsim)
        const isim = data.isim || data.İsim || '';
        const soyisim = data.soyisim || data.Soyisim || '';
        setKullaniciAd(`${isim} ${soyisim}`.trim());
        
        setEditPhone(data.telefonNumarası || data.TelefonNumarası || '');

        // 2. Get All Doctors to find my Doktor ID
        const doctorsRes = await getAllDoktorlar();
        if (!doctorsRes.isSuccess) throw new Error('Doktor listesi alınamadı');
        
        const myDoctorProfile = doctorsRes.data.find(d => {
          const doktorKullaniciId = d.kullanıcıId || d.KullanıcıId || d.kullaniciId || d.KullaniciId;
          return doktorKullaniciId === currentUserId;
        });
        
        if (!myDoctorProfile) {
          setLoading(false);
          return;
        }

        const myDoktorId = myDoctorProfile.id || myDoctorProfile.Id;
        setDoktorId(myDoktorId);

        // 3. Get My Appointments (directly by Doktor ID)
        const appointmentsRes = await getDoktorAppointments(myDoktorId);
        if (appointmentsRes.isSuccess) {
          const myAppointments = appointmentsRes.data
            .sort((a, b) => new Date(b.randevuTarihi || b.RandevuTarihi) - new Date(a.randevuTarihi || a.RandevuTarihi));

          const enrichedAppointments = myAppointments.map(app => ({
            ...app,
            patientName: (app.hastaAdi || app.HastaAdi) 
              ? `${app.hastaAdi || app.HastaAdi} ${app.hastaSoyadi || app.HastaSoyadi}`
              : 'Bilinmeyen Hasta',
            patientPhone: app.hastaTelefon || app.HastaTelefon || '-'
          }));

          const patientsMap = new Map();
          enrichedAppointments.forEach(app => {
             if(app.hastaId && (app.hastaAdi || app.HastaAdi) && !patientsMap.has(app.hastaId)) {
                 patientsMap.set(app.hastaId, {
                     id: app.hastaId,
                     isim: app.hastaAdi || app.HastaAdi,
                     soyisim: app.hastaSoyadi || app.HastaSoyadi,
                     tcNo: app.hastaTCNo || app.HastaTCNo,
                     email: app.hastaEmail || app.HastaEmail,
                     telefonNumarası: app.hastaTelefon || app.HastaTelefon,
                     oluşturulmaTarihi: app.recordDate || app.RecordDate
                 });
             }
          });
          const patientsList = Array.from(patientsMap.values());

          setPatients(patientsList);
          setAppointments(enrichedAppointments);

          const today = new Date().toDateString();
          setStats({
            totalPatients: patientsList.length,
            today: enrichedAppointments.filter(a => new Date(a.randevuTarihi || a.RandevuTarihi).toDateString() === today).length,
            pending: enrichedAppointments.filter(a => (a.durum || a.Durum) === 'Beklemede').length,
            completed: enrichedAppointments.filter(a => (a.durum || a.Durum) === 'Tamamlandı').length
          });
        }

      } catch (error) {
        console.error('Veri yükleme hatası:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const handleStatusUpdate = async (id, newStatus) => {
    try {
      const response = await updateRandevuDurum(id, newStatus);
      if (response.isSuccess) {
        setAppointments(prev => prev.map(app => 
          (app.id || app.Id) === id ? { ...app, durum: newStatus, Durum: newStatus } : app
        ));
        
        // Refresh stats logic
        const today = new Date().toDateString();
        const updatedApps = appointments.map(app => (app.id || app.Id) === id ? { ...app, durum: newStatus } : app);
        
        setStats({
            totalPatients: patients.length,
            today: updatedApps.filter(a => new Date(a.randevuTarihi || a.RandevuTarihi).toDateString() === today).length,
            pending: updatedApps.filter(a => (a.durum || a.Durum) === 'Beklemede').length,
            completed: updatedApps.filter(a => (a.durum || a.Durum) === 'Tamamlandı').length
        });

        Swal.fire({
            icon: 'success',
            title: 'Durum Güncellendi',
            text: `Randevu durumu ${newStatus} olarak değiştirildi.`,
            timer: 1500,
            showConfirmButton: false
        });
      } else {
        Swal.fire('Hata', 'Durum güncellenemedi: ' + response.message, 'error');
      }
    } catch (error) {
      console.error('Status update error:', error);
      Swal.fire('Hata', 'Bir hata oluştu.', 'error');
    }
  };

  const openNoteModal = (appointment) => {
    setSelectedAppointment(appointment);
    setNoteText(appointment.doktorNotları || appointment.DoktorNotları || '');
    setNoteModalOpen(true);
  };

  const handleSaveNote = async () => {
    if (!selectedAppointment || !noteText.trim()) {
      Swal.fire('Uyarı', 'Lütfen bir not girin.', 'warning');
      return;
    }

    try {
      const aptId = selectedAppointment.id || selectedAppointment.Id;
      const response = await addDoktorNote(aptId, noteText);
      
      if (response.isSuccess) {
        setAppointments(prev => prev.map(app => 
          (app.id || app.Id) === aptId 
            ? { ...app, doktorNotları: noteText, DoktorNotları: noteText } 
            : app
        ));
        setNoteModalOpen(false);
        setNoteText('');
        Swal.fire('Başarılı', 'Not başarıyla eklendi!', 'success');
      } else {
        Swal.fire('Hata', 'Not eklenemedi: ' + response.message, 'error');
      }
    } catch (error) {
      console.error('Note save error:', error);
      Swal.fire('Hata', 'Bir hata oluştu.', 'error');
    }
  };

  const handleUpdateProfile = async () => {
      try {
          if (!userId) return;
          const response = await updateUserPhone(userId, editPhone);
          if (response.isSuccess) {
              Swal.fire('Başarılı', 'Telefon numarası güncellendi.', 'success');
              setUserProfile(prev => ({ ...prev, telefonNumarası: editPhone, TelefonNumarası: editPhone }));
              setIsEditingProfile(false);
          } else {
              Swal.fire('Hata', response.message, 'error');
          }
      } catch (error) {
          console.error(error);
          Swal.fire('Hata', 'Profil güncellenirken bir hata oluştu.', 'error');
      }
  };

  const filteredAppointments = appointments.filter(apt => {
    const matchesSearch = apt.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         (apt.sikayet || apt.Sikayet || '').toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = filterStatus === 'Tümü' || (apt.durum || apt.Durum) === filterStatus;
    return matchesSearch && matchesStatus;
  });

  const filteredPatients = patients.filter(p => {
    const fullName = `${p.isim || p.İsim} ${p.soyisim || p.Soyisim}`.toLowerCase();
    const phone = (p.telefonNumarası || p.TelefonNumarası || '').toLowerCase();
    return fullName.includes(patientSearchTerm.toLowerCase()) || 
           phone.includes(patientSearchTerm.toLowerCase());
  });

  const currentDate = new Date().toLocaleDateString('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    weekday: 'long'
  });

  return (
    <div className="min-h-screen bg-slate-100 flex font-sans text-slate-800">
      {/* SIDEBAR */}
      <aside className={`bg-white w-72 flex-shrink-0 fixed h-full z-30 transition-transform duration-300 border-r border-slate-200 shadow-lg ${isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}`}>
        <div className="p-8 flex items-center gap-3 mb-6">
          <div className="bg-gradient-to-tr from-emerald-600 to-teal-600 p-2.5 rounded-xl shadow-lg shadow-emerald-500/30">
            <Activity className="w-6 h-6 text-white" />
          </div>
          <span className="text-2xl font-bold tracking-tight text-slate-800">ClinicTrack</span>
        </div>

        <nav className="px-4 space-y-2">
          <p className="px-4 text-xs font-bold text-slate-400 uppercase tracking-wider mb-4">Doktor Paneli</p>
          
          <button 
            onClick={() => setActiveTab('Dashboard')}
            className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${activeTab === 'Dashboard' ? 'bg-emerald-50 text-emerald-700 shadow-sm border border-emerald-100' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'}`}
          >
            <Activity className="w-5 h-5" />
            <span>Genel Bakış</span>
          </button>
          
          <button 
            onClick={() => setActiveTab('Randevular')}
            className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${activeTab === 'Randevular' ? 'bg-emerald-50 text-emerald-700 shadow-sm border border-emerald-100' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'}`}
          >
            <Calendar className="w-5 h-5" />
            <span>Randevularım</span>
          </button>
          
          <button 
            onClick={() => setActiveTab('Hastalar')}
            className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${activeTab === 'Hastalar' ? 'bg-emerald-50 text-emerald-700 shadow-sm border border-emerald-100' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'}`}
          >
            <Users className="w-5 h-5" />
            <span>Hastalarım</span>
          </button>

          <button 
            onClick={() => setActiveTab('Profil')}
            className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${activeTab === 'Profil' ? 'bg-emerald-50 text-emerald-700 shadow-sm border border-emerald-100' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'}`}
          >
            <User className="w-5 h-5" />
            <span>Profilim</span>
          </button>
        </nav>

        <div className="absolute bottom-0 w-full p-6 border-t border-slate-100">
          <button 
            onClick={handleLogout}
            className="flex items-center gap-3 text-slate-500 hover:text-red-600 transition-colors w-full px-2 font-medium text-sm"
          >
            <LogOut className="w-4 h-4" />
            Çıkış Yap
          </button>
        </div>
      </aside>

      {/* MAIN CONTENT */}
      <main className="flex-1 md:ml-72 min-h-screen flex flex-col">
        {/* HEADER */}
        <header className="md:hidden bg-white shadow-sm h-16 flex items-center justify-between px-4 sticky top-0 z-20">
            <button onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)} className="p-2">
              <Menu className="w-6 h-6 text-slate-600" />
            </button>
            <span className="font-bold text-lg">ClinicTrack</span>
            <div className="w-8"></div>
        </header>

        <div className="p-6 md:p-10 max-w-7xl mx-auto w-full">
          {loading ? (
            <div className="flex items-center justify-center h-64 text-slate-500">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-emerald-600 mr-3"></div>
              Veriler yükleniyor...
            </div>
          ) : (
            <>
              {/* DASHBOARD TAB */}
              {activeTab === 'Dashboard' && (
                <div className="space-y-8 animate-fade-in">
                  {/* Hero Section */}
                  <div className="bg-gradient-to-r from-emerald-600 to-teal-600 rounded-3xl p-8 md:p-12 text-white shadow-xl shadow-emerald-900/20 relative overflow-hidden">
                    <div className="absolute top-0 right-0 w-64 h-64 bg-white opacity-5 rounded-full -translate-y-1/2 translate-x-1/3 blur-3xl"></div>
                    <div className="absolute bottom-0 left-0 w-48 h-48 bg-teal-400 opacity-10 rounded-full translate-y-1/3 -translate-x-1/4 blur-2xl"></div>
                    
                    <div className="relative z-10">
                        <p className="text-emerald-100 font-medium mb-2 flex items-center gap-2">
                            <Calendar className="w-4 h-4" /> {currentDate}
                        </p>
                        <h1 className="text-3xl md:text-4xl font-bold mb-2">Hoş Geldiniz, Dr. {kullaniciAd?.split(' ')[0]}</h1>
                        <p className="text-emerald-100 max-w-md">Bugünkü randevularınızı ve hasta durumlarını buradan takip edebilirsiniz.</p>
                    </div>
                  </div>

                  {/* Stats */}
                  <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                    {[
                        { label: 'Bugünkü Randevular', value: stats.today, icon: Calendar, color: 'blue' },
                        { label: 'Bekleyen', value: stats.pending, icon: Clock, color: 'orange' },
                        { label: 'Tamamlanan', value: stats.completed, icon: CheckCircle, color: 'green' },
                        { label: 'Toplam Hasta', value: stats.totalPatients, icon: Users, color: 'purple' }
                    ].map((stat, idx) => (
                        <div key={idx} className="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center justify-between hover:shadow-md transition-all hover:-translate-y-1">
                            <div>
                                <p className="text-slate-500 text-sm font-medium mb-1">{stat.label}</p>
                                <h3 className="text-3xl font-bold text-slate-800">{stat.value}</h3>
                            </div>
                            <div className={`p-4 bg-${stat.color}-50 rounded-xl text-${stat.color}-600`}>
                                <stat.icon className="w-8 h-8" />
                            </div>
                        </div>
                    ))}
                  </div>

                  {/* Today's Schedule Preview */}
                  <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
                    <div className="p-6 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
                      <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                        <Clock className="w-5 h-5 text-emerald-600" />
                        Son Randevular
                      </h3>
                      <button onClick={() => setActiveTab('Randevular')} className="text-emerald-600 text-sm font-bold hover:underline flex items-center gap-1">
                        Tümünü Gör <ChevronRight className="w-4 h-4" />
                      </button>
                    </div>
                    <div className="divide-y divide-slate-100">
                      {appointments.slice(0, 5).map((apt) => (
                        <div key={apt.id || apt.Id} className="p-5 hover:bg-slate-50 transition flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                          <div className="flex items-center gap-4">
                            <div className="bg-emerald-50 text-emerald-700 px-4 py-2 rounded-xl text-center min-w-[80px] border border-emerald-100">
                              <div className="text-xs font-bold uppercase tracking-wider opacity-70">
                                {new Date(apt.randevuTarihi || apt.RandevuTarihi).toLocaleDateString('tr-TR', { month: 'short' })}
                              </div>
                              <div className="font-bold text-xl">
                                {new Date(apt.randevuTarihi || apt.RandevuTarihi).getDate()}
                              </div>
                            </div>
                            <div>
                              <h4 className="font-bold text-slate-800 text-lg">{apt.patientName}</h4>
                              <div className="flex items-center gap-2 text-sm text-slate-500 mt-1">
                                <Clock className="w-3 h-3" />
                                {new Date(apt.randevuTarihi || apt.RandevuTarihi).toLocaleTimeString('tr-TR', {hour: '2-digit', minute:'2-digit'})}
                              </div>
                            </div>
                          </div>
                          <span className={`px-4 py-1.5 rounded-full text-sm font-bold ${
                            (apt.durum || apt.Durum) === 'Beklemede' ? 'bg-orange-100 text-orange-700' :
                            (apt.durum || apt.Durum) === 'Tamamlandı' ? 'bg-emerald-100 text-emerald-700' :
                            'bg-red-100 text-red-700'
                          }`}>
                            {apt.durum || apt.Durum}
                          </span>
                        </div>
                      ))}
                      {appointments.length === 0 && (
                        <div className="p-10 text-center text-slate-400 flex flex-col items-center">
                          <Calendar className="w-12 h-12 mb-3 opacity-20" />
                          <p>Henüz randevu bulunmuyor.</p>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              )}

              {/* RANDEVULAR TAB */}
              {activeTab === 'Randevular' && (
                <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden animate-fade-in">
                  <div className="p-6 border-b border-slate-100 bg-slate-50/50">
                    <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                      <h3 className="font-bold text-xl text-slate-800">Tüm Randevularım</h3>
                      
                      <div className="flex flex-col sm:flex-row gap-3">
                        <div className="relative">
                          <Search className="w-5 h-5 absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400" />
                          <input
                            type="text"
                            placeholder="Hasta ara..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="pl-10 pr-4 py-2.5 border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-emerald-500 outline-none w-full"
                          />
                        </div>
                        
                        <select
                          value={filterStatus}
                          onChange={(e) => setFilterStatus(e.target.value)}
                          className="px-4 py-2.5 border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-emerald-500 outline-none bg-white cursor-pointer"
                        >
                          <option value="Tümü">Tüm Durumlar</option>
                          <option value="Beklemede">Beklemede</option>
                          <option value="Tamamlandı">Tamamlandı</option>
                          <option value="İptal">İptal</option>
                        </select>
                      </div>
                    </div>
                  </div>
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                        <tr>
                          <th className="px-6 py-4 text-left">Tarih & Saat</th>
                          <th className="px-6 py-4 text-left">Hasta Adı</th>
                          <th className="px-6 py-4 text-left">İletişim</th>
                          <th className="px-6 py-4 text-left">Şikayet</th>
                          <th className="px-6 py-4 text-left">Durum</th>
                          <th className="px-6 py-4 text-left">İşlemler</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-slate-100">
                        {filteredAppointments.map((apt) => (
                          <tr key={apt.id || apt.Id} className="hover:bg-slate-50 transition">
                            <td className="px-6 py-4">
                              <div className="font-bold text-slate-700">
                                {new Date(apt.randevuTarihi || apt.RandevuTarihi).toLocaleDateString('tr-TR')}
                              </div>
                              <div className="text-sm text-slate-500 flex items-center gap-1 mt-1">
                                <Clock className="w-3 h-3" />
                                {new Date(apt.randevuTarihi || apt.RandevuTarihi).toLocaleTimeString('tr-TR', {hour: '2-digit', minute:'2-digit'})}
                              </div>
                            </td>
                            <td className="px-6 py-4">
                                <div className="flex items-center gap-3">
                                    <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center font-bold text-sm">
                                        {apt.patientName.charAt(0)}
                                    </div>
                                    <span className="font-medium text-slate-800">{apt.patientName}</span>
                                </div>
                            </td>
                            <td className="px-6 py-4 text-slate-600 text-sm font-medium">
                              {apt.patientPhone}
                            </td>
                            <td className="px-6 py-4 text-slate-600 text-sm max-w-xs truncate">
                              {apt.sikayet || apt.Sikayet || '-'}
                            </td>
                            <td className="px-6 py-4">
                              <select
                                value={apt.durum || apt.Durum}
                                onChange={(e) => handleStatusUpdate(apt.id || apt.Id, e.target.value)}
                                className={`px-3 py-1.5 rounded-lg text-sm font-bold border outline-none cursor-pointer transition ${
                                  (apt.durum || apt.Durum) === 'Beklemede' ? 'bg-orange-50 text-orange-700 border-orange-200 hover:bg-orange-100' :
                                  (apt.durum || apt.Durum) === 'Tamamlandı' ? 'bg-emerald-50 text-emerald-700 border-emerald-200 hover:bg-emerald-100' :
                                  'bg-red-50 text-red-700 border-red-200 hover:bg-red-100'
                                }`}
                              >
                                <option value="Beklemede">Beklemede</option>
                                <option value="Tamamlandı">Tamamlandı</option>
                                <option value="İptal">İptal</option>
                              </select>
                            </td>
                            <td className="px-6 py-4">
                              <button
                                onClick={() => openNoteModal(apt)}
                                className={`flex items-center gap-2 px-4 py-2 rounded-xl transition text-sm font-bold shadow-sm ${
                                    apt.doktorNotları || apt.DoktorNotları 
                                    ? 'bg-indigo-50 text-indigo-600 hover:bg-indigo-100' 
                                    : 'bg-white border border-slate-200 text-slate-600 hover:bg-slate-50'
                                }`}
                              >
                                <MessageSquare className="w-4 h-4" />
                                {apt.doktorNotları || apt.DoktorNotları ? 'Notu Düzenle' : 'Not Ekle'}
                              </button>
                            </td>
                          </tr>
                        ))}
                        {filteredAppointments.length === 0 && (
                          <tr>
                            <td colSpan="6" className="p-12 text-center text-slate-400">
                              <Search className="w-12 h-12 mx-auto mb-3 opacity-20" />
                              <p>{searchTerm || filterStatus !== 'Tümü' ? 'Arama kriterlerine uygun randevu bulunamadı.' : 'Henüz randevunuz bulunmuyor.'}</p>
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {/* HASTALAR TAB */}
              {activeTab === 'Hastalar' && (
                <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden animate-fade-in">
                  <div className="p-6 border-b border-slate-100 bg-slate-50/50">
                    <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                      <h3 className="font-bold text-xl text-slate-800">Hasta Listem</h3>
                      
                      <div className="relative">
                        <Search className="w-5 h-5 absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400" />
                        <input
                          type="text"
                          placeholder="Hasta ara (isim veya telefon)..."
                          value={patientSearchTerm}
                          onChange={(e) => setPatientSearchTerm(e.target.value)}
                          className="pl-10 pr-4 py-2.5 border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-emerald-500 outline-none w-full md:w-80"
                        />
                      </div>
                    </div>
                  </div>
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                        <tr>
                          <th className="px-6 py-4 text-left">Hasta</th>
                          <th className="px-6 py-4 text-left">İletişim</th>
                          <th className="px-6 py-4 text-left">Kayıt Tarihi</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-slate-100">
                        {filteredPatients.map((p) => (
                          <tr key={p.id || p.Id} className="hover:bg-slate-50 transition">
                            <td className="px-6 py-4">
                              <div className="flex items-center gap-4">
                                <div className="w-12 h-12 rounded-2xl bg-gradient-to-br from-blue-500 to-blue-600 text-white flex items-center justify-center font-bold text-lg shadow-md shadow-blue-200">
                                  {(p.isim || p.İsim)?.charAt(0)}
                                </div>
                                <div>
                                  <div className="font-bold text-slate-800 text-base">
                                    {(p.isim || p.İsim)} {(p.soyisim || p.Soyisim)}
                                  </div>
                                  <div className="text-xs text-slate-500 font-medium bg-slate-100 px-2 py-0.5 rounded inline-block mt-1">
                                    TC: {p.tcNo || p.TCNo}
                                  </div>
                                </div>
                              </div>
                            </td>
                            <td className="px-6 py-4">
                              <div className="text-slate-700 font-medium mb-1">{p.email || p.Email}</div>
                              <div className="text-sm text-slate-500 flex items-center gap-1">
                                <Phone className="w-3 h-3" />
                                {p.telefonNumarası || p.TelefonNumarası || '-'}
                              </div>
                            </td>
                            <td className="px-6 py-4 text-slate-500 font-medium">
                              {new Date(p.oluşturulmaTarihi || p.OluşturulmaTarihi || new Date()).toLocaleDateString('tr-TR')}
                            </td>
                          </tr>
                        ))}
                        {filteredPatients.length === 0 && (
                          <tr>
                            <td colSpan="3" className="p-12 text-center text-slate-400">
                                <Users className="w-12 h-12 mx-auto mb-3 opacity-20" />
                                <p>{patientSearchTerm ? 'Arama kriterlerine uygun hasta bulunamadı.' : 'Henüz hasta kaydınız bulunmuyor.'}</p>
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {/* PROFIL TAB */}
              {activeTab === 'Profil' && (
                <div className="animate-fade-in space-y-8">
                    <div className="bg-gradient-to-r from-emerald-600 to-teal-600 rounded-3xl p-10 text-white shadow-xl shadow-emerald-900/20 flex flex-col md:flex-row items-center justify-between gap-6">
                        <div className="flex flex-col md:flex-row items-center gap-8">
                            <div className="w-32 h-32 bg-white/10 backdrop-blur-md rounded-full flex items-center justify-center border-4 border-white/20 text-4xl font-bold shadow-inner">
                                {kullaniciAd?.charAt(0).toUpperCase()}
                            </div>
                            <div className="text-center md:text-left">
                                <h2 className="text-3xl font-bold mb-2">{kullaniciAd}</h2>
                                <div className="flex flex-wrap justify-center md:justify-start gap-4 text-sm text-emerald-100">
                                    {(userProfile?.email || userProfile?.Email) && (
                                        <div className="flex items-center gap-2">
                                            <div className="w-8 h-8 rounded-full bg-white/10 flex items-center justify-center">@</div>
                                            {userProfile.email || userProfile.Email}
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                        <div>
                            {!isEditingProfile ? (
                                <button 
                                    onClick={() => setIsEditingProfile(true)}
                                    className="bg-white/10 hover:bg-white/20 text-white px-6 py-3 rounded-xl backdrop-blur-md border border-white/20 transition font-bold flex items-center gap-2"
                                >
                                    <Edit2 className="w-4 h-4" />
                                    Düzenle
                                </button>
                            ) : (
                                <div className="flex gap-3">
                                    <button 
                                        onClick={() => setIsEditingProfile(false)}
                                        className="bg-white/10 hover:bg-white/20 text-white px-6 py-3 rounded-xl backdrop-blur-md border border-white/20 transition font-bold"
                                    >
                                        İptal
                                    </button>
                                    <button 
                                        onClick={handleUpdateProfile}
                                        className="bg-white text-emerald-600 px-6 py-3 rounded-xl font-bold shadow-lg hover:bg-emerald-50 transition flex items-center gap-2"
                                    >
                                        <Save className="w-4 h-4" />
                                        Kaydet
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>

                    <div className="bg-white rounded-3xl shadow-md border border-slate-200 p-8">
                        <h3 className="text-xl font-bold text-slate-800 mb-6 flex items-center gap-3">
                            <UserCircle className="w-6 h-6 text-emerald-600" />
                            Kişisel Bilgiler
                        </h3>
                        <div className="space-y-4 max-w-2xl">
                            <div className="flex justify-between items-center py-4 border-b border-slate-100">
                                <span className="text-slate-500 font-medium">TC Kimlik No</span>
                                <span className="text-slate-800 font-bold text-lg">{userProfile?.tcNo || userProfile?.TCNo || '-'}</span>
                            </div>
                            <div className="flex justify-between items-center py-4 border-b border-slate-100">
                                <span className="text-slate-500 font-medium">Telefon</span>
                                {isEditingProfile ? (
                                    <input 
                                        type="text" 
                                        value={editPhone} 
                                        onChange={(e) => setEditPhone(e.target.value)}
                                        className="p-2 border border-slate-300 rounded-lg text-right font-bold text-slate-800 focus:ring-2 focus:ring-emerald-500 outline-none"
                                        placeholder="05..."
                                    />
                                ) : (
                                    <span className="text-slate-800 font-bold text-lg">{userProfile?.telefonNumarası || userProfile?.TelefonNumarası || '-'}</span>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
              )}
            </>
          )}
        </div>
      </main>

      {/* NOT EKLEME MODAL */}
      {noteModalOpen && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-3xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-auto animate-scale-in">
            {/* Modal Header */}
            <div className="p-6 border-b border-slate-100 flex items-center justify-between sticky top-0 bg-white z-10">
              <div>
                <h3 className="text-xl font-bold text-slate-800">Doktor Notu</h3>
                <p className="text-sm text-slate-500 mt-1">
                  {selectedAppointment?.patientName} - {selectedAppointment ? new Date(selectedAppointment.randevuTarihi || selectedAppointment.RandevuTarihi).toLocaleDateString('tr-TR') : ''}
                </p>
              </div>
              <button
                onClick={() => {
                  setNoteModalOpen(false);
                  setNoteText('');
                  setSelectedAppointment(null);
                }}
                className="p-2 hover:bg-slate-100 rounded-full transition"
              >
                <X className="w-6 h-6 text-slate-500" />
              </button>
            </div>

            {/* Modal Body */}
            <div className="p-8 space-y-6">
              <div className="bg-slate-50 p-5 rounded-2xl border border-slate-100">
                <label className="block text-xs font-bold text-slate-400 uppercase tracking-wider mb-2">
                  Hasta Şikayeti
                </label>
                <p className="text-slate-700 font-medium">
                  {selectedAppointment?.sikayet || selectedAppointment?.Sikayet || 'Belirtilmemiş'}
                </p>
              </div>

              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2 ml-1">
                  Notunuz
                </label>
                <textarea
                  value={noteText}
                  onChange={(e) => setNoteText(e.target.value)}
                  rows="6"
                  placeholder="Muayene notlarınızı, teşhis ve tedavi önerilerinizi buraya yazabilirsiniz..."
                  className="w-full p-4 border border-slate-200 rounded-2xl focus:ring-2 focus:ring-emerald-500 outline-none resize-none text-slate-700 placeholder:text-slate-400"
                />
              </div>
            </div>

            {/* Modal Footer */}
            <div className="p-6 border-t border-slate-100 flex gap-3 justify-end bg-slate-50/50">
              <button
                onClick={() => {
                  setNoteModalOpen(false);
                  setNoteText('');
                  setSelectedAppointment(null);
                }}
                className="px-6 py-3 text-slate-600 font-bold hover:bg-slate-200 rounded-xl transition"
              >
                İptal
              </button>
              <button
                onClick={handleSaveNote}
                className="px-8 py-3 bg-emerald-600 text-white rounded-xl hover:bg-emerald-700 transition font-bold shadow-lg shadow-emerald-200 flex items-center gap-2"
              >
                <Save className="w-4 h-4" />
                Notu Kaydet
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default DoktorDashboard;