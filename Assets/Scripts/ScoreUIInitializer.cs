using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreUIInitializer : MonoBehaviour
{
    [Tooltip("Inspector'dan atanmazsa otomatik olarak sahnedeki ilk ScoreManager bulunur.")]
    public ScoreManager scoreManager;

    void Awake()
    {
        // ScoreManager'ı otomatik bul
        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();

        // Canvas oluştur
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // TextMeshPro - Text oluştur
        GameObject textGO = new GameObject("ScoreText");
        textGO.transform.SetParent(canvas.transform);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 48;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.rectTransform.anchorMin = new Vector2(0, 1);
        tmp.rectTransform.anchorMax = new Vector2(0, 1);
        tmp.rectTransform.pivot = new Vector2(0, 1);
        tmp.rectTransform.anchoredPosition = new Vector2(50, -200);
        tmp.text = "Score: 0";

        // ScoreManager'a referans ata
        if (scoreManager != null)
            scoreManager.GetType().GetField("scoreText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(scoreManager, tmp);
    }
} 