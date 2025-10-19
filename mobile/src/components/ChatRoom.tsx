import React, {useState, useEffect, useRef} from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  FlatList,
  StyleSheet,
  ActivityIndicator,
  Alert,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import {messageService} from '../services/api';
import MessageItem from './MessageItem';
import {User, Message} from '../types';

interface ChatRoomProps {
  user: User;
  onLogout: () => void;
}

const ChatRoom: React.FC<ChatRoomProps> = ({user, onLogout}) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const flatListRef = useRef<FlatList>(null);

  // Mesajları yükle
  const loadMessages = async () => {
    try {
      const data = await messageService.getAll();
      setMessages(data);
      // Yeni mesaj geldiğinde en alta scroll
      setTimeout(() => {
        flatListRef.current?.scrollToEnd({animated: true});
      }, 100);
    } catch (error: any) {
      console.error('Mesajlar yüklenirken hata:', error);
      // İlk yükleme hatası göster, polling hataları sessiz
      if (messages.length === 0) {
        Alert.alert(
          'Hata',
          'Mesajlar yüklenirken bir hata oluştu. Backend servisinin çalıştığından emin olun.',
        );
      }
    }
  };

  // Component mount olduğunda mesajları yükle
  useEffect(() => {
    setIsLoading(true);
    loadMessages().finally(() => setIsLoading(false));

    // Her 5 saniyede bir mesajları güncelle (polling)
    const interval = setInterval(() => {
      loadMessages();
    }, 5000);

    return () => clearInterval(interval);
  }, []);

  // Mesaj gönder
  const handleSendMessage = async () => {
    if (!newMessage.trim()) {
      return;
    }

    const messageToSend = newMessage.trim();
    setNewMessage('');
    setIsSending(true);

    try {
      const sentMessage = await messageService.create(user.id, messageToSend);
      setMessages(prev => [...prev, sentMessage]);
      // Mesaj gönderildikten sonra en alta scroll
      setTimeout(() => {
        flatListRef.current?.scrollToEnd({animated: true});
      }, 100);
    } catch (error: any) {
      console.error('Mesaj gönderilirken hata:', error);
      setNewMessage(messageToSend); // Mesajı geri koy
      Alert.alert(
        'Hata',
        'Mesaj gönderilirken bir hata oluştu. Lütfen backend ve AI servisinin çalıştığından emin olun.',
      );
    } finally {
      setIsSending(false);
    }
  };

  const handleLogoutPress = () => {
    Alert.alert('Çıkış Yap', 'Sohbetten çıkmak istediğinize emin misiniz?', [
      {text: 'İptal', style: 'cancel'},
      {text: 'Çıkış Yap', onPress: onLogout, style: 'destructive'},
    ]);
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}
      keyboardVerticalOffset={Platform.OS === 'ios' ? 0 : 20}>
      {/* Header */}
      <View style={styles.header}>
        <View style={styles.userInfo}>
          <View style={styles.avatar}>
            <Text style={styles.avatarText}>
              {user.nickname.charAt(0).toUpperCase()}
            </Text>
          </View>
          <View>
            <Text style={styles.title}>Duygu Analizi Chat</Text>
            <Text style={styles.username}>{user.nickname}</Text>
          </View>
        </View>
        <TouchableOpacity onPress={handleLogoutPress} style={styles.logoutBtn}>
          <Text style={styles.logoutText}>Çıkış</Text>
        </TouchableOpacity>
      </View>

      {/* Messages */}
      {isLoading && messages.length === 0 ? (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color="#667eea" />
          <Text style={styles.loadingText}>Mesajlar yükleniyor...</Text>
        </View>
      ) : messages.length === 0 ? (
        <View style={styles.emptyContainer}>
          <Text style={styles.emptyEmoji}>🎉</Text>
          <Text style={styles.emptyText}>İlk mesajı sen gönder!</Text>
          <Text style={styles.emptySubtext}>
            Mesajın AI tarafından analiz edilecek
          </Text>
        </View>
      ) : (
        <FlatList
          ref={flatListRef}
          data={messages}
          keyExtractor={item => item.id.toString()}
          renderItem={({item}) => (
            <MessageItem message={item} isOwnMessage={item.userId === user.id} />
          )}
          contentContainerStyle={styles.messagesList}
          onContentSizeChange={() =>
            flatListRef.current?.scrollToEnd({animated: true})
          }
        />
      )}

      {/* Input */}
      <View style={styles.inputContainer}>
        <TextInput
          style={styles.input}
          placeholder="Mesajınızı yazın..."
          placeholderTextColor="#94a3b8"
          value={newMessage}
          onChangeText={setNewMessage}
          maxLength={500}
          multiline
          editable={!isSending}
          onSubmitEditing={handleSendMessage}
        />
        <TouchableOpacity
          style={[
            styles.sendButton,
            (!newMessage.trim() || isSending) && styles.sendButtonDisabled,
          ]}
          onPress={handleSendMessage}
          disabled={!newMessage.trim() || isSending}>
          {isSending ? (
            <ActivityIndicator color="#fff" size="small" />
          ) : (
            <Text style={styles.sendButtonText}>📤</Text>
          )}
        </TouchableOpacity>
      </View>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f7fa',
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#667eea',
    padding: 16,
    paddingTop: 12,
    elevation: 4,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.2,
    shadowRadius: 4,
  },
  userInfo: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  avatar: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#fff',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  avatarText: {
    color: '#667eea',
    fontSize: 18,
    fontWeight: 'bold',
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#fff',
  },
  username: {
    fontSize: 14,
    color: '#e0e7ff',
  },
  logoutBtn: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 8,
  },
  logoutText: {
    color: '#fff',
    fontWeight: '600',
    fontSize: 14,
  },
  messagesList: {
    paddingVertical: 8,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  loadingText: {
    marginTop: 12,
    fontSize: 16,
    color: '#64748b',
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 40,
  },
  emptyEmoji: {
    fontSize: 64,
    marginBottom: 16,
  },
  emptyText: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#1e293b',
    marginBottom: 8,
  },
  emptySubtext: {
    fontSize: 14,
    color: '#64748b',
    textAlign: 'center',
  },
  inputContainer: {
    flexDirection: 'row',
    padding: 12,
    backgroundColor: '#fff',
    borderTopWidth: 1,
    borderTopColor: '#e2e8f0',
    alignItems: 'flex-end',
  },
  input: {
    flex: 1,
    backgroundColor: '#f8fafc',
    borderRadius: 20,
    paddingHorizontal: 16,
    paddingVertical: 10,
    fontSize: 16,
    color: '#1e293b',
    maxHeight: 100,
    marginRight: 8,
  },
  sendButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: '#667eea',
    justifyContent: 'center',
    alignItems: 'center',
    elevation: 2,
    shadowColor: '#667eea',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.3,
    shadowRadius: 4,
  },
  sendButtonDisabled: {
    backgroundColor: '#cbd5e1',
    elevation: 0,
  },
  sendButtonText: {
    fontSize: 20,
  },
});

export default ChatRoom;

