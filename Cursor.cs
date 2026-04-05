class Cursor
{
    private static readonly string[] ValidDirections = ["left", "right", "up", "down", "next", "prev"];

    public Cursor(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public int Row { get; private set; } = -1;

    public int Col { get; private set; } = -1;

    public int BoardID { get; private set; } = -1;

    public void SetLocation(int boardID, int row, int col)
    {
        BoardID = boardID;
        Row = row;
        Col = col;
    }

    public void MoveLocation(string direction, Board[] boards)
    {
        if (!ValidDirections.Contains(direction)) return;
        Board board = boards[BoardID];

        int cur = Array.FindIndex(board.SquaresAvailable, square => square.Row == Row && square.Col == Col);
        // 1. Get available squares in relevant row or column 2. Filter by are greater or less than current position based on direction selected 3. orders them by size based on direction selected 4. set the first available as the new square
        Square moveTo = direction switch {
            "left"  => board.SquaresAvailable.Where(x => x.Row == Row && x.Col < Col).OrderByDescending(x => x.Col).FirstOrDefault(),
            "right" => board.SquaresAvailable.Where(x => x.Row == Row && x.Col > Col).OrderBy(x => x.Col).FirstOrDefault(),
            "up"    => board.SquaresAvailable.Where(x => x.Row < Row && x.Col == Col).OrderByDescending(x => x.Row).FirstOrDefault(),
            "down"  => board.SquaresAvailable.Where(x => x.Row > Row && x.Col == Col).OrderBy(x => x.Row).FirstOrDefault(),
            "next"  => (cur < board.SquaresAvailable.Length - 1) ? board.SquaresAvailable[cur + 1] : null,
            "prev"  => (cur > 0) ? board.SquaresAvailable[cur - 1] : null,
            _ => null
        };
        if (moveTo != null)
        {
            BoardID = moveTo.BoardID;
            Row = moveTo.Row;
            Col = moveTo.Col;
        }
        //Catch out of index for next/prev
        else if (direction is "next" or "prev") ConsoleHelper.WriteLine("That would take you off the board, try again.");
    }

    public void MoveBoard(int boardID, Board[] boards)
    {
        if (BoardID == boardID) return;
        if (boards[boardID].SquaresAvailable.Length == 0) return;

        Square firstSquare = boards[boardID].SquaresAvailable[0];
        BoardID = firstSquare.BoardID;
        Row = firstSquare.Row;
        Col = firstSquare.Col;
    }
}