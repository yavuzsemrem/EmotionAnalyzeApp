# 🚀 Deployment Rehberi

Bu doküman, Emotion Analyze App'i ücretsiz platformlara deploy etmek için adım adım talimatlar içerir.

## 📋 Genel Bakış

| Servis | Platform | Maliyet | Deploy Süresi |
|--------|----------|---------|---------------|
| AI Servisi | Hugging Face Spaces | Ücretsiz | ~5 dakika |
| Backend API | Render (Free Tier) | Ücretsiz | ~10 dakika |
| Frontend Web | Vercel | Ücretsiz | ~3 dakika |
| Mobil App | APK/TestFlight | Ücretsiz | ~20 dakika |

---

## 1️⃣ Hugging Face Spaces - AI Servisi

### Adımlar:

1. **Hugging Face Hesabı Oluştur**
   - https://huggingface.co/ adresine git
   - Ücretsiz hesap oluştur

2. **Yeni Space Oluştur**
   - Spaces sekmesine git
   - "Create new Space" tıkla
   - Space adı: `emotion-analyze-ai`
   - SDK: **Gradio**
   - Hardware: **CPU basic** (ücretsiz)

3. **Dosyaları Yükle**
   ```
   ai-service/
   ├── app.py              # Ana dosya
   ├── requirements.txt    # Dependencies
   └── README.md           # Açıklama
   ```

4. **requirements.txt Düzenle**
   ```txt
   transformers==4.36.2
   torch==2.1.2
   gradio==4.12.0
   flask==3.0.0
   flask-cors==4.0.0
   ```

5. **app.py'yi Kontrol Et**
   - Gradio `launch()` fonksiyonunda `share=True` olmalı
   - Flask port'u değiştir: `flask_app.run(host='0.0.0.0', port=7860)`

6. **Deploy Et**
   - Git push yap veya web arayüzünden dosyaları yükle
   - Build otomatik başlayacak (~5 dakika)
   - URL: `https://huggingface.co/spaces/USERNAME/emotion-analyze-ai`

7. **API URL'ini Not Et**
   ```
   https://USERNAME-emotion-analyze-ai.hf.space/analyze
   ```

---

## 2️⃣ Render - Backend API

### Adımlar:

1. **Render Hesabı Oluştur**
   - https://render.com adresine git
   - GitHub ile giriş yap

2. **Yeni Web Service Oluştur**
   - Dashboard > "New +" > "Web Service"
   - GitHub repo'nuzu bağla
   - Root Directory: `backend`

3. **Ayarları Yap**
   ```
   Name: emotion-analyze-backend
   Region: Frankfurt (EU)
   Branch: main
   Runtime: .NET
   Build Command: dotnet publish -c Release -o out
   Start Command: dotnet out/EmotionAnalyzeApi.dll
   Instance Type: Free
   ```

4. **Environment Variables Ekle**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   HuggingFaceUrl=https://USERNAME-emotion-analyze-ai.hf.space/analyze
   ```

5. **render.yaml Kontrol Et**
   Backend klasöründe `render.yaml` mevcut:
   ```yaml
   services:
     - type: web
       name: emotion-analyze-backend
       env: dotnet
       buildCommand: dotnet publish -c Release -o out
       startCommand: dotnet out/EmotionAnalyzeApi.dll
   ```

6. **Deploy Et**
   - "Create Web Service" tıkla
   - Build başlayacak (~10 dakika)
   - URL: `https://emotion-analyze-backend.onrender.com`

7. **CORS Kontrol**
   - `Program.cs` içinde `AllowAnyOrigin()` olduğundan emin ol

8. **SQLite Database**
   - Render free tier'da SQLite persistent değil
   - Production için PostgreSQL kullan veya
   - Backend her restart'ta yeni DB oluşturur

9. **Test Et**
   ```bash
   curl https://emotion-analyze-backend.onrender.com/swagger
   ```

---

## 3️⃣ Vercel - Frontend Web

### Adımlar:

1. **Vercel Hesabı Oluştur**
   - https://vercel.com adresine git
   - GitHub ile giriş yap

2. **Yeni Proje İmport Et**
   - Dashboard > "Add New" > "Project"
   - GitHub repo'nuzu seç
   - Root Directory: `frontend`

3. **Build Ayarları**
   ```
   Framework Preset: Vite
   Build Command: npm run build
   Output Directory: dist
   Install Command: npm install
   ```

4. **Environment Variables Ekle**
   ```
   VITE_API_URL=https://emotion-analyze-backend.onrender.com/api
   ```

5. **vercel.json Kontrol Et**
   Frontend klasöründe `vercel.json` mevcut:
   ```json
   {
     "rewrites": [{ "source": "/(.*)", "destination": "/index.html" }]
   }
   ```

6. **Deploy Et**
   - "Deploy" tıkla
   - Build başlayacak (~2 dakika)
   - URL: `https://emotion-analyze-app.vercel.app`

7. **Custom Domain (Opsiyonel)**
   - Settings > Domains
   - Ücretsiz `.vercel.app` domain veya kendi domain'inizi ekleyin

8. **Test Et**
   - URL'yi tarayıcıda aç
   - Console'da hata var mı kontrol et
   - Mesaj gönder ve duygu analizi çalışsın

---

## 4️⃣ Mobil App Deployment

### Android APK

1. **Production API URL'lerini Ayarla**
   `mobile/src/services/api.ts`:
   ```typescript
   const API_URL = 'https://emotion-analyze-backend.onrender.com/api';
   ```

2. **Keystore Oluştur**
   ```bash
   cd android/app
   keytool -genkeypair -v -storetype PKCS12 -keystore my-release-key.keystore -alias my-key-alias -keyalg RSA -keysize 2048 -validity 10000
   ```

