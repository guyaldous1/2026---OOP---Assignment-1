using System.Text.RegularExpressions;

class Gomoku : Game
{
    public Gomoku()
    {
    }

    public Gomoku(GameStateMemento state): base(state)
    {
    }

    public override string GameType => "gomoku";

    private List<string> LineStrings => Boards[0].Lines.Select(line => string.Concat(line.Select(sq => GetPieceValueForSquare(sq.SquareID)))).ToList();
    
    public override void ResolveTurn()
    {
        string Xpattern = @"(?<!X)XXXXX(?!X)";
        string Opattern = @"(?<!O)OOOOO(?!O)";

        string pattern = (WhoseTurn.Position == 1) ? Xpattern : Opattern;

        List<string> WinningStrings = LineStrings.Where(ls => Regex.IsMatch(ls, pattern)).ToList();

        if (WinningStrings.Count > 0)
        {
            Finished = true;
            ConsoleHelper.WriteLine($"Player {WhoseTurn.Position} Wins!");
            return;
        }

        if (AllAvailableSquares.Length <= 0)
        {
            Finished = true;
            ConsoleHelper.WriteLine("No winner, it's a tie!");
        }
    }

    public override void ShowRuleForTurn()
    {
        ConsoleHelper.WriteLine($"First line of 5 in a row wins, lines greater than 5 don't count");
    }

    public override IEnumerable<Move> GetStrategicMoves()
    {
        Piece[] currentPlayerPieces = WhoseTurn.PiecesAvailable;
        string pieceValue = currentPlayerPieces.FirstOrDefault()?.Value;
        if (pieceValue == null) yield break;

        //Find patterns of 4 but not 5
        string pattern = pieceValue == "O" ? "-OOOO|OOOO-" : "-XXXX|XXXX-";

        foreach (Square[] line in Boards[0].Lines)
        {
            string lineString = string.Concat(line.Select(sq => GetPieceValueForSquare(sq.SquareID)));
            Match match = Regex.Match(lineString, pattern);

            if (match.Success)
            {
                // Find the index of the '-' within the matched part and select that as the winning square
                int dashIndex = match.Value.StartsWith('-')
                    ? match.Index
                    : match.Index + 4;

                Square winningSquare = line[dashIndex];

                yield return new Move { PieceID = currentPlayerPieces.First().PieceID, SquareID = winningSquare.SquareID };
            }
        }
    }

    protected override void InitializeGameBoards()
    {
        //always make 1 board of size 15
        int size = 15;
        int boardCount = 1;

        InitializeBoards(size, boardCount, "xo");
    }

    protected override void GameSpecificHelp()
    {
        ConsoleHelper.WriteLine("You are playing a game of Gomoku. The first player to create a row, column or diagonal of exactly 5 of their pieces wins! 6 and above doesn't end the game.");
    }
}