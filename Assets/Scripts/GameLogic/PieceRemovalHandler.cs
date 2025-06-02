using UnityEngine;
using System.Collections.Generic;

public class PieceRemovalHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;
    
    [Header("Settings")]
    [SerializeField] private bool allowRemovalCascade = true;
    
    private List<GamePiece> highlightedPieces = new List<GamePiece>();
    
    private void Awake()
    {
        // Ensure we have references
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();
            
        if (turnManager == null)
            turnManager = FindObjectOfType<TurnManager>();
            
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
            
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }
    
    public void HighlightRemovablePieces(PlayerType playerType)
    {
        // Clear previous highlights
        ClearHighlights();
        
        // Get all pieces for the current player
        List<GamePiece> playerPieces = boardManager.GetPlayerPieces(playerType);
        
        // Filter for pieces that can be removed (no pieces above them)
        foreach (var piece in playerPieces)
        {
            if (CanRemovePiece(piece.Position))
            {
                // Highlight this piece
                piece.Highlight(true);
                highlightedPieces.Add(piece);
            }
        }
        
        // Update UI to show which pieces can be removed
        if (uiManager != null)
        {
            uiManager.UpdateRemovalUI(highlightedPieces.Count > 0);
        }
    }
    
    public bool CanRemovePiece(GridPosition position)
    {
        // Check if there's a piece above this one
        if (position.y < 2) // Not on top layer
        {
            GridPosition positionAbove = new GridPosition(position.x, position.y + 1, position.z);
            if (!boardManager.IsPositionEmpty(positionAbove))
            {
                return false; // Can't remove if there's a piece above
            }
        }
        
        return true;
    }
    
    private void ClearHighlights()
    {
        // Remove all visual highlights
        foreach (var piece in highlightedPieces)
        {
            if (piece != null)
            {
                piece.Highlight(false);
            }
        }
        
        highlightedPieces.Clear();
    }
    
    public bool RemovePiece(GridPosition position)
    {
        // Check if the position contains a removable piece
        GamePiece piece = boardManager.GetPieceAt(position);
        if (piece == null || !CanRemovePiece(position))
        {
            return false;
        }
        
        // Check if this piece belongs to the current player
        if (piece.Owner != turnManager.CurrentPlayer)
        {
            return false;
        }
        
        // Remove the piece
        boardManager.RemovePiece(position);
        
        // Clear highlights
        ClearHighlights();
        
        // Check for cascade removals if enabled
        if (allowRemovalCascade)
        {
            HandleCascadeRemovals();
        }
        
        // Notify game manager that a piece has been removed
        gameManager.OnPieceRemoved();
        
        return true;
    }
    
    private void HandleCascadeRemovals()
    {
        bool removedAny;
        
        do
        {
            removedAny = false;
            
            // Get all pieces on the board
            List<GamePiece> allPieces = boardManager.GetAllPieces();
            List<GamePiece> piecesToRemove = new List<GamePiece>();
            
            // Find unsupported pieces
            foreach (var piece in allPieces)
            {
                GridPosition position = piece.Position;
                
                // Skip pieces on the bottom layer
                if (position.y == 0)
                    continue;
                    
                // Check if there's support below
                GridPosition positionBelow = new GridPosition(position.x, position.y - 1, position.z);
                if (boardManager.IsPositionEmpty(positionBelow))
                {
                    // This piece has no support, mark for removal
                    piecesToRemove.Add(piece);
                }
            }
            
            // Remove all unsupported pieces
            foreach (var piece in piecesToRemove)
            {
                boardManager.RemovePiece(piece.Position);
                removedAny = true;
            }
            
        } while (removedAny); // Continue until no more pieces are removed
    }
    
    public bool HasRemovablePieces(PlayerType playerType)
    {
        List<GamePiece> playerPieces = boardManager.GetPlayerPieces(playerType);
        
        foreach (var piece in playerPieces)
        {
            if (CanRemovePiece(piece.Position))
            {
                return true;
            }
        }
        
        return false;
    }
}