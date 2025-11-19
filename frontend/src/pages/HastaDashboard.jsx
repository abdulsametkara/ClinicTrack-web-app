import { useNavigate } from 'react-router-dom';
import { LogOut, User, Calendar, Clock, Plus, FileText, Menu, Bell, Search, Activity, Home } from 'lucide-react'; // Home import edildi
import { useState } from 'react';

function HastaDashboard() {
  const navigate = useNavigate();
  const kullaniciAd = localStorage.getItem('kullaniciAd');
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const currentDate = new Date().toLocaleDateString('tr-TR', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    weekday: 'long'
  });

  return (
    <div className="min-h-screen bg-gray-100 flex">
      {/* SIDEBAR */}
      <aside className={`bg-slate-900 text-white w-64 flex-shrink-0 fixed h-full z-30 transition-transform duration-300 ${isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}`}>
        <div className="p-6 border-b border-slate-800 flex items-center gap-3">
          <div className="bg-blue-600 p-2 rounded-lg">
            <Activity className="w-6 h-6 text-white" />
          </div>
          <span className="text-xl font-bold tracking-wide">ClinicTrack</span>
        </div>

        <nav className="p-4 space-y-2">
          <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-4 px-4">Hasta Paneli</div>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 bg-blue-600 rounded-xl text-white shadow-lg shadow-blue-900/20">
            <Home className="w-5 h-5" />
            <span className="font-medium">Ana Sayfa</span>
          </a>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 text-slate-400 hover:bg-slate-800 hover:text-white rounded-xl transition">
            <Calendar className="w-5 h-5" />
            <span className="font-medium">Randevularım</span>
          </a>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 text-slate-400 hover:bg-slate-800 hover:text-white rounded-xl transition">
            <Plus className="w-5 h-5" />
            <span className="font-medium">Randevu Al</span>
          </a>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 text-slate-400 hover:bg-slate-800 hover:text-white rounded-xl transition">
            <User className="w-5 h-5" />
            <span className="font-medium">Profilim</span>
          </a>
        </nav>

        <div className="absolute bottom-0 w-full p-6 border-t border-slate-800">
          <button 
            onClick={handleLogout}
            className="flex items-center gap-3 text-slate-400 hover:text-red-400 transition w-full"
          >
            <LogOut className="w-5 h-5" />
            <span className="font-medium">Çıkış Yap</span>
          </button>
        </div>
      </aside>

      {/* MAIN CONTENT */}
      <main className="flex-1 md:ml-64 min-h-screen flex flex-col">
        {/* HEADER */}
        <header className="bg-white shadow-sm h-20 flex items-center justify-between px-8 sticky top-0 z-20">
          <div className="flex items-center gap-4">
            <button 
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="md:hidden p-2 text-gray-600 hover:bg-gray-100 rounded-lg"
            >
              <Menu className="w-6 h-6" />
            </button>
            
            <div className="hidden md:block">
              <h2 className="text-xl font-bold text-gray-800">Hoş Geldiniz, {kullaniciAd} 👋</h2>
              <p className="text-sm text-gray-500">{currentDate}</p>
            </div>
          </div>

          <div className="flex items-center gap-6">
            <div className="relative hidden md:block">
              <Search className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
              <input 
                type="text" 
                placeholder="Randevu ara..." 
                className="pl-10 pr-4 py-2 bg-gray-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 w-64"
              />
            </div>
            
            <button className="relative p-2 text-gray-600 hover:bg-gray-100 rounded-full">
              <Bell className="w-6 h-6" />
              <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full border border-white"></span>
            </button>

            <div className="flex items-center gap-3 pl-6 border-l">
              <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center text-blue-600 font-bold">
                {kullaniciAd?.charAt(0).toUpperCase()}
              </div>
              <div className="hidden md:block">
                <p className="text-sm font-bold text-gray-700">{kullaniciAd}</p>
                <p className="text-xs text-gray-500">Hasta</p>
              </div>
            </div>
          </div>
        </header>

        {/* CONTENT */}
        <div className="p-8">
          {/* Hızlı Randevu Al Butonu */}
          <div className="mb-8">
            <button className="w-full md:w-auto bg-gradient-to-r from-blue-600 to-blue-700 text-white px-8 py-4 rounded-2xl hover:from-blue-700 hover:to-blue-800 transition shadow-lg shadow-blue-900/20 flex items-center justify-center gap-3 font-bold text-lg transform hover:-translate-y-1">
              <Plus className="w-6 h-6" />
              Yeni Randevu Oluştur
            </button>
          </div>

          {/* Stats */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Toplam Randevu</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-blue-600 transition">12</h3>
              </div>
              <div className="p-4 bg-blue-50 rounded-xl group-hover:bg-blue-100 transition">
                <Calendar className="w-8 h-8 text-blue-600" />
              </div>
            </div>

            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Yaklaşan</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-blue-600 transition">2</h3>
              </div>
              <div className="p-4 bg-orange-50 rounded-xl group-hover:bg-orange-100 transition">
                <Clock className="w-8 h-8 text-orange-600" />
              </div>
            </div>

            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Tamamlanan</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-blue-600 transition">10</h3>
              </div>
              <div className="p-4 bg-green-50 rounded-xl group-hover:bg-green-100 transition">
                <FileText className="w-8 h-8 text-green-600" />
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Yaklaşan Randevular */}
            <div className="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
              <h3 className="font-bold text-lg text-gray-800 mb-6 flex items-center gap-2">
                <Calendar className="w-5 h-5 text-blue-600" />
                Yaklaşan Randevularım
              </h3>

              <div className="space-y-4">
                <div className="flex flex-col md:flex-row items-center justify-between p-6 bg-blue-50 rounded-2xl border border-blue-100 gap-6">
                  <div className="flex items-center gap-6 w-full md:w-auto">
                    <div className="bg-white text-blue-600 font-bold px-4 py-3 rounded-xl text-center shadow-sm min-w-[80px]">
                      <div className="text-xs uppercase tracking-wider text-gray-400">Kasım</div>
                      <div className="text-2xl">22</div>
                    </div>
                    <div>
                      <h4 className="font-bold text-gray-800 text-xl mb-1">Dr. Ayşe Demir</h4>
                      <p className="text-blue-600 font-medium">Kardiyoloji</p>
                      <p className="text-gray-500 text-sm mt-1 flex items-center gap-1">
                        <Clock className="w-4 h-4" /> 14:00
                      </p>
                    </div>
                  </div>
                  <div className="flex gap-3 w-full md:w-auto">
                    <button className="flex-1 md:flex-none bg-white text-gray-700 px-6 py-3 rounded-xl font-medium hover:bg-gray-50 border border-gray-200 transition shadow-sm">
                      Detaylar
                    </button>
                    <button className="flex-1 md:flex-none bg-blue-600 text-white px-6 py-3 rounded-xl font-medium hover:bg-blue-700 shadow-lg shadow-blue-600/20 transition">
                      Düzenle
                    </button>
                  </div>
                </div>

                <div className="flex flex-col md:flex-row items-center justify-between p-6 bg-white rounded-2xl border border-gray-100 hover:border-blue-100 transition gap-6">
                  <div className="flex items-center gap-6 w-full md:w-auto">
                    <div className="bg-gray-50 text-gray-600 font-bold px-4 py-3 rounded-xl text-center min-w-[80px]">
                      <div className="text-xs uppercase tracking-wider text-gray-400">Kasım</div>
                      <div className="text-2xl">25</div>
                    </div>
                    <div>
                      <h4 className="font-bold text-gray-800 text-xl mb-1">Dr. Mehmet Yılmaz</h4>
                      <p className="text-blue-600 font-medium">Nöroloji</p>
                      <p className="text-gray-500 text-sm mt-1 flex items-center gap-1">
                        <Clock className="w-4 h-4" /> 10:00
                      </p>
                    </div>
                  </div>
                  <div className="flex gap-3 w-full md:w-auto">
                    <button className="flex-1 md:flex-none bg-gray-50 text-gray-600 px-6 py-3 rounded-xl font-medium hover:bg-gray-100 transition">
                      Detaylar
                    </button>
                  </div>
                </div>
              </div>
            </div>

            {/* Geçmiş Randevular Timeline */}
            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
              <h3 className="font-bold text-lg text-gray-800 mb-6">Son Ziyaretler</h3>
              
              <div className="relative pl-4 border-l-2 border-gray-100 space-y-8">
                <div className="relative group cursor-pointer">
                  <span className="absolute -left-[21px] top-1 w-3 h-3 rounded-full bg-green-500 ring-4 ring-white group-hover:scale-125 transition"></span>
                  <h4 className="text-sm font-bold text-gray-800">Kardiyoloji Kontrolü</h4>
                  <p className="text-xs text-gray-500 mt-1">Dr. Ayşe Demir</p>
                  <span className="text-xs text-gray-400 mt-2 block">15 Kasım 2024</span>
                </div>
                
                <div className="relative group cursor-pointer">
                  <span className="absolute -left-[21px] top-1 w-3 h-3 rounded-full bg-gray-300 ring-4 ring-white group-hover:bg-blue-400 group-hover:scale-125 transition"></span>
                  <h4 className="text-sm font-bold text-gray-800">Göz Muayenesi</h4>
                  <p className="text-xs text-gray-500 mt-1">Dr. Canan Yılmaz</p>
                  <span className="text-xs text-gray-400 mt-2 block">20 Ekim 2024</span>
                </div>

                <div className="relative group cursor-pointer">
                  <span className="absolute -left-[21px] top-1 w-3 h-3 rounded-full bg-gray-300 ring-4 ring-white group-hover:bg-blue-400 group-hover:scale-125 transition"></span>
                  <h4 className="text-sm font-bold text-gray-800">Genel Check-up</h4>
                  <p className="text-xs text-gray-500 mt-1">Dr. Ali Veli</p>
                  <span className="text-xs text-gray-400 mt-2 block">10 Eylül 2024</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

export default HastaDashboard;
