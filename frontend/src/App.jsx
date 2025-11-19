// Ana uygulama - Sayfa yönlendirmeleri (routing)
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import AdminDashboard from './pages/AdminDashboard';
import DoktorDashboard from './pages/DoktorDashboard';
import HastaDashboard from './pages/HastaDashboard';

function App() {
  // Token var mı kontrol et (giriş yapmış mı?)
  const isLoggedIn = () => {
    return localStorage.getItem('token') !== null;
  };

  // Kullanıcı rolünü al
  const getUserRole = () => {
    return localStorage.getItem('kullaniciRol');
  };

  // Kullanıcıyı rolüne göre yönlendir
  const getDashboardPath = () => {
    const rol = getUserRole();
    if (rol === 'Admin') return '/admin';
    if (rol === 'Doktor') return '/doktor';
    if (rol === 'Hasta') return '/hasta';
    return '/login'; // Rol bulunamazsa login'e gönder
  };

  // Giriş yapmış kullanıcıyı koruma ve rol kontrolü
  const ProtectedRoute = ({ children, allowedRole }) => {
    if (!isLoggedIn()) {
      return <Navigate to="/login" />;
    }

    const userRole = getUserRole();
    
    // Eğer rol belirtilmişse ve kullanıcının rolü uyuşmuyorsa, doğru panele yönlendir
    if (allowedRole && userRole !== allowedRole) {
      return <Navigate to={getDashboardPath()} />;
    }

    return children;
  };

  return (
    <BrowserRouter>
      <Routes>
        {/* Ana sayfa - giriş yapmışsa rolüne göre dashboard'a yönlendir */}
        <Route 
          path="/" 
          element={isLoggedIn() ? <Navigate to={getDashboardPath()} /> : <Navigate to="/login" />} 
        />
        
        {/* Login sayfası */}
        <Route path="/login" element={<Login />} />
        
        {/* Register sayfası */}
        <Route path="/register" element={<Register />} />
        
        {/* Admin Panel - sadece Admin rolü erişebilir */}
        <Route 
          path="/admin" 
          element={
            <ProtectedRoute allowedRole="Admin">
              <AdminDashboard />
            </ProtectedRoute>
          } 
        />

        {/* Doktor Panel - sadece Doktor rolü erişebilir */}
        <Route 
          path="/doktor" 
          element={
            <ProtectedRoute allowedRole="Doktor">
              <DoktorDashboard />
            </ProtectedRoute>
          } 
        />

        {/* Hasta Panel - sadece Hasta rolü erişebilir */}
        <Route 
          path="/hasta" 
          element={
            <ProtectedRoute allowedRole="Hasta">
              <HastaDashboard />
            </ProtectedRoute>
          } 
        />

        {/* Eski /dashboard route'unu yönlendir */}
        <Route 
          path="/dashboard" 
          element={<Navigate to={getDashboardPath()} />} 
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
