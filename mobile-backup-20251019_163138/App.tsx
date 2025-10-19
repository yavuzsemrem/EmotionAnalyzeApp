import React, {useState} from 'react';
import {
  SafeAreaView,
  StyleSheet,
  StatusBar,
} from 'react-native';
import UserLogin from './src/components/UserLogin';
import ChatRoom from './src/components/ChatRoom';
import {User} from './src/types';

function App(): React.JSX.Element {
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  const handleUserSelect = (user: User) => {
    setCurrentUser(user);
  };

  const handleLogout = () => {
    setCurrentUser(null);
  };

  return (
    <SafeAreaView style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor="#667eea" />
      {!currentUser ? (
        <UserLogin onUserSelect={handleUserSelect} />
      ) : (
        <ChatRoom user={currentUser} onLogout={handleLogout} />
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f7fa',
  },
});

export default App;