3. **Gradle Konfigürasyonu**
   `android/gradle.properties`:
   ```
   MYAPP_RELEASE_STORE_FILE=my-release-key.keystore
   MYAPP_RELEASE_KEY_ALIAS=my-key-alias
   MYAPP_RELEASE_STORE_PASSWORD=*****
   MYAPP_RELEASE_KEY_PASSWORD=*****
   ```

4. **Release Build**
   ```bash
   cd android
   ./gradlew assembleRelease
   ```

5. **APK Lokasyonu**
   ```
   android/app/build/outputs/apk/release/app-release.apk
   ```

6. **Dağıtım**
   - APK'yı GitHub Releases'e yükle
   - Kullanıcılar direkt indirebilir
   - Google Play Store için signed APK gerekli

### iOS (macOS gerekli)

1. **Apple Developer Hesabı**
   - https://developer.apple.com
   - $99/yıl (ücretli)

2. **Xcode ile Build**
   ```bash
   cd mobile/ios
   pod install
   open EmotionAnalyzeMobile.xcworkspace
   ```

3. **Provisioning Profile**
   - Xcode > Signing & Capabilities
   - Team seç
   - Bundle Identifier ayarla

4. **Archive ve Export**
   - Product > Archive
   - Distribute App > Ad Hoc veya TestFlight

5. **TestFlight**
   - App Store Connect'e yükle
   - Test kullanıcıları ekle

---

## 🔧 Production Optimizasyonları

### Backend

1. **Logging**
   ```json
   "Logging": {
     "LogLevel": {
       "Default": "Warning",
       "Microsoft.AspNetCore": "Warning"
     }
   }
   ```

2. **Database**
   - SQLite yerine PostgreSQL kullan (Render'da ücretsiz)
   - Connection string'i environment variable'dan al

3. **Caching**
   - HttpClient singleton pattern
   - Response caching ekle

### Frontend

1. **Production Build**
   ```bash
   npm run build
   # dist/ klasörü optimize edilmiş dosyalar
   ```

2. **Environment Variables**
   - Development: `.env.development`
   - Production: `.env.production`

3. **Analytics**
   - Google Analytics ekle (opsiyonel)
   - Error tracking (Sentry)

### AI Servisi

1. **Model Cache**
   - İlk çalıştırmada model indirilir
   - Hugging Face otomatik cache yapar

2. **Timeout**
   - İlk istek yavaş olabilir (cold start)
   - Backend timeout'u 60 saniye yapın

3. **Rate Limiting**
   - Hugging Face Spaces rate limit var
   - Ücretsiz tier: sınırlı kullanım

---

## 🧪 Deployment Sonrası Test

### Checklist

- [ ] Hugging Face Space erişilebilir mi?
  ```bash
  curl https://USERNAME-emotion-analyze-ai.hf.space/analyze -X POST -H "Content-Type: application/json" -d '{"text":"test"}'
  ```

- [ ] Render Backend API çalışıyor mu?
  ```bash
  curl https://emotion-analyze-backend.onrender.com/swagger
  ```

- [ ] Vercel Frontend açılıyor mu?
  ```bash
  curl https://emotion-analyze-app.vercel.app
  ```

- [ ] Frontend → Backend bağlantısı var mı?
  - Chrome DevTools > Network tab
  - API istekleri başarılı mı?

- [ ] Backend → AI Servisi bağlantısı var mı?
  - Render logs kontrol et
  - Mesaj gönder ve analiz sonucu gelsin

- [ ] Mobil app API'ye bağlanıyor mu?
  - Android Logcat kontrol et
  - Mesaj gönder test et

---

## 🐛 Yaygın Sorunlar

### 1. Hugging Face Space Build Hatası
**Çözüm:**
- `requirements.txt` versiyonlarını kontrol et
- Torch boyutu büyükse CPU versiyonu kullan: `torch==2.1.2+cpu`

### 2. Render Backend Cold Start
**Sorun:** İlk istek 30+ saniye sürebilir (free tier)
**Çözüm:**
- Beklenen davranış
- Ping servisi ekle (keep alive)
- Upgrade to paid tier

### 3. CORS Hatası
**Çözüm:**
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### 4. Mobile API Connection Failed
**Çözüm:**
- HTTPS kullan
- Android network security config ekle
- Timeout'u artır

---

## 📊 Monitoring

### Render Logs
```bash
# Render Dashboard > Logs sekmesi
# Real-time log stream
```

### Vercel Analytics
```bash
# Vercel Dashboard > Analytics
# Visitor stats, performance
```

### Hugging Face Metrics
```bash
# Space > Settings > Metrics
# CPU/Memory kullanımı
```

---

## 💰 Maliyet Tahmini

| Platform | Ücretsiz Limit | Sonrası Maliyet |
|----------|----------------|-----------------|
| Hugging Face | Sınırsız (CPU) | GPU: $0.60/saat |
| Render | 750 saat/ay | $7/ay (Starter) |
| Vercel | 100GB bandwidth | $20/ay (Pro) |
| GitHub | 500MB storage | $4/ay (Team) |

**Toplam Ücretsiz Kullanım:** Tüm servisler tamamen ücretsiz (limitler dahilinde)

---

## ✅ Deployment Tamamlandı!

Tebrikler! Uygulamanız artık canlıda:

- 🌐 **Web:** https://emotion-analyze-app.vercel.app
- 🔌 **API:** https://emotion-analyze-backend.onrender.com
- 🤖 **AI:** https://huggingface.co/spaces/USERNAME/emotion-analyze-ai
- 📱 **Mobile:** APK/TestFlight

---

**Yardım mı gerekiyor?** GitHub issues'da soru sorabilirsiniz!

