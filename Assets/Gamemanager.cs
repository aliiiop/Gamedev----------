using UnityEngine;
using UnityEngine.SceneManagement;

// Повесить на пустой объект "GameManager" на игровой сцене.
// Назначить в инспекторе: deathPanel (панель "Вы умерли", выключенную по умолчанию).
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject deathPanel;

    public int kills = 0;
    public int killsToWin = 9;
    public string menuSceneName = "MainMenu";
    public string victorySceneName = "Victory";

    public bool IsGameOver { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Вызывается из PlayerHealth, когда HP игрока <= 0
    public void PlayerDied()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        Time.timeScale = 0f;

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Вызывается кнопкой "Вернуться в меню"
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    // Вызывается из EnemyHealth, когда враг умирает
    public void EnemyKilled()
    {
        if (IsGameOver) return;

        kills++;

        if (kills >= killsToWin)
        {
            IsGameOver = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(victorySceneName);
        }
    }
}