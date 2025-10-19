import { useState, useEffect, useRef } from 'react';
import { messageService } from '../services/api';
import MessageItem from './MessageItem';
import './ChatRoom.css';

function ChatRoom({ user, onLogout }) {
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const [error, setError] = useState('');
  const messagesEndRef = useRef(null);

  // Mesajları yükle
  const loadMessages = async () => {
    setIsLoading(true);
    setError('');
    try {
      const data = await messageService.getAll();
      setMessages(data);
    } catch (err) {
      setError('Mesajlar yüklenirken hata oluştu: ' + err.message);
    } finally {
      setIsLoading(false);
    }
  };

  // Component mount olduğunda mesajları yükle
  useEffect(() => {
    loadMessages();
    
    // Her 5 saniyede bir mesajları güncelle (polling)
    const interval = setInterval(loadMessages, 5000);
    
    return () => clearInterval(interval);
  }, []);

  // Yeni mesaj geldiğinde scroll down
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  // Mesaj gönder
  const handleSendMessage = async (e) => {
    e.preventDefault();
    
    if (!newMessage.trim()) return;

    setIsSending(true);
    setError('');

    try {
      const sentMessage = await messageService.create(user.id, newMessage.trim());
      setMessages(prev => [...prev, sentMessage]);
      setNewMessage('');
    } catch (err) {
      setError('Mesaj gönderilirken hata oluştu: ' + err.message);
    } finally {
      setIsSending(false);
    }
  };

  // Duygu skoruna göre emoji
  const getEmotionEmoji = (positive, negative, neutral) => {
    if (positive > negative && positive > neutral) return '😊';
    if (negative > positive && negative > neutral) return '😔';
    return '😐';
  };

  return (
    <div className="chat-room">
      {/* Header */}
      <div className="chat-header">
        <div className="user-info">
          <div className="user-avatar">{user.nickname.charAt(0).toUpperCase()}</div>
          <div>
            <h2>Duygu Analizi Chat</h2>
            <p className="user-name">{user.nickname}</p>
          </div>
        </div>
        <button className="logout-btn" onClick={onLogout}>
          Çıkış Yap
        </button>
      </div>

      {/* Messages */}
      <div className="chat-messages">
        {isLoading && messages.length === 0 ? (
          <div className="loading">Mesajlar yükleniyor...</div>
        ) : messages.length === 0 ? (
          <div className="no-messages">
            <p>🎉 İlk mesajı sen gönder!</p>
          </div>
        ) : (
          messages.map((message) => (
            <MessageItem
              key={message.id}
              message={message}
              isOwnMessage={message.userId === user.id}
            />
          ))
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Error Message */}
      {error && (
        <div className="error-banner">
          {error}
        </div>
      )}

      {/* Input */}
      <form className="chat-input" onSubmit={handleSendMessage}>
        <input
          type="text"
          placeholder="Mesajınızı yazın..."
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          disabled={isSending}
          maxLength={500}
        />
        <button type="submit" disabled={isSending || !newMessage.trim()}>
          {isSending ? '⏳' : '📤'}
        </button>
      </form>
    </div>
  );
}

export default ChatRoom;

