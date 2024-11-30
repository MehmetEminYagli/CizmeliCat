using UnityEngine;
using UnityEngine.Video;
[CreateAssetMenu(fileName = "NewQuestion", menuName = "InteractiveMovie/Question")]
public class QuestionAndVideo : ScriptableObject
{
    [Header("Video Settings")]
    public VideoClip videoClip;  // Video dosyası
    public float slowdownTime;   // Yavaşlamanın başlayacağı zaman
    public float slowdownSpeed = 0.2f; // Yavaşlatma hızı (1 = normal hız)

    [Header("Question Settings")]
    public string questionText;  // Sorunun metni
    public string[] answers;     // Cevap seçenekleri
    public int correctAnswerIndex; // Doğru cevabın indexi (0, 1, 2...)
}




