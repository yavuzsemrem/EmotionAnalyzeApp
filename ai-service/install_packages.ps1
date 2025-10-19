# AI Service Package Installation Script for Windows
# Bu script pre-built paketleri yükler (Rust gerektirmez)

Write-Host "=== AI Service Paket Kurulumu ===" -ForegroundColor Green

# Pip'i güncelle
Write-Host "`nPip güncelleniyor..." -ForegroundColor Yellow
python -m pip install --upgrade pip

# PyTorch'u yükle (CPU versiyonu, daha hızlı)
Write-Host "`nPyTorch yükleniyor..." -ForegroundColor Yellow
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cpu

# Transformers ve bağımlılıkları (pre-built)
Write-Host "`nTransformers yükleniyor..." -ForegroundColor Yellow
pip install transformers --only-binary :all:

# Gradio
Write-Host "`nGradio yükleniyor..." -ForegroundColor Yellow
pip install gradio

# Ek paketler
Write-Host "`nEk paketler yükleniyor..." -ForegroundColor Yellow
pip install protobuf accelerate safetensors

Write-Host "`n=== Kurulum Tamamlandi! ===" -ForegroundColor Green
Write-Host "`nYüklü paketleri kontrol etmek için:" -ForegroundColor Cyan
Write-Host "pip list | Select-String 'torch|transformers|gradio'" -ForegroundColor Cyan

Write-Host "`nAI servisini baslatmak için:" -ForegroundColor Cyan
Write-Host "python app.py" -ForegroundColor Cyan

