import './MessageItem.css';

function MessageItem({ message, isOwnMessage }) {
  // Duygu skorlarına göre emoji ve renk
  const getEmotionDisplay = () => {
    const { positiveScore, negativeScore, neutralScore } = message;
    
    if (positiveScore > negativeScore && positiveScore > neutralScore) {
      return { emoji: '😊', label: 'Pozitif', color: '#4caf50', score: positiveScore };
    }
    if (negativeScore > positiveScore && negativeScore > neutralScore) {
      return { emoji: '😔', label: 'Negatif', color: '#f44336', score: negativeScore };
    }
    return { emoji: '😐', label: 'Nötr', color: '#9e9e9e', score: neutralScore };
  };

  const emotion = getEmotionDisplay();
  
  // Tarih formatla
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleTimeString('tr-TR', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  };

  return (
    <div className={`message-item ${isOwnMessage ? 'own-message' : 'other-message'}`}>
      <div className="message-bubble">
        <div className="message-header">
          <span className="message-user">
            {message.user?.nickname || 'Anonim'}
          </span>
          <span className="message-time">
            {formatDate(message.createdAt)}
          </span>
        </div>
        
        <div className="message-content">
          {message.content}
        </div>
        
        <div className="message-emotion" style={{ borderLeftColor: emotion.color }}>
          <span className="emotion-emoji">{emotion.emoji}</span>
          <span className="emotion-label">{emotion.label}</span>
          <span className="emotion-score">{(emotion.score * 100).toFixed(0)}%</span>
        </div>
      </div>
    </div>
  );
}

export default MessageItem;

