import { useState } from 'react';
import { login } from '../api';
import { useNavigate } from 'react-router-dom';
import { Mail, Lock, Eye, EyeOff, Activity, ArrowRight } from 'lucide-react';

function Login() {
  const [email, setEmail] = useState('');
  const [parola, setParola] = useState('');
  const [hata, setHata] = useState('');
  const [yukleniyor, setYukleniyor] = useState(false);
  const [sifreGoster, setSifreGoster] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setHata('');
    setYukleniyor(true);

    try {
      const response = await login(email, parola);
      console.log('Login Response:', response);

      // Backend camelCase döndürüyor
      if (response.isSuccess) {
        console.log('Response data:', response.data); // Debug için
        
        const token = response.data.token;
        const isim = response.data.isim || response.data.İsim;
        const rol = response.data.rol || response.data.Rol;

        console.log('Token alındı:', token?.substring(0, 20) + '...', 'İsim:', isim, 'Rol:', rol, 'İlkGiris:', response.data.ilkGiris);

        // İlk giriş kontrolü
        if (response.data.ilkGiris === true) {
          // Parola belirleme sayfasına yönlendir
          navigate('/set-password', { state: { email: formData.email } });
          return;
        }

        localStorage.setItem('token', token);
        localStorage.setItem('kullaniciAd', isim);
        localStorage.setItem('kullaniciRol', rol);

        if (rol === 'Admin') navigate('/admin');
        else if (rol === 'Doktor') navigate('/doktor');
        else if (rol === 'Hasta') navigate('/hasta');
        else navigate('/');
      } else {
        setHata(response.message || 'Giriş başarısız!');
      }
    } catch (error) {
      console.error('Login hatası:', error);
      setHata('Sunucuya bağlanılamadı veya bir hata oluştu.');
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-gray-50">
      {/* SOL TARAF - Görsel ve Mesaj Alanı */}
      <div className="hidden lg:flex lg:w-1/2 bg-gradient-to-br from-blue-600 to-blue-800 relative overflow-hidden text-white flex-col justify-between p-12">
        {/* Dekoratif Daireler */}
        <div className="absolute top-0 left-0 w-64 h-64 bg-white opacity-5 rounded-full -translate-x-1/2 -translate-y-1/2"></div>
        <div className="absolute bottom-0 right-0 w-96 h-96 bg-white opacity-5 rounded-full translate-x-1/3 translate-y-1/3"></div>
        
        {/* Logo */}
        <div className="flex items-center gap-3 z-10">
          <div className="bg-white p-2 rounded-lg">
            <Activity className="w-6 h-6 text-blue-600" />
          </div>
          <span className="text-2xl font-bold tracking-wide">ClinicTrack</span>
        </div>

        {/* Orta Mesaj */}
        <div className="z-10 max-w-lg">
          <h1 className="text-5xl font-bold mb-6 leading-tight">
            Sağlığınız Bizim <br/>
            <span className="text-blue-200">Önceliğimiz</span>
          </h1>
          <p className="text-blue-100 text-lg mb-8 leading-relaxed">
            Online randevu sistemi ile doktorlarımızdan hemen randevu alın, 
            sağlık kayıtlarınızı kolayca takip edin.
          </p>
          
          {/* İstatistikler */}
          <div className="grid grid-cols-2 gap-8 mt-12">
            <div>
              <h3 className="text-3xl font-bold">15k+</h3>
              <p className="text-blue-200">Mutlu Hasta</p>
            </div>
            <div>
              <h3 className="text-3xl font-bold">50+</h3>
              <p className="text-blue-200">Uzman Doktor</p>
            </div>
          </div>
        </div>

        {/* Alt Footer */}
        <div className="z-10 text-blue-200 text-sm">
          © 2025 ClinicTrack. Tüm hakları saklıdır.
        </div>
      </div>

      {/* SAĞ TARAF - Form Alanı */}
      <div className="w-full lg:w-1/2 flex items-center justify-center p-8">
        <div className="w-full max-w-md bg-white p-8 rounded-2xl shadow-sm border border-gray-100">
          <div className="text-center mb-10">
            <h2 className="text-3xl font-bold text-gray-800 mb-2">Hoş Geldiniz</h2>
            <p className="text-gray-500">Lütfen hesabınıza giriş yapın</p>
          </div>

          {hata && (
            <div className="mb-6 bg-red-50 border-l-4 border-red-500 p-4 rounded-r text-red-700 text-sm">
              {hata}
            </div>
          )}

          <form onSubmit={handleLogin} className="space-y-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Email Adresi</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Mail className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type="email"
                  className="block w-full pl-10 pr-3 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition bg-gray-50 focus:bg-white"
                  placeholder="ornek@email.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Parola</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type={sifreGoster ? "text" : "password"}
                  className="block w-full pl-10 pr-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition bg-gray-50 focus:bg-white"
                  placeholder="••••••••"
                  value={parola}
                  onChange={(e) => setParola(e.target.value)}
                  required
                />
                <button
                  type="button"
                  className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600"
                  onClick={() => setSifreGoster(!sifreGoster)}
                >
                  {sifreGoster ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                </button>
              </div>
            </div>

            <div className="flex items-center justify-between text-sm">
              <div className="flex items-center">
                <input
                  id="remember-me"
                  type="checkbox"
                  className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <label htmlFor="remember-me" className="ml-2 block text-gray-600">
                  Beni hatırla
                </label>
              </div>
              <a href="#" className="font-medium text-blue-600 hover:text-blue-500">
                Şifremi unuttum?
              </a>
            </div>

            <button
              type="submit"
              disabled={yukleniyor}
              className="w-full flex justify-center items-center gap-2 py-3.5 px-4 border border-transparent rounded-xl shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
            >
              {yukleniyor ? 'Giriş Yapılıyor...' : 'Giriş Yap'}
              {!yukleniyor && <ArrowRight className="w-4 h-4" />}
            </button>
          </form>

          <div className="mt-8">
            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-gray-200"></div>
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-4 bg-white text-gray-500">Hesabınız yok mu?</span>
              </div>
            </div>

            <div className="mt-6">
              <button
                onClick={() => navigate('/register')}
                className="w-full flex justify-center py-3.5 px-4 border-2 border-blue-600 rounded-xl shadow-sm text-sm font-medium text-blue-600 bg-white hover:bg-blue-50 focus:outline-none transition-all"
              >
                Hemen Kayıt Ol
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;
