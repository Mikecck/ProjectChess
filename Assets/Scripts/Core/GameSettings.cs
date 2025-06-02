using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "TicTacToe/Settings")]
// This class is used to store default game settings, change the actual settings in the editor if needed
public class GameSettings : ScriptableObject
{
    [Header("Board Settings")]
    [Tooltip("Size of the board in each dimension (3 for a 3x3x3 grid)")]
    public int boardSize = 3;
    
    [Header("Removal Settings")]
    [Tooltip("Number of moves a player must make before they are required to remove one of their pieces")]
    public int movesBeforeRemoval = 6;
    
    [Tooltip("If true, removing a piece can cause pieces above to fall/be removed")]
    public bool allowRemovalCascade = true;
    
    [Header("Win Conditions")]
    [Tooltip("If true, diagonal lines through the 3D cube can form winning lines")]
    public bool enableDiagonal3D = true;
    
    [Tooltip("If true, vertical lines can form winning lines")]
    public bool enableVerticalWins = true;
    
    [Header("UI Settings")]
    [Tooltip("Color for Player X")]
    public Color playerXColor = Color.black;
    
    [Tooltip("Color for Player O")]
    public Color playerOColor = Color.white;
}