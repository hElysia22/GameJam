using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // 点击【开始游戏】按钮调用
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    // 返回主菜单
    public void BackToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }
}
