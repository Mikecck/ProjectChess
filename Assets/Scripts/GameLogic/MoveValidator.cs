using UnityEngine;

public class MoveValidator : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TurnManager turnManager;
    
    public bool ValidateMove(GridPosition position, PlayerType playerType)
    {
        // Basic validation
        if (!position.IsValid())
            return false;
            
        // Check if it's this player's turn
        if (turnManager.CurrentPlayer != playerType)
            return false;
            
        // Check if position is empty
        if (!boardManager.IsPositionEmpty(position))
            return false;
            
        // Check stacking rule - must have support below
        if (!boardManager.HasSupportBelow(position))
            return false;
            
        return true;
    }
    
    public bool ValidateRemoval(GridPosition position, PlayerType playerType)
    {
        // Check if position is valid and contains a piece
        if (!position.IsValid() || boardManager.IsPositionEmpty(position))
            return false;
            
        // Get the piece at this position
        GamePiece piece = boardManager.GetPieceAt(position);
        
        // Check if the piece belongs to the player
        if (piece.Owner != playerType)
            return false;
            
        // Check if removing would cause unsupported pieces
        if (position.y < 2 && !boardManager.IsPositionEmpty(new GridPosition(position.x, position.y + 1, position.z)))
            return false;
            
        return true;
    }
}