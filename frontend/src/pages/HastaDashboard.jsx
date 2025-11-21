import { useNavigate } from 'react-router-dom';
import { LogOut, User, Calendar, Clock, Plus, FileText, Menu, Activity, Home, X, ChevronRight, UserCircle, MapPin, Phone, Heart, Edit2, Save } from 'lucide-react';
import { useState, useEffect } from 'react';
import Swal from 'sweetalert2';
// getHastaProfil eklendi
import { getAllDoktorlar, getAllUzmanliklar, getAvailableSlots, createRandevu, getProfile, getPatientAppointments, cancelRandevu, updateHasta, updateUserPhone, getHastaProfil } from '../api';

function HastaDashboard() {
  const navigate = useNavigate();
  const kullaniciAd = localStorage.getItem('kullaniciAd');
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [activeTab, setActiveTab] = useState('dashboard');

  // Randevu Modal State
  const [showRandevuModal, setShowRandevuModal] = useState(false);
  const [step, setStep] = useState(1);
  const [departments, setDepartments] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [selectedDepartment, setSelectedDepartment] = useState('');
  const [selectedDoctor, setSelectedDoctor] = useState(null);
  const [selectedDate, setSelectedDate] = useState('');
  const [availableSlots, setAvailableSlots] = useState([]);
  const [selectedSlot, setSelectedSlot] = useState(null);
  const [sikayet, setSikayet] = useState('');
  const [loading, setLoading] = useState(false);

  // Randevular ve İstatistikler
  const [appointments, setAppointments] = useState([]);
  const [stats, setStats] = useState({
      total: 0,
      upcoming: 0,
      completed: 0
  });

  // Detay Modalı
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedAppointmentDetails, setSelectedAppointmentDetails] = useState(null);

  // Kullanıcı ve Hasta Verileri
  const [currentUserId, setCurrentUserId] = useState(null);
  const [hastaId, setHastaId] = useState(null);
  const [userProfile, setUserProfile] = useState(null);
  const [patientProfile, setPatientProfile] = useState(null);

  // Edit Mode State
  const [isEditing, setIsEditing] = useState(false);
  const [editForm, setEditForm] = useState({
      phone: '',
      address: '',
      emergencyContact: '',
      emergencyPhone: ''
  });

  useEffect(() => {
      const fetchProfileAndData = async () => {
          try {
              // 1. Profil al (Kullanıcı tablosu verileri)
              const profileRes = await getProfile();
              if(profileRes.isSuccess) {
                  setCurrentUserId(profileRes.data.id);
                  setUserProfile(profileRes.data);
                  
                  // 2. Hasta Profilini al (api.js üzerinden güvenli çağrı)
                  const hastaData = await getHastaProfil();
                  
                  if (hastaData.isSuccess) {
                      const hId = hastaData.data.id;
                      setHastaId(hId);
                      setPatientProfile(hastaData.data);
                      
                      // Initialize edit form
                      setEditForm({
                          phone: profileRes.data.telefonNumarası || '',
                          address: hastaData.data.adres || '',
                          emergencyContact: hastaData.data.acilDurumKişisi || '',
                          emergencyPhone: hastaData.data.acilDurumTelefon || ''
                      });

                      // 3. Randevuları al
                      fetchAppointments(hId);
                  }
              }
          } catch (error) {
              console.error("Veriler yüklenemedi", error);
          }
      };
      fetchProfileAndData();
  }, []);

  const fetchAppointments = async (hId) => {
      try {
          setLoading(true);
          const res = await getPatientAppointments(hId);
          if(res.isSuccess) {
              const sortedApps = res.data.sort((a, b) => new Date(b.randevuTarihi) - new Date(a.randevuTarihi));
              setAppointments(sortedApps);
              
              // İstatistikleri hesapla
              const now = new Date();
              setStats({
                  total: sortedApps.length,
                  upcoming: sortedApps.filter(a => new Date(a.randevuTarihi) > now && a.durum !== 'İptal' && a.durum !== 'Tamamlandı').length,
                  completed: sortedApps.filter(a => a.durum === 'Tamamlandı').length
              });
          }
      } catch (error) {
          console.error("Randevular yüklenemedi", error);
      } finally {
          setLoading(false);
      }
  };

  // Modal açıldığında verileri yükle (GÜNCELLENDİ)
  useEffect(() => {
    if (showRandevuModal && departments.length === 0) {
      const fetchData = async () => {
        try {
          setLoading(true);
          const deptRes = await getAllUzmanliklar();
          const docRes = await getAllDoktorlar();
          
          if (deptRes.isSuccess) setDepartments(deptRes.data);
          
          if (docRes.isSuccess) {
              console.log("✅ DOKTORLAR GELDİ:", docRes.data); 
              setDoctors(docRes.data);
          } else {
              console.error("❌ DOKTOR API HATASI:", docRes.message);
          }
        } catch (error) {
          console.error('Veri yükleme hatası:', error);
        } finally {
          setLoading(false);
        }
      };
      fetchData();
    }
  }, [showRandevuModal]);

  // Tarih veya doktor değiştiğinde saatleri getir
  useEffect(() => {
    if (selectedDoctor && selectedDate) {
      const fetchSlots = async () => {
        try {
          setLoading(true);
          const res = await getAvailableSlots(selectedDoctor.id, selectedDate);
          if (res.isSuccess) {
            setAvailableSlots(res.data);
          } else {
              setAvailableSlots([]);
              Swal.fire('Bilgi', res.message, 'info');
          }
        } catch (error) {
          console.error('Saatler yüklenemedi:', error);
        } finally {
          setLoading(false);
        }
      };
      fetchSlots();
    }
  }, [selectedDoctor, selectedDate]);

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const handleUpdateProfile = async () => {
      try {
          setLoading(true);
          
          // 1. Update User Phone
          if (currentUserId) {
              const phoneRes = await updateUserPhone(currentUserId, editForm.phone);
              if(!phoneRes.isSuccess) throw new Error(phoneRes.message);
          }

          // 2. Update Hasta Details
          if (hastaId) {
              const hastaDto = {
                  adres: editForm.address,
                  acilDurumKişisi: editForm.emergencyContact,
                  acilDurumTelefon: editForm.emergencyPhone
              };
              const hastaRes = await updateHasta(hastaId, hastaDto);
              if(!hastaRes.isSuccess) throw new Error(hastaRes.message);
          }

          Swal.fire('Başarılı', 'Profil bilgileriniz güncellendi.', 'success');
          setIsEditing(false);
          
          // Refresh data locally
          setUserProfile(prev => ({ ...prev, telefonNumarası: editForm.phone }));
          setPatientProfile(prev => ({ 
              ...prev, 
              adres: editForm.address,
              acilDurumKişisi: editForm.emergencyContact,
              acilDurumTelefon: editForm.emergencyPhone
          }));

      } catch (error) {
          console.error(error);
          Swal.fire('Hata', 'Profil güncellenirken bir hata oluştu.', 'error');
      } finally {
          setLoading(false);
      }
  };

  const submitRandevu = async () => {
       if(!hastaId) {
           Swal.fire('Hata', "Hasta profiliniz bulunamadı. Lütfen tekrar giriş yapın.", 'error');
           return;
       }

       try {
          setLoading(true);
          const [hours, minutes] = selectedSlot.split(':');
          const datePart = new Date(selectedDate);
          const randevuTarihi = new Date(datePart.getFullYear(), datePart.getMonth(), datePart.getDate(), parseInt(hours), parseInt(minutes));
          const formattedDate = new Date(randevuTarihi.getTime() - (randevuTarihi.getTimezoneOffset() * 60000)).toISOString().slice(0, 19);

          const dto = {
              hastaId: hastaId,
              doktorId: selectedDoctor.id,
              randevuTarihi: formattedDate,
              hastaŞikayeti: sikayet
          };

          const res = await createRandevu(dto);
          if(res.isSuccess) {
              Swal.fire('Başarılı', "Randevunuz başarıyla oluşturuldu!", 'success');
              setShowRandevuModal(false);
              // Reset states
              setStep(1);
              setSelectedDoctor(null);
              setSelectedDate('');
              setSelectedSlot(null);
              setSikayet('');
              // Listeyi yenile
              fetchAppointments(hastaId);
          } else {
              Swal.fire('Hata', res.message, 'error');
          }
       } catch(err) {
           console.error(err);
           Swal.fire('Hata', "Bir hata oluştu.", 'error');
       } finally {
           setLoading(false);
       }
  };

  const handleCancelRandevu = async (id) => {
      Swal.fire({
          title: 'Emin misiniz?',
          text: "Randevuyu iptal etmek istediğinize emin misiniz?",
          icon: 'warning',
          showCancelButton: true,
          confirmButtonColor: '#3085d6',
          cancelButtonColor: '#d33',
          confirmButtonText: 'Evet, iptal et!',
          cancelButtonText: 'Vazgeç'
      }).then(async (result) => {
          if (result.isConfirmed) {
              try {
                  const res = await cancelRandevu(id);
                  if(res.isSuccess) {
                      Swal.fire('İptal Edildi!', 'Randevunuz iptal edildi.', 'success');
                      fetchAppointments(hastaId);
                  } else {
                      Swal.fire('Hata', res.message, 'error');
                  }
              } catch (error) {
                  console.error("İptal hatası:", error);
                  Swal.fire('Hata', "Bir hata oluştu.", 'error');
              }
          }
      });
  };

  const openDetails = (appointment) => {
      setSelectedAppointmentDetails(appointment);
      setDetailsModalOpen(true);
  };

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
          <div className="bg-gradient-to-tr from-blue-600 to-indigo-600 p-2.5 rounded-xl shadow-lg shadow-blue-500/30">
            <Activity className="w-6 h-6 text-white" />
          </div>
          <span className="text-2xl font-bold tracking-tight text-slate-800">ClinicTrack</span>
        </div>

        <nav className="px-4 space-y-2">
          <p className="px-4 text-xs font-bold text-slate-400 uppercase tracking-wider mb-4">Menü</p>
          
          <button 
            onClick={() => { setActiveTab('dashboard'); setIsMobileMenuOpen(false); }}
            className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${activeTab === 'dashboard' ? 'bg-blue-50 text-blue-700 shadow-sm border border-blue-100' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'}`}
          >
            <Home className="w-5 h-5" />
            <span>Ana Sayfa</span>
          </button>
          
          <button 
            onClick={() => { setActiveTab('profile'); setIsMobileMenuOpen(false); }}
            className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${activeTab === 'profile' ? 'bg-blue-50 text-blue-700 shadow-sm border border-blue-100' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'}`}
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
        {/* HEADER (Mobile) */}
        <header className="md:hidden bg-white shadow-sm h-16 flex items-center justify-between px-4 sticky top-0 z-20">
            <button onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)} className="p-2">
              <Menu className="w-6 h-6 text-slate-600" />
            </button>
            <span className="font-bold text-lg">ClinicTrack</span>
            <div className="w-8"></div>
        </header>

        <div className="p-6 md:p-10 max-w-7xl mx-auto w-full">
          
          {activeTab === 'dashboard' && (
            <>
              {/* HERO SECTION */}
              <div className="mb-10 bg-gradient-to-r from-blue-600 to-indigo-700 rounded-3xl p-8 md:p-12 text-white shadow-xl shadow-blue-900/20 relative overflow-hidden">
                <div className="absolute top-0 right-0 w-64 h-64 bg-white opacity-5 rounded-full -translate-y-1/2 translate-x-1/3 blur-3xl"></div>
                <div className="absolute bottom-0 left-0 w-48 h-48 bg-indigo-400 opacity-10 rounded-full translate-y-1/3 -translate-x-1/4 blur-2xl"></div>
                
                <div className="relative z-10 flex flex-col md:flex-row justify-between items-start md:items-end gap-6">
                    <div>
                        <p className="text-blue-100 font-medium mb-2 flex items-center gap-2">
                            <Calendar className="w-4 h-4" /> {currentDate}
                        </p>
                        <h1 className="text-3xl md:text-4xl font-bold mb-2">Hoş Geldiniz, {kullaniciAd}</h1>
                        <p className="text-blue-100 max-w-md">Sağlığınız bizim için önemli. Randevularınızı buradan kolayca yönetebilirsiniz.</p>
                    </div>
                    
                    <button 
                        onClick={() => setShowRandevuModal(true)}
                        className="bg-white text-blue-600 px-6 py-3.5 rounded-xl font-bold hover:bg-blue-50 transition shadow-lg flex items-center gap-2 group active:scale-95 transform duration-200"
                    >
                        <Plus className="w-5 h-5 group-hover:rotate-90 transition-transform" />
                        Yeni Randevu
                    </button>
                </div>
              </div>

              {/* STATS */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
                {[
                    { label: 'Toplam Randevu', value: stats.total, icon: Calendar, color: 'blue' },
                    { label: 'Yaklaşan', value: stats.upcoming, icon: Clock, color: 'orange' },
                    { label: 'Tamamlanan', value: stats.completed, icon: FileText, color: 'emerald' }
                ].map((stat, idx) => (
                    <div key={idx} className="bg-white p-6 rounded-2xl shadow-md border border-slate-200 flex items-center gap-5 transition-all hover:-translate-y-1">
                        <div className={`p-4 rounded-xl bg-${stat.color}-50 text-${stat.color}-600`}>
                            <stat.icon className="w-7 h-7" />
                        </div>
                        <div>
                            <p className="text-slate-500 text-sm font-medium mb-1">{stat.label}</p>
                            <h3 className="text-3xl font-bold text-slate-800">{stat.value}</h3>
                        </div>
                    </div>
                ))}
              </div>

              <div className="grid grid-cols-1 xl:grid-cols-3 gap-8">
                {/* UPCOMING APPOINTMENTS */}
                <div className="xl:col-span-2 space-y-6">
                    <div className="flex items-center justify-between">
                        <h2 className="text-xl font-bold text-slate-800 flex items-center gap-2">
                            <Calendar className="w-5 h-5 text-blue-600" />
                            Randevularım
                        </h2>
                    </div>

                    <div className="space-y-4">
                        {appointments.length === 0 ? (
                            <div className="bg-white rounded-2xl p-10 text-center border border-slate-200 shadow-md">
                                <div className="bg-slate-50 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                                    <Calendar className="w-8 h-8 text-slate-400" />
                                </div>
                                <h3 className="text-lg font-bold text-slate-700 mb-1">Randevu Bulunmuyor</h3>
                                <p className="text-slate-500 mb-6">Henüz planlanmış bir randevunuz yok.</p>
                                <button 
                                    onClick={() => setShowRandevuModal(true)}
                                    className="text-blue-600 font-bold hover:underline"
                                >
                                    Hemen Randevu Al
                                </button>
                            </div>
                        ) : (
                            appointments.map((apt) => (
                                <div key={apt.id} className={`group bg-white rounded-2xl p-6 border border-slate-200 shadow-md hover:shadow-lg transition-all relative overflow-hidden ${apt.durum === 'Beklemede' ? 'border-l-4 border-l-orange-400' : apt.durum === 'Tamamlandı' ? 'border-l-4 border-l-emerald-500' : 'border-l-4 border-l-red-500'}`}>
                                    <div className="flex flex-col md:flex-row md:items-center gap-6">
                                        <div className="flex items-center gap-5 flex-1">
                                            <div className={`flex flex-col items-center justify-center w-16 h-16 rounded-2xl bg-slate-50 text-slate-600 font-bold flex-shrink-0 border border-slate-200`}>
                                                <span className="text-xs uppercase text-slate-400 font-semibold">{new Date(apt.randevuTarihi).toLocaleString('default', { month: 'short' })}</span>
                                                <span className="text-xl">{new Date(apt.randevuTarihi).getDate()}</span>
                                            </div>
                                            
                                            <div>
                                                <h4 className="font-bold text-lg text-slate-800 mb-1">
                                                    {apt.doktorAdi ? `Dr. ${apt.doktorAdi} ${apt.doktorSoyadi}` : 'Doktor Bilgisi Yok'}
                                                </h4>
                                                <p className="text-slate-500 text-sm font-medium mb-2">{apt.uzmanlıkAdi || 'Genel Muayene'}</p>
                                                
                                                <div className="flex flex-wrap items-center gap-3 text-sm">
                                                    <div className="flex items-center gap-1.5 text-slate-600 bg-slate-50 px-2.5 py-1 rounded-lg">
                                                        <Clock className="w-4 h-4 text-slate-400" />
                                                        {new Date(apt.randevuTarihi).toLocaleTimeString('tr-TR', {hour: '2-digit', minute:'2-digit'})}
                                                    </div>
                                                    <span className={`px-2.5 py-1 rounded-lg text-xs font-bold ${
                                                        apt.durum === 'Beklemede' ? 'bg-orange-50 text-orange-600' : 
                                                        apt.durum === 'Tamamlandı' ? 'bg-emerald-50 text-emerald-600' : 
                                                        'bg-red-50 text-red-600'
                                                    }`}>
                                                        {apt.durum}
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="flex items-center gap-3 md:border-l md:pl-6 md:border-slate-100">
                                            <button 
                                                onClick={() => openDetails(apt)}
                                                className="flex-1 md:flex-none px-5 py-2.5 rounded-xl bg-slate-50 text-slate-600 font-medium hover:bg-slate-100 transition text-sm"
                                            >
                                                Detaylar
                                            </button>
                                            {apt.durum === 'Beklemede' && (
                                                <button 
                                                    onClick={() => handleCancelRandevu(apt.id)}
                                                    className="flex-1 md:flex-none px-5 py-2.5 rounded-xl bg-red-50 text-red-600 font-medium hover:bg-red-100 transition text-sm"
                                                >
                                                    İptal
                                                </button>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>

                {/* RIGHT COLUMN - HISTORY & PROFILE SUMMARY */}
                <div className="space-y-8">
                    <div className="bg-white rounded-3xl p-6 shadow-md border border-slate-200">
                        <h3 className="font-bold text-lg text-slate-800 mb-6 flex items-center justify-between">
                            <span>Geçmiş Ziyaretler</span>
                            <button className="text-blue-600 text-sm font-medium hover:underline">Tümü</button>
                        </h3>
                        
                        <div className="space-y-6 relative before:absolute before:left-[19px] before:top-2 before:bottom-2 before:w-[2px] before:bg-slate-100">
                            {appointments.filter(a => a.durum === 'Tamamlandı').length === 0 ? (
                                <div className="text-center py-4 text-slate-400 text-sm">
                                    Geçmiş ziyaret kaydı bulunamadı.
                                </div>
                            ) : (
                                appointments.filter(a => a.durum === 'Tamamlandı').slice(0, 4).map(apt => (
                                    <div key={apt.id} onClick={() => openDetails(apt)} className="relative pl-10 group cursor-pointer">
                                        <div className="absolute left-3 top-1.5 w-4 h-4 rounded-full bg-white border-4 border-emerald-500 group-hover:scale-110 transition-transform z-10"></div>
                                        <h4 className="text-sm font-bold text-slate-800 group-hover:text-blue-600 transition-colors">
                                            {apt.uzmanlıkAdi || 'Kontrol'}
                                        </h4>
                                        <p className="text-xs text-slate-500 mt-0.5 mb-1">
                                            {apt.doktorAdi ? `Dr. ${apt.doktorAdi} ${apt.doktorSoyadi}` : '-'}
                                        </p>
                                        <p className="text-xs font-medium text-slate-400">
                                            {new Date(apt.randevuTarihi).toLocaleDateString('tr-TR')}
                                        </p>
                                    </div>
                                ))
                            )}
                        </div>
                    </div>

                    <div className="bg-gradient-to-br from-slate-800 to-slate-900 rounded-3xl p-6 text-white shadow-lg">
                        <h3 className="font-bold text-lg mb-4">Sağlık İpucu</h3>
                        <p className="text-slate-300 text-sm leading-relaxed mb-4">
                            Düzenli sağlık kontrolleri, olası hastalıkların erken teşhisi için hayati önem taşır. Yılda en az bir kez genel check-up yaptırmayı ihmal etmeyin.
                        </p>
                        <div className="flex items-center gap-2 text-xs font-medium text-blue-300">
                            <Activity className="w-4 h-4" />
                            <span>ClinicTrack Sağlık Ekibi</span>
                        </div>
                    </div>
                </div>
              </div>
            </>
          )}

          {activeTab === 'profile' && (
            <div className="animate-fade-in space-y-8">
                <div className="bg-gradient-to-r from-indigo-600 to-purple-700 rounded-3xl p-10 text-white shadow-xl shadow-indigo-900/20 flex flex-col md:flex-row items-center justify-between gap-6">
                    <div className="flex flex-col md:flex-row items-center gap-8">
                        <div className="w-32 h-32 bg-white/10 backdrop-blur-md rounded-full flex items-center justify-center border-4 border-white/20 text-4xl font-bold shadow-inner">
                            {userProfile?.isim ? userProfile.isim.charAt(0).toUpperCase() : <User className="w-12 h-12" />}
                        </div>
                        <div className="text-center md:text-left">
                            <h2 className="text-3xl font-bold mb-2">{userProfile?.isim} {userProfile?.soyisim}</h2>
                            <div className="flex flex-wrap justify-center md:justify-start gap-4 text-sm text-indigo-100">
                                {userProfile?.email && (
                                    <div className="flex items-center gap-2">
                                        <div className="w-8 h-8 rounded-full bg-white/10 flex items-center justify-center">@</div>
                                        {userProfile.email}
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                    <div>
                        {!isEditing ? (
                            <button 
                                onClick={() => setIsEditing(true)}
                                className="bg-white/10 hover:bg-white/20 text-white px-6 py-3 rounded-xl backdrop-blur-md border border-white/20 transition font-bold flex items-center gap-2"
                            >
                                <Edit2 className="w-4 h-4" />
                                Düzenle
                            </button>
                        ) : (
                            <div className="flex gap-3">
                                <button 
                                    onClick={() => setIsEditing(false)}
                                    className="bg-white/10 hover:bg-white/20 text-white px-6 py-3 rounded-xl backdrop-blur-md border border-white/20 transition font-bold"
                                >
                                    İptal
                                </button>
                                <button 
                                    onClick={handleUpdateProfile}
                                    className="bg-white text-indigo-600 px-6 py-3 rounded-xl font-bold shadow-lg hover:bg-indigo-50 transition flex items-center gap-2"
                                >
                                    <Save className="w-4 h-4" />
                                    Kaydet
                                </button>
                            </div>
                        )}
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    {/* Kişisel Bilgiler */}
                    <div className="bg-white rounded-3xl shadow-md border border-slate-200 p-8">
                        <h3 className="text-xl font-bold text-slate-800 mb-6 flex items-center gap-3">
                            <UserCircle className="w-6 h-6 text-blue-600" />
                            Kişisel Bilgiler
                        </h3>
                        <div className="space-y-4">
                            <div className="flex justify-between items-center py-3 border-b border-slate-50">
                                <span className="text-slate-500 text-sm font-medium">TC Kimlik No</span>
                                <span className="text-slate-800 font-bold">{userProfile?.tcNo || '-'}</span>
                            </div>
                            <div className="flex justify-between items-center py-3 border-b border-slate-50">
                                <span className="text-slate-500 text-sm font-medium">Telefon</span>
                                {isEditing ? (
                                    <input 
                                        type="text" 
                                        value={editForm.phone} 
                                        onChange={(e) => setEditForm({...editForm, phone: e.target.value})}
                                        className="p-2 border border-slate-300 rounded-lg text-right font-bold text-slate-800 focus:ring-2 focus:ring-blue-500 outline-none"
                                    />
                                ) : (
                                    <span className="text-slate-800 font-bold">{userProfile?.telefonNumarası || '-'}</span>
                                )}
                            </div>
                        </div>
                    </div>

                    {/* İletişim & Adres */}
                    <div className="bg-white rounded-3xl shadow-md border border-slate-200 p-8">
                        <h3 className="text-xl font-bold text-slate-800 mb-6 flex items-center gap-3">
                            <MapPin className="w-6 h-6 text-orange-500" />
                            İletişim & Adres
                        </h3>
                        <div className="space-y-6">
                            <div>
                                <span className="block text-slate-500 text-sm font-medium mb-2">Adres</span>
                                {isEditing ? (
                                    <textarea 
                                        value={editForm.address}
                                        onChange={(e) => setEditForm({...editForm, address: e.target.value})}
                                        className="w-full p-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none text-sm"
                                        rows="3"
                                    />
                                ) : (
                                    <p className="text-slate-700 bg-slate-50 p-4 rounded-xl border border-slate-100 text-sm leading-relaxed">
                                        {patientProfile?.adres || 'Adres bilgisi girilmemiş.'}
                                    </p>
                                )}
                            </div>
                            
                            <div className="pt-2">
                                <h4 className="font-bold text-slate-800 mb-3 flex items-center gap-2">
                                    <Heart className="w-4 h-4 text-red-500" />
                                    Acil Durum Kişisi
                                </h4>
                                <div className="bg-red-50 rounded-xl p-4 border border-red-100">
                                    <div className="grid grid-cols-1 gap-3">
                                        {isEditing ? (
                                            <>
                                                <input 
                                                    placeholder="Ad Soyad"
                                                    value={editForm.emergencyContact}
                                                    onChange={(e) => setEditForm({...editForm, emergencyContact: e.target.value})}
                                                    className="p-2 border border-red-200 rounded-lg text-sm w-full"
                                                />
                                                <input 
                                                    placeholder="Telefon"
                                                    value={editForm.emergencyPhone}
                                                    onChange={(e) => setEditForm({...editForm, emergencyPhone: e.target.value})}
                                                    className="p-2 border border-red-200 rounded-lg text-sm w-full"
                                                />
                                            </>
                                        ) : (
                                            <div className="flex items-center justify-between">
                                                <div>
                                                    <p className="text-red-900 font-bold text-sm">{patientProfile?.acilDurumKişisi || '-'}</p>
                                                    <p className="text-red-600 text-xs mt-1">{patientProfile?.acilDurumTelefon || '-'}</p>
                                                </div>
                                                <div className="bg-white p-2 rounded-full text-red-500 shadow-sm">
                                                    <Phone className="w-4 h-4" />
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
          )}

          {/* DETAILS MODAL */}
          {detailsModalOpen && selectedAppointmentDetails && (
            <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                <div className="bg-white rounded-3xl shadow-2xl max-w-md w-full p-0 overflow-hidden animate-fade-in-up">
                    <div className="bg-slate-50 p-6 border-b border-slate-100 flex justify-between items-center">
                        <h3 className="text-xl font-bold text-slate-800">Randevu Detayı</h3>
                        <button 
                            onClick={() => setDetailsModalOpen(false)}
                            className="p-2 hover:bg-slate-200 rounded-full transition"
                        >
                            <X className="w-5 h-5 text-slate-500" />
                        </button>
                    </div>
                    
                    <div className="p-6 space-y-6">
                        <div className="flex items-start gap-4">
                            <div className="w-14 h-14 bg-blue-50 rounded-2xl flex items-center justify-center text-blue-600">
                                <UserCircle className="w-8 h-8" />
                            </div>
                            <div>
                                <p className="text-xs font-bold text-slate-400 uppercase tracking-wider">Doktor</p>
                                <h4 className="text-lg font-bold text-slate-800">
                                    {selectedAppointmentDetails.doktorAdi ? `Dr. ${selectedAppointmentDetails.doktorAdi} ${selectedAppointmentDetails.doktorSoyadi}` : '-'}
                                </h4>
                                <p className="text-blue-600 font-medium text-sm">{selectedAppointmentDetails.uzmanlıkAdi}</p>
                            </div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div className="p-4 bg-slate-50 rounded-xl border border-slate-100">
                                <p className="text-xs font-bold text-slate-400 uppercase mb-1">Tarih</p>
                                <p className="font-bold text-slate-800">
                                    {new Date(selectedAppointmentDetails.randevuTarihi).toLocaleDateString('tr-TR')}
                                </p>
                            </div>
                            <div className="p-4 bg-slate-50 rounded-xl border border-slate-100">
                                <p className="text-xs font-bold text-slate-400 uppercase mb-1">Saat</p>
                                <p className="font-bold text-slate-800">
                                    {new Date(selectedAppointmentDetails.randevuTarihi).toLocaleTimeString('tr-TR', {hour: '2-digit', minute:'2-digit'})}
                                </p>
                            </div>
                        </div>

                        <div>
                            <p className="text-xs font-bold text-slate-400 uppercase mb-2">Şikayet</p>
                            <div className="p-4 bg-slate-50 rounded-xl border border-slate-100 text-slate-700 text-sm">
                                {selectedAppointmentDetails.hastaŞikayeti || 'Belirtilmemiş'}
                            </div>
                        </div>

                        {selectedAppointmentDetails.doktorNotları && (
                            <div>
                                <p className="text-xs font-bold text-emerald-600 uppercase mb-2">Doktor Notu</p>
                                <div className="p-4 bg-emerald-50 rounded-xl border border-emerald-100 text-slate-700 text-sm">
                                    {selectedAppointmentDetails.doktorNotları}
                                </div>
                            </div>
                        )}

                        <div className="pt-2 flex justify-end border-t border-slate-100 mt-4">
                            <span className={`px-4 py-2 rounded-full text-sm font-bold ${
                                selectedAppointmentDetails.durum === 'Beklemede' ? 'bg-orange-100 text-orange-700' : 
                                selectedAppointmentDetails.durum === 'Tamamlandı' ? 'bg-emerald-100 text-emerald-700' : 
                                'bg-red-100 text-red-700'
                            }`}>
                                {selectedAppointmentDetails.durum}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
          )}

        {/* RANDEVU MODAL */}
        {showRandevuModal && (
            <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                <div className="bg-white rounded-3xl shadow-2xl max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col animate-scale-in">
                    {/* Modal Header */}
                    <div className="p-6 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
                        <div>
                            <h3 className="text-2xl font-bold text-slate-800">Randevu Oluştur</h3>
                            <div className="flex items-center gap-2 text-sm mt-1">
                                <span className={`font-bold ${step >= 1 ? 'text-blue-600' : 'text-slate-400'}`}>Bölüm & Doktor</span>
                                <ChevronRight className="w-4 h-4 text-slate-300" />
                                <span className={`font-bold ${step >= 2 ? 'text-blue-600' : 'text-slate-400'}`}>Tarih & Saat</span>
                                <ChevronRight className="w-4 h-4 text-slate-300" />
                                <span className={`font-bold ${step >= 3 ? 'text-blue-600' : 'text-slate-400'}`}>Onay</span>
                            </div>
                        </div>
                        <button onClick={() => setShowRandevuModal(false)} className="p-2 hover:bg-slate-200 rounded-full transition">
                            <X className="w-6 h-6 text-slate-500" />
                        </button>
                    </div>

                    {/* Modal Body */}
                    <div className="p-8 overflow-y-auto flex-1">
                        {loading && (
                            <div className="absolute inset-0 bg-white/80 z-10 flex items-center justify-center">
                                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                            </div>
                        )}
                        
                        {step === 1 && (
                            <div className="space-y-8 animate-slide-in-right">
                                <div>
                                    <label className="block text-sm font-bold text-slate-700 mb-3 uppercase tracking-wide">Bölüm Seçin</label>
                                    <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                                        {departments.map(dept => (
                                            <button
                                                key={dept.id}
                                                onClick={() => setSelectedDepartment(dept.id)}
                                                className={`p-4 rounded-2xl text-left transition-all duration-200 font-medium ${
                                                    selectedDepartment === dept.id 
                                                    ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/30 scale-[1.02]' 
                                                    : 'bg-slate-50 text-slate-700 hover:bg-slate-100'
                                                }`}
                                            >
                                                {dept.uzmanlıkAdı}
                                            </button>
                                        ))}
                                    </div>
                                </div>

                                {selectedDepartment && (
                                    <div className="animate-fade-in">
                                        <label className="block text-sm font-bold text-slate-700 mb-3 uppercase tracking-wide">Doktor Seçin</label>
                                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                            {/* GÜNCELLENEN FİLTRELEME KODU */}
                                            {doctors.filter(d => {
                                                const docUzmanlik = d.uzmanlıkId || d.uzmanlikId || d.UzmanlıkId || d.UzmanlikId;
                                                return docUzmanlik == selectedDepartment;
                                            }).map(doc => (
                                                <button
                                                    key={doc.id}
                                                    onClick={() => {
                                                        setSelectedDoctor(doc);
                                                        setStep(2);
                                                    }}
                                                    className="flex items-center gap-4 p-4 rounded-2xl border border-slate-200 hover:border-blue-500 hover:shadow-md transition-all text-left group bg-white"
                                                >
                                                    <div className="w-12 h-12 bg-blue-50 rounded-full flex items-center justify-center text-blue-600 font-bold group-hover:bg-blue-600 group-hover:text-white transition-colors">
                                                        Dr
                                                    </div>
                                                    <div>
                                                        <h4 className="font-bold text-slate-800 group-hover:text-blue-600 transition-colors">
                                                            {doc.isim || doc.İsim} {doc.soyisim || doc.Soyisim}
                                                        </h4>
                                                        <p className="text-sm text-slate-500">
                                                            {departments.find(dep => (dep.id || dep.Id) == (doc.uzmanlıkId || doc.uzmanlikId))?.uzmanlıkAdı}
                                                        </p>
                                                    </div>
                                                    <ChevronRight className="w-5 h-5 text-slate-300 ml-auto group-hover:text-blue-600" />
                                                </button>
                                            ))}
                                            {doctors.filter(d => (d.uzmanlıkId || d.uzmanlikId) == selectedDepartment).length === 0 && (
                                                <div className="text-slate-500 text-sm col-span-2 bg-slate-50 p-4 rounded-xl border border-slate-200 text-center">
                                                    Bu bölümde henüz doktor bulunmuyor.
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                )}
                            </div>
                        )}

                        {step === 2 && selectedDoctor && (
                            <div className="space-y-8 animate-slide-in-right">
                                <div className="flex items-center gap-4 p-4 bg-blue-50/50 rounded-2xl border border-blue-100">
                                    <div className="w-12 h-12 bg-blue-600 rounded-xl flex items-center justify-center text-white font-bold shadow-lg shadow-blue-600/20">
                                        Dr
                                    </div>
                                    <div>
                                        <h4 className="font-bold text-slate-800">{selectedDoctor.isim} {selectedDoctor.soyisim}</h4>
                                        <p className="text-sm text-blue-600 font-medium">Seçilen Doktor</p>
                                    </div>
                                    <button onClick={() => setStep(1)} className="ml-auto text-sm font-bold text-slate-400 hover:text-slate-600 px-3 py-1.5 rounded-lg hover:bg-slate-100 transition">Değiştir</button>
                                </div>

                                <div>
                                    <label className="block text-sm font-bold text-slate-700 mb-3 uppercase tracking-wide">Tarih Seçin</label>
                                    <input 
                                        type="date" 
                                        min={new Date().toISOString().split('T')[0]}
                                        value={selectedDate}
                                        onChange={(e) => {
                                            setSelectedDate(e.target.value);
                                            setSelectedSlot(null);
                                        }}
                                        className="w-full p-4 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:bg-white outline-none transition font-medium"
                                    />
                                </div>

                                {selectedDate && (
                                    <div className="animate-fade-in">
                                        <label className="block text-sm font-bold text-slate-700 mb-3 uppercase tracking-wide">Saat Seçin</label>
                                        {availableSlots.length > 0 ? (
                                            <div className="grid grid-cols-4 sm:grid-cols-5 md:grid-cols-6 gap-3">
                                                {availableSlots.map(slot => (
                                                    <button
                                                        key={slot}
                                                        onClick={() => setSelectedSlot(slot)}
                                                        className={`py-3 px-2 rounded-xl text-sm font-bold transition-all ${
                                                            selectedSlot === slot 
                                                            ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/30 scale-105' 
                                                            : 'bg-white border border-slate-200 text-slate-600 hover:border-blue-400 hover:bg-blue-50'
                                                        }`}
                                                    >
                                                        {slot}
                                                    </button>
                                                ))}
                                            </div>
                                        ) : (
                                            <div className="p-6 bg-orange-50 text-orange-700 rounded-2xl text-sm border border-orange-100 flex items-center justify-center gap-3">
                                                <Clock className="w-5 h-5" />
                                                <span>Bu tarihte uygun randevu saati bulunamadı veya hafta sonu seçtiniz.</span>
                                            </div>
                                        )}
                                    </div>
                                )}

                                <div className="pt-4 flex justify-between items-center">
                                    <button onClick={() => setStep(1)} className="px-6 py-3 text-slate-500 font-bold hover:bg-slate-100 rounded-xl transition">Geri</button>
                                    <button 
                                        disabled={!selectedSlot}
                                        onClick={() => setStep(3)} 
                                        className="px-8 py-3.5 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition shadow-lg shadow-blue-600/20 flex items-center gap-2"
                                    >
                                        Devam Et
                                        <ChevronRight className="w-4 h-4" />
                                    </button>
                                </div>
                            </div>
                        )}

                        {step === 3 && (
                            <div className="space-y-8 animate-slide-in-right">
                                <div className="bg-gradient-to-br from-emerald-50 to-teal-50 border border-emerald-100 rounded-3xl p-8 text-center relative overflow-hidden">
                                    <div className="relative z-10">
                                        <div className="w-16 h-16 bg-emerald-100 text-emerald-600 rounded-full flex items-center justify-center mx-auto mb-4 shadow-inner">
                                            <Activity className="w-8 h-8" />
                                        </div>
                                        <h4 className="text-2xl font-bold text-emerald-900 mb-1">Randevu Özeti</h4>
                                        <p className="text-emerald-700/80 text-sm mb-6">Lütfen bilgileri kontrol edin</p>
                                        
                                        <div className="bg-white/60 backdrop-blur-sm rounded-2xl p-6 space-y-3 max-w-sm mx-auto shadow-sm border border-emerald-100/50">
                                            <div className="flex justify-between items-center">
                                                <span className="text-slate-500 text-sm">Doktor</span>
                                                <span className="font-bold text-slate-800">{selectedDoctor.isim} {selectedDoctor.soyisim}</span>
                                            </div>
                                            <div className="w-full h-[1px] bg-slate-200/50"></div>
                                            <div className="flex justify-between items-center">
                                                <span className="text-slate-500 text-sm">Tarih</span>
                                                <span className="font-bold text-slate-800">{new Date(selectedDate).toLocaleDateString('tr-TR')}</span>
                                            </div>
                                            <div className="w-full h-[1px] bg-slate-200/50"></div>
                                            <div className="flex justify-between items-center">
                                                <span className="text-slate-500 text-sm">Saat</span>
                                                <span className="font-bold text-slate-800">{selectedSlot}</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div>
                                    <label className="block text-sm font-bold text-slate-700 mb-3 uppercase tracking-wide">Şikayetiniz (Opsiyonel)</label>
                                    <textarea
                                        value={sikayet}
                                        onChange={(e) => setSikayet(e.target.value)}
                                        rows="3"
                                        className="w-full p-4 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none resize-none transition"
                                        placeholder="Kısaca şikayetinizi belirtebilirsiniz..."
                                    ></textarea>
                                </div>

                                <div className="pt-4 flex justify-between items-center">
                                    <button onClick={() => setStep(2)} className="px-6 py-3 text-slate-500 font-bold hover:bg-slate-100 rounded-xl transition">Geri</button>
                                    <button 
                                        onClick={submitRandevu} 
                                        className="px-10 py-4 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-xl shadow-blue-600/30 transition transform hover:-translate-y-1 active:translate-y-0"
                                    >
                                        Randevuyu Onayla
                                    </button>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        )}
        </div>
      </main>
    </div>
  );
}

export default HastaDashboard;