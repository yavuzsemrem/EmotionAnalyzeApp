# AI Service Manuel Kurulum (Python 3.14)

## Adım 1: Virtual Environment Hazırlığı
```powershell
# ai-service klasöründe
cd ai-service

# Eski venv varsa silin
Remove-Item -Recurse -Force venv -ErrorAction SilentlyContinue

# Yeni venv oluştur
python -m venv venv

# Aktif et
.\venv\Scripts\Activate.ps1
```

## Adım 2: Temel Araçları Yükle
```powershell
python -m pip install --upgrade pip setuptools wheel
```

## Adım 3: Paketleri Tek Tek Yükle
```powershell
# PyTorch (CPU versiyonu - büyük paket, 5-10 dk sürebilir)
pip install torch torchvision torchaudio

# Numpy
pip install numpy

# Transformers (Rust gerektirmeden)
pip install transformers --no-build-isolation --prefer-binary

# Gradio
pip install gradio

# Ek paketler
pip install protobuf huggingface-hub safetensors filelock
```

## Adım 4: Test Et
```powershell
python app.py
```

## Sorun Giderme

### Hata: "No matching distribution"
- Python 3.11 kullanın: `py -3.11 -m venv venv`

### Hata: "Rust/Cargo not found"
- `--no-build-isolation --prefer-binary` ekleyin

### Model İndirme Hatası
- İlk çalıştırmada 700MB+ model indirilir, sabırlı olun
- İnternet bağlantınızı kontrol edin

