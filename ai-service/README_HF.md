---
title: Turkish Emotion Analysis
emoji: 🎭
colorFrom: purple
colorTo: blue
sdk: gradio
sdk_version: "4.44.0"
app_file: app.py
pinned: false
license: mit
---

# 🎭 Türkçe Duygu Analizi API

Bu Space, Türkçe metinlerin duygu analizini yapar (Pozitif/Negatif/Nötr).

## Model

- **Model:** `savasy/bert-base-turkish-sentiment-cased`
- **Framework:** Transformers + PyTorch
- **UI:** Gradio

## Kullanım

### Web Arayüzü
Yukarıdaki metin kutusuna Türkçe bir cümle yazın ve analiz sonucunu görün.

### API Kullanımı

```python
import requests

response = requests.post(
    "https://YOUR-SPACE-NAME.hf.space/api/predict",
    json={"data": ["Bugün harika bir gün!"]}
)

result = response.json()
print(result)  # {"data": [{"Pozitif": 0.95, "Negatif": 0.03, "Nötr": 0.02}]}
```

### JavaScript API

```javascript
const response = await fetch(
  "https://YOUR-SPACE-NAME.hf.space/api/predict",
  {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ data: ["Bugün harika bir gün!"] })
  }
);

const result = await response.json();
console.log(result.data[0]); // {Pozitif: 0.95, Negatif: 0.03, Nötr: 0.02}
```

## Özellikler

- ✅ Türkçe BERT modeli
- ✅ Akıllı nötr algılama
- ✅ RESTful API
- ✅ Gradio Web UI

## Lisans

MIT

