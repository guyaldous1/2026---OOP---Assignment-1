using System.Text.RegularExpressions;

class Gomoku : Game
{
    public override string GameType => "gomoku";

    public Gomoku()
    {
    }

    public Gomoku(GameStateMemento state): base(state)
    {
    }
    List<string> LineStrings => Boards[0].Lines.Select(line => string.Concat(line.Select(sq => GetPieceValueForSquare(sq.SquareID)))).ToList();
    public override void ResolveTurn()
    {
        DrawBoards();

        string Xpattern = @"(?<!X)XXXXX(?!X)";
        string Opattern = @"(?<!O)OOOOO(?!O)";

        string pattern = (WhoseTurn.Position == 1) ? Xpattern : Opattern;

        List<string> WinningStrings = LineStrings.Where(ls => Regex.IsMatch(ls, pattern)).ToList();

        if (WinningStrings.Count > 0)
        {
            Finished = true;
            Console.WriteLine($"Player {WhoseTurn.Position} Wins!");
            return;
        }

        if (AllAvailableSquares.Length <= 0)
        {
            Finished = true;
            Console.WriteLine("No winner, it's a tie!");
        }
    }

    public override void ShowRuleForTurn()
    {
        Console.WriteLine($"First line of 5 in a row wins, lines greater than 5 don't count");
    }

    protected override void InitializeGameBoards()
    {
        //always make 1 board of size 15
        int size = 15;
        int boardCount = 1;

        InitializeBoards(size, boardCount, "xo");
    }

    public override bool CalculateComMove(Computer com, out Move move)
    {
        //Find patterns of 4 but not 5   
        string pattern = @"-OOOO|OOOO-";

        foreach (Square[] line in Boards[0].Lines)
        {
            string lineString = string.Concat(line.Select(sq => GetPieceValueForSquare(sq.SquareID)));
            Match match = Regex.Match(lineString, pattern);

            if (match.Success)
            {
                // Find the index of the '-' within the matched part and select that as the winning square
                int dashIndex = match.Value.StartsWith("-") 
                    ? match.Index 
                    : match.Index + 4;

                Square winningSquare = line[dashIndex];
                Piece piece = com.PiecesAvailable.First();

                piece.Place(winningSquare.SquareID);
                move = new Move { PieceID = piece.PieceID, SquareID = winningSquare.SquareID };
                return true;
            }
        }

        move = null;
        return false;
    }
}
