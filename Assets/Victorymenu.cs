using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Повесить на пустой объект на сцене Victory.
// Назначить в инспекторе: gameSceneName = "Game", restartBtn, exitBtn.
public class VictoryMenu : MonoBehaviour
{
    public string gameSceneName = "Game";
    public Button restartBtn;
    public Button exitBtn;

    void Start()
    {
        Time.timeScale = 1f; // на случай если игра была на паузе
        restartBtn.onClick.AddListener(Restart);
        exitBtn.onClick.AddListener(ExitGame);
    }

    void Restart()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}