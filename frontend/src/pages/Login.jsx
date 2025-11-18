// Giriş sayfası - Email ve Parola ile giriş
import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { login } from '../api';
import { LogIn } from 'lucide-react';

function Login() {
  // Form verileri için state
  const [email, setEmail] = useState('');
  const [parola, setParola] = useState('');
  const [hata, setHata] = useState('');
  const [yukleniyor, setYukleniyor] = useState(false);

  // Sayfa yönlendirme için
  const navigate = useNavigate();

  // Giriş butonuna tıklandığında
  const handleLogin = async (e) => {
    e.preventDefault(); // Sayfanın yenilenmesini engelle
    setHata(''); // Önceki hataları temizle
    setYukleniyor(true);

    try {
      // API'ye istek at
      const response = await login(email, parola);

      console.log('Login Response:', response); // Debug için

      // Başarılı ise
      if (response.isSuccess) {
        // Token'ı sakla (Backend büyük harfle döndürüyor)
        const token = response.data.token || response.data.Token;
        const isim = response.data.isim || response.data.İsim;
        const rol = response.data.rol || response.data.Rol;

        console.log('Token:', token, 'İsim:', isim, 'Rol:', rol); // Debug

        localStorage.setItem('token', token);
        localStorage.setItem('kullaniciAd', isim);
        localStorage.setItem('kullaniciRol', rol);

        // Ana sayfaya yönlendir
        navigate('/dashboard');
      } else {
        // Hata varsa göster
        setHata(response.message || 'Giriş başarısız!');
      }
    } catch (error) {
      // API hatası
      console.error('Login hatası:', error);
      setHata(error.response?.data?.message || 'Bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-blue-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-lg shadow-xl p-8 w-full max-w-md">
        {/* Logo ve Başlık */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-500 rounded-full mb-4">
            <LogIn className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-gray-800">ClinickTrack</h1>
          <p className="text-gray-600 mt-2">Randevu Yönetim Sistemi</p>
        </div>

        {/* Hata mesajı */}
        {hata && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {hata}
          </div>
        )}

        {/* Login formu */}
        <form onSubmit={handleLogin}>
          {/* Email */}
          <div className="mb-4">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              Email
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
              placeholder="ornek@email.com"
              required
            />
          </div>

          {/* Parola */}
          <div className="mb-6">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              Parola
            </label>
            <input
              type="password"
              value={parola}
              onChange={(e) => setParola(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
              placeholder="••••••••"
              required
            />
          </div>

          {/* Giriş butonu */}
          <button
            type="submit"
            disabled={yukleniyor}
            className="w-full bg-blue-500 text-white py-3 rounded-lg font-semibold hover:bg-blue-600 transition duration-200 disabled:bg-gray-400"
          >
            {yukleniyor ? 'Giriş yapılıyor...' : 'Giriş Yap'}
          </button>
        </form>

        {/* Kayıt linki */}
        <div className="text-center mt-6">
          <p className="text-gray-600">
            Hesabınız yok mu?{' '}
            <Link to="/register" className="text-blue-500 hover:text-blue-700 font-semibold">
              Kayıt Ol
            </Link>
          </p>
        </div>

        {/* Test kullanıcı bilgisi */}
        <div className="mt-6 p-4 bg-gray-50 rounded border border-gray-200">
          <p className="text-xs text-gray-600 font-semibold mb-2">Test için:</p>
          <p className="text-xs text-gray-600">Admin: admin@clinicktrack.com / admin123</p>
        </div>
      </div>
    </div>
  );
}

export default Login;

