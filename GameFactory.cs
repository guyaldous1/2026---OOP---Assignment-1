using System;
using System.Collections.Generic;
using System.Text;



// ============================================================
// GAME FACTORY - Factory pattern
// ============================================================
static class GameFactory
{
    public static Game CreateGame(string type)
    {
        return type switch
        {
            "tictactoe" => new TicTacToe(),
            "notakto" => new Notakto(),
            "gomoku" => new Gomoku(),
            _ => throw new ArgumentException($"Unknown game type: {type}")
        };
    }

    public static Game CreateFromState(GameStateMemento state)
    {
        Game game = state.GameType switch
        {
            "tictactoe" => new TicTacToe(state),
            "notakto" => new Notakto(state),
            "gomoku" => new Gomoku(state),
            _ => throw new ArgumentException($"Unknown game type: {state.GameType}")
        };

        return game;
    }
}
