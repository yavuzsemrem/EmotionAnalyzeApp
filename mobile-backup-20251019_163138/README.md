# 📱 Emotion Analyze Mobile App (React Native CLI)

React Native CLI ile geliştirilmiş mobil uygulama. Web uygulamasıyla aynı özelliklere sahip, native performans sunar.

## 🎯 Özellikler

- ✅ **Native UI**: React Native component'leri ile native deneyim
- ✅ **Gerçek Zamanlı Chat**: 5 saniyede bir otomatik güncelleme
- ✅ **Duygu Analizi**: Backend API üzerinden AI servisi entegrasyonu
- ✅ **Emoji Göstergeler**: Görsel duygu durumu
- ✅ **Responsive**: Tüm ekran boyutlarına uyumlu
- ✅ **TypeScript**: Tip güvenli kod

## 📋 Gereksinimler

### Android Geliştirme İçin:
- **Node.js** 18+
- **JDK** 17+
- **Android Studio** (Android SDK, Emulator)
- **React Native CLI**: `npm install -g react-native-cli`

### iOS Geliştirme İçin (sadece macOS):
- **Node.js** 18+
- **Xcode** 14+
- **CocoaPods**: `sudo gem install cocoapods`
- **React Native CLI**: `npm install -g react-native-cli`

### Kurulum Kontrol:
```bash
node --version
npm --version
java -version
```

## 🚀 Kurulum

### 1. Bağımlılıkları Yükle

```bash
cd mobile
npm install
```

### 2. iOS (sadece macOS):
```bash
cd ios
pod install
cd ..
```

## ▶️ Çalıştırma

### Android Emulator veya Cihaz

1. **Android Studio'yu aç** ve bir emulator başlat veya fiziksel cihazınızı USB ile bağlayın (USB debugging açık olmalı)

2. **Metro Bundler'ı başlat** (yeni terminal):
```bash
npm start
```

3. **Android uygulamasını çalıştır** (başka bir terminal):
```bash
npm run android
```

### iOS Simulator (sadece macOS)

1. **Metro Bundler'ı başlat** (yeni terminal):
```bash
npm start
```

2. **iOS uygulamasını çalıştır** (başka bir terminal):
```bash
npm run ios
```

## 🔧 Konfigürasyon

### Backend API URL'ini Ayarlama

`src/services/api.ts` dosyasını açın ve API URL'ini ortamınıza göre güncelleyin:

```typescript
// Android Emulator için (localhost)
const API_URL = 'http://10.0.2.2:5053/api';

// Gerçek cihaz için (bilgisayarınızın IP adresi)
const API_URL = 'http://192.168.1.100:5053/api';

// Production için
const API_URL = 'https://your-render-url.onrender.com/api';
```

### IP Adresinizi Bulma:

**Windows:**
```bash
ipconfig
# "IPv4 Address" değerini kullanın
```

**macOS/Linux:**
```bash
ifconfig
# "inet" değerini kullanın
```

## 📱 Test

### 1. Backend ve AI Servisini Çalıştırın

Mobil uygulama çalışmadan önce backend ve AI servisinin çalışıyor olması gerekir:

```bash
# Terminal 1 - AI Servisi
cd ai-service
python app.py

# Terminal 2 - Backend
cd backend
dotnet run

# Terminal 3 - Mobile
cd mobile
npm run android  # veya npm run ios
```

### 2. Uygulamayı Test Edin

1. Bir rumuz girin ve "Sohbete Katıl" butonuna tıklayın
2. Mesaj yazın ve gönderin
3. Duygu analizi sonuçlarını görün (emoji + yüzde)
4. Web uygulamasından da mesaj gönderin, mobilde otomatik güncellensin

## 🏗️ Proje Yapısı

```
mobile/
├── android/              # Android native dosyalar
├── ios/                  # iOS native dosyalar (macOS'ta)
├── src/
│   ├── components/       # UI komponentleri
│   │   ├── UserLogin.tsx
│   │   ├── ChatRoom.tsx
│   │   └── MessageItem.tsx
│   ├── services/         # API servisleri
│   │   └── api.ts
│   └── types.ts          # TypeScript tipleri
├── App.tsx               # Ana uygulama
├── index.js              # Entry point
└── package.json
```

