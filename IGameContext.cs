using System;
using System.Collections.Generic;
using System.Text;

// ============================================================
// GAME CONTEXT - Interface for players to query game state
// ============================================================
interface IGameContext
{
    Board GetBoard(int index = 0);

    Board[] GetBoards();

    Piece[] GetPieces();

    Square[] AllAvailableSquares { get; }
    List<Square[]> AllFullLines { get; }

    string PlayerGameMode { get; }

    string GameType { get; }

    string GetPieceValueForSquare(Square sqaure);
    int GetPieceValueForSquareAsInt(Square sqaure);
    void DrawBoards();

    bool CalculateComMove(Computer com);

    string PlayerMoveInstructions();
}

