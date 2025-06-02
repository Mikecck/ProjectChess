using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private MoveValidator moveValidator;
    [SerializeField] private LayerMask gridCellLayer;
    [SerializeField] private LayerMask piecesLayer;
    
    private void Awake()
    {
        // Ensure we have a camera reference
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    private void Update()
    {
        // Handle input based on current game state
        switch (gameManager.CurrentState)
        {
            case GameState.Playing:
                HandlePlacementInput();
                break;
                
            case GameState.RemovingPiece:
                HandleRemovalInput();
                break;
        }
    }

    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, gridCellLayer))
            {
                GameObject cell = hit.collider.gameObject;
                GridPosition position;
                bool positionValid = false;

                // First try: Get from GridCell component
                GridCell gridCell = cell.GetComponent<GridCell>();
                if (gridCell != null)
                {
                    position = gridCell.Position; // CORRECTED property name
                    positionValid = true;
                    Debug.Log($"Using GridCell component: {position}");
                }
                // Second try: Parse from name
                else
                {
                    try
                    {
                        position = ParseCellName(cell.name);
                        positionValid = true;
                        Debug.Log($"Parsed from name: {position}");
                    }
                    catch
                    {
                        Debug.LogError($"Failed to get position for cell: {cell.name}");
                        position = new GridPosition(0, 0, 0);
                    }
                }

                if (positionValid)
                {
                    if (moveValidator.ValidateMove(position, gameManager.CurrentPlayer))
                    {
                        // Use the position variable we already set
                        gameManager.HandlePiecePlacement(position);
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid move at position {position}");
                    }
                }
            }
        }
    }
    
    private void HandleRemovalInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f, piecesLayer))
            {
                // Get the piece that was clicked
                GamePiece piece = hit.collider.gameObject.GetComponent<GamePiece>();
                if (piece != null)
                {
                    // Validate and handle the removal
                    if (moveValidator.ValidateRemoval(piece.Position, gameManager.CurrentPlayer))
                    {
                        gameManager.HandlePieceRemoval(piece.Position);
                    }
                }
            }
        }
    }
    
    private GridPosition ParseCellName(string cellName)
    {
        // Extract coordinates from cell name format "Cell(x,y,z)"
        // This part expects names like "Cell(0,0,0)", not "Cube"
        string[] parts = cellName.Replace("Cell(", "").Replace(")", "").Split(',');
        
        if (parts.Length == 3)
        {
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            int z = int.Parse(parts[2]);
            
            return new GridPosition(x, y, z);
        }
        
        // Default position if parsing fails
        Debug.LogError($"Failed to parse cell name: {cellName}");
        return new GridPosition(0, 0, 0);
    }
}