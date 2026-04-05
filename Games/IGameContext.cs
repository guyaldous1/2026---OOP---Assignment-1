// ============================================================
// GAME CONTEXT - Interface for players to query game state
// ============================================================
interface IGameContext
{
    Square[] AllAvailableSquares { get; }
    List<Square[]> AllFullLines { get; }
    string GameType { get; }
    bool PlayerSelectsPiece { get; }
    Board GetBoard(int index = 0);
    Board[] GetBoards();
    void DrawBoards();
    Piece[] GetPieces();
    string GetPieceValueForSquare(int squareID);
    int GetPieceValueForSquareAsInt(int squareID);
    IEnumerable<Move> GetStrategicMoves();
    string PlayerMoveInstructions();
}