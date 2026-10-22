using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OxygenSystem : MonoBehaviour
{
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float oxygenDrainPerSecond = 1f;
    [SerializeField] private TMP_Text oxygenText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    private float currentOxygen;
    private bool gameEnded = false;

    private void Start()
    {
        currentOxygen = maxOxygen;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);
        UpdateOxygenUI();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        currentOxygen -= oxygenDrainPerSecond * Time.deltaTime;
        currentOxygen = Mathf.Max(currentOxygen, 0f);

        UpdateOxygenUI();
        if (currentOxygen <= 0f)
        {
            GameOver();
        }
    }

    private void UpdateOxygenUI()
    {
        if (oxygenText != null)
        {
            oxygenText.text = "Oxygen: "
                + Mathf.CeilToInt(currentOxygen) + "%";
        }
    }

    public void GameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
