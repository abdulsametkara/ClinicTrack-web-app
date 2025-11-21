import { useNavigate } from 'react-router-dom';
import { LogOut, User, Users, Calendar, Activity, Clock, LayoutDashboard, Search, Menu, Trash2, Edit, Filter, X, Plus, Stethoscope, Save, Edit2, Phone, Mail, CreditCard } from 'lucide-react';
import { useState, useEffect } from 'react';
import Swal from 'sweetalert2';
import { getAllUsers, getAllAppointments, deleteUser, createUser, updateUser, getAllUzmanliklar, getAllDoktorlar, getAllHasta, createUzmanlik, updateUzmanlik, deleteUzmanlik, updateRandevuDurum, deleteRandevu, completePastAppointments, updateUserPhone } from '../api';

function AdminDashboard() {
  const navigate = useNavigate();
  const [kullaniciAd, setKullaniciAd] = useState(localStorage.getItem('kullaniciAd') || 'Admin');
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  
  // Aktif Sayfa State'i
  const [activeTab, setActiveTab] = useState('Dashboard');

  // State - Veriler
  const [stats, setStats] = useState({
    totalHasta: 0,
    totalDoktor: 0,
    activeRandevu: 0
  });
  const [sonAktiviteler, setSonAktiviteler] = useState([]);
  const [tumKullanicilar, setTumKullanicilar] = useState([]); // Tüm liste
  const [filteredUsers, setFilteredUsers] = useState([]); // Filtrelenmiş liste
  const [tumRandevular, setTumRandevular] = useState([]); // Tüm randevular
  const [yukleniyor, setYukleniyor] = useState(true);

  // Uzmanlıklar ve Doktorlar State'i
  const [uzmanliklar, setUzmanliklar] = useState([]);
  const [doktorlar, setDoktorlar] = useState([]); // Doktor tablosundan
  const [hastalar, setHastalar] = useState([]); // Hasta tablosundan

  // Kullanıcı Tablosu State'leri
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState('Hepsi');

  // Modal State'leri (Kullanıcı)
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState('create'); // 'create' veya 'edit'
  const [selectedUser, setSelectedUser] = useState(null);
  const [formData, setFormData] = useState({
    isim: '',
    soyisim: '',
    email: '',
    parola: '',
    tcNo: '',
    telefonNumarası: '',
    dogumTarihi: '',
    rol: 'Hasta',
    uzmanlikId: ''
  });

  // Modal State'leri (Uzmanlık)
  const [isUzmanlikModalOpen, setIsUzmanlikModalOpen] = useState(false);
  const [uzmanlikModalMode, setUzmanlikModalMode] = useState('create'); // 'create' veya 'edit'
  const [selectedUzmanlik, setSelectedUzmanlik] = useState(null);
  const [uzmanlikFormData, setUzmanlikFormData] = useState({
    uzmanlikAdi: ''
  });

  // Verileri Çekme Fonksiyonu
  const fetchData = async () => {
    try {
      setYukleniyor(true);
      
      // Önce geçmiş randevuları tamamla
      try {
        await completePastAppointments();
      } catch (err) {
        console.error('Geçmiş randevular tamamlanamadı:', err);
      }
      
      // Paralel istek at
      const [usersRes, appointmentsRes] = await Promise.all([
        getAllUsers(),
        getAllAppointments()
      ]);

      // Kullanıcı İstatistikleri & Listesi
      let hastaSayisi = 0;
      let doktorSayisi = 0;
      let kullaniciListesi = [];

      // Backend camelCase döndürüyor
      if (usersRes.isSuccess) {
        kullaniciListesi = usersRes.data;
        setTumKullanicilar(kullaniciListesi);
        setFilteredUsers(kullaniciListesi); // Başlangıçta hepsi

        hastaSayisi = usersRes.data.filter(u => (u.rol || u.Rol) === 'Hasta').length;
        doktorSayisi = usersRes.data.filter(u => (u.rol || u.Rol) === 'Doktor').length;
      }

      // Randevu İstatistikleri
      let randevuSayisi = 0;
      let sonRandevular = [];

      if (appointmentsRes.isSuccess) {
        const randevular = appointmentsRes.data;
        setTumRandevular(randevular); // Randevuları state'e kaydet
        randevuSayisi = randevular.filter(r => (r.durum || r.Durum) === 'Beklemede').length;

        // Önce tüm hasta kayıtlarını bir kere çek
        const hastaListesiRes = await getAllHasta();
        const hastaListesi = hastaListesiRes.data || [];
        setHastalar(hastaListesi); // State'e kaydet

        // Son 5 randevuyu al ve işle
        sonRandevular = randevular
          .sort((a, b) => new Date(b.randevuTarihi || b.RandevuTarihi) - new Date(a.randevuTarihi || a.RandevuTarihi))
          .slice(0, 5)
          .map(r => {
            // Hasta bilgisini HastaId'den bul
            try {
              const hastaId = r.hastaId || r.HastaId;
              const hastaKaydi = hastaListesi.find(h => (h.id || h.Id) === hastaId);
              
              if (hastaKaydi) {
                // Hasta kaydındaki KullaniciId ile kullanıcı bilgisini al
                const kullaniciId = hastaKaydi.kullanıcıId || hastaKaydi.KullanıcıId;
                const hasta = kullaniciListesi.find(u => (u.id || u.Id) === kullaniciId);
                
                const isim = hasta?.isim || hasta?.İsim || 'Bilinmeyen';
                const soyisim = hasta?.soyisim || hasta?.Soyisim || '';
                const hastaAd = hasta ? `${isim} ${soyisim}` : `Bilinmeyen (ID: ${kullaniciId})`;
                  
                return {
                  id: r.id || r.Id,
                  kullanici: hastaAd,
                  islem: 'Randevu Oluşturdu',
                  tarih: new Date(r.randevuTarihi || r.RandevuTarihi).toLocaleString('tr-TR'),
                  durum: r.durum || r.Durum
                };
              } else {
                return {
                  id: r.id || r.Id,
                  kullanici: 'Bilinmeyen Hasta',
                  islem: 'Randevu Oluşturdu',
                  tarih: new Date(r.randevuTarihi || r.RandevuTarihi).toLocaleString('tr-TR'),
                  durum: r.durum || r.Durum
                };
              }
            } catch (err) {
              console.error('Hasta bilgisi alınamadı:', err);
              return {
                id: r.id || r.Id,
                kullanici: 'Hata',
                islem: 'Randevu Oluşturdu',
                tarih: new Date(r.randevuTarihi || r.RandevuTarihi).toLocaleString('tr-TR'),
                durum: r.durum || r.Durum
              };
            }
          });
      }

      setStats({
        totalHasta: hastaSayisi,
        totalDoktor: doktorSayisi,
        activeRandevu: randevuSayisi
      });
      setSonAktiviteler(sonRandevular);

    } catch (error) {
      console.error("Veri çekme hatası:", error);
    } finally {
      setYukleniyor(false);
    }
  };

  // Uzmanlıkları Çek
  const fetchUzmanliklar = async () => {
    try {
      const response = await getAllUzmanliklar();
      if (response.isSuccess) {
        setUzmanliklar(response.data);
      }
    } catch (error) {
      console.error('Uzmanlıklar çekilemedi:', error);
    }
  };

  // Doktorları Çek
  const fetchDoktorlar = async () => {
    try {
      const response = await getAllDoktorlar();
      if (response && response.isSuccess) {
        setDoktorlar(response.data);
      } else {
        setDoktorlar([]);
      }
    } catch (error) {
      console.error('Doktorlar çekilemedi:', error);
      setDoktorlar([]);
    }
  };

  // Sayfa yüklendiğinde verileri çek
  useEffect(() => {
    fetchData();
    fetchUzmanliklar();
    fetchDoktorlar();
  }, []);

  // Arama veya Filtreleme değiştiğinde listeyi güncelle
  useEffect(() => {
    let result = tumKullanicilar;

    // Rol Filtreleme
    if (roleFilter !== 'Hepsi') {
      result = result.filter(user => (user.rol || user.Rol) === roleFilter);
    }

    // Arama (İsim, Soyisim, Email, TC)
    if (searchTerm) {
      const lowerTerm = searchTerm.toLowerCase();
      result = result.filter(user => 
        ((user.isim || user.İsim) || '').toLowerCase().includes(lowerTerm) ||
        ((user.soyisim || user.Soyisim) || '').toLowerCase().includes(lowerTerm) ||
        ((user.email || user.Email) || '').toLowerCase().includes(lowerTerm) ||
        ((user.tcNo || user.TCNo) || '').includes(lowerTerm)
      );
    }

    setFilteredUsers(result);
  }, [searchTerm, roleFilter, tumKullanicilar]);

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const handleDeleteUser = async (id) => {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu kullanıcıyı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          const response = await deleteUser(id);
          if (response.isSuccess) {
            Swal.fire('Silindi!', 'Kullanıcı başarıyla silindi.', 'success');
            fetchData(); 
          } else {
            Swal.fire('Hata', response.message, 'error');
          }
        } catch (error) {
          Swal.fire('Hata', 'Silme işlemi sırasında bir hata oluştu.', 'error');
        }
      }
    });
  };

  // Modal Açma - Yeni Kullanıcı
  const handleOpenCreateModal = () => {
    setModalMode('create');
    setSelectedUser(null);
    setFormData({
      isim: '',
      soyisim: '',
      email: '',
      parola: '',
      tcNo: '',
      telefonNumarası: '',
      dogumTarihi: '',
      rol: 'Hasta',
      uzmanlikId: ''
    });
    setIsModalOpen(true);
  };

  // Modal Açma - Kullanıcı Düzenle
  const handleOpenEditModal = (user) => {
    setModalMode('edit');
    setSelectedUser(user);
    setFormData({
      isim: user.isim || user.İsim || '',
      soyisim: user.soyisim || user.Soyisim || '',
      email: user.email || user.Email || '',
      parola: '', // Şifre değiştirilmeyecekse boş
      tcNo: user.tcNo || user.TCNo || '',
      telefonNumarası: user.telefonNumarası || user.TelefonNumarası || '',
      dogumTarihi: user.dogumTarihi || user.DoğumTarihi || '',
      rol: user.rol || user.Rol || 'Hasta',
      uzmanlikId: user.uzmanlikId || user.UzmanlıkId || ''
    });
    setIsModalOpen(true);
  };

  // Modal Kapatma
  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedUser(null);
    setFormData({
      isim: '',
      soyisim: '',
      email: '',
      parola: '',
      tcNo: '',
      telefonNumarası: '',
      dogumTarihi: '',
      rol: 'Hasta',
      uzmanlikId: ''
    });
  };

  // UZMANLIK YÖNETİMİ FONKSİYONLARI
  const handleOpenCreateUzmanlikModal = () => {
    setUzmanlikModalMode('create');
    setSelectedUzmanlik(null);
    setUzmanlikFormData({ uzmanlikAdi: '' });
    setIsUzmanlikModalOpen(true);
  };

  const handleOpenEditUzmanlikModal = (uzmanlik) => {
    setUzmanlikModalMode('edit');
    setSelectedUzmanlik(uzmanlik);
    setUzmanlikFormData({
      uzmanlikAdi: uzmanlik.uzmanlıkAdı || uzmanlik.UzmanlıkAdı || ''
    });
    setIsUzmanlikModalOpen(true);
  };

  const handleCloseUzmanlikModal = () => {
    setIsUzmanlikModalOpen(false);
    setSelectedUzmanlik(null);
    setUzmanlikFormData({ uzmanlikAdi: '' });
  };

  const handleSubmitUzmanlik = async (e) => {
    e.preventDefault();
    
    try {
      if (uzmanlikModalMode === 'create') {
        const response = await createUzmanlik(uzmanlikFormData.uzmanlikAdi);
        if (response.isSuccess) {
          Swal.fire('Başarılı', 'Uzmanlık başarıyla oluşturuldu!', 'success');
          handleCloseUzmanlikModal();
          fetchUzmanliklar();
        } else {
          Swal.fire('Hata', response.message, 'error');
        }
      } else {
        const uzmanlikId = selectedUzmanlik.id || selectedUzmanlik.Id;
        const response = await updateUzmanlik(uzmanlikId, uzmanlikFormData.uzmanlikAdi);
        if (response.isSuccess) {
          Swal.fire('Başarılı', 'Uzmanlık başarıyla güncellendi!', 'success');
          handleCloseUzmanlikModal();
          fetchUzmanliklar();
        } else {
          Swal.fire('Hata', response.message, 'error');
        }
      }
    } catch (error) {
      console.error('Uzmanlık işlemi hatası:', error);
      Swal.fire('Hata', 'İşlem sırasında bir hata oluştu.', 'error');
    }
  };

  const handleDeleteUzmanlik = async (id) => {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu uzmanlığı silmek istediğinize emin misiniz? Bu uzmanlığa bağlı doktorlar etkilenebilir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          const response = await deleteUzmanlik(id);
          if (response.isSuccess) {
            Swal.fire('Silindi!', 'Uzmanlık başarıyla silindi.', 'success');
            fetchUzmanliklar();
            fetchData(); // Doktorların uzmanlık bilgilerini güncelle
          } else {
            Swal.fire('Hata', response.message, 'error');
          }
        } catch (error) {
          Swal.fire('Hata', 'Silme işlemi sırasında bir hata oluştu.', 'error');
        }
      }
    });
  };

  // RANDEVU YÖNETİMİ FONKSİYONLARI
  const handleUpdateRandevuDurum = async (randevuId, yeniDurum) => {
    try {
      const response = await updateRandevuDurum(randevuId, yeniDurum);
      if (response.isSuccess) {
        Swal.fire({
            icon: 'success',
            title: 'Güncellendi',
            text: `Randevu durumu "${yeniDurum}" olarak güncellendi.`,
            timer: 1500,
            showConfirmButton: false
        });
        fetchData(); // Randevuları yeniden çek
      } else {
        Swal.fire('Hata', response.message, 'error');
      }
    } catch (error) {
      console.error('Randevu güncelleme hatası:', error);
      Swal.fire('Hata', 'Güncelleme sırasında bir hata oluştu.', 'error');
    }
  };

  const handleDeleteRandevu = async (id) => {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu randevuyu silmek istediğinize emin misiniz?",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          const response = await deleteRandevu(id);
          if (response.isSuccess) {
            Swal.fire('Silindi!', 'Randevu başarıyla silindi.', 'success');
            fetchData(); // Randevuları yeniden çek
          } else {
            Swal.fire('Hata', response.message, 'error');
          }
        } catch (error) {
          Swal.fire('Hata', 'Silme işlemi sırasında bir hata oluştu.', 'error');
        }
      }
    });
  };

  // Form Submit - Yeni Kullanıcı veya Güncelleme
  const handleSubmitUser = async (e) => {
    e.preventDefault();
    
    try {
      // Doktor için uzmanlık alanı kontrolü
      if (formData.rol === 'Doktor' && !formData.uzmanlikId) {
        Swal.fire('Uyarı', 'Doktor için uzmanlık alanı seçilmelidir!', 'warning');
        return;
      }

      // Backend'e gönderilecek data (camelCase)
      const userData = {
        isim: formData.isim,
        soyisim: formData.soyisim,
        email: formData.email,
        parola: formData.parola || undefined, // Boşsa gönderme
        tcNo: formData.tcNo,
        telefonNumarası: formData.telefonNumarası || '',
        doğumTarihi: formData.dogumTarihi || null,
        rol: formData.rol
      };
      
      // Doktor ise uzmanlıkId ekle
      if (formData.rol === 'Doktor' && formData.uzmanlikId) {
        userData.uzmanlıkId = parseInt(formData.uzmanlikId);
      }

      // Parola boşsa ve edit mode'daysa, parola alanını kaldır
      if (modalMode === 'edit' && !userData.parola) {
        delete userData.parola;
      }
      
      if (modalMode === 'create') {
        const response = await createUser(userData);
        if (response.isSuccess) {
          Swal.fire('Başarılı', 'Kullanıcı başarıyla oluşturuldu!', 'success');
          handleCloseModal();
          fetchData();
        } else {
          Swal.fire('Hata', response.message, 'error');
        }
      } else {
        const userId = selectedUser.id || selectedUser.Id;
        const response = await updateUser(userId, userData);
        if (response.isSuccess) {
          Swal.fire('Başarılı', 'Kullanıcı başarıyla güncellendi!', 'success');
          handleCloseModal();
          fetchData();
        } else {
          Swal.fire('Hata', response.message, 'error');
        }
      }
    } catch (error) {
      console.error('Kullanıcı işlemi hatası:', error);
      Swal.fire('Hata', 'İşlem sırasında bir hata oluştu.', 'error');
    }
  };

  const currentDate = new Date().toLocaleDateString('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    weekday: 'long'
  });

  // Menü öğesi bileşeni
  const MenuItem = ({ icon: Icon, label, tabName }) => (
    <button
      onClick={() => setActiveTab(tabName)}
      className={`w-full flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all font-medium ${
        activeTab === tabName 
          ? 'bg-blue-600 text-white shadow-lg shadow-blue-900/20' 
          : 'text-slate-400 hover:bg-slate-800 hover:text-white'
      }`}
    >
      <Icon className="w-5 h-5" />
      <span className="font-medium">{label}</span>
    </button>
  );

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
          <p className="px-4 text-xs font-bold text-slate-400 uppercase tracking-wider mb-4">Yönetim Paneli</p>
          <MenuItem icon={LayoutDashboard} label="Dashboard" tabName="Dashboard" />
          <MenuItem icon={Users} label="Kullanıcılar" tabName="Kullanıcılar" />
          <MenuItem icon={Activity} label="Doktorlar" tabName="Doktorlar" />
          <MenuItem icon={Calendar} label="Randevular" tabName="Randevular" />
          
          <div className="mt-8 px-4">
            {/* Sistem ayracı kaldırıldı */}
          </div>
          <MenuItem icon={Stethoscope} label="Uzmanlıklar" tabName="Uzmanlıklar" />
          {/* Ayarlar menüsü kaldırıldı */}
        </nav>

        <div className="absolute bottom-0 w-full p-6 border-t border-slate-100">
          <button onClick={handleLogout} className="flex items-center gap-3 text-slate-500 hover:text-red-600 transition-colors w-full px-2 font-medium text-sm">
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

        {/* CONTENT AREA */}
        <div className="p-6 md:p-10 max-w-7xl mx-auto w-full">
          {/* Dashboard İçeriği */}
          {activeTab === 'Dashboard' && (
            yukleniyor ? (
              <div className="flex items-center justify-center h-64 text-slate-500">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mr-3"></div>
                Veriler yükleniyor...
              </div>
            ) : (
              <div className="space-y-8 animate-fade-in">
                {/* Hero Section */}
                <div className="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-3xl p-8 md:p-12 text-white shadow-xl shadow-blue-900/20 relative overflow-hidden">
                    <div className="absolute top-0 right-0 w-64 h-64 bg-white opacity-5 rounded-full -translate-y-1/2 translate-x-1/3 blur-3xl"></div>
                    <div className="absolute bottom-0 left-0 w-48 h-48 bg-indigo-400 opacity-10 rounded-full translate-y-1/3 -translate-x-1/4 blur-2xl"></div>
                    
                    <div className="relative z-10 flex items-center justify-between">
                        <div>
                            <p className="text-blue-100 font-medium mb-2 flex items-center gap-2">
                                <Calendar className="w-4 h-4" /> {currentDate}
                            </p>
                            <h1 className="text-3xl md:text-4xl font-bold mb-2">Hoş Geldiniz, {kullaniciAd}</h1>
                            <p className="text-blue-100 max-w-md">Sistem genel durumu ve istatistikleri aşağıda özetlenmiştir.</p>
                        </div>
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                  {[
                      { label: 'Toplam Hasta', value: stats.totalHasta, icon: Users, color: 'blue' },
                      { label: 'Toplam Doktor', value: stats.totalDoktor, icon: Activity, color: 'purple' },
                      { label: 'Aktif Randevular', value: stats.activeRandevu, icon: Calendar, color: 'green' }
                  ].map((stat, idx) => (
                      <div key={idx} className="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center justify-between hover:shadow-md transition-all hover:-translate-y-1">
                          <div>
                              <p className="text-slate-500 text-sm font-medium mb-1">{stat.label}</p>
                              <h3 className="text-4xl font-bold text-slate-800">{stat.value}</h3>
                          </div>
                          <div className={`p-4 bg-${stat.color}-50 rounded-2xl border border-${stat.color}-100 text-${stat.color}-600`}>
                              <stat.icon className="w-8 h-8" />
                          </div>
                      </div>
                  ))}
                </div>

                <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
                    <div className="p-6 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
                      <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                        <Activity className="w-5 h-5 text-blue-600" />
                        Son Aktiviteler
                      </h3>
                    </div>
                    
                    <div className="overflow-x-auto">
                      <table className="w-full">
                        <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                          <tr>
                            <th className="px-6 py-4 text-left">Kullanıcı / Hasta</th>
                            <th className="px-6 py-4 text-left">İşlem</th>
                            <th className="px-6 py-4 text-left">Tarih</th>
                            <th className="px-6 py-4 text-left">Durum</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100">
                          {sonAktiviteler.length > 0 ? (
                            sonAktiviteler.map((item, index) => (
                              <tr key={index} className="hover:bg-slate-50 transition">
                                <td className="px-6 py-4 font-bold text-slate-700 flex items-center gap-3">
                                  <div className="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-slate-500 text-sm font-bold">
                                    {item.kullanici.charAt(0)}
                                  </div>
                                  {item.kullanici}
                                </td>
                                <td className="px-6 py-4 text-slate-600 text-sm">{item.islem}</td>
                                <td className="px-6 py-4 text-slate-500 text-sm font-medium">{item.tarih}</td>
                                <td className="px-6 py-4">
                                  <span className={`px-3 py-1 rounded-full text-xs font-bold ${
                                    item.durum === 'Beklemede' ? 'bg-orange-50 text-orange-700' :
                                    item.durum === 'Tamamlandı' ? 'bg-emerald-50 text-emerald-700' :
                                    item.durum === 'İptal' ? 'bg-red-50 text-red-700' :
                                    'bg-slate-100 text-slate-600'
                                  }`}>
                                    {item.durum}
                                  </span>
                                </td>
                              </tr>
                            ))
                          ) : (
                            <tr>
                              <td colSpan="4" className="px-6 py-12 text-center text-slate-400">Henüz bir aktivite bulunmuyor.</td>
                            </tr>
                          )}
                        </tbody>
                      </table>
                    </div>
                  </div>
              </div>
            )
          )}

          {/* KULLANICILAR TABLOSU */}
          {activeTab === 'Kullanıcılar' && (
            <div className="space-y-6 animate-fade-in">
              {/* Üst Araç Çubuğu */}
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white p-5 rounded-2xl border border-slate-100 shadow-sm">
                <div className="relative flex-1 max-w-md">
                  <Search className="w-5 h-5 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
                  <input 
                    type="text" 
                    placeholder="İsim, email veya TC ile ara..." 
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="w-full pl-10 pr-4 py-2.5 border border-slate-200 rounded-xl text-sm focus:ring-2 focus:ring-blue-500 outline-none transition"
                  />
                </div>

                <div className="flex items-center gap-3">
                  <div className="flex items-center gap-2">
                    <select 
                      value={roleFilter}
                      onChange={(e) => setRoleFilter(e.target.value)}
                      className="bg-white border border-slate-200 text-slate-700 text-sm rounded-xl focus:ring-2 focus:ring-blue-500 block p-2.5 outline-none cursor-pointer font-medium"
                    >
                      <option value="Hepsi">Tüm Roller</option>
                      <option value="Hasta">Hastalar</option>
                      <option value="Doktor">Doktorlar</option>
                      <option value="Admin">Adminler</option>
                    </select>
                  </div>

                  <button 
                    onClick={handleOpenCreateModal}
                    className="px-5 py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-bold text-sm flex items-center gap-2 shadow-lg shadow-blue-200"
                  >
                    <Plus className="w-4 h-4" />
                    Kullanıcı Ekle
                  </button>
                </div>
              </div>

              {/* Tablo */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">Kullanıcı</th>
                        <th className="px-6 py-4 text-left">İletişim</th>
                        <th className="px-6 py-4 text-left">Rol</th>
                        <th className="px-6 py-4 text-left">Kayıt Tarihi</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {filteredUsers.length > 0 ? (
                        filteredUsers.map((user) => (
                          <tr key={user.id || user.Id} className="hover:bg-slate-50 transition group">
                            <td className="px-6 py-4">
                              <div className="flex items-center gap-3">
                                <div className="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold shadow-sm">
                                  {(user.isim || user.İsim)?.charAt(0)}
                                </div>
                                <div>
                                  <div className="font-bold text-slate-800">{(user.isim || user.İsim)} {(user.soyisim || user.Soyisim)}</div>
                                  <div className="text-xs text-slate-500 font-medium bg-slate-100 px-2 py-0.5 rounded inline-block mt-1">TC: {user.tcNo || user.TCNo}</div>
                                </div>
                              </div>
                            </td>
                            <td className="px-6 py-4">
                              <div className="text-slate-700 font-medium text-sm">{user.email || user.Email}</div>
                              <div className="text-xs text-slate-400 mt-0.5">{user.telefonNumarası || user.TelefonNumarası || '-'}</div>
                            </td>
                            <td className="px-6 py-4">
                              <span className={`px-3 py-1 rounded-full text-xs font-bold border ${
                                (user.rol || user.Rol) === 'Admin' ? 'bg-purple-50 text-purple-700 border-purple-200' :
                                (user.rol || user.Rol) === 'Doktor' ? 'bg-emerald-50 text-emerald-700 border-emerald-200' :
                                'bg-blue-50 text-blue-700 border-blue-200'
                              }`}>
                                {user.rol || user.Rol}
                              </span>
                            </td>
                            <td className="px-6 py-4 text-slate-500 text-sm font-medium">
                              {new Date(user.oluşturulmaTarihi || user.OluşturulmaTarihi || Date.now()).toLocaleDateString('tr-TR')}
                            </td>
                            <td className="px-6 py-4 text-right">
                              <div className="flex items-center justify-end gap-2">
                                <button 
                                  onClick={() => handleOpenEditModal(user)}
                                  className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                                  title="Düzenle"
                                >
                                  <Edit2 className="w-4 h-4" />
                                </button>
                                <button 
                                  onClick={() => handleDeleteUser(user.id || user.Id)}
                                  className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                  title="Sil"
                                >
                                  <Trash2 className="w-4 h-4" />
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))
                      ) : (
                        <tr>
                          <td colSpan="5" className="px-6 py-12 text-center text-slate-400">
                            <Users className="w-12 h-12 mx-auto mb-3 opacity-20" />
                            <p>Aradığınız kriterlere uygun kullanıcı bulunamadı.</p>
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          )}

          {/* DOKTORLAR SAYFASI */}
          {activeTab === 'Doktorlar' && (
            <div className="space-y-6 animate-fade-in">
              {/* Doktor İstatistikleri */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Doktor</p>
                      <h3 className="text-3xl font-bold text-slate-800">
                        {tumKullanicilar.filter(u => (u.rol || u.Rol) === 'Doktor').length}
                      </h3>
                    </div>
                    <div className="p-4 bg-emerald-50 rounded-2xl border border-emerald-100 text-emerald-600">
                      <Stethoscope className="w-8 h-8" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Aktif Doktorlar</p>
                      <h3 className="text-3xl font-bold text-slate-800">
                        {tumKullanicilar.filter(u => (u.rol || u.Rol) === 'Doktor').length}
                      </h3>
                    </div>
                    <div className="p-4 bg-blue-50 rounded-2xl border border-blue-100 text-blue-600">
                      <Activity className="w-8 h-8" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Randevu</p>
                      <h3 className="text-3xl font-bold text-slate-800">{stats.activeRandevu}</h3>
                    </div>
                    <div className="p-4 bg-purple-50 rounded-2xl border border-purple-100 text-purple-600">
                      <Calendar className="w-8 h-8" />
                    </div>
                  </div>
                </div>
              </div>

              {/* Doktor Listesi Tablosu */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
                <div className="p-6 border-b border-slate-100 bg-slate-50/50">
                  <h3 className="font-bold text-xl text-slate-800 flex items-center gap-2">
                    <Stethoscope className="w-5 h-5 text-emerald-600" />
                    Doktor Listesi ve İstatistikleri
                  </h3>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">Doktor</th>
                        <th className="px-6 py-4 text-left">Uzmanlık</th>
                        <th className="px-6 py-4 text-left">ILETISIM</th>
                        <th className="px-6 py-4 text-center">Randevu Sayısı</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {tumKullanicilar
                        .filter(u => (u.rol || u.Rol) === 'Doktor')
                        .map((kullaniciDoktor) => {
                          const kullaniciId = kullaniciDoktor.id || kullaniciDoktor.Id;
                          const doktorBilgisi = doktorlar.find(d => (d.kullanıcıId || d.KullanıcıId) == kullaniciId);
                          const doktorId = doktorBilgisi?.id || doktorBilgisi?.Id;
                          const doktorUzmanlikId = doktorBilgisi?.uzmanlıkId || doktorBilgisi?.UzmanlıkId;
                          const doktorUzmanlik = uzmanliklar.find(uz => (uz.id || uz.Id) === doktorUzmanlikId);
                          const randevuSayisi = doktorId ? tumRandevular.filter(r => (r.doktorId || r.DoktorId) === doktorId).length : 0;
                          
                          return (
                            <tr key={kullaniciDoktor.id || kullaniciDoktor.Id} className="hover:bg-slate-50 transition group">
                              <td className="px-6 py-4">
                                <div className="flex items-center gap-3">
                                  <div className="w-10 h-10 rounded-full bg-emerald-100 flex items-center justify-center text-emerald-700 font-bold shadow-sm">
                                    {(kullaniciDoktor.isim || kullaniciDoktor.İsim)?.charAt(0)}
                                  </div>
                                  <div>
                                    <div className="font-bold text-slate-800 text-sm">
                                      Dr. {(kullaniciDoktor.isim || kullaniciDoktor.İsim)} {(kullaniciDoktor.soyisim || kullaniciDoktor.Soyisim)}
                                    </div>
                                    <div className="text-xs text-slate-500 font-medium bg-slate-100 px-2 py-0.5 rounded inline-block mt-1">TC: {kullaniciDoktor.tcNo || kullaniciDoktor.TCNo}</div>
                                  </div>
                                </div>
                              </td>
                              <td className="px-6 py-4">
                                <span className="px-3 py-1 rounded-full text-xs font-bold bg-emerald-50 text-emerald-700 border border-emerald-200">
                                  {doktorUzmanlik?.uzmanlıkAdı || doktorUzmanlik?.UzmanlıkAdı || 'Belirsiz'}
                                </span>
                              </td>
                              <td className="px-6 py-4">
                                <div className="text-slate-700 text-sm font-medium">{kullaniciDoktor.email || kullaniciDoktor.Email}</div>
                                <div className="text-xs text-slate-400 mt-0.5">{kullaniciDoktor.telefonNumarası || kullaniciDoktor.TelefonNumarası || '-'}</div>
                              </td>
                              <td className="px-6 py-4 text-center">
                                <div className="inline-flex items-center gap-2 px-3 py-1 bg-blue-50 rounded-lg border border-blue-100">
                                  <Calendar className="w-3 h-3 text-blue-600" />
                                  <span className="font-bold text-blue-700 text-sm">{randevuSayisi}</span>
                                </div>
                              </td>
                              <td className="px-6 py-4 text-right">
                                <div className="flex items-center justify-end gap-2">
                                  <button 
                                    onClick={() => handleOpenEditModal(kullaniciDoktor)}
                                    className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                                    title="Düzenle"
                                  >
                                    <Edit2 className="w-4 h-4" />
                                  </button>
                                  <button 
                                    onClick={() => handleDeleteUser(kullaniciDoktor.id || kullaniciDoktor.Id)}
                                    className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                    title="Sil"
                                  >
                                    <Trash2 className="w-4 h-4" />
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })}
                      {tumKullanicilar.filter(u => (u.rol || u.Rol) === 'Doktor').length === 0 && (
                        <tr>
                          <td colSpan="5" className="px-6 py-12 text-center text-slate-400">
                            <Stethoscope className="w-12 h-12 mx-auto mb-3 opacity-20" />
                            <p>Henüz kayıtlı doktor bulunmuyor.</p>
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          )}

          {/* RANDEVULAR SAYFASI */}
          {activeTab === 'Randevular' && (
            <div className="space-y-6 animate-fade-in">
              {/* Üst İstatistikler */}
              <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                {[
                    { label: 'Toplam Randevu', value: tumRandevular.length, icon: Calendar, color: 'blue' },
                    { label: 'Beklemede', value: tumRandevular.filter(r => (r.durum || r.Durum) === 'Beklemede').length, icon: Clock, color: 'orange' },
                    { label: 'Tamamlandı', value: tumRandevular.filter(r => (r.durum || r.Durum) === 'Tamamlandı').length, icon: Activity, color: 'green' },
                    { label: 'İptal', value: tumRandevular.filter(r => (r.durum || r.Durum) === 'İptal').length, icon: X, color: 'red' }
                ].map((stat, idx) => (
                    <div key={idx} className="bg-white p-5 rounded-2xl shadow-sm border border-slate-100 flex items-center justify-between hover:shadow-md transition">
                        <div>
                            <p className="text-slate-500 text-xs font-bold uppercase tracking-wider mb-1">{stat.label}</p>
                            <h3 className={`text-2xl font-bold text-${stat.color}-600`}>{stat.value}</h3>
                        </div>
                        <div className={`p-3 bg-${stat.color}-50 rounded-xl text-${stat.color}-600`}>
                            <stat.icon className="w-6 h-6" />
                        </div>
                    </div>
                ))}
              </div>

              {/* Randevu Listesi */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
                <div className="p-6 border-b border-slate-100 bg-slate-50/50">
                  <h3 className="font-bold text-xl text-slate-800 flex items-center gap-2">
                    <Calendar className="w-5 h-5 text-blue-600" />
                    Tüm Randevular
                  </h3>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">ID</th>
                        <th className="px-6 py-4 text-left">Hasta</th>
                        <th className="px-6 py-4 text-left">Doktor</th>
                        <th className="px-6 py-4 text-left">Tarih & Saat</th>
                        <th className="px-6 py-4 text-left">Şikayet</th>
                        <th className="px-6 py-4 text-left">Durum</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {tumRandevular.length > 0 ? (
                        tumRandevular.map((randevu) => {
                          // Robust ID retrieval
                          const randevuId = randevu.id || randevu.Id;
                          const hastaId = randevu.hastaId || randevu.HastaId;
                          const doktorId = randevu.doktorId || randevu.DoktorId;
                          
                          // Find patient record
                          const hastaKaydi = hastalar.find(h => (h.id || h.Id) === hastaId);
                          const hastaKullaniciId = hastaKaydi?.kullanıcıId || hastaKaydi?.KullanıcıId;
                          const hasta = tumKullanicilar.find(u => (u.id || u.Id) === hastaKullaniciId);
                          
                          // Find doctor record
                          const doktorBilgisi = doktorlar.find(d => (d.id || d.Id) === doktorId);
                          const doktorKullaniciId = doktorBilgisi?.kullanıcıId || doktorBilgisi?.KullanıcıId;
                          const doktorKullanici = tumKullanicilar.find(u => (u.id || u.Id) === doktorKullaniciId);
                          
                          const randevuDurum = randevu.durum || randevu.Durum;
                          const randevuTarihi = randevu.randevuTarihi || randevu.RandevuTarihi;
                          const sikayet = randevu.sikayet || randevu.Sikayet;

                          // Helper for names
                          const hastaAd = hasta ? `${hasta.isim || hasta.İsim || 'Bilinmiyor'} ${hasta.soyisim || hasta.Soyisim || ''}` : 'Bilinmiyor';
                          const hastaTC = hasta?.tcNo || hasta?.TCNo || '-';
                          const doktorAd = doktorKullanici ? `Dr. ${doktorKullanici.isim || doktorKullanici.İsim || ''} ${doktorKullanici.soyisim || doktorKullanici.Soyisim || ''}` : 'Bilinmiyor';

                          return (
                            <tr key={randevuId} className="hover:bg-slate-50 transition group">
                              <td className="px-6 py-4 font-bold text-slate-500 text-sm">
                                #{randevuId}
                              </td>
                              <td className="px-6 py-4">
                                <div className="font-bold text-slate-800 text-sm">
                                  {hastaAd}
                                </div>
                                <div className="text-xs text-slate-500 font-medium">TC: {hastaTC}</div>
                              </td>
                              <td className="px-6 py-4">
                                <div className="font-medium text-slate-700 text-sm">
                                  {doktorAd}
                                </div>
                              </td>
                              <td className="px-6 py-4">
                                <div className="text-slate-700 font-bold text-sm">
                                  {new Date(randevuTarihi).toLocaleDateString('tr-TR')}
                                </div>
                                <div className="text-xs text-slate-500 font-medium mt-0.5">
                                  {new Date(randevuTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })}
                                </div>
                              </td>
                              <td className="px-6 py-4">
                                <div className="text-slate-600 text-sm max-w-xs truncate font-medium">
                                  {sikayet || '-'}
                                </div>
                              </td>
                              <td className="px-6 py-4">
                                <select
                                  value={randevuDurum}
                                  onChange={(e) => handleUpdateRandevuDurum(randevuId, e.target.value)}
                                  className={`px-3 py-1.5 rounded-lg text-xs font-bold border outline-none cursor-pointer transition ${
                                    randevuDurum === 'Beklemede' ? 'bg-orange-50 text-orange-700 border-orange-200' :
                                    randevuDurum === 'Tamamlandı' ? 'bg-emerald-50 text-emerald-700 border-emerald-200' :
                                    'bg-red-50 text-red-700 border-red-200'
                                  }`}
                                >
                                  <option value="Beklemede">Beklemede</option>
                                  <option value="Tamamlandı">Tamamlandı</option>
                                  <option value="İptal">İptal</option>
                                </select>
                              </td>
                              <td className="px-6 py-4 text-right">
                                <button 
                                  onClick={() => handleDeleteRandevu(randevuId)}
                                  className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                  title="Sil"
                                >
                                  <Trash2 className="w-4 h-4" />
                                </button>
                              </td>
                            </tr>
                          );
                        })
                      ) : (
                        <tr>
                          <td colSpan="7" className="px-6 py-12 text-center text-slate-400">
                            <Calendar className="w-12 h-12 mx-auto mb-3 opacity-20" />
                            <p>Henüz kayıtlı randevu bulunmuyor.</p>
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          )}

          {/* UZMANLIKLAR SAYFASI */}
          {activeTab === 'Uzmanlıklar' && (
            <div className="space-y-6 animate-fade-in">
              {/* Üst Başlık ve Buton */}
              <div className="flex items-center justify-between bg-white p-5 rounded-2xl border border-slate-100 shadow-sm">
                <div>
                  <h2 className="text-xl font-bold text-slate-800">Uzmanlık Alanları</h2>
                  <p className="text-slate-500 text-sm mt-1">Sistemdeki tıbbi uzmanlıkları yönetin</p>
                </div>
                <button 
                  onClick={handleOpenCreateUzmanlikModal}
                  className="px-5 py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-bold text-sm flex items-center gap-2 shadow-lg shadow-blue-200"
                >
                  <Plus className="w-4 h-4" />
                  Uzmanlık Ekle
                </button>
              </div>

              {/* Uzmanlık Listesi */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
                <div className="p-6 border-b border-slate-100 bg-slate-50/50">
                  <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                    <Stethoscope className="w-5 h-5 text-blue-600" />
                    Kayıtlı Uzmanlıklar
                  </h3>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-xs uppercase font-bold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">ID</th>
                        <th className="px-6 py-4 text-left">Uzmanlık Adı</th>
                        <th className="px-6 py-4 text-left">Kayıt Tarihi</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {uzmanliklar.length > 0 ? (
                        uzmanliklar.map((uzmanlik) => (
                          <tr key={uzmanlik.id || uzmanlik.Id} className="hover:bg-slate-50 transition group">
                            <td className="px-6 py-4 font-bold text-slate-500 text-sm">
                              #{uzmanlik.id || uzmanlik.Id}
                            </td>
                            <td className="px-6 py-4">
                              <span className="px-4 py-2 rounded-lg text-sm font-bold bg-blue-50 text-blue-700 border border-blue-100">
                                {uzmanlik.uzmanlıkAdı || uzmanlik.UzmanlıkAdı}
                              </span>
                            </td>
                            <td className="px-6 py-4 text-slate-500 text-sm font-medium">
                              {new Date(uzmanlik.recordDate || uzmanlik.RecordDate || Date.now()).toLocaleDateString('tr-TR')}
                            </td>
                            <td className="px-6 py-4 text-right">
                              <div className="flex items-center justify-end gap-2">
                                <button 
                                  onClick={() => handleOpenEditUzmanlikModal(uzmanlik)}
                                  className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                                  title="Düzenle"
                                >
                                  <Edit2 className="w-4 h-4" />
                                </button>
                                <button 
                                  onClick={() => handleDeleteUzmanlik(uzmanlik.id || uzmanlik.Id)}
                                  className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                  title="Sil"
                                >
                                  <Trash2 className="w-4 h-4" />
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))
                      ) : (
                        <tr>
                          <td colSpan="4" className="px-6 py-12 text-center text-slate-400">
                            <Stethoscope className="w-12 h-12 mx-auto mb-3 opacity-20" />
                            <p>Henüz kayıtlı uzmanlık alanı bulunmuyor.</p>
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          )}
        </div>
      </main>

      {/* MODAL - Kullanıcı Ekle/Düzenle */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto animate-scale-in">
            {/* Modal Header */}
            <div className="sticky top-0 bg-white border-b border-slate-100 px-6 py-4 flex items-center justify-between z-10">
              <h2 className="text-xl font-bold text-slate-800 flex items-center gap-2">
                {modalMode === 'create' ? <Plus className="w-5 h-5 text-blue-600" /> : <Edit2 className="w-5 h-5 text-blue-600" />}
                {modalMode === 'create' ? 'Yeni Kullanıcı Ekle' : 'Kullanıcı Düzenle'}
              </h2>
              <button 
                onClick={handleCloseModal}
                className="p-2 hover:bg-slate-100 rounded-lg transition"
              >
                <X className="w-5 h-5 text-slate-500" />
              </button>
            </div>

            {/* Modal Body - Form */}
            <form onSubmit={handleSubmitUser} className="p-6 space-y-5">
              {/* Rol Seçimi */}
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">
                  Kullanıcı Rolü <span className="text-red-500">*</span>
                </label>
                <select
                  value={formData.rol}
                  onChange={(e) => setFormData({ ...formData, rol: e.target.value, uzmanlikId: '' })}
                  className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium text-slate-700 bg-slate-50/50"
                  required
                >
                  <option value="Hasta">Hasta</option>
                  <option value="Doktor">Doktor</option>
                  <option value="Admin">Admin</option>
                </select>
              </div>

              {/* Uzmanlık Seçimi (Sadece Doktor için) */}
              {formData.rol === 'Doktor' && (
                <div className="animate-fade-in">
                  <label className="block text-sm font-bold text-slate-700 mb-2">
                    Uzmanlık Alanı <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={formData.uzmanlikId}
                    onChange={(e) => setFormData({ ...formData, uzmanlikId: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium text-slate-700 bg-slate-50/50"
                    required
                  >
                    <option value="">Uzmanlık Seçiniz</option>
                    {uzmanliklar.map((uzmanlik) => (
                      <option key={uzmanlik.id || uzmanlik.Id} value={uzmanlik.id || uzmanlik.Id}>
                        {uzmanlik.uzmanlıkAdı || uzmanlik.UzmanlıkAdı}
                      </option>
                    ))}
                  </select>
                </div>
              )}

              {/* İsim ve Soyisim */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">
                    İsim <span className="text-red-500">*</span>
                  </label>
                  <div className="relative">
                    <User className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
                    <input
                        type="text"
                        value={formData.isim}
                        onChange={(e) => setFormData({ ...formData, isim: e.target.value })}
                        className="w-full pl-10 pr-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                        placeholder="Ad"
                        required
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">
                    Soyisim <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={formData.soyisim}
                    onChange={(e) => setFormData({ ...formData, soyisim: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                    placeholder="Soyad"
                    required
                  />
                </div>
              </div>

              {/* Email */}
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">
                  Email <span className="text-red-500">*</span>
                </label>
                <div className="relative">
                    <Mail className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
                    <input
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    className="w-full pl-10 pr-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                    placeholder="ornek@email.com"
                    required
                    />
                </div>
              </div>

              {/* Şifre */}
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">
                  Şifre {modalMode === 'create' && <span className="text-red-500">*</span>}
                </label>
                <input
                  type="password"
                  value={formData.parola}
                  onChange={(e) => setFormData({ ...formData, parola: e.target.value })}
                  className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                  placeholder={modalMode === 'create' ? "********" : "Değiştirmek için yeni şifre girin"}
                  required={modalMode === 'create'}
                />
              </div>

              {/* TC No */}
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">
                  TC Kimlik No <span className="text-red-500">*</span>
                </label>
                <div className="relative">
                    <CreditCard className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
                    <input
                    type="text"
                    value={formData.tcNo}
                    onChange={(e) => setFormData({ ...formData, tcNo: e.target.value })}
                    className="w-full pl-10 pr-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                    placeholder="11 haneli TC No"
                    maxLength="11"
                    required
                    />
                </div>
              </div>

              {/* Telefon ve Doğum Tarihi */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">
                    Telefon
                  </label>
                  <div className="relative">
                    <Phone className="w-5 h-5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
                    <input
                        type="tel"
                        value={formData.telefonNumarası}
                        onChange={(e) => setFormData({ ...formData, telefonNumarası: e.target.value })}
                        className="w-full pl-10 pr-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                        placeholder="05XXXXXXXXX"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">
                    Doğum Tarihi
                  </label>
                  <input
                    type="date"
                    value={formData.dogumTarihi}
                    onChange={(e) => setFormData({ ...formData, dogumTarihi: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium text-slate-700"
                  />
                </div>
              </div>

              {/* Modal Footer - Butonlar */}
              <div className="flex items-center justify-end gap-3 pt-4 border-t border-slate-100 mt-2">
                <button
                  type="button"
                  onClick={handleCloseModal}
                  className="px-6 py-3 text-slate-600 font-bold hover:bg-slate-100 rounded-xl transition"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="px-8 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-bold shadow-lg shadow-blue-200 flex items-center gap-2"
                >
                  <Save className="w-4 h-4" />
                  {modalMode === 'create' ? 'Kullanıcı Oluştur' : 'Kaydet'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL - Uzmanlık Ekle/Düzenle */}
      {isUzmanlikModalOpen && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md animate-scale-in">
            {/* Modal Header */}
            <div className="sticky top-0 bg-white border-b border-slate-100 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <h2 className="text-xl font-bold text-slate-800 flex items-center gap-2">
                {uzmanlikModalMode === 'create' ? <Plus className="w-5 h-5 text-blue-600" /> : <Edit2 className="w-5 h-5 text-blue-600" />}
                {uzmanlikModalMode === 'create' ? 'Yeni Uzmanlık' : 'Uzmanlık Düzenle'}
              </h2>
              <button 
                onClick={handleCloseUzmanlikModal}
                className="p-2 hover:bg-slate-100 rounded-lg transition"
              >
                <X className="w-5 h-5 text-slate-500" />
              </button>
            </div>

            {/* Modal Body - Form */}
            <form onSubmit={handleSubmitUzmanlik} className="p-6 space-y-5">
              {/* Uzmanlık Adı */}
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">
                  Uzmanlık Adı <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={uzmanlikFormData.uzmanlikAdi}
                  onChange={(e) => setUzmanlikFormData({ ...uzmanlikFormData, uzmanlikAdi: e.target.value })}
                  className="w-full px-4 py-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none font-medium"
                  placeholder="Örn: Kardiyoloji"
                  required
                />
                <p className="text-xs text-slate-400 mt-2 font-medium">Bu uzmanlık alanı doktor eklerken kullanılabilecek.</p>
              </div>

              {/* Modal Footer - Butonlar */}
              <div className="flex items-center justify-end gap-3 pt-4 border-t border-slate-100 mt-4">
                <button
                  type="button"
                  onClick={handleCloseUzmanlikModal}
                  className="px-6 py-3 text-slate-600 font-bold hover:bg-slate-100 rounded-xl transition"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="px-8 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-bold shadow-lg shadow-blue-200 flex items-center gap-2"
                >
                  <Save className="w-4 h-4" />
                  {uzmanlikModalMode === 'create' ? 'Ekle' : 'Kaydet'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default AdminDashboard;