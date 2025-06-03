using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player Turn UI")]
    [SerializeField] private GameObject playerTurnPanel;
    [SerializeField] private TextMeshProUGUI playerTurnText;
    [SerializeField] private Image playerTurnIcon;
    [SerializeField] private Sprite xPlayerSprite;
    [SerializeField] private Sprite oPlayerSprite;
    
    [Header("Move Counter UI")]
    [SerializeField] private GameObject moveCounterPanel;
    [SerializeField] private TextMeshProUGUI moveCounterText;
    
    [Header("Removal UI")]
    [SerializeField] private GameObject removalPanel;
    [SerializeField] private TextMeshProUGUI removalInstructionText;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;
    
    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    
    private void Start()
    {
        if (playerTurnPanel != null) playerTurnPanel.SetActive(true);
        if (moveCounterPanel != null) moveCounterPanel.SetActive(true);
        if (removalPanel != null) removalPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
    }
    
    public void UpdatePlayerTurn(PlayerType currentPlayer)
    {
        if (playerTurnText != null)
        {
            playerTurnText.text = $"Player {currentPlayer}'s Turn";
            
            playerTurnText.color = currentPlayer == PlayerType.X ? 
                gameSettings.playerXColor : gameSettings.playerOColor;
        }
        
        if (playerTurnIcon != null)
        {
            playerTurnIcon.sprite = currentPlayer == PlayerType.X ? xPlayerSprite : oPlayerSprite;
        }
    }
    
    public void UpdateMoveCounter(int moveCount)
    {
        if (moveCounterText != null)
        {
            moveCounterText.text = $"Move: {moveCount}";
        }
    }
    
    public void ShowRemovalUI(PlayerType currentPlayer)
    {
        if (removalPanel != null)
        {
            removalPanel.SetActive(true);
            
            if (removalInstructionText != null)
            {
                removalInstructionText.text = $"Player {currentPlayer}, remove one of your pieces";
                removalInstructionText.color = currentPlayer == PlayerType.X ? 
                    gameSettings.playerXColor : gameSettings.playerOColor;
            }
        }
    }
    
    public void HideRemovalUI()
    {
        if (removalPanel != null)
        {
            removalPanel.SetActive(false);
        }
    }
    
    public void UpdateRemovalUI(bool hasRemovablePieces)
    {
        if (removalInstructionText != null)
        {
            if (!hasRemovablePieces)
            {
                removalInstructionText.text += "\nNo pieces can be removed. Skip your turn.";
            }
        }
    }
    
    // Add these fields to the UIManager class
    [Header("Win Counter UI")]
    [SerializeField] private GameObject winCounterPanel;
    [SerializeField] private TextMeshProUGUI playerXWinsText;
    [SerializeField] private TextMeshProUGUI playerOWinsText;
    [SerializeField] private TextMeshProUGUI winsRequiredText;
    
    // Add this method to the UIManager class
    public void UpdateWinCounters(int playerXWins, int playerOWins, int winsRequired)
    {
        if (winCounterPanel != null)
        {
            winCounterPanel.SetActive(true);
        }
        
        if (playerXWinsText != null)
        {
            playerXWinsText.text = $"Player X: {playerXWins}/{winsRequired}";
            playerXWinsText.color = gameSettings.playerXColor;
        }
        
        if (playerOWinsText != null)
        {
            playerOWinsText.text = $"Player O: {playerOWins}/{winsRequired}";
            playerOWinsText.color = gameSettings.playerOColor;
        }
        
        if (winsRequiredText != null)
        {
            winsRequiredText.text = $"First to {winsRequired} wins!";
        }
    }
    
    // Modify the ShowGameOverUI method
    public void ShowGameOverUI(PlayerType winner)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
            {
                WinConditionChecker winChecker = GameManager.Instance.GetWinConditionChecker();
                int winCount = winner == PlayerType.X ? winChecker.PlayerXWins : winChecker.PlayerOWins;
                
                gameOverText.text = $"Player {winner} Wins!\nScore: {winCount}/{winChecker.WinsRequired}";
                gameOverText.color = winner == PlayerType.X ? 
                    gameSettings.playerXColor : gameSettings.playerOColor;
            }
        }
    }
    
    private void OnRestartButtonClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        GameManager.Instance.RestartGame();
    }
    
    public void UpdateGameUI()
    {
        TurnManager turnManager = GameManager.Instance.GetComponent<TurnManager>();
        
        if (turnManager != null)
        {
            UpdatePlayerTurn(turnManager.CurrentPlayer);
            
            UpdateMoveCounter(turnManager.MoveCount);
            
            HideRemovalUI();
        }
    }
}