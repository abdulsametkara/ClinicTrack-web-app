import { useNavigate } from 'react-router-dom';
import { LogOut, User, Users, Calendar, Activity, Settings, LayoutDashboard, Search, Menu, Trash2, Edit, Filter, X, Plus, Stethoscope } from 'lucide-react';
import { useState, useEffect } from 'react';
import { getAllUsers, getAllAppointments, deleteUser, createUser, updateUser, getAllUzmanliklar, getAllDoktorlar, createUzmanlik, updateUzmanlik, deleteUzmanlik, updateRandevuDurum, deleteRandevu } from '../api';

function AdminDashboard() {
  const navigate = useNavigate();
  const kullaniciAd = localStorage.getItem('kullaniciAd') || 'Admin';
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
      // Paralel istek at
      const [usersRes, appointmentsRes] = await Promise.all([
        getAllUsers(),
        getAllAppointments()
      ]);

      // Kullanıcı İstatistikleri & Listesi
      let hastaSayisi = 0;
      let doktorSayisi = 0;
      let kullaniciListesi = [];

      console.log('Users Response:', usersRes); // Debug
      
      // Backend camelCase döndürüyor
      if (usersRes.isSuccess) {
        kullaniciListesi = usersRes.data;
        console.log('Kullanıcı Listesi:', kullaniciListesi); // Debug
        setTumKullanicilar(kullaniciListesi);
        setFilteredUsers(kullaniciListesi); // Başlangıçta hepsi

        hastaSayisi = usersRes.data.filter(u => u.rol === 'Hasta').length;
        doktorSayisi = usersRes.data.filter(u => u.rol === 'Doktor').length;
        console.log('Hasta Sayısı:', hastaSayisi, 'Doktor Sayısı:', doktorSayisi); // Debug
      }

      // Randevu İstatistikleri
      let randevuSayisi = 0;
      let sonRandevular = [];

      if (appointmentsRes.isSuccess) {
        const randevular = appointmentsRes.data;
        setTumRandevular(randevular); // Randevuları state'e kaydet
        randevuSayisi = randevular.filter(r => r.durum === 'Beklemede').length;

        // Son 5 randevuyu al ve işle
        sonRandevular = randevular
          .sort((a, b) => new Date(b.randevuTarihi) - new Date(a.randevuTarihi))
          .slice(0, 5)
          .map(r => {
            // Hasta adını bul
            const hasta = kullaniciListesi.find(u => u.id === r.hastaId);
            const hastaAd = hasta ? `${hasta.isim} ${hasta.soyisim}` : 'Bilinmeyen Hasta';
            
            return {
              id: r.id,
              kullanici: hastaAd,
              islem: 'Randevu Oluşturdu',
              tarih: new Date(r.randevuTarihi).toLocaleString('tr-TR'),
              durum: r.durum
            };
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
        (user.isim || user.İsim).toLowerCase().includes(lowerTerm) ||
        (user.soyisim || user.Soyisim).toLowerCase().includes(lowerTerm) ||
        (user.email || user.Email).toLowerCase().includes(lowerTerm) ||
        (user.tcNo || user.TCNo)?.includes(lowerTerm)
      );
    }

    setFilteredUsers(result);
  }, [searchTerm, roleFilter, tumKullanicilar]);

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const handleDeleteUser = async (id) => {
    if (window.confirm('Bu kullanıcıyı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.')) {
      try {
        const response = await deleteUser(id);
        if (response.isSuccess) {
          alert('Kullanıcı başarıyla silindi.');
          fetchData(); 
        } else {
          alert('Hata: ' + response.message);
        }
      } catch (error) {
        alert('Silme işlemi sırasında bir hata oluştu.');
      }
    }
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
          alert('Uzmanlık başarıyla oluşturuldu!');
          handleCloseUzmanlikModal();
          fetchUzmanliklar();
        } else {
          alert('Hata: ' + response.message);
        }
      } else {
        const uzmanlikId = selectedUzmanlik.id || selectedUzmanlik.Id;
        const response = await updateUzmanlik(uzmanlikId, uzmanlikFormData.uzmanlikAdi);
        if (response.isSuccess) {
          alert('Uzmanlık başarıyla güncellendi!');
          handleCloseUzmanlikModal();
          fetchUzmanliklar();
        } else {
          alert('Hata: ' + response.message);
        }
      }
    } catch (error) {
      console.error('Uzmanlık işlemi hatası:', error);
      alert('İşlem sırasında bir hata oluştu: ' + (error.response?.data?.message || error.message));
    }
  };

  const handleDeleteUzmanlik = async (id) => {
    if (window.confirm('Bu uzmanlığı silmek istediğinize emin misiniz? Bu uzmanlığa bağlı doktorlar etkilenebilir!')) {
      try {
        const response = await deleteUzmanlik(id);
        if (response.isSuccess) {
          alert('Uzmanlık başarıyla silindi.');
          fetchUzmanliklar();
          fetchData(); // Doktorların uzmanlık bilgilerini güncelle
        } else {
          alert('Hata: ' + response.message);
        }
      } catch (error) {
        alert('Silme işlemi sırasında bir hata oluştu.');
      }
    }
  };

  // RANDEVU YÖNETİMİ FONKSİYONLARI
  const handleUpdateRandevuDurum = async (randevuId, yeniDurum) => {
    try {
      const response = await updateRandevuDurum(randevuId, yeniDurum);
      if (response.isSuccess) {
        alert(`Randevu durumu "${yeniDurum}" olarak güncellendi.`);
        fetchData(); // Randevuları yeniden çek
      } else {
        alert('Hata: ' + response.message);
      }
    } catch (error) {
      console.error('Randevu güncelleme hatası:', error);
      alert('Güncelleme sırasında bir hata oluştu.');
    }
  };

  const handleDeleteRandevu = async (id) => {
    if (window.confirm('Bu randevuyu silmek istediğinize emin misiniz?')) {
      try {
        const response = await deleteRandevu(id);
        if (response.isSuccess) {
          alert('Randevu başarıyla silindi.');
          fetchData(); // Randevuları yeniden çek
        } else {
          alert('Hata: ' + response.message);
        }
      } catch (error) {
        alert('Silme işlemi sırasında bir hata oluştu.');
      }
    }
  };

  // Form Submit - Yeni Kullanıcı veya Güncelleme
  const handleSubmitUser = async (e) => {
    e.preventDefault();
    
    try {
      // Backend'e gönderilecek data (PascalCase)
      const userData = {
        İsim: formData.isim,
        Soyisim: formData.soyisim,
        Email: formData.email,
        Parola: formData.parola,
        TCNo: formData.tcNo,
        TelefonNumarası: formData.telefonNumarası || '',
        DoğumTarihi: formData.dogumTarihi || null,
        Rol: formData.rol,
        UzmanlıkId: formData.uzmanlikId ? parseInt(formData.uzmanlikId) : null
      };

      if (modalMode === 'create') {
        const response = await createUser(userData);
        if (response.isSuccess) {
          alert('Kullanıcı başarıyla oluşturuldu!');
          handleCloseModal();
          fetchData();
        } else {
          alert('Hata: ' + response.message);
        }
      } else {
        const userId = selectedUser.id || selectedUser.Id;
        const response = await updateUser(userId, userData);
        if (response.isSuccess) {
          alert('Kullanıcı başarıyla güncellendi!');
          handleCloseModal();
          fetchData();
        } else {
          alert('Hata: ' + response.message);
        }
      }
    } catch (error) {
      console.error('Kullanıcı işlemi hatası:', error);
      alert('İşlem sırasında bir hata oluştu: ' + (error.response?.data?.message || error.message));
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
      className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl transition ${
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
    <div className="min-h-screen bg-slate-100 flex font-sans">
      {/* SIDEBAR */}
      <aside className={`bg-slate-900 text-white w-64 flex-shrink-0 fixed h-full z-30 transition-transform duration-300 ${isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}`}>
        <div className="p-6 border-b border-slate-800 flex items-center gap-3">
          <div className="bg-blue-600 p-2 rounded-lg">
            <Activity className="w-6 h-6 text-white" />
          </div>
          <span className="text-xl font-bold tracking-wide">ClinicTrack</span>
        </div>

        <nav className="p-4 space-y-2">
          <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-4 px-4">Genel</div>
          <MenuItem icon={LayoutDashboard} label="Dashboard" tabName="Dashboard" />
          <MenuItem icon={Users} label="Kullanıcılar" tabName="Kullanıcılar" />
          <MenuItem icon={Activity} label="Doktorlar" tabName="Doktorlar" />
          <MenuItem icon={Calendar} label="Randevular" tabName="Randevular" />
          <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mt-8 mb-4 px-4">Sistem</div>
          <MenuItem icon={Stethoscope} label="Uzmanlıklar" tabName="Uzmanlıklar" />
          <MenuItem icon={Settings} label="Ayarlar" tabName="Ayarlar" />
        </nav>

        <div className="absolute bottom-0 w-full p-6 border-t border-slate-800">
          <button onClick={handleLogout} className="flex items-center gap-3 text-slate-400 hover:text-red-400 transition w-full">
            <LogOut className="w-5 h-5" />
            <span className="font-medium">Çıkış Yap</span>
          </button>
        </div>
      </aside>

      {/* MAIN CONTENT */}
      <main className="flex-1 md:ml-64 min-h-screen flex flex-col">
        {/* HEADER */}
        <header className="bg-white shadow-sm h-20 flex items-center justify-between px-8 sticky top-0 z-20 border-b border-slate-200">
          <div className="flex items-center gap-4">
            <button onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)} className="md:hidden p-2 text-gray-600 hover:bg-gray-100 rounded-lg">
              <Menu className="w-6 h-6" />
            </button>
            <div className="hidden md:block">
              <h2 className="text-xl font-bold text-slate-800">Hoş Geldiniz, {kullaniciAd} 👋</h2>
              <p className="text-sm text-slate-500">{currentDate}</p>
            </div>
          </div>

          <div className="flex items-center gap-8">
            <div className="relative hidden md:block">
              <Search className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
              <input type="text" placeholder="Ara..." className="pl-10 pr-4 py-2 bg-slate-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 w-64 border border-transparent focus:bg-white transition" />
            </div>
            
            <div className="flex items-center gap-4 pl-6 border-l border-slate-200">
              <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center text-blue-600 text-xl font-bold shadow-sm border-2 border-blue-50">
                {kullaniciAd?.charAt(0).toUpperCase()}
              </div>
              <div className="hidden md:block">
                <p className="text-base font-bold text-slate-700">{kullaniciAd}</p>
                <p className="text-sm text-slate-500 font-medium">Sistem Yöneticisi</p>
              </div>
            </div>
          </div>
        </header>

        {/* CONTENT AREA */}
        <div className="p-8">
          {/* Dashboard İçeriği */}
          {activeTab === 'Dashboard' && (
            yukleniyor ? (
              <div className="text-center py-10 text-slate-500">Veriler yükleniyor...</div>
            ) : (
              <>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                  <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 flex items-center justify-between hover:shadow-md transition duration-300">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Hasta</p>
                      <h3 className="text-4xl font-bold text-slate-800">{stats.totalHasta}</h3>
                    </div>
                    <div className="p-4 bg-blue-50 rounded-2xl border border-blue-100">
                      <Users className="w-8 h-8 text-blue-600" />
                    </div>
                  </div>
                  <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 flex items-center justify-between hover:shadow-md transition duration-300">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Doktor</p>
                      <h3 className="text-4xl font-bold text-slate-800">{stats.totalDoktor}</h3>
                    </div>
                    <div className="p-4 bg-purple-50 rounded-2xl border border-purple-100">
                      <Activity className="w-8 h-8 text-purple-600" />
                    </div>
                  </div>
                  <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 flex items-center justify-between hover:shadow-md transition duration-300">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Aktif Randevular</p>
                      <h3 className="text-4xl font-bold text-slate-800">{stats.activeRandevu}</h3>
                    </div>
                    <div className="p-4 bg-green-50 rounded-2xl border border-green-100">
                      <Calendar className="w-8 h-8 text-green-600" />
                    </div>
                  </div>
                </div>

                <div className="grid grid-cols-1 gap-8">
                  <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                    <div className="p-6 border-b border-slate-100 flex items-center justify-between bg-white">
                      <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                        <Activity className="w-5 h-5 text-blue-500" />
                        Son Aktiviteler
                      </h3>
                      <button className="text-blue-600 text-sm font-medium hover:underline px-4 py-2 bg-blue-50 rounded-lg transition">Tümünü Gör</button>
                    </div>
                    
                    <div className="overflow-x-auto">
                      <table className="w-full">
                        <thead className="bg-slate-50 text-slate-500 text-sm uppercase font-semibold tracking-wider">
                          <tr>
                            <th className="px-6 py-4 text-left">Kullanıcı / Hasta</th>
                            <th className="px-6 py-4 text-left">İşlem</th>
                            <th className="px-6 py-4 text-left">Tarih</th>
                            <th className="px-6 py-4 text-left">Durum</th>
                          </tr>
                        </thead>
                        <tbody className="text-base divide-y divide-slate-100">
                          {sonAktiviteler.length > 0 ? (
                            sonAktiviteler.map((item, index) => (
                              <tr key={index} className="hover:bg-slate-50 transition duration-150">
                                <td className="px-6 py-5 font-medium text-slate-800 flex items-center gap-3">
                                  <div className="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-600 font-bold text-base">
                                    {item.kullanici.charAt(0)}
                                  </div>
                                  {item.kullanici}
                                </td>
                                <td className="px-6 py-5 text-slate-600">{item.islem}</td>
                                <td className="px-6 py-5 text-slate-500 font-medium">{item.tarih}</td>
                                <td className="px-6 py-5">
                                  <span className={`px-4 py-1.5 rounded-full text-sm font-semibold border ${
                                    item.durum === 'Beklemede' ? 'bg-yellow-50 text-yellow-700 border-yellow-200' :
                                    item.durum === 'Tamamlandı' ? 'bg-green-50 text-green-700 border-green-200' :
                                    item.durum === 'İptal' ? 'bg-red-50 text-red-700 border-red-200' :
                                    'bg-gray-100 text-gray-600'
                                  }`}>
                                    {item.durum}
                                  </span>
                                </td>
                              </tr>
                            ))
                          ) : (
                            <tr>
                              <td colSpan="4" className="px-6 py-10 text-center text-slate-400 italic text-base">Henüz bir aktivite bulunmuyor.</td>
                            </tr>
                          )}
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </>
            )
          )}

          {/* KULLANICILAR TABLOSU */}
          {activeTab === 'Kullanıcılar' && (
            <div className="space-y-6">
              {/* Üst Araç Çubuğu */}
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 bg-white p-4 rounded-2xl border border-slate-200 shadow-sm">
                <div className="relative flex-1 max-w-md">
                  <Search className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
                  <input 
                    type="text" 
                    placeholder="İsim, email veya TC ile ara..." 
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="w-full pl-10 pr-4 py-2.5 bg-slate-50 rounded-xl text-base focus:outline-none focus:ring-2 focus:ring-blue-500 border border-slate-200 transition"
                  />
                </div>

                <div className="flex items-center gap-3">
                  <div className="flex items-center gap-2">
                    <Filter className="w-5 h-5 text-gray-500" />
                    <select 
                      value={roleFilter}
                      onChange={(e) => setRoleFilter(e.target.value)}
                      className="bg-slate-50 border border-slate-200 text-slate-700 text-base rounded-xl focus:ring-blue-500 focus:border-blue-500 block p-2.5 outline-none"
                    >
                      <option value="Hepsi">Tüm Roller</option>
                      <option value="Hasta">Hastalar</option>
                      <option value="Doktor">Doktorlar</option>
                      <option value="Admin">Adminler</option>
                    </select>
                  </div>

                  <button 
                    onClick={handleOpenCreateModal}
                    className="px-5 py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-medium text-base flex items-center gap-2 shadow-sm"
                  >
                    <User className="w-4 h-4" />
                    Kullanıcı Ekle
                  </button>
                </div>
              </div>

              {/* Tablo */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-sm uppercase font-semibold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">Kullanıcı</th>
                        <th className="px-6 py-4 text-left">ILETISIM</th>
                        <th className="px-6 py-4 text-left">Rol</th>
                        <th className="px-6 py-4 text-left">Kayıt Tarihi</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="text-base divide-y divide-slate-100">
                      {filteredUsers.length > 0 ? (
                        filteredUsers.map((user) => (
                          <tr key={user.id || user.Id} className="hover:bg-slate-50 transition duration-150 group">
                            <td className="px-6 py-5">
                              <div className="flex items-center gap-3">
                                <div className="w-12 h-12 rounded-full bg-gradient-to-br from-blue-100 to-blue-200 flex items-center justify-center text-blue-700 font-bold text-lg shadow-sm">
                                  {(user.isim || user.İsim)?.charAt(0)}
                                </div>
                                <div>
                                  <div className="font-bold text-slate-800 text-base">{(user.isim || user.İsim)} {(user.soyisim || user.Soyisim)}</div>
                                  <div className="text-sm text-slate-500">TC: {user.tcNo || user.TCNo}</div>
                                </div>
                              </div>
                            </td>
                            <td className="px-6 py-5">
                              <div className="text-slate-600 text-base">{user.email || user.Email}</div>
                              <div className="text-sm text-slate-400">{user.telefonNumarası || user.TelefonNumarası || '-'}</div>
                            </td>
                            <td className="px-6 py-5">
                              <span className={`px-4 py-1.5 rounded-full text-sm font-semibold border ${
                                (user.rol || user.Rol) === 'Admin' ? 'bg-purple-50 text-purple-700 border-purple-200' :
                                (user.rol || user.Rol) === 'Doktor' ? 'bg-green-50 text-green-700 border-green-200' :
                                'bg-blue-50 text-blue-700 border-blue-200'
                              }`}>
                                {user.rol || user.Rol}
                              </span>
                            </td>
                            <td className="px-6 py-5 text-slate-500 text-base">
                              {new Date(user.oluşturulmaTarihi || user.OluşturulmaTarihi || Date.now()).toLocaleDateString('tr-TR')}
                            </td>
                            <td className="px-6 py-5 text-right">
                              <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                                <button 
                                  onClick={() => handleOpenEditModal(user)}
                                  className="p-2.5 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                                  title="Düzenle"
                                >
                                  <Edit className="w-5 h-5" />
                                </button>
                                <button 
                                  onClick={() => handleDeleteUser(user.id || user.Id)}
                                  className="p-2.5 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                  title="Sil"
                                >
                                  <Trash2 className="w-5 h-5" />
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))
                      ) : (
                        <tr>
                          <td colSpan="5" className="px-6 py-12 text-center text-slate-400">
                            <Users className="w-12 h-12 mx-auto mb-3 text-slate-200" />
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
            <div className="space-y-6">
              {/* Doktor İstatistikleri */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Doktor</p>
                      <h3 className="text-4xl font-bold text-slate-800">
                        {tumKullanicilar.filter(u => (u.rol || u.Rol) === 'Doktor').length}
                      </h3>
                    </div>
                    <div className="p-4 bg-green-50 rounded-2xl border border-green-100">
                      <Activity className="w-8 h-8 text-green-600" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Aktif Doktorlar</p>
                      <h3 className="text-4xl font-bold text-slate-800">
                        {tumKullanicilar.filter(u => (u.rol || u.Rol) === 'Doktor').length}
                      </h3>
                    </div>
                    <div className="p-4 bg-blue-50 rounded-2xl border border-blue-100">
                      <Users className="w-8 h-8 text-blue-600" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Randevu</p>
                      <h3 className="text-4xl font-bold text-slate-800">{stats.activeRandevu}</h3>
                    </div>
                    <div className="p-4 bg-purple-50 rounded-2xl border border-purple-100">
                      <Calendar className="w-8 h-8 text-purple-600" />
                    </div>
                  </div>
                </div>
              </div>

              {/* Doktor Listesi Tablosu */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                <div className="p-6 border-b border-slate-100 bg-white">
                  <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                    <Activity className="w-5 h-5 text-green-500" />
                    Doktor Listesi ve İstatistikleri
                  </h3>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-sm uppercase font-semibold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">Doktor</th>
                        <th className="px-6 py-4 text-left">Uzmanlık</th>
                        <th className="px-6 py-4 text-left">ILETISIM</th>
                        <th className="px-6 py-4 text-center">Randevu Sayısı</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="text-base divide-y divide-slate-100">
                      {tumKullanicilar
                        .filter(u => (u.rol || u.Rol) === 'Doktor')
                        .map((kullaniciDoktor) => {
                          const kullaniciId = kullaniciDoktor.id || kullaniciDoktor.Id; // Kullanıcı ID'si (örn: 2001)
                          
                          // Doktor tablosundan Kullanıcı ID'sine göre Doktor ID'sini bul
                          const doktorBilgisi = doktorlar.find(d => {
                            const doktorKullaniciId = d.kullaniciId || d.KullanıcıId;
                            return doktorKullaniciId == kullaniciId; // == kullan (type coercion)
                          });
                          const doktorId = doktorBilgisi ? (doktorBilgisi.id || doktorBilgisi.Id) : null; // Doktor ID'si (örn: 1)
                          
                          // Uzmanlık bilgisini Doktor tablosundan al (camelCase: uzmanlikId - İngilizce i)
                          const doktorUzmanlikId = doktorBilgisi ? (doktorBilgisi.uzmanlikId || doktorBilgisi.UzmanlıkId) : null;
                          const doktorUzmanlik = uzmanliklar.find(uz => (uz.id || uz.Id) === doktorUzmanlikId);
                          
                          // Doktora ait randevu sayısını hesapla (Doktor ID ile)
                          const randevuSayisi = doktorId ? tumRandevular.filter(r => {
                            const randevuDoktorId = r.doktorId || r.DoktorId;
                            return randevuDoktorId === doktorId;
                          }).length : 0;
                          
                          console.log('Randevu Sayısı:', randevuSayisi);
                          
                          return (
                            <tr key={kullaniciDoktor.id || kullaniciDoktor.Id} className="hover:bg-slate-50 transition duration-150 group">
                              <td className="px-6 py-5">
                                <div className="flex items-center gap-3">
                                  <div className="w-12 h-12 rounded-full bg-gradient-to-br from-green-100 to-green-200 flex items-center justify-center text-green-700 font-bold text-lg shadow-sm">
                                    {(kullaniciDoktor.isim || kullaniciDoktor.İsim)?.charAt(0)}
                                  </div>
                                  <div>
                                    <div className="font-bold text-slate-800 text-base">
                                      Dr. {(kullaniciDoktor.isim || kullaniciDoktor.İsim)} {(kullaniciDoktor.soyisim || kullaniciDoktor.Soyisim)}
                                    </div>
                                    <div className="text-sm text-slate-500">TC: {kullaniciDoktor.tcNo || kullaniciDoktor.TCNo}</div>
                                  </div>
                                </div>
                              </td>
                              <td className="px-6 py-5">
                                <span className="px-4 py-1.5 rounded-full text-sm font-semibold bg-green-50 text-green-700 border border-green-200">
                                  {doktorUzmanlik ? (doktorUzmanlik.uzmanlıkAdı || doktorUzmanlik.UzmanlıkAdı) : 'Belirsiz'}
                                </span>
                              </td>
                              <td className="px-6 py-5">
                                <div className="text-slate-600 text-base">{kullaniciDoktor.email || kullaniciDoktor.Email}</div>
                                <div className="text-sm text-slate-400">{kullaniciDoktor.telefonNumarası || kullaniciDoktor.TelefonNumarası || '-'}</div>
                              </td>
                              <td className="px-6 py-5 text-center">
                                <div className="inline-flex items-center gap-2 px-4 py-2 bg-blue-50 rounded-lg">
                                  <Calendar className="w-4 h-4 text-blue-600" />
                                  <span className="font-bold text-blue-700">{randevuSayisi}</span>
                                  <span className="text-sm text-blue-600">randevu</span>
                                </div>
                              </td>
                              <td className="px-6 py-5 text-right">
                                <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                                  <button 
                                    onClick={() => handleOpenEditModal(kullaniciDoktor)}
                                    className="p-2.5 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                                    title="Düzenle"
                                  >
                                    <Edit className="w-5 h-5" />
                                  </button>
                                  <button 
                                    onClick={() => handleDeleteUser(kullaniciDoktor.id || kullaniciDoktor.Id)}
                                    className="p-2.5 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                    title="Sil"
                                  >
                                    <Trash2 className="w-5 h-5" />
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })}
                      {tumKullanicilar.filter(u => (u.rol || u.Rol) === 'Doktor').length === 0 && (
                        <tr>
                          <td colSpan="5" className="px-6 py-12 text-center text-slate-400">
                            <Activity className="w-12 h-12 mx-auto mb-3 text-slate-200" />
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
            <div className="space-y-6">
              {/* Üst İstatistikler */}
              <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Toplam Randevu</p>
                      <h3 className="text-3xl font-bold text-slate-800">{tumRandevular.length}</h3>
                    </div>
                    <div className="p-3 bg-blue-50 rounded-xl border border-blue-100">
                      <Calendar className="w-6 h-6 text-blue-600" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Beklemede</p>
                      <h3 className="text-3xl font-bold text-yellow-600">
                        {tumRandevular.filter(r => (r.durum || r.Durum) === 'Beklemede').length}
                      </h3>
                    </div>
                    <div className="p-3 bg-yellow-50 rounded-xl border border-yellow-100">
                      <Calendar className="w-6 h-6 text-yellow-600" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">Tamamlandı</p>
                      <h3 className="text-3xl font-bold text-green-600">
                        {tumRandevular.filter(r => (r.durum || r.Durum) === 'Tamamlandı').length}
                      </h3>
                    </div>
                    <div className="p-3 bg-green-50 rounded-xl border border-green-100">
                      <Calendar className="w-6 h-6 text-green-600" />
                    </div>
                  </div>
                </div>

                <div className="bg-white p-6 rounded-2xl shadow-sm border border-slate-200 hover:shadow-md transition">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-slate-500 text-sm font-medium mb-1">İptal</p>
                      <h3 className="text-3xl font-bold text-red-600">
                        {tumRandevular.filter(r => (r.durum || r.Durum) === 'İptal').length}
                      </h3>
                    </div>
                    <div className="p-3 bg-red-50 rounded-xl border border-red-100">
                      <Calendar className="w-6 h-6 text-red-600" />
                    </div>
                  </div>
                </div>
              </div>

              {/* Randevu Listesi */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                <div className="p-6 border-b border-slate-100 bg-white">
                  <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                    <Calendar className="w-5 h-5 text-purple-500" />
                    Tüm Randevular
                  </h3>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-sm uppercase font-semibold tracking-wider">
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
                    <tbody className="text-base divide-y divide-slate-100">
                      {tumRandevular.length > 0 ? (
                        tumRandevular.map((randevu) => {
                          const hasta = tumKullanicilar.find(u => (u.id || u.Id) === (randevu.hastaId || randevu.HastaId));
                          
                          // Doktor ID'den kullanıcıyı bul
                          const randevuDoktorId = randevu.doktorId || randevu.DoktorId;
                          const doktorBilgisi = doktorlar.find(d => (d.id || d.Id) === randevuDoktorId);
                          const doktorKullaniciId = doktorBilgisi ? (doktorBilgisi.kullaniciId || doktorBilgisi.KullanıcıId) : null;
                          const doktorKullanici = tumKullanicilar.find(u => (u.id || u.Id) === doktorKullaniciId);
                          
                          const randevuDurum = randevu.durum || randevu.Durum;

                          return (
                            <tr key={randevu.id || randevu.Id} className="hover:bg-slate-50 transition duration-150 group">
                              <td className="px-6 py-5 font-medium text-slate-600">
                                #{randevu.id || randevu.Id}
                              </td>
                              <td className="px-6 py-5">
                                <div className="flex items-center gap-3">
                                  <div className="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold">
                                    {hasta ? (hasta.isim || hasta.İsim)?.charAt(0) : '?'}
                                  </div>
                                  <div>
                                    <div className="font-bold text-slate-800">
                                      {hasta ? `${hasta.isim || hasta.İsim} ${hasta.soyisim || hasta.Soyisim}` : 'Bilinmiyor'}
                                    </div>
                                    <div className="text-sm text-slate-500">TC: {hasta?.tcNo || hasta?.TCNo || '-'}</div>
                                  </div>
                                </div>
                              </td>
                              <td className="px-6 py-5">
                                <div className="font-medium text-slate-700">
                                  {doktorKullanici ? `Dr. ${doktorKullanici.isim || doktorKullanici.İsim} ${doktorKullanici.soyisim || doktorKullanici.Soyisim}` : 'Bilinmiyor'}
                                </div>
                              </td>
                              <td className="px-6 py-5">
                                <div className="text-slate-700 font-medium">
                                  {new Date(randevu.randevuTarihi || randevu.RandevuTarihi).toLocaleDateString('tr-TR')}
                                </div>
                                <div className="text-sm text-slate-500">
                                  {new Date(randevu.randevuTarihi || randevu.RandevuTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })}
                                </div>
                              </td>
                              <td className="px-6 py-5">
                                <div className="text-slate-600 text-sm max-w-xs truncate">
                                  {randevu.sikayet || randevu.Sikayet || '-'}
                                </div>
                              </td>
                              <td className="px-6 py-5">
                                <select
                                  value={randevuDurum}
                                  onChange={(e) => handleUpdateRandevuDurum(randevu.id || randevu.Id, e.target.value)}
                                  className={`px-4 py-2 rounded-lg text-sm font-semibold border cursor-pointer outline-none ${
                                    randevuDurum === 'Beklemede' ? 'bg-yellow-50 text-yellow-700 border-yellow-200' :
                                    randevuDurum === 'Tamamlandı' ? 'bg-green-50 text-green-700 border-green-200' :
                                    randevuDurum === 'İptal' ? 'bg-red-50 text-red-700 border-red-200' :
                                    'bg-gray-50 text-gray-700 border-gray-200'
                                  }`}
                                >
                                  <option value="Beklemede">Beklemede</option>
                                  <option value="Tamamlandı">Tamamlandı</option>
                                  <option value="İptal">İptal</option>
                                </select>
                              </td>
                              <td className="px-6 py-5 text-right">
                                <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                                  <button 
                                    onClick={() => handleDeleteRandevu(randevu.id || randevu.Id)}
                                    className="p-2.5 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                    title="Sil"
                                  >
                                    <Trash2 className="w-5 h-5" />
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })
                      ) : (
                        <tr>
                          <td colSpan="7" className="px-6 py-12 text-center text-slate-400">
                            <Calendar className="w-12 h-12 mx-auto mb-3 text-slate-200" />
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
            <div className="space-y-6">
              {/* Üst Başlık ve Buton */}
              <div className="flex items-center justify-between bg-white p-4 rounded-2xl border border-slate-200 shadow-sm">
                <div>
                  <h2 className="text-2xl font-bold text-slate-800">Uzmanlık Alanları Yönetimi</h2>
                  <p className="text-slate-500 text-sm mt-1">Doktorların uzmanlık alanlarını yönetin</p>
                </div>
                <button 
                  onClick={handleOpenCreateUzmanlikModal}
                  className="px-5 py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-medium text-base flex items-center gap-2 shadow-sm"
                >
                  <Plus className="w-4 h-4" />
                  Uzmanlık Ekle
                </button>
              </div>

              {/* Uzmanlık Listesi */}
              <div className="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
                <div className="p-6 border-b border-slate-100 bg-white">
                  <h3 className="font-bold text-lg text-slate-800 flex items-center gap-2">
                    <Stethoscope className="w-5 h-5 text-blue-500" />
                    Kayıtlı Uzmanlıklar
                  </h3>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-slate-50 text-slate-500 text-sm uppercase font-semibold tracking-wider">
                      <tr>
                        <th className="px-6 py-4 text-left">ID</th>
                        <th className="px-6 py-4 text-left">Uzmanlık Adı</th>
                        <th className="px-6 py-4 text-left">Kayıt Tarihi</th>
                        <th className="px-6 py-4 text-right">İşlemler</th>
                      </tr>
                    </thead>
                    <tbody className="text-base divide-y divide-slate-100">
                      {uzmanliklar.length > 0 ? (
                        uzmanliklar.map((uzmanlik) => (
                          <tr key={uzmanlik.id || uzmanlik.Id} className="hover:bg-slate-50 transition duration-150 group">
                            <td className="px-6 py-5 font-medium text-slate-600">
                              #{uzmanlik.id || uzmanlik.Id}
                            </td>
                            <td className="px-6 py-5">
                              <span className="px-4 py-2 rounded-lg text-base font-semibold bg-blue-50 text-blue-700 border border-blue-200">
                                {uzmanlik.uzmanlıkAdı || uzmanlik.UzmanlıkAdı}
                              </span>
                            </td>
                            <td className="px-6 py-5 text-slate-500 text-base">
                              {new Date(uzmanlik.recordDate || uzmanlik.RecordDate || Date.now()).toLocaleDateString('tr-TR')}
                            </td>
                            <td className="px-6 py-5 text-right">
                              <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                                <button 
                                  onClick={() => handleOpenEditUzmanlikModal(uzmanlik)}
                                  className="p-2.5 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                                  title="Düzenle"
                                >
                                  <Edit className="w-5 h-5" />
                                </button>
                                <button 
                                  onClick={() => handleDeleteUzmanlik(uzmanlik.id || uzmanlik.Id)}
                                  className="p-2.5 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                                  title="Sil"
                                >
                                  <Trash2 className="w-5 h-5" />
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))
                      ) : (
                        <tr>
                          <td colSpan="4" className="px-6 py-12 text-center text-slate-400">
                            <Stethoscope className="w-12 h-12 mx-auto mb-3 text-slate-200" />
                            <p>Henüz kayıtlı uzmanlık alanı bulunmuyor.</p>
                            <p className="text-sm mt-2">Yukarıdaki "Uzmanlık Ekle" butonunu kullanarak yeni uzmanlık ekleyebilirsiniz.</p>
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          )}

          {activeTab === 'Ayarlar' && (
            <div className="bg-white p-8 rounded-2xl shadow-sm border border-slate-200 text-center py-20">
              <Settings className="w-16 h-16 text-slate-300 mx-auto mb-4" />
              <h3 className="text-xl font-bold text-slate-700 mb-2">Sistem Ayarları</h3>
              <p className="text-slate-500">Genel sistem ayarları burada olacak.</p>
            </div>
          )}
        </div>
      </main>

      {/* MODAL - Kullanıcı Ekle/Düzenle */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            {/* Modal Header */}
            <div className="sticky top-0 bg-white border-b border-slate-200 px-6 py-4 flex items-center justify-between">
              <h2 className="text-2xl font-bold text-slate-800">
                {modalMode === 'create' ? '🆕 Yeni Kullanıcı Ekle' : '✏️ Kullanıcı Düzenle'}
              </h2>
              <button 
                onClick={handleCloseModal}
                className="p-2 hover:bg-slate-100 rounded-lg transition"
              >
                <X className="w-6 h-6 text-slate-600" />
              </button>
            </div>

            {/* Modal Body - Form */}
            <form onSubmit={handleSubmitUser} className="p-6 space-y-4">
              {/* Rol Seçimi */}
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-2">
                  Kullanıcı Rolü *
                </label>
                <select
                  value={formData.rol}
                  onChange={(e) => setFormData({ ...formData, rol: e.target.value, uzmanlikId: '' })}
                  className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                  required
                >
                  <option value="Hasta">Hasta</option>
                  <option value="Doktor">Doktor</option>
                  <option value="Admin">Admin</option>
                </select>
              </div>

              {/* Uzmanlık Seçimi (Sadece Doktor için) */}
              {formData.rol === 'Doktor' && (
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-2">
                    Uzmanlık Alanı *
                  </label>
                  <select
                    value={formData.uzmanlikId}
                    onChange={(e) => setFormData({ ...formData, uzmanlikId: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
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
                  <label className="block text-sm font-semibold text-slate-700 mb-2">
                    İsim *
                  </label>
                  <input
                    type="text"
                    value={formData.isim}
                    onChange={(e) => setFormData({ ...formData, isim: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                    placeholder="Ahmet"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-2">
                    Soyisim *
                  </label>
                  <input
                    type="text"
                    value={formData.soyisim}
                    onChange={(e) => setFormData({ ...formData, soyisim: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                    placeholder="Yılmaz"
                    required
                  />
                </div>
              </div>

              {/* Email */}
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-2">
                  Email *
                </label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                  placeholder="ahmet@example.com"
                  required
                />
              </div>

              {/* Şifre */}
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-2">
                  Şifre {modalMode === 'create' ? '*' : '(Değiştirmek istemiyorsanız boş bırakın)'}
                </label>
                <input
                  type="password"
                  value={formData.parola}
                  onChange={(e) => setFormData({ ...formData, parola: e.target.value })}
                  className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                  placeholder="********"
                  required={modalMode === 'create'}
                />
              </div>

              {/* TC No */}
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-2">
                  TC Kimlik No *
                </label>
                <input
                  type="text"
                  value={formData.tcNo}
                  onChange={(e) => setFormData({ ...formData, tcNo: e.target.value })}
                  className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                  placeholder="12345678901"
                  maxLength="11"
                  required
                />
              </div>

              {/* Telefon ve Doğum Tarihi */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-2">
                    Telefon
                  </label>
                  <input
                    type="tel"
                    value={formData.telefonNumarası}
                    onChange={(e) => setFormData({ ...formData, telefonNumarası: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                    placeholder="05XXXXXXXXX"
                  />
                </div>
                <div>
                  <label className="block text-sm font-semibold text-slate-700 mb-2">
                    Doğum Tarihi
                  </label>
                  <input
                    type="date"
                    value={formData.dogumTarihi}
                    onChange={(e) => setFormData({ ...formData, dogumTarihi: e.target.value })}
                    className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                  />
                </div>
              </div>

              {/* Modal Footer - Butonlar */}
              <div className="flex items-center justify-end gap-3 pt-4 border-t border-slate-200 mt-6">
                <button
                  type="button"
                  onClick={handleCloseModal}
                  className="px-6 py-3 bg-slate-100 text-slate-700 rounded-xl hover:bg-slate-200 transition font-medium text-base"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="px-6 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-medium text-base shadow-sm"
                >
                  {modalMode === 'create' ? '✅ Kullanıcı Oluştur' : '💾 Değişiklikleri Kaydet'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL - Uzmanlık Ekle/Düzenle */}
      {isUzmanlikModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md">
            {/* Modal Header */}
            <div className="sticky top-0 bg-white border-b border-slate-200 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <h2 className="text-2xl font-bold text-slate-800">
                {uzmanlikModalMode === 'create' ? '🩺 Yeni Uzmanlık Ekle' : '✏️ Uzmanlık Düzenle'}
              </h2>
              <button 
                onClick={handleCloseUzmanlikModal}
                className="p-2 hover:bg-slate-100 rounded-lg transition"
              >
                <X className="w-6 h-6 text-slate-600" />
              </button>
            </div>

            {/* Modal Body - Form */}
            <form onSubmit={handleSubmitUzmanlik} className="p-6 space-y-4">
              {/* Uzmanlık Adı */}
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-2">
                  Uzmanlık Adı *
                </label>
                <input
                  type="text"
                  value={uzmanlikFormData.uzmanlikAdi}
                  onChange={(e) => setUzmanlikFormData({ ...uzmanlikFormData, uzmanlikAdi: e.target.value })}
                  className="w-full px-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none text-base"
                  placeholder="Örn: Kardiyoloji, Ortopedi, KBB..."
                  required
                />
                <p className="text-xs text-slate-500 mt-2">Bu uzmanlık alanı doktor eklerken kullanılabilecek.</p>
              </div>

              {/* Modal Footer - Butonlar */}
              <div className="flex items-center justify-end gap-3 pt-4 border-t border-slate-200 mt-6">
                <button
                  type="button"
                  onClick={handleCloseUzmanlikModal}
                  className="px-6 py-3 bg-slate-100 text-slate-700 rounded-xl hover:bg-slate-200 transition font-medium text-base"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="px-6 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition font-medium text-base shadow-sm"
                >
                  {uzmanlikModalMode === 'create' ? '✅ Uzmanlık Ekle' : '💾 Değişiklikleri Kaydet'}
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
