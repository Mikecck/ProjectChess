using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private WinConditionChecker winConditionChecker;
    [SerializeField] private TurnManager turnManager;
    
    [Header("AI Settings")]
    [SerializeField] private PlayerType aiPlayerType = PlayerType.O;
    [SerializeField] private float moveDelay = 1.0f; // Delay before AI makes a move
    [SerializeField] private int searchDepth = 2; // Minimax search depth
    
    private bool isAITurn = false;
    private Coroutine aiMoveCoroutine;
    
    private void Awake()
    {
        // Get references if not set in inspector
        if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
        if (boardManager == null) boardManager = FindObjectOfType<BoardManager>();
        if (winConditionChecker == null) winConditionChecker = FindObjectOfType<WinConditionChecker>();
        if (turnManager == null) turnManager = FindObjectOfType<TurnManager>();
    }
    
    private void OnEnable()
    {
        // Subscribe to game state changes
        StartCoroutine(CheckAITurn());
    }
    
    private void OnDisable()
    {
        // Stop any ongoing AI move coroutine
        if (aiMoveCoroutine != null)
        {
            StopCoroutine(aiMoveCoroutine);
            aiMoveCoroutine = null;
        }
    }
    
    private IEnumerator CheckAITurn()
    {
        // Wait a frame to ensure game is initialized
        yield return null;
        
        while (true)
        {
            // Check if it's AI's turn
            if (gameManager.CurrentState == GameState.Playing && 
                turnManager.CurrentPlayer == aiPlayerType)
            {
                if (aiMoveCoroutine == null)
                {
                    aiMoveCoroutine = StartCoroutine(MakeAIMove());
                }
            }
            else if (gameManager.CurrentState == GameState.RemovingPiece && 
                     turnManager.CurrentPlayer == aiPlayerType)
            {
                if (aiMoveCoroutine == null)
                {
                    aiMoveCoroutine = StartCoroutine(RemoveAIPiece());
                }
            }
            else
            {
                // Reset the coroutine reference when it's not AI's turn
                aiMoveCoroutine = null;
            }
            
            yield return new WaitForSeconds(0.1f); // Check periodically
        }
    }
    
    private IEnumerator MakeAIMove()
    {
        // Add a delay to make the AI seem like it's "thinking"
        yield return new WaitForSeconds(moveDelay);
        
        // Find the best move
        GridPosition bestMove = FindBestMove();
        
        // Make the move
        gameManager.HandlePiecePlacement(bestMove);
        
        // Reset the coroutine reference
        aiMoveCoroutine = null;
    }
    
    private IEnumerator RemoveAIPiece()
    {
        // Add a delay to make the AI seem like it's "thinking"
        yield return new WaitForSeconds(moveDelay);
        
        // Find the best piece to remove
        GridPosition bestRemoval = FindBestRemoval();
        
        // Remove the piece
        gameManager.HandlePieceRemoval(bestRemoval);
        
        // Reset the coroutine reference
        aiMoveCoroutine = null;
    }
    
    private GridPosition FindBestMove()
    {
        List<GridPosition> validMoves = GetValidMoves();
        
        if (validMoves.Count == 0)
            return new GridPosition(0, 0, 0); // Should never happen in a valid game state
            
        // For the first few moves, we might want to use a simpler strategy
        if (turnManager.MoveCount < 2)
        {
            // Start with a corner or center position if available
            foreach (var move in validMoves)
            {
                // Prefer the center position of the bottom layer
                if (move.x == 1 && move.y == 0 && move.z == 1)
                    return move;
                    
                // Then prefer corners of the bottom layer
                if (move.y == 0 && (move.x == 0 || move.x == 2) && (move.z == 0 || move.z == 2))
                    return move;
            }
        }
        
        // Use minimax for more complex positions
        int bestScore = int.MinValue;
        GridPosition bestMove = validMoves[0];
        
        foreach (var move in validMoves)
        {
            // Simulate making this move
            GamePiece piece = SimulatePlacePiece(move, aiPlayerType);
            
            // Evaluate with minimax
            int score = Minimax(searchDepth, false, int.MinValue, int.MaxValue);
            
            // Undo the move
            UndoSimulatedMove(move);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        
        return bestMove;
    }
    
    private GridPosition FindBestRemoval()
    {
        List<GamePiece> playerPieces = boardManager.GetPlayerPieces(aiPlayerType);
        List<GridPosition> validRemovals = new List<GridPosition>();
        
        // Find all valid pieces to remove
        foreach (var piece in playerPieces)
        {
            if (CanRemovePiece(piece.Position))
            {
                validRemovals.Add(piece.Position);
            }
        }
        
        if (validRemovals.Count == 0)
            return new GridPosition(0, 0, 0); // Should never happen in a valid game state
            
        // If only one option, return it
        if (validRemovals.Count == 1)
            return validRemovals[0];
            
        // Use minimax to find the best piece to remove
        int bestScore = int.MinValue;
        GridPosition bestRemoval = validRemovals[0];
        
        foreach (var position in validRemovals)
        {
            // Simulate removing this piece
            GamePiece removedPiece = boardManager.GetPieceAt(position);
            SimulateRemovePiece(position);
            
            // Evaluate with minimax
            int score = Minimax(searchDepth, false, int.MinValue, int.MaxValue);
            
            // Undo the removal
            UndoSimulatedRemoval(position, removedPiece);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestRemoval = position;
            }
        }
        
        return bestRemoval;
    }
    
    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        // Check for terminal states
        if (CheckForWin(aiPlayerType))
            return 100 + depth; // Win for AI
            
        PlayerType opponentType = (aiPlayerType == PlayerType.X) ? PlayerType.O : PlayerType.X;
        if (CheckForWin(opponentType))
            return -100 - depth; // Win for opponent
            
        if (depth == 0)
            return EvaluateBoard(); // Evaluate the current board state
            
        if (isMaximizing) // AI's turn
        {
            int maxScore = int.MinValue;
            List<GridPosition> validMoves = GetValidMoves();
            
            foreach (var move in validMoves)
            {
                GamePiece piece = SimulatePlacePiece(move, aiPlayerType);
                int score = Minimax(depth - 1, false, alpha, beta);
                UndoSimulatedMove(move);
                
                maxScore = Mathf.Max(maxScore, score);
                alpha = Mathf.Max(alpha, score);
                
                if (beta <= alpha)
                    break; // Beta cutoff
            }
            
            return maxScore;
        }
        else // Opponent's turn
        {
            int minScore = int.MaxValue;
            List<GridPosition> validMoves = GetValidMoves();
            
            foreach (var move in validMoves)
            {
                GamePiece piece = SimulatePlacePiece(move, opponentType);
                int score = Minimax(depth - 1, true, alpha, beta);
                UndoSimulatedMove(move);
                
                minScore = Mathf.Min(minScore, score);
                beta = Mathf.Min(beta, score);
                
                if (beta <= alpha)
                    break; // Alpha cutoff
            }
            
            return minScore;
        }
    }
    
    private int EvaluateBoard()
    {
        int score = 0;
        PlayerType opponentType = (aiPlayerType == PlayerType.X) ? PlayerType.O : PlayerType.X;
        
        // Get all win lines from WinConditionChecker
        var winLines = GetWinLines();
        
        foreach (var line in winLines)
        {
            int aiCount = 0;
            int opponentCount = 0;
            int emptyCount = 0;
            
            foreach (var pos in line)
            {
                GamePiece piece = boardManager.GetPieceAt(pos);
                if (piece == null)
                    emptyCount++;
                else if (piece.Owner == aiPlayerType)
                    aiCount++;
                else
                    opponentCount++;
            }
            
            // Score the line based on piece counts
            if (opponentCount == 0) // Only AI pieces and empty spaces
            {
                if (aiCount == 2) score += 10; // Two in a row
                if (aiCount == 1) score += 1;  // One in a row
            }
            
            if (aiCount == 0) // Only opponent pieces and empty spaces
            {
                if (opponentCount == 2) score -= 10; // Block opponent's two in a row
                if (opponentCount == 1) score -= 1;  // Block opponent's one in a row
            }
        }
        
        return score;
    }
    
    // Helper methods for board manipulation and validation
    
    private List<GridPosition> GetValidMoves()
    {
        List<GridPosition> validMoves = new List<GridPosition>();
        
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    GridPosition pos = new GridPosition(x, y, z);
                    if (boardManager.IsPositionEmpty(pos) && boardManager.HasSupportBelow(pos))
                    {
                        validMoves.Add(pos);
                    }
                }
            }
        }
        
        return validMoves;
    }
    
    private bool CanRemovePiece(GridPosition position)
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
    
    private bool CheckForWin(PlayerType playerType)
    {
        // Check all win lines for a win
        var winLines = GetWinLines();
        
        foreach (var line in winLines)
        {
            bool isWin = true;
            foreach (var pos in line)
            {
                GamePiece piece = boardManager.GetPieceAt(pos);
                if (piece == null || piece.Owner != playerType)
                {
                    isWin = false;
                    break;
                }
            }
            
            if (isWin)
                return true;
        }
        
        return false;
    }
    
    // Update these methods in the AIPlayer class
    
    private GamePiece SimulatePlacePiece(GridPosition position, PlayerType playerType)
    {
        boardManager.SimulatePlacePiece(position, playerType);
        return boardManager.GetPieceAt(position);
    }
    
    private void UndoSimulatedMove(GridPosition position)
    {
        boardManager.UndoSimulatedMove(position);
    }
    
    private void SimulateRemovePiece(GridPosition position)
    {
        boardManager.SimulateRemovePiece(position);
    }
    
    private void UndoSimulatedRemoval(GridPosition position, GamePiece piece)
    {
        boardManager.UndoSimulatedRemoval(position, piece);
    }
    
    private List<List<GridPosition>> GetWinLines()
    {
        return winConditionChecker.WinLines;
    }
}