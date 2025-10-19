import { useState } from 'react';
import './UserLogin.css';

function UserLogin({ onUserSelect }) {
  const [nickname, setNickname] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!nickname.trim()) {
      setError('Lütfen bir rumuz girin');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      await onUserSelect(nickname.trim());
    } catch (err) {
      setError('Giriş yapılırken bir hata oluştu: ' + err.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="user-login-container">
      <div className="user-login-card">
        <h1>🎭 Duygu Analizi Chat</h1>
        <p className="subtitle">Mesajlarınız anlık olarak duygu analizi ile değerlendirilir</p>
        
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              type="text"
              placeholder="Rumuzunuzu girin..."
              value={nickname}
              onChange={(e) => setNickname(e.target.value)}
              disabled={isLoading}
              maxLength={50}
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Giriş yapılıyor...' : 'Sohbete Katıl'}
          </button>
        </form>

        <div className="info-box">
          <p>💡 İpucu: Mesajlarınızdaki duygular otomatik olarak analiz edilecek!</p>
        </div>
      </div>
    </div>
  );
}

export default UserLogin;

