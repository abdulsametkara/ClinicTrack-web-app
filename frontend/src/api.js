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
  console.log('Token gönderiliyor:', token ? 'Var' : 'Yok', token?.substring(0, 20) + '...'); // Debug
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

// İLK PAROLA BELİRLE
export const ilkParolaBelirle = async (email, yeniParola) => {
  const response = await api.post('/Kullanici/ilkParolaBelirle', {
    email: email,
    yeniParola: yeniParola
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

// TÜM KULLANICILARI GETİR (Admin için)
export const getAllUsers = async () => {
  const response = await api.get('/Kullanici/getAll');
  return response.data;
};

// ID İLE KULLANICI GETİR (Admin ve Doktor için)
export const getUserById = async (id) => {
  const response = await api.get(`/Kullanici/getById/${id}`);
  return response.data;
};

// KULLANICI SİL (Admin için)
export const deleteUser = async (id) => {
  const response = await api.delete(`/Kullanici/delete/${id}`);
  return response.data;
};

// YENİ KULLANICI OLUŞTUR (Admin için - Doktor, Admin, Hasta)
export const createUser = async (userData) => {
  const response = await api.post('/Kullanici/createUser', userData);
  return response.data;
};

// KULLANICI GÜNCELLE (Admin için)
export const updateUser = async (id, userData) => {
  const response = await api.put(`/Kullanici/update/${id}`, userData);
  return response.data;
};

// TÜM RANDEVULARI GETİR (Admin için)
export const getAllAppointments = async () => {
  const response = await api.get('/Randevu/getAll');
  return response.data;
};

// HASTA RANDEVULARINI GETİR
export const getPatientAppointments = async (hastaId) => {
  const response = await api.get(`/Randevu/hasta/${hastaId}`);
  return response.data;
};

// RANDEVU İPTAL
export const cancelRandevu = async (id) => {
  const response = await api.put(`/Randevu/cancel/${id}`);
  return response.data;
};

// HASTA BİLGİLERİNİ GÜNCELLE (Hasta için)
export const updateHasta = async (id, hastaData) => {
  const response = await api.put(`/Hasta/update/${id}`, hastaData);
  return response.data;
};

// TELEFON NUMARASI GÜNCELLE
export const updateUserPhone = async (id, yeniTelefon) => {
  const response = await api.put(`/Kullanici/updatePhone/${id}`, JSON.stringify(yeniTelefon), {
    headers: { 'Content-Type': 'application/json' }
  });
  return response.data;
};

// DOKTOR RANDEVULARINI GETİR (Doktor için)
export const getDoktorAppointments = async (doktorId) => {
  const response = await api.get(`/Randevu/doktor/${doktorId}`);
  return response.data;
};

// RANDEVU DURUM GUNCELLE
export const updateRandevuDurum = async (id, durum) => {
  const response = await api.put(`/Randevu/updateStatus/${id}`, JSON.stringify(durum), {
    headers: { 'Content-Type': 'application/json' }
  });
  return response.data;
};

// RANDEVU EKLE
export const createRandevu = async (randevuData) => {
  const response = await api.post('/Randevu/add', randevuData);
  return response.data;
};

// RANDEVU SIL
export const deleteRandevu = async (id) => {
  const response = await api.delete(`/Randevu/delete/${id}`);
  return response.data;
};

// DOKTOR NOTU EKLE
export const addDoktorNote = async (id, note) => {
  const response = await api.put(`/Randevu/addNote/${id}`, JSON.stringify(note), {
    headers: { 'Content-Type': 'application/json' }
  });
  return response.data;
};

// GEÇMİŞ RANDEVULARI TAMAMLA
export const completePastAppointments = async () => {
  const response = await api.post('/Randevu/completePastAppointments');
  return response.data;
};

// MÜSAİT RANDEVU SAATLERİNİ GETİR
export const getAvailableSlots = async (doktorId, tarih) => {
  const response = await api.get(`/Randevu/getAvailableSlots?doktorId=${doktorId}&tarih=${tarih}`);
  return response.data;
};

// TÜM UZMANLIKLARI GETİR
export const getAllUzmanliklar = async () => {
  const response = await api.get('/Uzmanlik/getAll');
  return response.data;
};

// UZMANLIK EKLE
export const createUzmanlik = async (uzmanlikAdi) => {
  const response = await api.post('/Uzmanlik/add', JSON.stringify(uzmanlikAdi), {
    headers: { 'Content-Type': 'application/json' }
  });
  return response.data;
};

// UZMANLIK GUNCELLE
export const updateUzmanlik = async (id, uzmanlikAdi) => {
  const response = await api.put(`/Uzmanlik/update/${id}`, JSON.stringify(uzmanlikAdi), {
    headers: { 'Content-Type': 'application/json' }
  });
  return response.data;
};

// UZMANLIK SIL
export const deleteUzmanlik = async (id) => {
  const response = await api.delete(`/Uzmanlik/delete/${id}`);
  return response.data;
};

// TÜM DOKTORLARI GETİR
export const getAllDoktorlar = async () => {
  const response = await api.get('/Doktor/getAll');
  return response.data;
};

// TÜM HASTALARI GETİR
export const getAllHasta = async () => {
  const response = await api.get('/Hasta/getAll');
  return response.data;
};

export default api;

