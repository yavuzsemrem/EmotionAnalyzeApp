# React Native Projesi Oluşturma ve Kurulum
Write-Host "🚀 React Native CLI Projesi Oluşturuluyor..." -ForegroundColor Green

# Mevcut dizini kontrol et
$currentDir = Get-Location
Write-Host "📁 Çalışma dizini: $currentDir" -ForegroundColor Cyan

# Eski mobile klasörünü kontrol et
if (Test-Path "mobile") {
    Write-Host "⚠️  Mevcut 'mobile' klasörü bulundu" -ForegroundColor Yellow
    $backup = Read-Host "Yedeklemek istiyor musunuz? (y/n)"
    
    if ($backup -eq "y") {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupName = "mobile-backup-$timestamp"
        Rename-Item -Path "mobile" -NewName $backupName
        Write-Host "✅ Yedeklendi: $backupName" -ForegroundColor Green
    } else {
        Write-Host "❌ Eski mobile klasörünü manuel olarak silin veya taşıyın" -ForegroundColor Red
        exit
    }
}

Write-Host ""
Write-Host "📦 React Native CLI projesi oluşturuluyor..." -ForegroundColor Cyan
Write-Host "Bu işlem 5-10 dakika sürebilir..." -ForegroundColor Yellow
Write-Host ""

# React Native projesi oluştur
npx @react-native-community/cli@latest init EmotionAnalyzeMobile --directory mobile --skip-install

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Proje oluşturulamadı. Hata kodu: $LASTEXITCODE" -ForegroundColor Red
    exit
}

Write-Host "✅ React Native projesi oluşturuldu!" -ForegroundColor Green

# Klasöre gir
Set-Location mobile

Write-Host ""
Write-Host "📦 Bağımlılıklar yükleniyor..." -ForegroundColor Cyan
npm install

Write-Host ""
Write-Host "📦 Axios ekleniyor..." -ForegroundColor Cyan
npm install axios

Write-Host ""
Write-Host "✅ Kurulum tamamlandı!" -ForegroundColor Green
Write-Host ""
Write-Host "📱 Sonraki adımlar:" -ForegroundColor Cyan
Write-Host "1. mobile-backup klasöründeki kodları kopyalayın" -ForegroundColor Yellow
Write-Host "2. Android Emulator'ü başlatın (Android Studio > Device Manager)" -ForegroundColor Yellow
Write-Host "3. 'npm run android' komutuyla uygulamayı çalıştırın" -ForegroundColor Yellow
Write-Host ""

