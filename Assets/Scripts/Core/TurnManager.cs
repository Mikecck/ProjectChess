using UnityEngine;
using System.Collections.Generic;

public enum PlayerType
{
    X,
    O
}

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject xPiecePrefab;
    [SerializeField] private GameObject oPiecePrefab;
    [SerializeField] private Transform piecesParent;
    
    [Header("Settings")]
    [SerializeField] private int movesBeforeRemoval = 6;
    
    private PlayerType _currentPlayer = PlayerType.X;
    private int _moveCount = 0;
    private Queue<GamePiece> xPieceHistory = new Queue<GamePiece>();
    private Queue<GamePiece> oPieceHistory = new Queue<GamePiece>();
    
    public PlayerType CurrentPlayer => _currentPlayer;
    public int MoveCount => _moveCount;
    
    public void Initialize(int removalThreshold)
    {
        movesBeforeRemoval = removalThreshold;
        Reset();
    }
    
    public void Reset()
    {
        _currentPlayer = PlayerType.X;
        _moveCount = 0;
        xPieceHistory.Clear();
        oPieceHistory.Clear();
    }
    

    public GamePiece CreatePiece(PlayerType playerType)
    {
        GameObject prefab = playerType == PlayerType.X ? xPiecePrefab : oPiecePrefab;
        GameObject pieceObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, piecesParent);
        
        GamePiece piece = pieceObject.GetComponent<GamePiece>();
        if (piece == null)
        {
            piece = pieceObject.AddComponent<GamePiece>();
        }
        
        piece.Initialize(playerType);
        
        // Add to history
        if (playerType == PlayerType.X)
            xPieceHistory.Enqueue(piece);
        else
            oPieceHistory.Enqueue(piece);
            
        return piece;
    }
    
    // Add these methods for turn management
    public void IncrementMoveCount()
    {
        _moveCount++;
    }
    
    public void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;
    }
    
    public void AdvanceTurn()
    {
        IncrementMoveCount();
        SwitchPlayer();
    }
    
    public bool ShouldRemovePiece()
    {
        Queue<GamePiece> currentPlayerHistory = _currentPlayer == PlayerType.X ? xPieceHistory : oPieceHistory;
        return currentPlayerHistory.Count >= movesBeforeRemoval;
    }
    
    public GamePiece GetOldestPiece(PlayerType playerType)
    {
        Queue<GamePiece> history = playerType == PlayerType.X ? xPieceHistory : oPieceHistory;
        return history.Count > 0 ? history.Peek() : null;
    }
    
    public void RemovePieceFromHistory(GamePiece piece)
    {
        
        Queue<GamePiece> history = piece.Owner == PlayerType.X ? xPieceHistory : oPieceHistory;
        Queue<GamePiece> newHistory = new Queue<GamePiece>();
        
        while (history.Count > 0)
        {
            GamePiece current = history.Dequeue();
            if (current != piece)
            {
                newHistory.Enqueue(current);
            }
        }
        
        if (piece.Owner == PlayerType.X)
        {
            xPieceHistory = newHistory;
        }
        else
        {
            oPieceHistory = newHistory;
        }
    }
    
    public GamePiece CreatePieceForCurrentPlayer()
    {
        return CreatePiece(_currentPlayer);
    }
}