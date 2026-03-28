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
    string GameType { get; }
    string GetPieceValueForSquare(int squareID);
    int GetPieceValueForSquareAsInt(int squareID);
    void DrawBoards();
    IEnumerable<Move> GetStrategicMoves();
    string PlayerMoveInstructions();
    bool PlayerSelectsPiece { get; }
}