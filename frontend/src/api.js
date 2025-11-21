// API çağrıları için tek dosya - DÜZELTİLMİŞ VERSİYON
import axios from 'axios';

// Backend URL'i
const API_URL = 'http://35.242.200.6/api';

// Axios instance
const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Token ekleme
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// ----------------------------------------------------------------------
// KULLANICI İŞLEMLERİ (Route: api/Kullanici)
// Backend'de [Route("api/Kullanici")] tanımlı olduğu için 'Kullanici' yazıyoruz.
// ----------------------------------------------------------------------

export const login = async (email, parola) => {
  const response = await api.post('/Kullanici/giris', { Email: email, Parola: parola });
  return response.data;
};

export const ilkParolaBelirle = async (email, yeniParola) => {
  const response = await api.post('/Kullanici/ilkParolaBelirle', { email, yeniParola });
  return response.data;
};

export const register = async (userData) => {
  const response = await api.post('/Kullanici/kayitOl', userData);
  return response.data;
};

export const getProfile = async () => {
  const response = await api.get('/Kullanici/profil');
  return response.data;
};

export const getAllUsers = async () => {
  const response = await api.get('/Kullanici/getAll');
  return response.data;
};

export const getUserById = async (id) => {
  const response = await api.get(`/Kullanici/getById/${id}`);
  return response.data;
};

export const deleteUser = async (id) => {
  const response = await api.delete(`/Kullanici/delete/${id}`);
  return response.data;
};

export const createUser = async (userData) => {
  const response = await api.post('/Kullanici/createUser', userData);
  return response.data;
};

export const updateUser = async (id, userData) => {
  const response = await api.put(`/Kullanici/update/${id}`, userData);
  return response.data;
};

export const updateUserPhone = async (id, yeniTelefon) => {
  const response = await api.put(`/Kullanici/updatePhone/${id}`, JSON.stringify(yeniTelefon));
  return response.data;
};

// ----------------------------------------------------------------------
// RANDEVU İŞLEMLERİ (Route: api/Randevu)
// Backend'de [Route("api/[controller]")] var, sınıf adı RandevuController -> 'Randevu'
// ----------------------------------------------------------------------

export const getAllAppointments = async () => {
  const response = await api.get('/Randevu/getAll');
  return response.data;
};

export const getPatientAppointments = async (hastaId) => {
  // Backend: [HttpGet("hasta/{hastaId}")]
  const response = await api.get(`/Randevu/hasta/${hastaId}`);
  return response.data;
};

export const getDoktorAppointments = async (doktorId) => {
  // Backend: [HttpGet("doktor/{doktorId}")]
  const response = await api.get(`/Randevu/doktor/${doktorId}`);
  return response.data;
};

export const cancelRandevu = async (id) => {
  const response = await api.put(`/Randevu/cancel/${id}`);
  return response.data;
};

export const updateRandevuDurum = async (id, durum) => {
  const response = await api.put(`/Randevu/updateStatus/${id}`, JSON.stringify(durum));
  return response.data;
};

export const createRandevu = async (randevuData) => {
  const response = await api.post('/Randevu/add', randevuData);
  return response.data;
};

export const deleteRandevu = async (id) => {
  const response = await api.delete(`/Randevu/delete/${id}`);
  return response.data;
};

export const addDoktorNote = async (id, note) => {
  const response = await api.put(`/Randevu/addNote/${id}`, JSON.stringify(note));
  return response.data;
};

export const completePastAppointments = async () => {
  const response = await api.post('/Randevu/completePastAppointments');
  return response.data;
};

export const getAvailableSlots = async (doktorId, tarih) => {
  const response = await api.get(`/Randevu/getAvailableSlots?doktorId=${doktorId}&tarih=${tarih}`);
  return response.data;
};

// ----------------------------------------------------------------------
// UZMANLIK İŞLEMLERİ (Route: api/Uzmanlik)
// Backend'de [Route("api/Uzmanlik")] tanımlı -> 'Uzmanlik'
// ----------------------------------------------------------------------

export const getAllUzmanliklar = async () => {
  const response = await api.get('/Uzmanlik/getAll');
  return response.data;
};

export const createUzmanlik = async (uzmanlikAdi) => {
  const response = await api.post('/Uzmanlik/add', JSON.stringify(uzmanlikAdi));
  return response.data;
};

export const updateUzmanlik = async (id, uzmanlikAdi) => {
  const response = await api.put(`/Uzmanlik/update/${id}`, JSON.stringify(uzmanlikAdi));
  return response.data;
};

export const deleteUzmanlik = async (id) => {
  const response = await api.delete(`/Uzmanlik/delete/${id}`);
  return response.data;
};

// ----------------------------------------------------------------------
// DOKTOR İŞLEMLERİ (Route: api/Doktor)
// Backend'de [Route("api/[controller]")] var, sınıf adı DoktorController -> 'Doktor'
// ----------------------------------------------------------------------

export const getAllDoktorlar = async () => {
  const response = await api.get('/Doktor/getAll');
  return response.data;
};

// ----------------------------------------------------------------------
// HASTA İŞLEMLERİ (Route: api/Hasta)
// Backend'de [Route("api/[controller]")] var, sınıf adı HastaController -> 'Hasta'
// ----------------------------------------------------------------------

export const getAllHasta = async () => {
  const response = await api.get('/Hasta/getAll');
  return response.data;
};

export const updateHasta = async (id, hastaData) => {
  const response = await api.put(`/Hasta/update/${id}`, hastaData);
  return response.data;
};

// HASTA PROFİLİNİ GETİR (Hasta Dashboard için)
export const getHastaProfil = async () => {
  const response = await api.get('/Hasta/profil');
  return response.data;
};

export default api;