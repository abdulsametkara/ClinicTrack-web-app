import { useNavigate } from 'react-router-dom';
import { LogOut, User, Calendar, Clock, CheckCircle, Users, Menu, Bell, Search, FileText } from 'lucide-react';
import { useState } from 'react';

function DoktorDashboard() {
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
          <div className="bg-emerald-600 p-2 rounded-lg">
            <Activity className="w-6 h-6 text-white" />
          </div>
          <span className="text-xl font-bold tracking-wide">ClinicTrack</span>
        </div>

        <nav className="p-4 space-y-2">
          <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-4 px-4">Doktor Paneli</div>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 bg-emerald-600 rounded-xl text-white shadow-lg shadow-emerald-900/20">
            <Calendar className="w-5 h-5" />
            <span className="font-medium">Randevularım</span>
          </a>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 text-slate-400 hover:bg-slate-800 hover:text-white rounded-xl transition">
            <Users className="w-5 h-5" />
            <span className="font-medium">Hastalarım</span>
          </a>
          
          <a href="#" className="flex items-center gap-3 px-4 py-3 text-slate-400 hover:bg-slate-800 hover:text-white rounded-xl transition">
            <FileText className="w-5 h-5" />
            <span className="font-medium">Raporlar</span>
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
              <h2 className="text-xl font-bold text-gray-800">Hoş Geldiniz, {kullaniciAd} 👨‍⚕️</h2>
              <p className="text-sm text-gray-500">{currentDate}</p>
            </div>
          </div>

          <div className="flex items-center gap-6">
            <div className="relative hidden md:block">
              <Search className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" />
              <input 
                type="text" 
                placeholder="Hasta ara..." 
                className="pl-10 pr-4 py-2 bg-gray-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 w-64"
              />
            </div>
            
            <button className="relative p-2 text-gray-600 hover:bg-gray-100 rounded-full">
              <Bell className="w-6 h-6" />
              <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full border border-white"></span>
            </button>

            <div className="flex items-center gap-3 pl-6 border-l">
              <div className="w-10 h-10 bg-emerald-100 rounded-full flex items-center justify-center text-emerald-600 font-bold">
                {kullaniciAd?.charAt(0).toUpperCase()}
              </div>
              <div className="hidden md:block">
                <p className="text-sm font-bold text-gray-700">{kullaniciAd}</p>
                <p className="text-xs text-gray-500">Kardiyoloji Uzmanı</p>
              </div>
            </div>
          </div>
        </header>

        {/* CONTENT */}
        <div className="p-8">
          {/* Stats */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Bugünkü Randevular</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-emerald-600 transition">8</h3>
              </div>
              <div className="p-4 bg-blue-50 rounded-xl group-hover:bg-blue-100 transition">
                <Calendar className="w-8 h-8 text-blue-600" />
              </div>
            </div>

            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Bekleyen</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-emerald-600 transition">3</h3>
              </div>
              <div className="p-4 bg-orange-50 rounded-xl group-hover:bg-orange-100 transition">
                <Clock className="w-8 h-8 text-orange-600" />
              </div>
            </div>

            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Tamamlanan</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-emerald-600 transition">5</h3>
              </div>
              <div className="p-4 bg-green-50 rounded-xl group-hover:bg-green-100 transition">
                <CheckCircle className="w-8 h-8 text-green-600" />
              </div>
            </div>

            <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 flex items-center justify-between group hover:shadow-md transition">
              <div>
                <p className="text-gray-500 text-sm mb-1">Toplam Hasta</p>
                <h3 className="text-3xl font-bold text-gray-800 group-hover:text-emerald-600 transition">142</h3>
              </div>
              <div className="p-4 bg-purple-50 rounded-xl group-hover:bg-purple-100 transition">
                <Users className="w-8 h-8 text-purple-600" />
              </div>
            </div>
          </div>

          {/* Appointments List */}
          <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <div className="flex items-center justify-between mb-6">
              <h3 className="font-bold text-lg text-gray-800 flex items-center gap-2">
                <Calendar className="w-5 h-5 text-emerald-600" />
                Bugünkü Program
              </h3>
              <button className="bg-emerald-50 text-emerald-600 px-4 py-2 rounded-lg text-sm font-medium hover:bg-emerald-100 transition">
                Takvimi Görüntüle
              </button>
            </div>

            <div className="space-y-4">
              {[
                { time: '09:00', name: 'Ahmet Yılmaz', type: 'İlk Muayene', status: 'Tamamlandı', statusColor: 'bg-green-100 text-green-700' },
                { time: '10:30', name: 'Zeynep Kaya', type: 'Kontrol', status: 'Bekliyor', statusColor: 'bg-blue-100 text-blue-700' },
                { time: '11:00', name: 'Mehmet Demir', type: 'Acil', status: 'İptal', statusColor: 'bg-red-100 text-red-700' },
                { time: '14:00', name: 'Ayşe Çelik', type: 'Rapor', status: 'Bekliyor', statusColor: 'bg-orange-100 text-orange-700' },
              ].map((apt, index) => (
                <div key={index} className="flex items-center justify-between p-4 border border-gray-100 rounded-xl hover:bg-gray-50 transition">
                  <div className="flex items-center gap-6">
                    <div className="bg-slate-100 text-slate-700 font-bold px-4 py-3 rounded-xl text-center min-w-[80px]">
                      {apt.time}
                    </div>
                    <div>
                      <h4 className="font-bold text-gray-800 text-lg">{apt.name}</h4>
                      <p className="text-gray-500 text-sm">{apt.type}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-4">
                    <span className={`px-3 py-1 rounded-full text-xs font-semibold ${apt.statusColor}`}>
                      {apt.status}
                    </span>
                    <button className="text-gray-400 hover:text-emerald-600 transition">
                      <FileText className="w-5 h-5" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

import { Activity } from 'lucide-react'; // Eksik import eklendi

export default DoktorDashboard;
