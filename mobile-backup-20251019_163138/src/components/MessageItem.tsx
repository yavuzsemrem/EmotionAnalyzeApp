import React from 'react';
import {View, Text, StyleSheet} from 'react-native';
import {Message} from '../types';

interface MessageItemProps {
  message: Message;
  isOwnMessage: boolean;
}

const MessageItem: React.FC<MessageItemProps> = ({message, isOwnMessage}) => {
  // Duygu skoruna göre emoji seç
  const getEmotionEmoji = () => {
    const {positiveScore, negativeScore, neutralScore} = message;
    if (positiveScore > negativeScore && positiveScore > neutralScore) {
      return '😊';
    }
    if (negativeScore > positiveScore && negativeScore > neutralScore) {
      return '😔';
    }
    return '😐';
  };

  // Dominant duyguyu bul
  const getDominantEmotion = () => {
    const {positiveScore, negativeScore, neutralScore} = message;
    if (positiveScore > negativeScore && positiveScore > neutralScore) {
      return {name: 'Pozitif', score: positiveScore, color: '#10b981'};
    }
    if (negativeScore > positiveScore && negativeScore > neutralScore) {
      return {name: 'Negatif', score: negativeScore, color: '#ef4444'};
    }
    return {name: 'Nötr', score: neutralScore, color: '#94a3b8'};
  };

  const emotion = getDominantEmotion();
  const emoji = getEmotionEmoji();

  // Tarihi formatla
  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleTimeString('tr-TR', {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <View
      style={[
        styles.container,
        isOwnMessage ? styles.ownMessage : styles.otherMessage,
      ]}>
      {/* Kullanıcı bilgisi (başkasının mesajıysa) */}
      {!isOwnMessage && (
        <View style={styles.userInfo}>
          <View style={styles.avatar}>
            <Text style={styles.avatarText}>
              {message.user?.nickname.charAt(0).toUpperCase()}
            </Text>
          </View>
          <Text style={styles.username}>{message.user?.nickname}</Text>
        </View>
      )}

      {/* Mesaj balonu */}
      <View
        style={[
          styles.bubble,
          isOwnMessage ? styles.ownBubble : styles.otherBubble,
        ]}>
        {/* Mesaj içeriği */}
        <Text
          style={[
            styles.content,
            isOwnMessage ? styles.ownContent : styles.otherContent,
          ]}>
          {message.content}
        </Text>

        {/* Duygu analizi sonucu */}
        <View style={styles.emotionContainer}>
          <Text style={styles.emoji}>{emoji}</Text>
          <Text
            style={[
              styles.emotionText,
              isOwnMessage ? styles.ownEmotionText : styles.otherEmotionText,
            ]}>
            {emotion.name} {Math.round(emotion.score * 100)}%
          </Text>
        </View>

        {/* Saat */}
        <Text
          style={[
            styles.time,
            isOwnMessage ? styles.ownTime : styles.otherTime,
          ]}>
          {formatTime(message.createdAt)}
        </Text>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginVertical: 8,
    marginHorizontal: 16,
  },
  ownMessage: {
    alignItems: 'flex-end',
  },
  otherMessage: {
    alignItems: 'flex-start',
  },
  userInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 6,
    marginLeft: 4,
  },
  avatar: {
    width: 28,
    height: 28,
    borderRadius: 14,
    backgroundColor: '#667eea',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 8,
  },
  avatarText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: 'bold',
  },
  username: {
    fontSize: 13,
    fontWeight: '600',
    color: '#64748b',
  },
  bubble: {
    maxWidth: '80%',
    padding: 12,
    borderRadius: 16,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.1,
    shadowRadius: 2,
  },
  ownBubble: {
    backgroundColor: '#667eea',
    borderBottomRightRadius: 4,
  },
  otherBubble: {
    backgroundColor: '#fff',
    borderBottomLeftRadius: 4,
  },
  content: {
    fontSize: 16,
    lineHeight: 22,
    marginBottom: 8,
  },
  ownContent: {
    color: '#fff',
  },
  otherContent: {
    color: '#1e293b',
  },
  emotionContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 4,
  },
  emoji: {
    fontSize: 16,
    marginRight: 6,
  },
  emotionText: {
    fontSize: 13,
    fontWeight: '600',
  },
  ownEmotionText: {
    color: '#fff',
    opacity: 0.9,
  },
  otherEmotionText: {
    color: '#667eea',
  },
  time: {
    fontSize: 11,
  },
  ownTime: {
    color: '#fff',
    opacity: 0.7,
    textAlign: 'right',
  },
  otherTime: {
    color: '#94a3b8',
    textAlign: 'right',
  },
});

export default MessageItem;

