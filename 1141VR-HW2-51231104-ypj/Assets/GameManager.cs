using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject instructionCanvas;
    public Text instructionText;
    
    void Start()
    {
        CreateInstructionUI();
    }
    
    void Update()
    {
        // 按R键重新开始游戏，兼容新旧输入系统
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartGame();
            }
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Application.Quit();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R)) RestartGame();
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }
#else
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
#endif
    }
    
    void CreateInstructionUI()
    {
        // 创建Canvas
        GameObject canvas = new GameObject("InstructionCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // 创建文本对象
        GameObject textObj = new GameObject("InstructionText");
        textObj.transform.SetParent(canvas.transform);
        
    Text text = textObj.AddComponent<Text>();
    // 新版 Unity 内置字体改为 LegacyRuntime.ttf
    text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = "使用方向鍵或WAD移動和跳躍\n到達黃色終點獲勝！";
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        
        // 设置文本位置
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(10, -10);
        rectTransform.sizeDelta = new Vector2(400, 150);
        
        instructionCanvas = canvas;
        instructionText = text;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}