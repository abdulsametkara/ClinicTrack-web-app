import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { ilkParolaBelirle } from '../api';
import { Lock, Eye, EyeOff } from 'lucide-react';

function SetPassword() {
  const navigate = useNavigate();
  const location = useLocation();
  const email = location.state?.email || '';
  
  const [formData, setFormData] = useState({
    yeniParola: '',
    parolaOnay: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // Validasyonlar
    if (formData.yeniParola.length < 6) {
      setError('Parola en az 6 karakter olmalıdır.');
      return;
    }

    if (formData.yeniParola !== formData.parolaOnay) {
      setError('Parolalar eşleşmiyor.');
      return;
    }

    setLoading(true);

    try {
      const response = await ilkParolaBelirle(email, formData.yeniParola);
      
      if (response.isSuccess) {
        alert('Parola başarıyla belirlendi! Şimdi giriş yapabilirsiniz.');
        // Token'ı ve kullanıcı bilgilerini temizle
        localStorage.removeItem('token');
        localStorage.removeItem('kullaniciRol');
        localStorage.removeItem('kullaniciIsim');
        // Login sayfasına yönlendir
        navigate('/login');
      } else {
        setError(response.message || 'Bir hata oluştu.');
      }
    } catch (err) {
      console.error('Parola belirleme hatası:', err);
      setError('Parola belirlenirken bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo ve Başlık */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-2xl mb-4 shadow-lg">
            <Lock className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Parola Belirle</h1>
          <p className="text-gray-600">İlk girişiniz için lütfen bir parola belirleyin</p>
        </div>

        {/* Form */}
        <div className="bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Email (disabled) */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Email
              </label>
              <input
                type="email"
                value={email}
                disabled
                className="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl text-gray-500 cursor-not-allowed"
              />
            </div>

            {/* Yeni Parola */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Yeni Parola
              </label>
              <div className="relative">
                <input
                  type={showPassword ? 'text' : 'password'}
                  value={formData.yeniParola}
                  onChange={(e) => setFormData({ ...formData, yeniParola: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition"
                  placeholder="En az 6 karakter"
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
            </div>

            {/* Parola Onay */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Parola Onay
              </label>
              <div className="relative">
                <input
                  type={showConfirmPassword ? 'text' : 'password'}
                  value={formData.parolaOnay}
                  onChange={(e) => setFormData({ ...formData, parolaOnay: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition"
                  placeholder="Parolayı tekrar girin"
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showConfirmPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
            </div>

            {/* Hata Mesajı */}
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
                {error}
              </div>
            )}

            {/* Submit Button */}
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gradient-to-r from-blue-500 to-purple-600 text-white py-3 rounded-xl font-semibold hover:from-blue-600 hover:to-purple-700 transition duration-200 shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? 'Kaydediliyor...' : 'Parolayı Kaydet'}
            </button>
          </form>
        </div>

        {/* Bilgilendirme */}
        <div className="mt-6 text-center text-sm text-gray-600">
          <p>Bu işlemden sonra belirlediğiniz parola ile giriş yapabileceksiniz.</p>
        </div>
      </div>
    </div>
  );
}

export default SetPassword;




