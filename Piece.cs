class Piece
{
    static private int lastPieceID = 0;
    protected IGameContext GameContext;

    public int PieceID = ++lastPieceID;
    public string Value { get; set; }
    public int LocationSquareID { get; private set; } = -1;
    public int OwnerPosition { get; }

    public Piece(string val, IGameContext gameContext, int ownerPosition)
    {
        Value = val;
        GameContext = gameContext;
        OwnerPosition = ownerPosition;
    }

    public Piece(IGameContext gameContext, string state)
    {
        this.GameContext = gameContext;
        string[] values;
        try
        {
            values = state.Split('|');
            PieceID = Convert.ToInt32(values[0]);
            Value = values[1];
            OwnerPosition = Convert.ToInt32(values[2]);
            LocationSquareID = Convert.ToInt32(values[3]);
        }
        catch
        {
            throw new DeserialisationException($"Invalid format deserialising {nameof(Piece)}");
        }
    }

    public string CaptureState()
    {
        return $"{PieceID}|{Value}|{OwnerPosition}|{LocationSquareID}";
    }

    public void Place(Square square)
    {
        LocationSquareID = square.SquareID;
        square.IsOccupied = true;
    }

    public void Unplace(Square square)
    {
        LocationSquareID = -1;
        square.IsOccupied = false;
    }
}

class Cursor(string val, IGameContext gameContext, int ownerPosition) : Piece(val, gameContext, ownerPosition)
{
    private static readonly string[] ValidDirections = ["left", "right", "up", "down", "next", "prev"];
    public Square Location { get; set; }
    public void MoveLocation(string direction)
    {
        if (!ValidDirections.Contains(direction)) return;
        var currentBoard = this.Location.BoardID;
        var Board = this.GameContext.GetBoard(currentBoard); 
        int cur = Array.IndexOf(Board.SquaresAvailable, this.Location);
        // 1. Get available squares in relevant row or column 2. Filter by are greater or less than current position based on direction selected 3. orders them by size based on direction selected 4. set the first available as the new square
        Square moveTo = direction switch {
            "left"  => Board.SquaresAvailable.Where(x => x.Row == this.Location.Row && x.Col < this.Location.Col).OrderByDescending(x => x.Col).FirstOrDefault(),
            "right" => Board.SquaresAvailable.Where(x => x.Row == this.Location.Row && x.Col > this.Location.Col).OrderBy(x => x.Col).FirstOrDefault(),
            "up"    => Board.SquaresAvailable.Where(x => x.Row < this.Location.Row && x.Col == this.Location.Col).OrderByDescending(x => x.Row).FirstOrDefault(),
            "down"  => Board.SquaresAvailable.Where(x => x.Row > this.Location.Row && x.Col == this.Location.Col).OrderBy(x => x.Row).FirstOrDefault(),
            "next"  => (cur < Board.SquaresAvailable.Length - 1) ? Board.SquaresAvailable[cur + 1] : null,
            "prev"  => (cur > 0) ? Board.SquaresAvailable[cur - 1] : null,
            _ => null
        };
        if (moveTo != null) this.Location = moveTo;
        //Catch out of index for next/prev
        else if (direction is "next" or "prev") Console.WriteLine("That would take you off the board, try again.");
    }

    public void MoveBoard(int boardID)
    {
        var currentBoard = this.Location.BoardID;
        if(currentBoard == boardID) return;

        var NewBoard = this.GameContext.GetBoard(boardID);

        var FirstSquare = NewBoard.SquaresAvailable[0];

        this.Location = FirstSquare;
    }
}