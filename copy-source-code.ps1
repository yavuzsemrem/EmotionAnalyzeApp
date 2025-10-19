# Kaynak kodları yeni React Native projesine kopyala
Write-Host "📁 Kaynak kodlar kopyalanıyor..." -ForegroundColor Green

$source = "mobile-backup"
$dest = "mobile"

# Src klasörünü kopyala
if (Test-Path "$source\src") {
    Write-Host "📦 src/ klasörü kopyalanıyor..." -ForegroundColor Cyan
    Copy-Item -Path "$source\src" -Destination "$dest\" -Recurse -Force
    Write-Host "✅ src/ kopyalandı" -ForegroundColor Green
}

# App.tsx'i kopyala
if (Test-Path "$source\App.tsx") {
    Write-Host "📦 App.tsx kopyalanıyor..." -ForegroundColor Cyan
    Copy-Item -Path "$source\App.tsx" -Destination "$dest\App.tsx" -Force
    Write-Host "✅ App.tsx kopyalandı" -ForegroundColor Green
}

# tsconfig.json'u kopyala
if (Test-Path "$source\tsconfig.json") {
    Write-Host "📦 tsconfig.json kopyalanıyor..." -ForegroundColor Cyan
    Copy-Item -Path "$source\tsconfig.json" -Destination "$dest\tsconfig.json" -Force
    Write-Host "✅ tsconfig.json kopyalandı" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ Tüm dosyalar kopyalandı!" -ForegroundColor Green
Write-Host ""
Write-Host "📱 Şimdi Android Emulator'ü başlatın:" -ForegroundColor Cyan
Write-Host "1. Android Studio'yu açın" -ForegroundColor Yellow
Write-Host "2. Tools > Device Manager" -ForegroundColor Yellow
Write-Host "3. Bir cihaz seçin ve ▶️ Play butonuna tıklayın" -ForegroundColor Yellow
Write-Host ""
Write-Host "Emulator hazır olunca 'cd mobile' ve 'npm run android' çalıştırın" -ForegroundColor Cyan

