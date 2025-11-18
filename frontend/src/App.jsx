// Ana uygulama - Sayfa yönlendirmeleri (routing)
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';

function App() {
  // Token var mı kontrol et (giriş yapmış mı?)
  const isLoggedIn = () => {
    return localStorage.getItem('token') !== null;
  };

  // Giriş yapmış kullanıcıyı koruma (Dashboard'a erişim için)
  const ProtectedRoute = ({ children }) => {
    if (!isLoggedIn()) {
      return <Navigate to="/login" />;
    }
    return children;
  };

  return (
    <BrowserRouter>
      <Routes>
        {/* Ana sayfa - eğer giriş yapmışsa dashboard'a yönlendir */}
        <Route 
          path="/" 
          element={isLoggedIn() ? <Navigate to="/dashboard" /> : <Navigate to="/login" />} 
        />
        
        {/* Login sayfası */}
        <Route path="/login" element={<Login />} />
        
        {/* Register sayfası */}
        <Route path="/register" element={<Register />} />
        
        {/* Dashboard - sadece giriş yapmış kullanıcılar erişebilir */}
        <Route 
          path="/dashboard" 
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          } 
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
