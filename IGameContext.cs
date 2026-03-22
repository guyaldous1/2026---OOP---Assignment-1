using System;
using System.Collections.Generic;
using System.Text;

// ============================================================
// GAME CONTEXT - Interface for players to query game state
// ============================================================
interface IGameContext
{
    Board GetBoard(int index = 0);

    Piece[] GetPieces();

    string GetPieceValueForSquare(Square sqaure);

    int GetPieceValueForSquareAsInt(Square sqaure);

    void DrawBoards();
}

