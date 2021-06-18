using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerPrefsExtension
{
    private static readonly string ScoreSave = "score";
    private static readonly string ScoreCleared = "scoreCleared";
    private static readonly string ScoreTitleSave = "scoreTitle";

    public static bool HasSavedScore()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString(ScoreTitleSave, defaultValue: null));
    }

    public static string GetScoreHeader()
    {
        int lastClared = PlayerPrefs.GetInt(ScoreCleared);
        if (lastClared == 0)
        {
            return $"VICTORY";
        }
        else
        {
            return $"TRY AGAIN";
        }
    }

    public static string GetFormattedScoreText()
    {
        string lastScore = PlayerPrefs.GetString(ScoreSave);
        string lastTitle = PlayerPrefs.GetString(ScoreTitleSave);

        StringBuilder scoreText = new StringBuilder();
        scoreText.AppendLine($"{lastTitle}");
        scoreText.AppendLine($"{lastScore}");

        return scoreText.ToString();
    }

    public static void SaveScore(string title, string score, bool cleared)
    {
        PlayerPrefs.SetString(ScoreSave, score);
        PlayerPrefs.SetString(ScoreTitleSave, title);
        PlayerPrefs.SetInt(ScoreCleared, cleared ? 0 : -1);
    }
}

public class MainMenu : MonoBehaviour
{
    public GameObject ScoreBox;
    public TextMeshProUGUI ScoreHeader;
    public TextMeshProUGUI ScoreText;

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (PlayerPrefsExtension.HasSavedScore())
        {
            ScoreBox.SetActive(true);
            ScoreHeader.text = PlayerPrefsExtension.GetScoreHeader();
            ScoreText.text = PlayerPrefsExtension.GetFormattedScoreText();
        }
        else
        {
            ScoreBox.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}