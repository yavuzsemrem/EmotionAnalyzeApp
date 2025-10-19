# ==================================================
# Android Environment Otomatik Kurulum Script
# React Native icin gerekli tum ayarlari yapar
# ==================================================

Write-Host "`nAndroid Environment Kurulumu Basliyor...`n" -ForegroundColor Cyan

# Administrator kontrolu
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "Bu script Administrator olarak calistirilmali!" -ForegroundColor Red
    Write-Host "PowerShell'i sag tiklayip 'Run as Administrator' secin.`n" -ForegroundColor Yellow
    pause
    exit
}

# ==================================================
# 1. ANDROID_HOME Ayarla
# ==================================================
Write-Host "1. ANDROID_HOME ayarlaniyor..." -ForegroundColor Yellow

$androidHome = "C:\Users\$env:USERNAME\AppData\Local\Android\Sdk"
if (Test-Path $androidHome) {
    [System.Environment]::SetEnvironmentVariable('ANDROID_HOME', $androidHome, 'Machine')
    Write-Host "   ANDROID_HOME = $androidHome" -ForegroundColor Green
} else {
    Write-Host "   Android SDK bulunamadi: $androidHome" -ForegroundColor Red
    Write-Host "   Android Studio'yu acip SDK Manager'dan SDK indirin." -ForegroundColor Yellow
    pause
    exit
}

# ==================================================
# 2. JAVA_HOME Ayarla (Java 17 - JBR)
# ==================================================
Write-Host "`n2. JAVA_HOME ayarlaniyor..." -ForegroundColor Yellow

$jdkPaths = @(
    "C:\Program Files\Android\Android Studio\jbr",
    "C:\Program Files\Android\Android Studio\jre"
)

$jdkFound = $false
foreach ($jdkPath in $jdkPaths) {
    if (Test-Path $jdkPath) {
        [System.Environment]::SetEnvironmentVariable('JAVA_HOME', $jdkPath, 'Machine')
        Write-Host "   JAVA_HOME = $jdkPath" -ForegroundColor Green
        $jdkFound = $true
        break
    }
}

if (-not $jdkFound) {
    Write-Host "   Android Studio JDK bulunamadi. Java 17 manuel kurulumu gerekli." -ForegroundColor Yellow
    Write-Host "   Indirme: https://adoptium.net/temurin/releases/?version=17" -ForegroundColor Cyan
}

# ==================================================
# 3. PATH Guncelle
# ==================================================
Write-Host "`n3. PATH guncelleniyor..." -ForegroundColor Yellow

$currentPath = [System.Environment]::GetEnvironmentVariable('Path', 'Machine')

# Eklenecek yollar
$pathsToAdd = @(
    "$androidHome\platform-tools",
    "$androidHome\emulator",
    "$androidHome\cmdline-tools\latest\bin",
    "$androidHome\tools",
    "$androidHome\tools\bin"
)

# JAVA_HOME varsa bin'i de ekle
if ($jdkFound) {
    $javaHome = [System.Environment]::GetEnvironmentVariable('JAVA_HOME', 'Machine')
    $pathsToAdd += "$javaHome\bin"
}

$addedCount = 0
foreach ($path in $pathsToAdd) {
    if (Test-Path $path) {
        if ($currentPath -notlike "*$path*") {
            $currentPath = "$currentPath;$path"
            Write-Host "   Eklendi: $path" -ForegroundColor Green
            $addedCount++
        } else {
            Write-Host "   Zaten var: $path" -ForegroundColor Gray
        }
    } else {
        Write-Host "   Bulunamadi: $path" -ForegroundColor Yellow
    }
}

if ($addedCount -gt 0) {
    [System.Environment]::SetEnvironmentVariable('Path', $currentPath, 'Machine')
    Write-Host "`n   $addedCount yeni path eklendi!" -ForegroundColor Cyan
} else {
    Write-Host "`n   Tum yollar zaten PATH'te mevcut." -ForegroundColor Gray
}

# ==================================================
# 4. local.properties Olustur
# ==================================================
Write-Host "`n4. local.properties dosyasi olusturuluyor..." -ForegroundColor Yellow

$mobileAndroidPath = "$PSScriptRoot\mobile\android"
if (Test-Path $mobileAndroidPath) {
    $localPropsPath = "$mobileAndroidPath\local.properties"
    $sdkDirEscaped = $androidHome -replace '\\', '\\'
    
    @"
sdk.dir=$sdkDirEscaped
"@ | Out-File -FilePath $localPropsPath -Encoding ASCII -Force
    
    Write-Host "   Dosya olusturuldu: $localPropsPath" -ForegroundColor Green
} else {
    Write-Host "   mobile\android klasoru bulunamadi" -ForegroundColor Yellow
}

# ==================================================
# 5. Ozet ve Kontrol
# ==================================================
Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host "KURULUM TAMAMLANDI!" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Cyan

Write-Host "`nAyarlanan Environment Variables:" -ForegroundColor Yellow
Write-Host "   ANDROID_HOME = $androidHome" -ForegroundColor White
if ($jdkFound) {
    $javaHome = [System.Environment]::GetEnvironmentVariable('JAVA_HOME', 'Machine')
    Write-Host "   JAVA_HOME    = $javaHome" -ForegroundColor White
}

Write-Host "`nONEMLI UYARI:" -ForegroundColor Red
Write-Host "   1. Bu PowerShell penceresini KAPAT" -ForegroundColor Yellow
Write-Host "   2. TUM acik terminal/cmd/PowerShell pencerelerini KAPAT" -ForegroundColor Yellow
Write-Host "   3. YENI bir PowerShell ac" -ForegroundColor Yellow
Write-Host "   4. Su komutlari calistirarak test et:" -ForegroundColor Yellow
Write-Host ""
Write-Host "      java -version" -ForegroundColor Cyan
Write-Host "      adb version" -ForegroundColor Cyan
Write-Host "      emulator -list-avds" -ForegroundColor Cyan
Write-Host ""
Write-Host "   5. Hepsi calisiyorsa:" -ForegroundColor Green
Write-Host "      cd mobile" -ForegroundColor Cyan
Write-Host "      npm run android" -ForegroundColor Cyan

Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host ""
pause

