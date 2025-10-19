import React, {useState} from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import {userService} from '../services/api';
import {User} from '../types';

interface UserLoginProps {
  onUserSelect: (user: User) => void;
}

const UserLogin: React.FC<UserLoginProps> = ({onUserSelect}) => {
  const [nickname, setNickname] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async () => {
    if (!nickname.trim()) {
      Alert.alert('Hata', 'Lütfen bir rumuz girin');
      return;
    }

    setIsLoading(true);
    try {
      // Yeni kullanıcı oluştur
      const newUser = await userService.create(nickname.trim());
      onUserSelect(newUser);
    } catch (error: any) {
      console.error('Kullanıcı oluşturulurken hata:', error);
      Alert.alert(
        'Hata',
        'Kullanıcı oluşturulurken bir hata oluştu. Lütfen backend servisinin çalıştığından emin olun.',
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}>
      <View style={styles.content}>
        {/* Header */}
        <View style={styles.header}>
          <Text style={styles.emoji}>🎭</Text>
          <Text style={styles.title}>Duygu Analizi Chat</Text>
          <Text style={styles.subtitle}>
            Mesajlarınızın duygusal tonunu AI ile analiz edelim
          </Text>
        </View>

        {/* Input */}
        <View style={styles.inputContainer}>
          <Text style={styles.label}>Rumuzunuzu Girin</Text>
          <TextInput
            style={styles.input}
            placeholder="Örn: Ahmet, Ayşe, Mehmet..."
            placeholderTextColor="#94a3b8"
            value={nickname}
            onChangeText={setNickname}
            maxLength={20}
            autoCapitalize="words"
            autoCorrect={false}
            editable={!isLoading}
            onSubmitEditing={handleLogin}
          />
          <Text style={styles.hint}>
            💡 Sohbete katılmak için sadece bir rumuz yeterli
          </Text>
        </View>

        {/* Button */}
        <TouchableOpacity
          style={[styles.button, isLoading && styles.buttonDisabled]}
          onPress={handleLogin}
          disabled={isLoading || !nickname.trim()}>
          {isLoading ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text style={styles.buttonText}>Sohbete Katıl 🚀</Text>
          )}
        </TouchableOpacity>

        {/* Info */}
        <View style={styles.infoBox}>
          <Text style={styles.infoTitle}>✨ Özellikler</Text>
          <Text style={styles.infoText}>• Gerçek zamanlı duygu analizi</Text>
          <Text style={styles.infoText}>• Pozitif/Negatif/Nötr algılama</Text>
          <Text style={styles.infoText}>• Türkçe AI modeli</Text>
          <Text style={styles.infoText}>• Emoji ile görsel gösterim</Text>
        </View>
      </View>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f7fa',
  },
  content: {
    flex: 1,
    padding: 24,
    justifyContent: 'center',
  },
  header: {
    alignItems: 'center',
    marginBottom: 40,
  },
  emoji: {
    fontSize: 80,
    marginBottom: 16,
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#1e293b',
    marginBottom: 8,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: 16,
    color: '#64748b',
    textAlign: 'center',
    paddingHorizontal: 20,
  },
  inputContainer: {
    marginBottom: 24,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    color: '#334155',
    marginBottom: 8,
  },
  input: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    fontSize: 16,
    color: '#1e293b',
    borderWidth: 2,
    borderColor: '#e2e8f0',
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.1,
    shadowRadius: 4,
  },
  hint: {
    fontSize: 14,
    color: '#64748b',
    marginTop: 8,
    fontStyle: 'italic',
  },
  button: {
    backgroundColor: '#667eea',
    borderRadius: 12,
    padding: 16,
    alignItems: 'center',
    elevation: 4,
    shadowColor: '#667eea',
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.3,
    shadowRadius: 8,
  },
  buttonDisabled: {
    backgroundColor: '#94a3b8',
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
  infoBox: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 20,
    marginTop: 32,
    borderLeftWidth: 4,
    borderLeftColor: '#667eea',
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.1,
    shadowRadius: 4,
  },
  infoTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#1e293b',
    marginBottom: 12,
  },
  infoText: {
    fontSize: 14,
    color: '#64748b',
    marginBottom: 6,
    lineHeight: 20,
  },
});

export default UserLogin;

