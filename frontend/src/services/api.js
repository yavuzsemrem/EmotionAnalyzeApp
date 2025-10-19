import axios from 'axios';

// Backend API URL'i - localhost için
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5053/api';

// Axios instance oluştur
const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// API fonksiyonları
export const userService = {
  // Tüm kullanıcıları getir
  getAll: async () => {
    const response = await api.get('/users');
    return response.data;
  },

  // Yeni kullanıcı oluştur
  create: async (nickname) => {
    const response = await api.post('/users', { nickname });
    return response.data;
  },

  // Kullanıcının mesajlarını getir
  getMessages: async (userId) => {
    const response = await api.get(`/users/${userId}/messages`);
    return response.data;
  },
};

export const messageService = {
  // Tüm mesajları getir
  getAll: async () => {
    const response = await api.get('/messages');
    return response.data;
  },

  // Yeni mesaj gönder
  create: async (userId, content) => {
    const response = await api.post('/messages', {
      userId,
      content,
      createdAt: new Date().toISOString(),
    });
    return response.data;
  },
};

export default api;

