// ============================================================
// GAME CONTEXT - Interface for players to query game state
// ============================================================
interface IGameContext
{
    string GameType { get; }
    bool PlayerSelectsPiece { get; }
    Square[] AllAvailableSquares { get; }
    List<Square[]> AllFullLines { get; }
    Board[] Boards { get; }
    Piece[] Pieces { get; }
    void DrawBoards();
    string GetPieceValueForSquare(int squareID);
    int GetPieceValueForSquareAsInt(int squareID);
    IEnumerable<Move> GetStrategicMoves();
    string PlayerMoveInstructions();
}