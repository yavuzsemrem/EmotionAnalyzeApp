# 📱 Mobil Uygulama Kurulum Rehberi

## ⚠️ React Native CLI - Native Proje Gereksinimleri

React Native CLI kullanmak için aşağıdaki native geliştirme ortamı gereklidir:

### Windows'ta Android için:
1. ✅ **Node.js 18+** (mevcut)
2. ❌ **JDK 17** - Java Development Kit
3. ❌ **Android Studio** - Android SDK, Emulator
4. ❌ **Android SDK** (API 33+)
5. ❌ **Environment Variables** (ANDROID_HOME, PATH)

### macOS'ta iOS için:
1. ✅ **Node.js 18+**
2. ❌ **Xcode 14+**
3. ❌ **CocoaPods**
4. ❌ **iOS Simulator**

---

## 🚀 Hızlı Çözüm: Expo Go (Önerilen)

Expo, native build gerektirmeden mobil uygulamanızı test etmenizi sağlar.

### Avantajlar:
- ✅ Android Studio gerekmez
- ✅ Xcode gerekmez
- ✅ 5 dakikada hazır
- ✅ QR kod ile telefonunuzda test
- ✅ Hot reload
- ✅ Kolay deployment

### Kurulum:

```bash
# 1. Expo CLI yükle (global)
npm install -g expo-cli

# 2. Yeni Expo projesi oluştur
cd ..
npx create-expo-app EmotionAnalyzeMobileExpo --template blank-typescript

# 3. Proje klasörüne git
cd EmotionAnalyzeMobileExpo

# 4. Axios ekle
npm install axios

# 5. Expo Go uygulamasını telefonunuza indirin
# Android: https://play.google.com/store/apps/details?id=host.exp.exponent
# iOS: https://apps.apple.com/app/expo-go/id982107779

# 6. Uygulamayı başlat
npm start

# 7. Telefonunuzda Expo Go'yu açın ve QR kodu tarayın
```

### Kodları Kopyala:

```bash
# mobile-backup/src/ klasöründeki tüm dosyaları
# EmotionAnalyzeMobileExpo/src/ klasörüne kopyalayın

# App.tsx dosyasını da güncelleyin
```

---

## 🔧 Detaylı Yol: React Native CLI (İleri Seviye)

Eğer tam native özellikleri istiyorsanız:

### 1. Java Development Kit (JDK) Kurulumu

```bash
# JDK 17 indirin:
https://www.oracle.com/java/technologies/downloads/#java17

# Kurulum sonrası kontrol:
java -version
# Çıktı: java version "17.x.x"
```

### 2. Android Studio Kurulumu

1. İndirin: https://developer.android.com/studio
2. Kurun ve açın
3. **SDK Manager** > **SDK Platforms** sekmesi:
   - ✅ Android 13.0 (Tiramisu) - API 33
   - ✅ Android 12.0 (S) - API 31
4. **SDK Tools** sekmesi:
   - ✅ Android SDK Build-Tools
   - ✅ Android Emulator
   - ✅ Android SDK Platform-Tools
   - ✅ Google Play Services

### 3. Environment Variables Ayarlama

**Windows - System Environment Variables:**

```
ANDROID_HOME = C:\Users\YourUsername\AppData\Local\Android\Sdk

PATH ekleyin:
%ANDROID_HOME%\platform-tools
%ANDROID_HOME%\emulator
%ANDROID_HOME%\tools
%ANDROID_HOME%\tools\bin
```

Kontrol:
```bash
adb version
# Android Debug Bridge version x.x.x
```

### 4. Android Emulator Oluştur

1. Android Studio > **Tools** > **Device Manager**
2. **Create Device**
3. **Pixel 5** veya **Pixel 6** seçin
4. **System Image**: **API 33** (Tiramisu)
5. **Finish**

### 5. React Native CLI ile Proje Oluştur

```bash
# Eski mobile klasörünü sil (dikkatli!)
# PowerShell'de:
Remove-Item -Path "mobile" -Recurse -Force

# Yeni React Native projesi oluştur
npx @react-native-community/cli init EmotionAnalyzeMobile --directory mobile

# Klasöre git
cd mobile

# Bağımlılıkları yükle
npm install
npm install axios
```

### 6. Kodları Kopyala

`mobile-backup/src/` klasöründeki dosyaları yeni `mobile/src/` klasörüne kopyalayın.

### 7. Uygulamayı Çalıştır

```bash
# Terminal 1: Metro bundler
npm start

# Terminal 2: Android emulator açık olmalı
npm run android
```

---

## 🎯 Hangi Yöntemi Seçmeliyim?

| Özellik | Expo Go | React Native CLI |
|---------|---------|------------------|
| Kurulum Süresi | 5 dakika | 1-2 saat |
| Native Kod | ❌ Erişim yok | ✅ Tam erişim |
| Android Studio | ❌ Gerekmez | ✅ Gerekli |
| Xcode | ❌ Gerekmez | ✅ Gerekli (iOS) |
| Test | 📱 Telefon (QR kod) | 🖥️ Emulator |
| Hot Reload | ✅ Çok hızlı | ✅ Hızlı |
| Custom Native Modules | ❌ Sınırlı | ✅ Sınırsız |
| APK Build | ✅ EAS Build (cloud) | ✅ Local build |
| Öğrenme Eğrisi | 🟢 Kolay | 🔴 Zor |

---

## 💡 Öneri

Bu proje için (chat + API + emoji) **Expo Go yeterlidir**. 

Native modüllere ihtiyacınız yok, bu yüzden Expo ile:
- ✅ Hızlıca test edebilirsiniz
- ✅ Gerçek cihazda çalıştırabilirsiniz
- ✅ Android Studio kurulumu gerekmez
- ✅ Deploy kolaydır

---

## 📞 Destek

Expo kurulumu mu, yoksa React Native CLI full setup mu istersiniz?

**Expo için:** "Expo ile devam et" deyin, hemen kodları hazırlayalım.

**React Native CLI için:** Android Studio'yu yukardaki adımlarla kurun, sonra devam edelim.

---

**Not:** Zaten web uygulamanız mükemmel çalışıyor. Mobil uygulama opsiyonel. MVP için web yeterli!

