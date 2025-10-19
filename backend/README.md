# Emotion Analyze API

Bu API, kullanıcıların mesajlarını kaydeder ve Hugging Face üzerinden duygu analizi yapar.

## Özellikler

- Kullanıcı yönetimi (sadece rumuz ile)
- Mesaj kaydı ve listeleme
- Hugging Face ile duygu analizi entegrasyonu
- SQLite veritabanı
- Swagger API dokümantasyonu

## Kurulum

1. .NET 9.0 SDK'yı yükleyin
2. Projeyi klonlayın
3. Gerekli paketleri yükleyin:
```bash
dotnet restore
```
4. Veritabanını migrate edin:
```bash
dotnet ef database update
```
5. Uygulamayı çalıştırın:
```bash
dotnet run
```

## API Endpoints

### Kullanıcılar

- `GET /api/users` - Tüm kullanıcıları listele
- `GET /api/users/{id}` - Belirli bir kullanıcıyı getir
- `POST /api/users` - Yeni kullanıcı oluştur
- `GET /api/users/{id}/messages` - Kullanıcının mesajlarını getir

### Mesajlar

- `GET /api/messages` - Tüm mesajları listele
- `GET /api/messages/{id}` - Belirli bir mesajı getir
- `POST /api/messages` - Yeni mesaj oluştur (duygu analizi otomatik yapılır)

## Render Deployment

1. Render'da yeni bir Web Service oluşturun
2. Repository'yi bağlayın
3. Environment Variables:
   - `ASPNETCORE_ENVIRONMENT`: Production
   - `HuggingFaceUrl`: Hugging Face Space URL'iniz
4. Build Command: `dotnet publish -c Release -o out`
5. Start Command: `dotnet out/EmotionAnalyzeApi.dll`

