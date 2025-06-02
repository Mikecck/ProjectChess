using UnityEngine;

public enum GameState
{
    Menu,
    Playing,
    RemovingPiece,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private WinConditionChecker winConditionChecker;
    [SerializeField] private UIManager uiManager;
    
    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    
    private GameState _currentState = GameState.Menu;
    public GameState CurrentState => _currentState;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    private void Start()
    {
        InitializeGame();
    }
    
    public void InitializeGame()
    {
        boardManager.Initialize();
        turnManager.Initialize(gameSettings.movesBeforeRemoval);
        winConditionChecker.Initialize();
        
        ChangeState(GameState.Playing);
        
        if (uiManager != null)
        {
            uiManager.UpdatePlayerTurn(turnManager.CurrentPlayer);
            uiManager.UpdateMoveCounter(turnManager.MoveCount);
        }
    }
    
    public void ChangeState(GameState newState)
    {
        _currentState = newState;
        
        switch (newState)
        {
            case GameState.Menu:
                break;
                
            case GameState.Playing:
                break;
                
            case GameState.RemovingPiece:
                if (uiManager != null)
                {
                    uiManager.ShowRemovalUI(turnManager.CurrentPlayer);
                }
                break;
                
            case GameState.GameOver:
                if (uiManager != null)
                {
                    uiManager.ShowGameOverUI(turnManager.CurrentPlayer);
                }
                break;
        }
    }
    
    public void HandlePiecePlacement(GridPosition position)
    {
        if (_currentState != GameState.Playing) return;
        
        GamePiece piece = turnManager.CreatePieceForCurrentPlayer();
        
        if (boardManager.PlacePiece(piece, position))
        {
            piece.Position = position;
            piece.MoveNumber = turnManager.MoveCount;
            
            if (winConditionChecker.CheckForWin(position, turnManager.CurrentPlayer))
            {
                ChangeState(GameState.GameOver);
                return;
            }
            
            turnManager.AdvanceTurn();
            
            if (turnManager.ShouldRemovePiece())
            {
                ChangeState(GameState.RemovingPiece);
            }
            
            if (uiManager != null)
            {
                uiManager.UpdatePlayerTurn(turnManager.CurrentPlayer);
                uiManager.UpdateMoveCounter(turnManager.MoveCount);
            }
        }
    }
    
    public void HandlePieceRemoval(GridPosition position)
    {
        if (_currentState != GameState.RemovingPiece) return;
        
        if (boardManager.RemovePiece(position))
        {
            ChangeState(GameState.Playing);
            
            if (winConditionChecker.CheckBoardState())
            {
                ChangeState(GameState.GameOver);
            }
        }
    }
    
    public void RestartGame()
    {
        boardManager.ClearBoard();
        turnManager.Reset();
        
        InitializeGame();
    }
    
    public GamePiece CreatePiece(PlayerType playerType)
    {
        return turnManager.CreatePiece(playerType);
    }
    
    public void OnPieceRemoved()
    {
        _currentState = GameState.Playing;
        
        if (uiManager != null)
        {
            uiManager.UpdateGameUI();
        }
    }
    
    public PlayerType CurrentPlayer => turnManager.CurrentPlayer;
}