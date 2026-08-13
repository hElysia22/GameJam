using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("玩家物体拖拽赋值")]
    public PlayerManager player;

    public bool IsGamePause { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region 暂停系统
    public void SetPause(bool pause)
    {
        IsGamePause = pause;
        Time.timeScale = pause ? 0 : 1;
    }
    #endregion

    #region 关卡重置功能
    /// <summary>
    /// 轻量重置：仅复位方块、机关、玩家，不重载场景
    /// </summary>
    public void SoftResetLevel()
    {
        ResettableObject[] allResetObjs = Object.FindObjectsByType<ResettableObject>(FindObjectsSortMode.None);
        foreach (var obj in allResetObjs)
        {
            obj.ResetState();
        }

        if (player != null)
        {
            player.transform.position = player.respPos;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    /// <summary>
    /// 完整重置：重载整个当前场景，所有物体重建
    /// </summary>
    public void FullResetLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    #endregion
}
