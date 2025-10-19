# Duygu Analizi AI Servisi

Bu servis, Türkçe metinler üzerinde duygu analizi yapan bir API sunar. [savasy/bert-base-turkish-sentiment-cased](https://huggingface.co/savasy/bert-base-turkish-sentiment-cased) modelini kullanarak metinlerin duygusal tonunu (pozitif/negatif/nötr) analiz eder.

## Kurulum

1. Gerekli paketleri yükleyin:
```bash
pip install -r requirements.txt
```

2. Uygulamayı başlatın:
```bash
python app.py
```

## API Kullanımı

Servis bir Gradio arayüzü üzerinden çalışır ve aşağıdaki özellikleri sunar:

- Metin girişi için bir textbox
- Duygu analizi sonuçlarını gösteren bir label
- Örnek metinlerle test imkanı

## Model Hakkında

Kullanılan model, Türkçe metinler için özel olarak eğitilmiş BERT tabanlı bir modeldir. Metinleri pozitif, negatif ve nötr olarak sınıflandırır ve her bir kategori için bir güven skoru üretir.

