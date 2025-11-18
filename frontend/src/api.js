// API çağrıları için tek dosya - basit ve anlaşılır
import axios from 'axios';

// Backend URL'i
const API_URL = 'https://localhost:7117/api';

// Axios instance (her istekte kullanılacak)
const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Her istekte token'ı ekle (giriş yapmış kullanıcılar için)
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// LOGIN - Email ve parola ile giriş
export const login = async (email, parola) => {
  const response = await api.post('/Kullanici/giris', { 
    Email: email, 
    Parola: parola 
  });
  return response.data;
};

// REGISTER - Hasta kaydı (self-registration)
export const register = async (userData) => {
  const response = await api.post('/Kullanici/kayitOl', userData);
  return response.data;
};

// Profil bilgisi çek (giriş yapmış kullanıcı)
export const getProfile = async () => {
  const response = await api.get('/Kullanici/profil');
  return response.data;
};

export default api;

