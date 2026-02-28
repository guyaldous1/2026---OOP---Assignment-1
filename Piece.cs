class Piece(int val, Player Player)
{
    public int Value = val;
    public Player Owner = Player;
    public Square? Location {set; get;}
}

class Cursor(int val, Player Player) : Piece(val, Player)
{
    private static readonly string[] ValidDirections = ["left", "right", "up", "down", "next", "prev"];
    public void MoveLocation(string direction)
    {
        if (!ValidDirections.Contains(direction)) return;
        var Board = this.Owner.Game.Board;

        Square? moveTo = null;

        // 1. Get available squares in relevant row or column
        // 2. Filter by are greater or less than current position based on direction selected
        // 3. orders them by size based on direction selected
        // 4. set the first available as the new square
        if(direction == "left")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row == this.Location.Row && x.Col < this.Location.Col)
            .OrderByDescending(x => x.Col)
            .FirstOrDefault();
        else if(direction == "right")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row == this.Location.Row && x.Col > this.Location.Col)
            .OrderBy(x => x.Col)
            .FirstOrDefault();
        else if(direction == "up")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row < this.Location.Row && x.Col == this.Location.Col)
            .OrderByDescending(x => x.Row)
            .FirstOrDefault();
        else if(direction == "down")
            moveTo = Board.SquaresAvailable
            .Where(x => x.Row > this.Location.Row && x.Col == this.Location.Col)
            .OrderBy(x => x.Row)
            .FirstOrDefault();
        else if(direction == "next"){
            try{
                int cur = Array.IndexOf(Board.SquaresAvailable, this.Location);
                moveTo = Board.SquaresAvailable[cur+1];
            } catch(IndexOutOfRangeException)
            {
                Console.WriteLine("That would take you off the board, try again.");
            }
        }
        else if(direction == "prev"){
            try{
                int cur = Array.IndexOf(Board.SquaresAvailable, this.Location);
                moveTo = Board.SquaresAvailable[cur-1];
            } catch(IndexOutOfRangeException)
            {
                Console.WriteLine("That would take you off the board, try again.");
            }
        }

        if (moveTo != null) this.Location = moveTo;
    }
}