import { useState } from 'react';
import { register, login } from '../api';
import { useNavigate } from 'react-router-dom';
import { User, Mail, Lock, Phone, Calendar, Activity, ArrowRight, CheckCircle } from 'lucide-react';

function Register() {
  const [formData, setFormData] = useState({
    isim: '',
    soyisim: '',
    email: '',
    parola: '',
    tcNo: '',
    telefonNumarası: '',
    doğumTarihi: '',
  });

  const [rol, setRol] = useState('Hasta'); // Varsayılan rol
  const [hata, setHata] = useState('');
  const [yukleniyor, setYukleniyor] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setHata('');
    setYukleniyor(true);

    try {
      // Kayıt isteği gönder
      const response = await register({
        İsim: formData.isim,
        Soyisim: formData.soyisim,
        TCNo: formData.tcNo,
        Email: formData.email,
        Parola: formData.parola,
        TelefonNumarası: formData.telefonNumarası || '',
        DoğumTarihi: formData.doğumTarihi || null,
        Rol: rol, // Seçilen rolü gönder (Backend destekliyorsa)
        Cinsiyet: '', // Opsiyonel
      });

      // Backend camelCase döndürüyor
      if (response.isSuccess) {
        // Kayıt başarılı, otomatik giriş yap
        try {
          const loginResponse = await login(formData.email, formData.parola);
          
          if (loginResponse.isSuccess) {
            const token = loginResponse.data.token;
            const isim = loginResponse.data.isim;
            const userRol = loginResponse.data.rol;

            localStorage.setItem('token', token);
            localStorage.setItem('kullaniciAd', isim);
            localStorage.setItem('kullaniciRol', userRol);
            
            // Rolüne göre yönlendir
            if (userRol === 'Admin') navigate('/admin');
            else if (userRol === 'Doktor') navigate('/doktor');
            else if (userRol === 'Hasta') navigate('/hasta');
            else navigate('/');
          } else {
            navigate('/login');
          }
        } catch (loginError) {
          navigate('/login');
        }
      } else {
        setHata(response.message || 'Kayıt işlemi başarısız!');
      }
    } catch (error) {
      console.error('Kayıt hatası:', error);
      setHata('Sunucuya bağlanılamadı.');
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="min-h-screen flex bg-gray-50">
      {/* SOL TARAF - Görsel Alan */}
      <div className="hidden lg:flex lg:w-1/2 bg-gradient-to-br from-blue-600 to-blue-800 relative overflow-hidden text-white flex-col justify-between p-12">
        <div className="absolute top-0 left-0 w-64 h-64 bg-white opacity-5 rounded-full -translate-x-1/2 -translate-y-1/2"></div>
        <div className="absolute bottom-0 right-0 w-96 h-96 bg-white opacity-5 rounded-full translate-x-1/3 translate-y-1/3"></div>
        
        <div className="flex items-center gap-3 z-10">
          <div className="bg-white p-2 rounded-lg">
            <Activity className="w-6 h-6 text-blue-600" />
          </div>
          <span className="text-2xl font-bold tracking-wide">ClinicTrack</span>
        </div>

        <div className="z-10 max-w-lg">
          <h1 className="text-5xl font-bold mb-6 leading-tight">
            Aramıza <br/>
            <span className="text-blue-200">Katılın</span>
          </h1>
          <p className="text-blue-100 text-lg mb-8 leading-relaxed">
            Binlerce hasta ve doktorun buluştuğu güvenilir platformda yerinizi alın.
            Hızlı, kolay ve güvenli sağlık hizmeti.
          </p>
          
          <div className="space-y-4">
            <div className="flex items-center gap-3">
              <div className="bg-blue-500 p-2 rounded-full bg-opacity-30">
                <CheckCircle className="w-5 h-5" />
              </div>
              <span>7/24 Online Randevu</span>
            </div>
            <div className="flex items-center gap-3">
              <div className="bg-blue-500 p-2 rounded-full bg-opacity-30">
                <CheckCircle className="w-5 h-5" />
              </div>
              <span>Uzman Doktor Ağı</span>
            </div>
            <div className="flex items-center gap-3">
              <div className="bg-blue-500 p-2 rounded-full bg-opacity-30">
                <CheckCircle className="w-5 h-5" />
              </div>
              <span>Geçmiş Sağlık Kayıtları</span>
            </div>
          </div>
        </div>

        <div className="z-10 text-blue-200 text-sm">
          © 2025 ClinicTrack. Tüm hakları saklıdır.
        </div>
      </div>

      {/* SAĞ TARAF - Kayıt Formu */}
      <div className="w-full lg:w-1/2 flex items-center justify-center p-8">
        <div className="w-full max-w-lg bg-white p-8 rounded-2xl shadow-sm border border-gray-100">
          <div className="text-center mb-8">
            <h2 className="text-3xl font-bold text-gray-800 mb-2">Hesap Oluşturun</h2>
            <p className="text-gray-500">Birkaç adımda üyeliğinizi tamamlayın</p>
          </div>

          {hata && (
            <div className="mb-6 bg-red-50 border-l-4 border-red-500 p-4 rounded-r text-red-700 text-sm">
              {hata}
            </div>
          )}

          <form onSubmit={handleRegister} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Ad</label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <User className="h-5 w-5 text-gray-400" />
                  </div>
                  <input
                    name="isim"
                    type="text"
                    className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                    placeholder="Adınız"
                    required
                    onChange={handleChange}
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Soyad</label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <User className="h-5 w-5 text-gray-400" />
                  </div>
                  <input
                    name="soyisim"
                    type="text"
                    className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                    placeholder="Soyadınız"
                    required
                    onChange={handleChange}
                  />
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">TC Kimlik No</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Activity className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  name="tcNo"
                  type="text"
                  maxLength="11"
                  className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                  placeholder="11 Haneli TC Kimlik No"
                  required
                  onChange={handleChange}
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Mail className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  name="email"
                  type="email"
                  className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                  placeholder="ornek@email.com"
                  required
                  onChange={handleChange}
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Telefon</label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Phone className="h-5 w-5 text-gray-400" />
                  </div>
                  <input
                    name="telefonNumarası"
                    type="tel"
                    className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                    placeholder="5XX..."
                    onChange={handleChange}
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Doğum Tarihi</label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Calendar className="h-5 w-5 text-gray-400" />
                  </div>
                  <input
                    name="doğumTarihi"
                    type="date"
                    className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                    onChange={handleChange}
                  />
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Parola</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  name="parola"
                  type="password"
                  className="block w-full pl-10 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none bg-gray-50 focus:bg-white transition"
                  placeholder="••••••••"
                  required
                  onChange={handleChange}
                />
              </div>
            </div>

          

            <div className="flex items-start gap-2 pt-2">
              <input
                type="checkbox"
                id="terms"
                className="mt-1 h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                required
              />
              <label htmlFor="terms" className="text-sm text-gray-500">
                <a href="#" className="text-blue-600 hover:underline">Kullanım şartlarını</a> ve <a href="#" className="text-blue-600 hover:underline">gizlilik politikasını</a> kabul ediyorum.
              </label>
            </div>

            <button
              type="submit"
              disabled={yukleniyor}
              className="w-full flex justify-center items-center gap-2 py-3.5 px-4 border border-transparent rounded-xl shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 transition-all mt-6"
            >
              {yukleniyor ? 'Kayıt Yapılıyor...' : 'Kayıt Ol'}
              {!yukleniyor && <ArrowRight className="w-4 h-4" />}
            </button>

            <div className="text-center mt-4">
              <span className="text-sm text-gray-500">Zaten hesabınız var mı? </span>
              <button
                type="button"
                onClick={() => navigate('/login')}
                className="text-sm font-medium text-blue-600 hover:text-blue-500"
              >
                Giriş Yapın
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Register;
