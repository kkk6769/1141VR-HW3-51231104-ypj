using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [Header("终点设置")]
    public string winMessage = "恭喜！你到达了终点！";
    public float restartDelay = 2f;
    
    private bool hasWon = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            hasWon = true;
            WinGame();
        }
    }
    
    void WinGame()
    {
        Debug.Log(winMessage);
        
        // 停止玩家移动
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // 显示胜利效果
        StartCoroutine(WinSequence());
    }
    
    System.Collections.IEnumerator WinSequence()
    {
        // 闪烁效果
        Renderer renderer = GetComponent<Renderer>();
        for (int i = 0; i < 6; i++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(0.2f);
            renderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
        
        // 等待后重新开始
        yield return new WaitForSeconds(restartDelay);
        
        // 重新加载场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void Update()
    {
        // 旋转效果让终点更明显
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}