## 🐛 Sorun Giderme

### Metro Bundler Port Hatası

Eğer port 8081 kullanımdaysa:
```bash
npx react-native start --port=8082
```

### Android Build Hatası

Cache'i temizleyin:
```bash
cd android
./gradlew clean
cd ..
npm start -- --reset-cache
```

### iOS Build Hatası (macOS)

Pods'ları yeniden yükleyin:
```bash
cd ios
pod deintegrate
pod install
cd ..
```

### Backend'e Bağlanamıyor

1. Backend'in çalıştığından emin olun (`http://localhost:5053/swagger`)
2. Firewall'u kontrol edin
3. Android Emulator için `10.0.2.2`, gerçek cihaz için IP adresi kullanın
4. API URL'ini `src/services/api.ts` dosyasında doğru ayarlayın

## 📦 APK Build (Production)

### Android APK

```bash
cd android
./gradlew assembleRelease
```

APK dosyası: `android/app/build/outputs/apk/release/app-release.apk`

### Signed APK (Google Play için)

1. Keystore oluşturun:
```bash
keytool -genkeypair -v -storetype PKCS12 -keystore my-release-key.keystore -alias my-key-alias -keyalg RSA -keysize 2048 -validity 10000
```

2. `android/gradle.properties` dosyasına ekleyin:
```
MYAPP_RELEASE_STORE_FILE=my-release-key.keystore
MYAPP_RELEASE_KEY_ALIAS=my-key-alias
MYAPP_RELEASE_STORE_PASSWORD=*****
MYAPP_RELEASE_KEY_PASSWORD=*****
```

3. Build:
```bash
cd android
./gradlew assembleRelease
```

## 🎨 Özelleştirme

### Uygulama İsmi

`app.json` dosyasında:
```json
{
  "name": "EmotionAnalyzeMobile",
  "displayName": "Emotion Analyze"
}
```

### Uygulama İkonu

- Android: `android/app/src/main/res/mipmap-*/ic_launcher.png`
- iOS: `ios/EmotionAnalyzeMobile/Images.xcassets/AppIcon.appiconset/`

### Splash Screen

React Native Splash Screen kütüphanesi kullanabilirsiniz:
```bash
npm install react-native-splash-screen
```

## 📚 Öğrenilen Kavramlar

- ✅ **React Native CLI** setup ve konfigürasyon
- ✅ **TypeScript** ile tip güvenli React Native
- ✅ **Native Components**: FlatList, KeyboardAvoidingView, Alert
- ✅ **API Integration**: Axios ile RESTful API çağrıları
- ✅ **State Management**: useState, useEffect hooks
- ✅ **Styling**: StyleSheet ile native styling
- ✅ **Navigation**: Component-based navigation
- ✅ **Real-time Updates**: Polling stratejisi

## 📝 Notlar

- Geliştirme sırasında Hot Reload aktif (Cmd/Ctrl + R ile reload)
- Debug menu için cihazı sallayın veya Cmd/Ctrl + M yapın
- Chrome DevTools ile debug: Menu > Debug > Debug JS Remotely
- Network isteklerini görmek için Flipper kullanabilirsiniz

## 🚀 Sonraki Adımlar

- [ ] Push notification entegrasyonu
- [ ] Offline mode (AsyncStorage ile cache)
- [ ] React Navigation ile çoklu ekran
- [ ] Dark mode desteği
- [ ] Biometric authentication
- [ ] Google Play / App Store'a yükleme

## 📧 Destek

Sorun yaşarsanız:
1. `node_modules` sil ve `npm install` yap
2. Cache temizle: `npm start -- --reset-cache`
3. Native build temizle: `cd android && ./gradlew clean`

---

**Not:** Bu mobil uygulama React Native CLI ile geliştirilmiştir. Expo değil, tam native özelliklere sahiptir.

