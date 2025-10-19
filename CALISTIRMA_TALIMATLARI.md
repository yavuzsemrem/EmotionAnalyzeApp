# 🚀 Emotion Analyze App - Çalıştırma Talimatları

Bu doküman, projeyi yerel ortamda çalıştırmak için gereken tüm adımları içerir.

## 📋 Gereksinimler

### Sistem Gereksinimleri (Web Uygulaması)
- **Python 3.8+** (AI servisi için)
- **.NET 9.0 SDK** (Backend API için)
- **Node.js 18+** ve npm (Frontend için)

### Ek Gereksinimler (Mobil Uygulama - Opsiyonel)
- **JDK 17+** (Android için)
- **Android Studio** (Android Emulator)
- **Xcode 14+** (iOS için - sadece macOS)
- **React Native CLI**: `npm install -g react-native-cli`

### Kurulum Kontrolü
Terminalinizde aşağıdaki komutları çalıştırarak kurulumları kontrol edin:

```bash
python --version
dotnet --version
node --version
npm --version
```

---

## 🎯 Adım Adım Kurulum

### 1️⃣ AI Servisi (Python + Gradio)

#### Windows PowerShell:
```powershell
# 1. AI servisi klasörüne git
cd ai-service

# 2. Virtual environment oluştur (önerilen)
python -m venv venv
.\venv\Scripts\Activate.ps1

# 3. Bağımlılıkları yükle
pip install -r requirements.txt

# 4. Servisi başlat
python app.py
```

#### Beklenen Çıktı:
```
Running on local URL:  http://127.0.0.1:7860
Running on public URL: https://xxxxx.gradio.live
```

**🎉 AI Servisi hazır!** Tarayıcınızda `http://127.0.0.1:7860` adresini açarak test edebilirsiniz.

**ÖNEMLİ:** Bu terminal penceresini açık bırakın!

---

### 2️⃣ Backend API (.NET Core)

Yeni bir terminal penceresi açın:

#### Windows PowerShell:
```powershell
# 1. Backend klasörüne git
cd backend

# 2. Bağımlılıkları yükle
dotnet restore

# 3. Database migration'ları uygula (otomatik yapılıyor ama manuel de yapabilirsiniz)
dotnet ef database update

# 4. Backend'i başlat
dotnet run
```

#### Beklenen Çıktı:
```
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

**🎉 Backend API hazır!** Tarayıcınızda `http://localhost:5000/swagger` adresini açarak API dokümantasyonunu görebilirsiniz.

**ÖNEMLİ:** Bu terminal penceresini de açık bırakın!

---

### 3️⃣ Frontend (React + Vite)

Yeni bir üçüncü terminal penceresi açın:

#### Windows PowerShell:
```powershell
# 1. Frontend klasörüne git
cd frontend

# 2. Bağımlılıkları yükle
npm install

# 3. Development sunucusunu başlat
npm run dev
```

#### Beklenen Çıktı:
```
VITE v7.x.x  ready in XXX ms

➜  Local:   http://localhost:5173/
➜  Network: use --host to expose
```

**🎉 Frontend hazır!** Tarayıcınızda `http://localhost:5173` adresini açın.

---

### 4️⃣ Mobil Uygulama (React Native CLI - Opsiyonel)

Yeni bir dördüncü terminal penceresi açın:

#### Windows / macOS / Linux:
```bash
# 1. Mobile klasörüne git
cd mobile

# 2. Bağımlılıkları yükle
npm install

# 3a. Android için
npm run android
# NOT: Android Emulator veya fiziksel cihaz bağlı olmalı

# 3b. iOS için (sadece macOS)
cd ios
pod install
cd ..
npm run ios
```

#### Beklenen Çıktı:
```
info Launching emulator...
info Installing the app...
BUILD SUCCESSFUL
```

**🎉 Mobil uygulama hazır!** Android Emulator veya iOS Simulator'da uygulama açılacak.

**ÖNEMLİ:** Mobil uygulama için API URL'ini ayarlayın:
- `mobile/src/services/api.ts` dosyasını açın
- Android Emulator: `http://10.0.2.2:5053/api`
- Gerçek cihaz: `http://192.168.1.XXX:5053/api` (IP adresinizi girin)

Detaylı mobil kurulum için: **[mobile/README.md](./mobile/README.md)**

---

## 🧪 Uygulamayı Test Etme

### Adım 1: Kullanıcı Girişi
1. Tarayıcıda `http://localhost:5173` açın
2. Bir rumuz (nickname) girin (örn: "Yavuz")
3. "Sohbete Katıl" butonuna tıklayın

### Adım 2: Mesaj Gönderme ve Duygu Analizi
1. Mesaj kutusuna bir şeyler yazın:
   - **Pozitif test:** "Bugün harika bir gün! Çok mutluyum!"
   - **Negatif test:** "Bu çok kötü, hiç beğenmedim."
   - **Nötr test:** "Saat şu an 3'te."

2. 📤 butonuna tıklayın veya Enter tuşuna basın

3. Mesajınız ekranda görünecek ve duygu analizi sonucu:
   - 😊 Pozitif
   - 😔 Negatif
   - 😐 Nötr
   
   şeklinde gösterilecek.

