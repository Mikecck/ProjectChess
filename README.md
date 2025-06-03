# 3D Tic-Tac-Toe Game

## Overview

This project is a 3D implementation of the classic Tic-Tac-Toe game, featuring a 3x3x3 grid where players can place their pieces (X or O) in three dimensions. The game includes advanced mechanics such as piece stacking, piece removal, and multiple win conditions.

## Features

- 3D gameplay on a 3x3x3 grid
- Two-player mode (Player X and Player O)
- Optional AI opponent
- Piece stacking mechanics
- Piece removal after specific number of moves
- Multiple win conditions (horizontal, vertical, diagonal, and 3D diagonal)
- Win counter system requiring multiple wins to achieve victory
- Interactive UI with game state feedback

## Installation

1. Clone this repository or download the project files
2. Open the project in Unity (recommended version: Unity 2022.3.51f1 or newer)
3. Open the main scene located in `Assets/Scenes/MainLevel`
4. Press Play to start the game

## How to Play

### Basic Rules

- Players take turns placing their pieces (X or O) on the 3D grid
- Pieces can be stacked on top of each other
- After a certain number of moves, players must remove one of their own pieces
- To win, a player must form a line of three of their pieces in any direction (horizontal, vertical, diagonal, or 3D diagonal)
- The game uses a "double win" system - players must achieve the win condition twice to win the game

### Controls

- Click on a grid cell to place a piece
- When in removal mode, click on one of your pieces to remove it
- The UI will indicate whose turn it is and what action to take

## Project Structure

### Core Scripts

- `GameManager.cs`: Manages game state and coordinates between different systems
- `BoardManager.cs`: Handles the game board and piece placement
- `TurnManager.cs`: Manages player turns and move counting
- `WinConditionChecker.cs`: Checks for win conditions and tracks win counts

### Game Logic Scripts

- `GamePiece.cs`: Represents individual game pieces with owner and position
- `GridPosition.cs`: Represents 3D coordinates on the game grid
- `MoveValidator.cs`: Validates piece placement and removal
- `PieceRemovalHandler.cs`: Handles piece removal logic and cascading effects
- `AIPlayer.cs`: Implements AI opponent logic

### Input Scripts

- `InputHandler.cs`: Processes player input for piece placement and removal
- `CameraController.cs`: Manages camera movement and rotation

### UI Scripts

- `UIManager.cs`: Manages all UI elements and updates
- `GameOverUI.cs`: Handles game over screen and statistics
- `PlayerIndicator.cs`: Shows current player turn
- `PauseMenu.cs`: Implements pause functionality

## Configuration

The game can be configured through the Inspector in Unity:

- **Game Settings**: Adjust player colors (X or O), move limits, and other game parameters
- **Win Condition Checker**: Set the number of wins required to win the game
- **AI Settings**: Enable/disable AI and set difficulty

## Development

### Adding New Features

To extend the game with new features:

1. Study the existing architecture to understand how components interact
2. Make changes to the appropriate scripts or create new ones
3. Update the UI as needed to reflect new gameplay elements

## Credits

- Developed as a Unity learning project
- Inspired by the classic Tic-Tac-Toe game