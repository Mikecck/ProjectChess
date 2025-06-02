using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TurnManager turnManager;

    [Header("UI Elements")]
    [SerializeField] private Image playerXIndicator;
    [SerializeField] private Image playerOIndicator;
    [SerializeField] private TextMeshProUGUI playerXText;
    [SerializeField] private TextMeshProUGUI playerOText;

    [Header("Settings")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private GameSettings gameSettings;

    [Header("Alpha Settings")]
    [SerializeField] private float activeAlpha = 1.0f;
    [SerializeField] private float inactiveAlpha = 0.0f;

    private void Start()
    {
        UpdateIndicator();
    }

    private void Update()
    {
        if (gameManager.CurrentState == GameState.Playing ||
            gameManager.CurrentState == GameState.RemovingPiece)
        {
            UpdateIndicator();
        }
    }

    private void UpdateIndicator()
    {
        PlayerType currentPlayer = turnManager.CurrentPlayer;

        if (playerXIndicator != null)
        {
            playerXIndicator.color = currentPlayer == PlayerType.X ? activeColor : inactiveColor;
        }

        if (playerOIndicator != null)
        {
            playerOIndicator.color = currentPlayer == PlayerType.O ? activeColor : inactiveColor;
        }

        if (playerXText != null)
        {
            Color xColor = gameSettings.playerXColor;
            xColor.a = currentPlayer == PlayerType.X ? activeAlpha : inactiveAlpha;
            playerXText.color = xColor;
        }

        if (playerOText != null)
        {
            Color oColor = gameSettings.playerOColor;
            oColor.a = currentPlayer == PlayerType.O ? activeAlpha : inactiveAlpha;
            playerOText.color = oColor;
        }

        if (gameManager.CurrentState == GameState.RemovingPiece)
        {
            if (currentPlayer == PlayerType.X && playerXText != null)
            {
                playerXText.text = "Player X (Remove)";
            }
            else if (currentPlayer == PlayerType.O && playerOText != null)
            {
                playerOText.text = "Player O (Remove)";
            }
        }
        else
        {
            // Reset text
            if (playerXText != null) playerXText.text = "Player X";
            if (playerOText != null) playerOText.text = "Player O";
        }
    }
}