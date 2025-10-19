import { useState } from 'react'
import UserLogin from './components/UserLogin'
import ChatRoom from './components/ChatRoom'
import { userService } from './services/api'
import './App.css'

function App() {
  const [currentUser, setCurrentUser] = useState(null)

  // Kullanıcı giriş yap veya oluştur
  const handleUserSelect = async (nickname) => {
    try {
      // Yeni kullanıcı oluştur
      const newUser = await userService.create(nickname)
      setCurrentUser(newUser)
    } catch (error) {
      console.error('Kullanıcı oluşturulurken hata:', error)
      throw error
    }
  }

  // Kullanıcı çıkış yap
  const handleLogout = () => {
    setCurrentUser(null)
  }

  return (
    <div className="App">
      {!currentUser ? (
        <UserLogin onUserSelect={handleUserSelect} />
      ) : (
        <ChatRoom user={currentUser} onLogout={handleLogout} />
      )}
    </div>
  )
}

export default App
