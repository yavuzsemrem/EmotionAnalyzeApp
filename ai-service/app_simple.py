import gradio as gr
from transformers import pipeline, AutoModelForSequenceClassification, AutoTokenizer

# Türkçe duygu analizi modeli
model_name = "savasy/bert-base-turkish-sentiment-cased"
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForSequenceClassification.from_pretrained(model_name)
sentiment_pipeline = pipeline("sentiment-analysis", model=model, tokenizer=tokenizer)

# Nötr algılama için eşik değerleri
STRONG_EMOTION_THRESHOLD = 0.95  # Bu değerin üstü kesinlikle pozitif/negatif
WEAK_EMOTION_THRESHOLD = 0.70    # Bu değerin altı nötr ağırlıklı
NEUTRAL_BOOST_WORDS = [
    'normal', 'normalim', 'tamam', 'evet', 'hayır', 'belki', 'olabilir',
    'saat', 'bugün', 'yarın', 'dün', 'şimdi', 'sonra', 'önce',
    'gidiyorum', 'geliyorum', 'geldim', 'gittim',
    'anladım', 'anlıyorum', 'peki', 'bilmiyorum', 'biliyorum',
    'var', 'yok', 'olmaz', 'olur', 'toplantı', 'iş', 'okul'
]

def analyze_emotion(text):
    if not text.strip():
        return {"Pozitif": 0.0, "Negatif": 0.0, "Nötr": 1.0}
    
    # Nötr kelime kontrolü (büyük/küçük harf duyarsız)
    text_lower = text.lower().strip()
    for neutral_word in NEUTRAL_BOOST_WORDS:
        if neutral_word in text_lower:
            print(f"Nötr kelime tespit edildi: {neutral_word}")
            return {"Pozitif": 0.0, "Negatif": 0.0, "Nötr": 1.0}
    
    result = sentiment_pipeline(text)[0]
    label = result["label"]
    score = result["score"]
    
    print(f"Debug - Raw Result: {result}")
    
    # Çok düşük skor: Kesinlikle nötr
    if score < 0.55:
        return {
            "Pozitif": 0.0,
            "Negatif": 0.0,
            "Nötr": 1.0
        }
    
    # Düşük-orta skor: Çoğunlukla nötr
    if score < WEAK_EMOTION_THRESHOLD:
        # Nötr ağırlıklı ama hafif duygu da var
        emotion_weight = (score - 0.5) / (WEAK_EMOTION_THRESHOLD - 0.5) * 0.4
        neutral_weight = 1.0 - emotion_weight
        
        if label == "positive":
            return {
                "Pozitif": emotion_weight,
                "Negatif": 0.0,
                "Nötr": neutral_weight
            }
        else:  # negative
            return {
                "Pozitif": 0.0,
                "Negatif": emotion_weight,
                "Nötr": neutral_weight
            }
    
    # Orta-yüksek skor: Karışık (duygu + nötr)
    if score < STRONG_EMOTION_THRESHOLD:
        # Duygu var ama tam net değil
        emotion_weight = score * 0.7
        neutral_weight = 1.0 - emotion_weight
        
        if label == "positive":
            return {
                "Pozitif": emotion_weight,
                "Negatif": 0.0,
                "Nötr": neutral_weight
            }
        else:  # negative
            return {
                "Pozitif": 0.0,
                "Negatif": emotion_weight,
                "Nötr": neutral_weight
            }
    
    # Çok yüksek skor: Net duygu
    if label == "positive":
        return {
            "Pozitif": score,
            "Negatif": 0.0,
            "Nötr": max(0.0, 1.0 - score)
        }
    else:  # negative
        return {
            "Pozitif": 0.0,
            "Negatif": score,
            "Nötr": max(0.0, 1.0 - score)
        }

# Gradio arayüzü
interface = gr.Interface(
    fn=analyze_emotion,
    inputs=gr.Textbox(
        label="Metin Girin",
        placeholder="Analiz edilecek metni buraya yazın...",
        lines=3
    ),
    outputs=gr.Label(label="Duygu Analizi Sonucu"),
    title="Türkçe Duygu Analizi",
    description="Metninizin duygusal tonunu analiz eder (Pozitif/Negatif/Nötr)",
    examples=[
        ["Bu film muhteşemdi, çok beğendim!"],
        ["Maalesef hiç beğenmedim, çok kötüydü."],
        ["Bugün normal bir gündü."],
    ]
)

# API modunu aktif et
if __name__ == "__main__":
    interface.launch(server_port=7860, server_name="127.0.0.1", share=False)

