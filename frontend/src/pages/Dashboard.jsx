// Ana sayfa - Giriş yapan kullanıcı için
import { useNavigate } from 'react-router-dom';
import { LogOut, User } from 'lucide-react';

function Dashboard() {
  const navigate = useNavigate();

  // LocalStorage'dan kullanıcı bilgilerini al
  const kullaniciAd = localStorage.getItem('kullaniciAd');
  const kullaniciRol = localStorage.getItem('kullaniciRol');

  // Çıkış yap
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('kullaniciAd');
    localStorage.removeItem('kullaniciRol');
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Üst navbar */}
      <nav className="bg-white shadow-md">
        <div className="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center">
              <User className="w-6 h-6 text-white" />
            </div>
            <div>
              <h2 className="font-bold text-gray-800">{kullaniciAd}</h2>
              <p className="text-sm text-gray-500">{kullaniciRol}</p>
            </div>
          </div>

          <button
            onClick={handleLogout}
            className="flex items-center gap-2 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600 transition"
          >
            <LogOut className="w-4 h-4" />
            Çıkış Yap
          </button>
        </div>
      </nav>

      {/* Ana içerik */}
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="bg-white rounded-lg shadow-md p-8 text-center">
          <h1 className="text-3xl font-bold text-gray-800 mb-4">
            Hoş Geldiniz!
          </h1>
          <p className="text-gray-600 mb-8">
            ClinickTrack Randevu Yönetim Sistemi'ne başarıyla giriş yaptınız.
          </p>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
            {/* Placeholder kartlar */}
            <div className="bg-blue-50 p-6 rounded-lg border border-blue-200">
              <h3 className="font-bold text-blue-800 mb-2">Randevularım</h3>
              <p className="text-sm text-gray-600">Yakında eklenecek...</p>
            </div>

            <div className="bg-green-50 p-6 rounded-lg border border-green-200">
              <h3 className="font-bold text-green-800 mb-2">Profilim</h3>
              <p className="text-sm text-gray-600">Yakında eklenecek...</p>
            </div>

            <div className="bg-purple-50 p-6 rounded-lg border border-purple-200">
              <h3 className="font-bold text-purple-800 mb-2">Bildirimler</h3>
              <p className="text-sm text-gray-600">Yakında eklenecek...</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;



