using UnityEngine;
using System.Collections.Generic;

public class WinConditionChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManager boardManager;
    
    [Header("Settings")]
    [SerializeField] private bool enableDiagonal3D = true;
    [SerializeField] private bool enableVerticalWins = true;
    
    // Win line definitions
    private List<List<GridPosition>> winLines = new List<List<GridPosition>>();
    
    public void Initialize()
    {
        GenerateWinLines();
    }
    
    private void GenerateWinLines()
    {
        winLines.Clear();
        
        // Generate all possible win lines for a 3x3x3 grid
        
        // 1. Straight lines along each axis (27 lines: 9 in each direction)
        GenerateStraightLines();
        
        // 2. Diagonal lines on each layer (6 lines per layer = 18 total)
        GenerateLayerDiagonals();
        
        // 3. Vertical lines (9 lines)
        if (enableVerticalWins)
        {
            GenerateVerticalLines();
        }
        
        // 4. 3D diagonals (4 lines that go through the center of the cube)
        if (enableDiagonal3D)
        {
            Generate3DDiagonals();
        }
    }
    
    private void GenerateStraightLines()
    {
        // X-axis lines
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                List<GridPosition> line = new List<GridPosition>();
                for (int x = 0; x < 3; x++)
                {
                    line.Add(new GridPosition(x, y, z));
                }
                winLines.Add(line);
            }
        }
        
        // Z-axis lines
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                List<GridPosition> line = new List<GridPosition>();
                for (int z = 0; z < 3; z++)
                {
                    line.Add(new GridPosition(x, y, z));
                }
                winLines.Add(line);
            }
        }
        
        // Y-axis lines (only if vertical wins are enabled)
        if (enableVerticalWins)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    List<GridPosition> line = new List<GridPosition>();
                    for (int y = 0; y < 3; y++)
                    {
                        line.Add(new GridPosition(x, y, z));
                    }
                    winLines.Add(line);
                }
            }
        }
    }
    
    private void GenerateLayerDiagonals()
    {
        // For each layer (y value)
        for (int y = 0; y < 3; y++)
        {
            // Diagonal 1: (0,y,0) to (2,y,2)
            List<GridPosition> diag1 = new List<GridPosition>
            {
                new GridPosition(0, y, 0),
                new GridPosition(1, y, 1),
                new GridPosition(2, y, 2)
            };
            winLines.Add(diag1);
            
            // Diagonal 2: (2,y,0) to (0,y,2)
            List<GridPosition> diag2 = new List<GridPosition>
            {
                new GridPosition(2, y, 0),
                new GridPosition(1, y, 1),
                new GridPosition(0, y, 2)
            };
            winLines.Add(diag2);
        }
    }
    
    private void GenerateVerticalLines()
    {
        // Already handled in GenerateStraightLines
    }
    
    private void Generate3DDiagonals()
    {
        // Diagonal 1: (0,0,0) to (2,2,2)
        List<GridPosition> diag1 = new List<GridPosition>
        {
            new GridPosition(0, 0, 0),
            new GridPosition(1, 1, 1),
            new GridPosition(2, 2, 2)
        };
        winLines.Add(diag1);
        
        // Diagonal 2: (2,0,0) to (0,2,2)
        List<GridPosition> diag2 = new List<GridPosition>
        {
            new GridPosition(2, 0, 0),
            new GridPosition(1, 1, 1),
            new GridPosition(0, 2, 2)
        };
        winLines.Add(diag2);
        
        // Diagonal 3: (0,0,2) to (2,2,0)
        List<GridPosition> diag3 = new List<GridPosition>
        {
            new GridPosition(0, 0, 2),
            new GridPosition(1, 1, 1),
            new GridPosition(2, 2, 0)
        };
        winLines.Add(diag3);
        
        // Diagonal 4: (2,0,2) to (0,2,0)
        List<GridPosition> diag4 = new List<GridPosition>
        {
            new GridPosition(2, 0, 2),
            new GridPosition(1, 1, 1),
            new GridPosition(0, 2, 0)
        };
        winLines.Add(diag4);
    }
    
    public bool CheckForWin(GridPosition lastPlacedPosition, PlayerType playerType)
    {
        // Check all possible win lines
        foreach (var line in winLines)
        {
            // Only check lines that contain the last placed position
            if (ContainsPosition(line, lastPlacedPosition))
            {
                if (CheckLine(line, playerType))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private bool ContainsPosition(List<GridPosition> line, GridPosition position)
    {
        foreach (var pos in line)
        {
            if (pos.x == position.x && pos.y == position.y && pos.z == position.z)
            {
                return true;
            }
        }
        return false;
    }
    
    private bool CheckLine(List<GridPosition> line, PlayerType playerType)
    {
        foreach (var position in line)
        {
            GamePiece piece = boardManager.GetPieceAt(position);
            if (piece == null || piece.Owner != playerType)
            {
                return false;
            }
        }
        return true;
    }
    
    public bool CheckBoardState()
    {
        // Check if any player has won
        foreach (var line in winLines)
        {
            if (CheckLine(line, PlayerType.X))
            {
                return true;
            }
            
            if (CheckLine(line, PlayerType.O))
            {
                return true;
            }
        }
        
        return false;
    }
}