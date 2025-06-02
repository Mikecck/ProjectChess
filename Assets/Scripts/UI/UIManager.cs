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
    
    public void ShowGameOverUI(PlayerType winner)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
            {
                gameOverText.text = $"Player {winner} Wins!";
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