import axios from 'axios';
import {User, Message} from '../types';

// Backend API URL'i - production için değiştir
// Localhost yerine bilgisayarınızın IP adresini kullanın (örn: 192.168.1.100)
const API_URL = 'http://10.0.2.2:5053/api'; // Android Emulator için
// const API_URL = 'http://192.168.1.100:5053/api'; // Gerçek cihaz için IP adresinizi girin
// const API_URL = 'https://your-render-url.onrender.com/api'; // Production için

// Axios instance oluştur
const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 60000, // 60 saniye (AI servisi yavaş olabilir)
});

// API fonksiyonları
export const userService = {
  // Tüm kullanıcıları getir
  getAll: async (): Promise<User[]> => {
    const response = await api.get<User[]>('/users');
    return response.data;
  },

  // Yeni kullanıcı oluştur
  create: async (nickname: string): Promise<User> => {
    const response = await api.post<User>('/users', {nickname});
    return response.data;
  },

  // Kullanıcının mesajlarını getir
  getMessages: async (userId: number): Promise<Message[]> => {
    const response = await api.get<Message[]>(`/users/${userId}/messages`);
    return response.data;
  },
};

export const messageService = {
  // Tüm mesajları getir
  getAll: async (): Promise<Message[]> => {
    const response = await api.get<Message[]>('/messages');
    return response.data;
  },

  // Yeni mesaj gönder
  create: async (userId: number, content: string): Promise<Message> => {
    const response = await api.post<Message>('/messages', {
      userId,
      content,
      createdAt: new Date().toISOString(),
    });
    return response.data;
  },
};

export default api;

