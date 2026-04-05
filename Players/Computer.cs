class Computer(int pos, IGameContext gameContext) : Player(pos, gameContext)
{
    public override Move DoMove()
    {
        Square[] availSquares = gameContext.AllAvailableSquares;

        Move[] strategicMoves = gameContext.GetStrategicMoves()
            .Where(move => PiecesAvailable.Any(piece => piece.PieceID == move.PieceID))
            .ToArray();
        if (strategicMoves.Length > 0)
        {
            Random strategicMoveRng = new Random();
            int moveNum = strategicMoveRng.Next(0, strategicMoves.Length);

            Move strategicMove = strategicMoves[moveNum];
            Piece strategicPiece = PiecesAvailable.FirstOrDefault(piece => piece.PieceID == strategicMove.PieceID);
            Square strategicSquare = availSquares.FirstOrDefault(square => square.SquareID == strategicMove.SquareID);
            if (strategicPiece is null || strategicSquare is null)
            {
                throw new GamePlayException("Error in strategic move creation: pieces and squares state error.");
            }

            strategicPiece.Place(strategicSquare);

            return strategicMoves[moveNum];
        }

        // no strategic move is found, randomly pick a piece to place in a random square 
        Random randomMoveRng = new Random();
        int sqrnum = randomMoveRng.Next(0, availSquares.Length);
        int piecenum = randomMoveRng.Next(0, PiecesAvailable.Length);

        Square sq = availSquares[sqrnum];
        Piece p = PiecesAvailable[piecenum];
        p.Place(sq);

        return new Move { PieceID = p.PieceID, SquareID = sq.SquareID };
    }
}