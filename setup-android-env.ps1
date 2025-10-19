# React Native Android Environment Setup Script
# Bu scripti Administrator olarak PowerShell'de çalıştırın

Write-Host "🔧 React Native Android Ortamı Hazırlanıyor..." -ForegroundColor Green

# Android Studio'nun JDK'sını bul
$androidStudioPath = "C:\Program Files\Android\Android Studio"
$jdkPath = "$androidStudioPath\jbr"

if (Test-Path $jdkPath) {
    Write-Host "✅ JDK bulundu: $jdkPath" -ForegroundColor Green
} else {
    Write-Host "❌ JDK bulunamadı. Android Studio'yu doğru kurdunuz mu?" -ForegroundColor Red
    Write-Host "Alternatif yol: C:\Program Files\Android\Android Studio\jre" -ForegroundColor Yellow
    $jdkPath = "$androidStudioPath\jre"
}

# Android SDK Path (genellikle burası)
$androidSdkPath = "$env:LOCALAPPDATA\Android\Sdk"

Write-Host ""
Write-Host "📋 Environment Variables Ayarlanıyor..." -ForegroundColor Cyan
Write-Host "JAVA_HOME = $jdkPath"
Write-Host "ANDROID_HOME = $androidSdkPath"
Write-Host ""

# Kullanıcı Environment Variables'a ekle
[System.Environment]::SetEnvironmentVariable("JAVA_HOME", $jdkPath, "User")
[System.Environment]::SetEnvironmentVariable("ANDROID_HOME", $androidSdkPath, "User")

# PATH'e ekle
$currentPath = [System.Environment]::GetEnvironmentVariable("Path", "User")

$pathsToAdd = @(
    "$jdkPath\bin",
    "$androidSdkPath\platform-tools",
    "$androidSdkPath\emulator",
    "$androidSdkPath\tools",
    "$androidSdkPath\tools\bin"
)

foreach ($pathToAdd in $pathsToAdd) {
    if ($currentPath -notlike "*$pathToAdd*") {
        $currentPath += ";$pathToAdd"
        Write-Host "✅ PATH'e eklendi: $pathToAdd" -ForegroundColor Green
    }
}

[System.Environment]::SetEnvironmentVariable("Path", $currentPath, "User")

Write-Host ""
Write-Host "✅ Environment Variables ayarlandı!" -ForegroundColor Green
Write-Host ""
Write-Host "⚠️  ÖNEMLİ: Değişikliklerin aktif olması için:" -ForegroundColor Yellow
Write-Host "1. Tüm PowerShell pencerelerini kapatın" -ForegroundColor Yellow
Write-Host "2. Yeni bir PowerShell penceresi açın" -ForegroundColor Yellow
Write-Host "3. 'java -version' komutuyla test edin" -ForegroundColor Yellow
Write-Host ""
Write-Host "Sonra 'setup-react-native.ps1' scriptini çalıştırın." -ForegroundColor Cyan

