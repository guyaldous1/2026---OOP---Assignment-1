# OOP Assignment 3 - Board Games Framework

A C# console application featuring three board games: **Numerical Tic-Tac-Toe**, **Notakto**, and **Gomoku**.

## Requirements to Run

- [.NET 10](https://dotnet.microsoft.com/download)

Verify your version: `dotnet --version`

## How to Run

From the project folder: `dotnet run`

## Games

- **Numerical Tic-Tac-Toe** — Players place odd/even numbers; first to sum N in a line wins
- **Notakto** — Impartial Tic-Tac-Toe; both players use X, avoid completing a line on all three boards.
- **Gomoku** — First to place 5 pieces in a row wins. More than 5 doesn't count.

All games support **Human vs Human** and **Human vs Computer**, and Numerical supports variable board sizes.

## In-Game Commands

- *(Enter)* — Take your turn
- `help` — Show rules and commands
- `save` — Save current game
- `load` — Load a saved game
- `undo` — Undo last move
- `redo` — Redo undone move
- `quit` — Exit the game
