class MoveHistory
{
    private List<Move> moves = [];
    private int currentMove = -1;

    public MoveHistory() { }

    public MoveHistory(string state)
    {
        // Format is length,move0.pieceId,move0.squareId,move1.pieceId,move1.squareId...
        int[] values = [];
        try
        {
            values = state.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(MoveHistory)}");
        }

        if (values.Length < 1 || values[0] <= 0)
        {
            throw new DeserialisationException($"Invalid length marker in {nameof(MoveHistory)}");
        }
        if (values.Length != values[0] * 2 + 1)
        {
            throw new DeserialisationException($"Invalid move length for {nameof(MoveHistory)}");
        }

        currentMove = values[0];
        for (int i=1; i < values.Length; i += 2)
        {
            moves.Add(
                new Move {
                    PieceID = values[i],
                    SquareID = values[i + 1]
                }
            );
        }
    }

    public void StoreNewMove(Move move)
    {
        // If the current move is not the latest move in the list, it now needs to be. The old "future" is discarded by playing a real move.
        moves.RemoveRange(currentMove + 1, moves.Count - currentMove - 1);
        moves.Add(move);
        currentMove = moves.Count - 1;
    }

    public Move GetPreviousMove()
    {
        if (currentMove < 0)
        {
            throw new MoveHistoryException("No more backward moves!");
        }

        return moves[currentMove--]; // We want to return the currently pointed-to move, so we decrement after.
    }

    public Move GetNextMove()
    {
        if (currentMove >= moves.Count - 1)
        {
            throw new MoveHistoryException("No more forward moves!");
        }

        return moves[++currentMove];
    }

    public string CaptureState()
    {
        string result = $"{currentMove}";
        foreach (Move move in moves)
            result += $",{move.PieceID},{move.SquareID}";
        return result;
    }
}
