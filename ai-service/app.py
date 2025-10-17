import gradio as gr
from transformers import pipeline, AutoModelForSequenceClassification, AutoTokenizer

# Türkçe duygu analizi modeli
model_name = "savasy/bert-base-turkish-sentiment-cased"
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForSequenceClassification.from_pretrained(model_name)
sentiment_pipeline = pipeline("sentiment-analysis", model=model, tokenizer=tokenizer)

# Nötr algılama için eşik değeri
CONFIDENCE_THRESHOLD = 0.8  # Bu değer ayarlanabilir (0.0 ile 1.0 arası)

def analyze_emotion(text):
    if not text.strip():
        return {"Pozitif": 0.0, "Negatif": 0.0, "Nötr": 1.0}
    
    result = sentiment_pipeline(text)[0]
    label = result["label"]
    score = result["score"]
    
    print(f"Debug - Raw Result: {result}")  # Debug çıktısı
    
    # Eğer skor eşik değerinin altındaysa, nötr olarak değerlendir
    if score < CONFIDENCE_THRESHOLD:
        neutral_weight = 1.0 - (score / CONFIDENCE_THRESHOLD)
        if label == "positive":
            return {
                "Pozitif": score,
                "Negatif": 0.0,
                "Nötr": neutral_weight
            }
        else:  # negative
            return {
                "Pozitif": 0.0,
                "Negatif": score,
                "Nötr": neutral_weight
            }
    
    # Skor eşik değerinin üstündeyse, net bir duygu vardır
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

# Test fonksiyonu
def test_sentiment():
    test_cases = [
        "Bu film muhteşemdi!",                    # Çok pozitif
        "Hiç beğenmedim, çok kötüydü.",          # Çok negatif
        "Normal bir gündü.",                      # Nötr
        "Bugün hava güzel.",                      # Hafif pozitif
        "Biraz yorgunum.",                        # Hafif negatif
        "Toplantı saat 3'te.",                    # Nötr
        "Çok mutluyum ve heyecanlıyım!",         # Çok pozitif
        "Bu durum beni mahvetti.",               # Çok negatif
        "Şu an işe gidiyorum.",                  # Nötr
        "Yemek fena değildi."                    # Hafif pozitif
    ]
    
    print("\nTest Sonuçları:")
    print("-" * 50)
    for text in test_cases:
        raw_result = sentiment_pipeline(text)[0]
        processed_result = analyze_emotion(text)
        print(f"\nTest Metni: {text}")
        print(f"Ham Sonuç: {raw_result}")
        print(f"İşlenmiş Sonuç: {processed_result}")
        print("-" * 30)

# Başlangıçta test fonksiyonunu çalıştır
test_sentiment()

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
        ["Bu film muhteşemdi, çok beğendim!"],      # Çok pozitif
        ["Maalesef hiç beğenmedim, çok kötüydü."],  # Çok negatif
        ["Bugün normal bir gündü."],                 # Nötr
        ["Hava güzel görünüyor."],                  # Hafif pozitif
        ["Biraz yoruldum."],                        # Hafif negatif
        ["Saat şu an 3."],                          # Nötr
    ]
)

# Uygulamayı başlat
if __name__ == "__main__":
    interface.launch()