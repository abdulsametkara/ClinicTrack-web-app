// Kayıt sayfası - Yeni hasta kaydı
import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { register, login } from '../api';
import { UserPlus } from 'lucide-react';

function Register() {
  // Form verileri
  const [formData, setFormData] = useState({
    isim: '',
    soyisim: '',
    tcNo: '',
    email: '',
    parola: '',
    parolaOnay: '',
    telefonNumarası: '',
    doğumTarihi: '',
  });

  const [hata, setHata] = useState('');
  const [yukleniyor, setYukleniyor] = useState(false);
  const navigate = useNavigate();

  // Input değiştiğinde
  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  // Kayıt butonuna tıklandığında
  const handleRegister = async (e) => {
    e.preventDefault();
    setHata('');

    // Parola kontrolü
    if (formData.parola !== formData.parolaOnay) {
      setHata('Parolalar eşleşmiyor!');
      return;
    }

    // TC No kontrolü (11 haneli olmalı)
    if (formData.tcNo.length !== 11) {
      setHata('TC Kimlik No 11 haneli olmalıdır!');
      return;
    }

    setYukleniyor(true);

    try {
      // API'ye kayıt isteği gönder (Backend büyük harfle bekliyor)
      const response = await register({
        İsim: formData.isim,
        Soyisim: formData.soyisim,
        TCNo: formData.tcNo,
        Email: formData.email,
        Parola: formData.parola,
        TelefonNumarası: formData.telefonNumarası || '',
        DoğumTarihi: formData.doğumTarihi || null,
        Cinsiyet: '',
      });

      // Başarılı ise otomatik giriş yap
      if (response.isSuccess) {
        // Kayıt başarılı, şimdi otomatik giriş yap
        try {
          const loginResponse = await login(formData.email, formData.parola);
          
          console.log('Auto Login Response:', loginResponse); // Debug
          
          if (loginResponse.isSuccess) {
            // Token'ı sakla (camelCase veya PascalCase olabilir)
            const token = loginResponse.data.token || loginResponse.data.Token;
            const isim = loginResponse.data.isim || loginResponse.data.İsim;
            const rol = loginResponse.data.rol || loginResponse.data.Rol;

            localStorage.setItem('token', token);
            localStorage.setItem('kullaniciAd', isim);
            localStorage.setItem('kullaniciRol', rol);
            
            // Dashboard'a yönlendir
            alert('Kayıt başarılı! Hoş geldiniz!');
            navigate('/dashboard');
          } else {
            // Kayıt başarılı ama login olmadı
            alert('Kayıt başarılı! Lütfen giriş yapın.');
            navigate('/login');
          }
        } catch (loginError) {
          // Login hatası
          alert('Kayıt başarılı! Lütfen giriş yapın.');
          navigate('/login');
        }
      } else {
        setHata(response.message || 'Kayıt başarısız!');
      }
    } catch (error) {
      console.error('Register hatası:', error);
      setHata(error.response?.data?.message || 'Bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setYukleniyor(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-50 to-green-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-lg shadow-xl p-8 w-full max-w-2xl">
        {/* Başlık */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-green-500 rounded-full mb-4">
            <UserPlus className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-gray-800">Hasta Kaydı</h1>
          <p className="text-gray-600 mt-2">Bilgilerinizi girerek kayıt olun</p>
        </div>

        {/* Hata mesajı */}
        {hata && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {hata}
          </div>
        )}

        {/* Kayıt formu */}
        <form onSubmit={handleRegister}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* İsim */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                İsim *
              </label>
              <input
                type="text"
                name="isim"
                value={formData.isim}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                required
              />
            </div>

            {/* Soyisim */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Soyisim *
              </label>
              <input
                type="text"
                name="soyisim"
                value={formData.soyisim}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                required
              />
            </div>

            {/* TC No */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                TC Kimlik No *
              </label>
              <input
                type="text"
                name="tcNo"
                value={formData.tcNo}
                onChange={handleChange}
                maxLength="11"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                placeholder="12345678901"
                required
              />
            </div>

            {/* Email */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Email *
              </label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                placeholder="ornek@email.com"
                required
              />
            </div>

            {/* Telefon */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Telefon
              </label>
              <input
                type="tel"
                name="telefonNumarası"
                value={formData.telefonNumarası}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                placeholder="05XX XXX XX XX"
              />
            </div>

            {/* Doğum Tarihi */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Doğum Tarihi
              </label>
              <input
                type="date"
                name="doğumTarihi"
                value={formData.doğumTarihi}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
              />
            </div>

            {/* Parola */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Parola *
              </label>
              <input
                type="password"
                name="parola"
                value={formData.parola}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                placeholder="••••••••"
                required
              />
            </div>

            {/* Parola Onay */}
            <div>
              <label className="block text-gray-700 text-sm font-bold mb-2">
                Parola Onay *
              </label>
              <input
                type="password"
                name="parolaOnay"
                value={formData.parolaOnay}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-green-500"
                placeholder="••••••••"
                required
              />
            </div>
          </div>

          {/* Kayıt butonu */}
          <button
            type="submit"
            disabled={yukleniyor}
            className="w-full mt-6 bg-green-500 text-white py-3 rounded-lg font-semibold hover:bg-green-600 transition duration-200 disabled:bg-gray-400"
          >
            {yukleniyor ? 'Kayıt yapılıyor...' : 'Kayıt Ol'}
          </button>
        </form>

        {/* Login linki */}
        <div className="text-center mt-6">
          <p className="text-gray-600">
            Zaten hesabınız var mı?{' '}
            <Link to="/login" className="text-green-500 hover:text-green-700 font-semibold">
              Giriş Yap
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}

export default Register;

