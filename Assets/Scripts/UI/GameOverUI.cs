using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    
    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }
    
    public void ShowGameOver(PlayerType winner, int totalMoves)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (winnerText != null)
        {
            winnerText.text = $"Player {winner} Wins!";
            winnerText.color = winner == PlayerType.X ? 
                gameSettings.playerXColor : gameSettings.playerOColor;
        }
        
        if (statsText != null)
        {
            statsText.text = $"Total Moves: {totalMoves}";
        }
        
        // PlayWinAnimation();
    }
    
    public void ShowDraw(int totalMoves)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (winnerText != null)
        {
            winnerText.text = "It's a Draw!";
            winnerText.color = Color.white;
        }
        
        if (statsText != null)
        {
            statsText.text = $"Total Moves: {totalMoves}";
        }
    }
    
    public void OnRestartClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        GameManager.Instance.RestartGame();
    }
    
    public void OnMainMenuClicked()
    {
        Debug.Log("Return to main menu");
    }
    
    // private void PlayWinAnimation()
    // {
    //     if (winnerText != null)
    //     {
    //         winnerText.transform.localScale = Vector3.zero;
    //         StartCoroutine(ScaleTextAnimation(winnerText.gameObject, Vector3.one, 0.5f));
    //     }
    // }
    
}