### Adım 3: Çoklu Kullanıcı Testi
1. Farklı bir tarayıcı veya gizli pencere açın
2. Farklı bir rumuz ile giriş yapın
3. Her iki pencereden de mesaj gönderin
4. 5 saniyede bir otomatik olarak mesajlar güncellenir (polling)

---

## 🔍 Sorun Giderme

### Backend "HuggingFace URL bulunamadı" Hatası
**Çözüm:** Backend, henüz deploy edilmemiş Hugging Face URL'ini kullanmaya çalışıyor. Şu adımları izleyin:

1. `backend/appsettings.json` dosyasını açın
2. `HuggingFaceUrl` satırını bulun
3. Gradio'nun size verdiği local URL ile değiştirin:
   ```json
   "HuggingFaceUrl": "http://127.0.0.1:7860/api/predict"
   ```
4. Backend'i yeniden başlatın (`Ctrl+C` sonra `dotnet run`)

### Frontend API'ye Bağlanamıyor
**Çözüm:** 
1. `frontend/src/services/api.js` dosyasını açın
2. `API_URL` değişkeninin `http://localhost:5000/api` olduğundan emin olun
3. Backend'in çalıştığından emin olun (`http://localhost:5000/swagger` açılıyor mu?)

### AI Servisi Model İndiremiyor
**Çözüm:**
1. İnternet bağlantınızı kontrol edin
2. İlk çalıştırmada model indirilir (700MB+), sabırlı olun
3. Model cache'lenir, bir sonraki çalıştırmada hızlı başlar

### SQLite Database Hatası
**Çözüm:**
```powershell
cd backend
dotnet ef database drop --force
dotnet ef database update
```

---

## 📊 API Endpoints Referansı

### Users
- **POST** `/api/users` - Yeni kullanıcı oluştur
  ```json
  { "nickname": "Yavuz" }
  ```

- **GET** `/api/users` - Tüm kullanıcıları listele
- **GET** `/api/users/{id}` - Kullanıcı detayı
- **GET** `/api/users/{id}/messages` - Kullanıcının mesajları

### Messages
- **POST** `/api/messages` - Yeni mesaj gönder (duygu analizi otomatik yapılır)
  ```json
  {
    "userId": 1,
    "content": "Bugün harika bir gün!"
  }
  ```

- **GET** `/api/messages` - Tüm mesajları listele

---

## 🎨 Özellikler

✅ **Türkçe Duygu Analizi** - BERT tabanlı Türkçe model  
✅ **Gerçek Zamanlı Chat** - 5 saniyede bir otomatik güncelleme  
✅ **Modern UI** - Gradient renkler ve animasyonlar  
✅ **Emoji Göstergeler** - Görsel duygu analizi sonuçları  
✅ **Çoklu Kullanıcı** - Sınırsız kullanıcı desteği  
✅ **Responsive Tasarım** - Mobil ve desktop uyumlu  

---

## 📝 Veri Akışı

```
1. Kullanıcı mesaj yazar → Frontend (React)
2. Frontend → Backend API (POST /api/messages)
3. Backend → AI Servisi (Python/Gradio)
4. AI Servisi → Duygu Analizi (BERT Model)
5. AI Servisi → Backend (JSON: Pozitif/Negatif/Nötr skorları)
6. Backend → SQLite Database (Mesaj + skorlar kaydedilir)
7. Backend → Frontend (Sonuç JSON)
8. Frontend → Kullanıcı (Mesaj + emoji + yüzde gösterimi)
```

---

## 🐛 Debug Modu

### Backend Logları
Backend otomatik olarak console'a log yazar. Daha detaylı log için:
```json
// appsettings.json
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft.AspNetCore": "Debug"
  }
}
```

### Frontend Console
Tarayıcıda F12 > Console sekmesinde hataları görebilirsiniz.

### AI Servisi Debug
`ai-service/app.py` içinde test sonuçları otomatik yazdırılır.

---

## ✅ Başarılı Kurulum Kontrolü

Tüm servisler çalışıyorsa:

1. ✅ `http://127.0.0.1:7860` - Gradio AI arayüzü görünüyor
2. ✅ `http://localhost:5000/swagger` - Swagger API dökümantasyonu görünüyor
3. ✅ `http://localhost:5173` - React chat arayüzü görünüyor
4. ✅ Mesaj gönderince duygu analizi çalışıyor ve emoji görünüyor
5. ✅ (Opsiyonel) Mobil uygulama Android/iOS'ta açılıyor ve çalışıyor

---

## 🚀 Sonraki Adımlar

- [ ] Hugging Face Spaces'e AI servisini deploy et
- [ ] Render'a backend'i deploy et
- [ ] Vercel'e frontend'i deploy et
- [x] React Native CLI mobil uygulaması geliştir ✅
- [ ] Android APK build ve test
- [ ] iOS build (macOS gerekli)

---

## 💡 İpuçları

- **Model İndirme:** İlk çalıştırmada model indirileceği için 2-3 dakika bekleyin
- **Port Çakışması:** Eğer portlar kullanımdaysa, farklı portları kullanabilirsiniz
- **SQLite Viewer:** `emotion_analyze.db` dosyasını DB Browser for SQLite ile açarak verileri görebilirsiniz

---

**Keyifli Kodlamalar! 🎉**

