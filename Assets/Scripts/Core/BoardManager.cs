using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private float cellSpacing = 1.0f;
    [SerializeField] private float layerHeight = 1.0f;

    private GamePiece[,,] grid = new GamePiece[3, 3, 3];
    private GameObject[,,] gridCells = new GameObject[3, 3, 3];
    private GamePiece[,,] simulationGrid = new GamePiece[3, 3, 3];

    public void Initialize()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        // Destroy existing grid if rebuilding
        if (gridCells[0, 0, 0] != null)
            ClearGridCells();

        for (int y = 0; y < 3; y++)
        {
            GameObject layerParent = new GameObject($"Layer{y}");
            layerParent.transform.SetParent(gridParent);
            layerParent.transform.localPosition = new Vector3(0, y * layerHeight, 0);

            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Vector3 localPosition = new Vector3(x * cellSpacing, 0, z * cellSpacing);
                    GameObject cell = Instantiate(gridCellPrefab, layerParent.transform);
                    cell.transform.localPosition = localPosition;

                    // Get GridCell component - add if missing
                    GridCell gridCellComp = cell.GetComponent<GridCell>();
                    if (gridCellComp == null)
                    {
                        gridCellComp = cell.AddComponent<GridCell>();
                    }

                    // Set position
                    gridCellComp.Position = new GridPosition(x, y, z);


                    cell.name = $"Cell({x},{y},{z})";
                    gridCells[x, y, z] = cell;

                    // Debug position assignment
                    Debug.Log($"Created cell at ({x},{y},{z})");
                }
            }
        }
    }

    private void ClearGridCells()
    {
        // Destroy all children of gridParent (clears layers and cells)
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        gridCells = new GameObject[3, 3, 3];
    }
    
    public bool HasSupportBelow(GridPosition position)
    {
        // If on bottom layer, always valid
        if (position.y == 0) return true;
        
        // Check if there's a piece directly below
        return grid[position.x, position.y - 1, position.z] != null;
    }
    
    public bool IsPositionEmpty(GridPosition position)
    {
        return grid[position.x, position.y, position.z] == null;
    }
    
    public bool PlacePiece(GamePiece piece, GridPosition position)
    {
        if (!position.IsValid() || !IsPositionEmpty(position) || !HasSupportBelow(position))
            return false;
            
        grid[position.x, position.y, position.z] = piece;
        
        Vector3 worldPos = gridCells[position.x, position.y, position.z].transform.position;
        piece.transform.position = worldPos;
        
        return true;
    }
    
    public bool RemovePiece(GridPosition position)
    {
        if (!position.IsValid() || IsPositionEmpty(position))
            return false;
            
        if (position.y < 2 && grid[position.x, position.y + 1, position.z] != null)
        {
            return false;
        }
        
        GamePiece piece = grid[position.x, position.y, position.z];
        grid[position.x, position.y, position.z] = null;
        
        Destroy(piece.gameObject);
        
        return true;
    }
    
    public List<GamePiece> GetPlayerPieces(PlayerType playerType)
    {
        List<GamePiece> pieces = new List<GamePiece>();
        
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    GamePiece piece = grid[x, y, z];
                    if (piece != null && piece.Owner == playerType)
                    {
                        pieces.Add(piece);
                    }
                }
            }
        }
        
        return pieces;
    }
    
    public List<GamePiece> GetAllPieces()
    {
        List<GamePiece> pieces = new List<GamePiece>();
        
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (grid[x, y, z] != null)
                    {
                        pieces.Add(grid[x, y, z]);
                    }
                }
            }
        }
        
        return pieces;
    }
    
    public GamePiece GetPieceAt(GridPosition position)
    {
        if (!position.IsValid())
            return null;
            
        return grid[position.x, position.y, position.z];
    }
    
    public void ClearBoard()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    GamePiece piece = grid[x, y, z];
                    if (piece != null)
                    {
                        Destroy(piece.gameObject);
                        grid[x, y, z] = null;
                    }
                }
            }
        }
    }
    
    public bool IsBoardFull()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (grid[x, y, z] == null)
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }


    //Extra functions for simulation
    public void StartSimulation()
    {
        // Copy the current grid to the simulation grid
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    simulationGrid[x, y, z] = grid[x, y, z];
                }
            }
        }
    }

    public void EndSimulation()
    {
        // Restore the original grid
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    grid[x, y, z] = simulationGrid[x, y, z];
                }
            }
        }
    }

    public bool SimulatePlacePiece(GridPosition position, PlayerType playerType)
    {
        if (!position.IsValid() || !IsPositionEmpty(position) || !HasSupportBelow(position))
            return false;

        // Create a temporary piece for simulation
        GameObject tempObject = new GameObject("TempPiece");
        GamePiece tempPiece = tempObject.AddComponent<GamePiece>();
        tempPiece.Initialize(playerType);
        tempPiece.Position = position;

        grid[position.x, position.y, position.z] = tempPiece;

        return true;
    }

    public void UndoSimulatedMove(GridPosition position)
    {
        if (!position.IsValid())
            return;

        GamePiece piece = grid[position.x, position.y, position.z];
        if (piece != null)
        {
            Destroy(piece.gameObject);
            grid[position.x, position.y, position.z] = null;
        }
    }

    public GamePiece SimulateRemovePiece(GridPosition position)
    {
        if (!position.IsValid() || IsPositionEmpty(position))
            return null;

        GamePiece piece = grid[position.x, position.y, position.z];
        grid[position.x, position.y, position.z] = null;

        return piece;
    }

    public void UndoSimulatedRemoval(GridPosition position, GamePiece piece)
    {
        if (!position.IsValid() || !IsPositionEmpty(position))
            return;

        grid[position.x, position.y, position.z] = piece;
    }
}
