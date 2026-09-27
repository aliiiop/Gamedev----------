using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Повесить на пустой объект на сцене MainMenu.
// Назначить в инспекторе: sceneName = "Game", playBtn, exitBtn.
public class MainMenu : MonoBehaviour
{
    public string sceneName = "Game";
    public Button playBtn;
    public Button exitBtn;

    void Start()
    {
        playBtn.onClick.AddListener(PlayScene);
        exitBtn.onClick.AddListener(ExitGame);
    }

    void PlayScene()
    {
        SceneManager.LoadScene(sceneName);
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