using Microsoft.VisualBasic;

class Piece(string val, IGameContext gameContext, int ownerPosition)
{
    protected IGameContext GameContext = gameContext;
    public string Value = val;
    public Square? Location { get; set; }
    public int OwnerPosition = ownerPosition;
}

class Cursor(string val, IGameContext gameContext, int ownerPosition) : Piece(val, gameContext, ownerPosition)
{
    private static readonly string[] ValidDirections = ["left", "right", "up", "down", "next", "prev"];
    public void MoveLocation(string direction)
    {
        if (!ValidDirections.Contains(direction)) return;
        var currentBoard = this.Location.BoardID;
        var Board = this.GameContext.GetBoard(currentBoard); // TODO this will need to become board count agnostic
        int cur = Array.IndexOf(Board.SquaresAvailable, this.Location);
        // 1. Get available squares in relevant row or column 2. Filter by are greater or less than current position based on direction selected 3. orders them by size based on direction selected 4. set the first available as the new square
        Square? moveTo = direction switch {
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