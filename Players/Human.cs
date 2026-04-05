class Human : Player
{
    public Cursor Cursor { get; }

    public Human(int pos, IGameContext gameContext) : base(pos, gameContext)
    {
        Cursor = new Cursor("0");
    }

    public override Move DoMove()
    {
        Console.ResetColor();
        //Select A Piece or force one for non neumerical games
        Piece piece = gameContext.PlayerSelectsPiece ? null : PiecesAvailable.FirstOrDefault();
        while (piece == null)
        {
            ConsoleHelper.WriteLine($"Player {Position}, enter the number of the piece you'd like to use and press enter to confirm:");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) { piece = PiecesAvailable.FirstOrDefault(x => x.Value == input); }
            if (piece == null) ConsoleHelper.WriteLine($"That's not a valid piece, try again Player {Position}");
        }

        // Initialise Cursor
        Square startingLocation = gameContext.GetBoards().First(b => b.SquaresAvailable.Length > 0).SquaresAvailable[0];
        Cursor.SetLocation(startingLocation.BoardID, startingLocation.Row, startingLocation.Col);
        Cursor.Value = piece.Value;
        gameContext.DrawBoards();
        //Only accept valid inputs based on the keystrokes in this array
        ConsoleKey[] validKeys = [ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.N, ConsoleKey.M, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];
        ConsoleKeyInfo key;
        bool selected = false;
        do
        {
            ConsoleHelper.Write(gameContext.PlayerMoveInstructions());

            key = Console.ReadKey(true);
            if (validKeys.Contains(key.Key))
            {

                Board[] boards = gameContext.GetBoards();
                if (key.Key == ConsoleKey.LeftArrow) Cursor.MoveLocation("left", boards);
                if (key.Key == ConsoleKey.RightArrow) Cursor.MoveLocation("right", boards);
                if (key.Key == ConsoleKey.UpArrow) Cursor.MoveLocation("up", boards);
                if (key.Key == ConsoleKey.DownArrow) Cursor.MoveLocation("down", boards);
                if (key.Key == ConsoleKey.N) Cursor.MoveLocation("prev", boards);
                if (key.Key == ConsoleKey.M) Cursor.MoveLocation("next", boards);

                //move boards with numbers keys if possible
                if (key.Key == ConsoleKey.D1) Cursor.MoveBoard(0, boards);
                if (key.Key == ConsoleKey.D2 && boards.Length > 1) Cursor.MoveBoard(1, boards);
                if (key.Key == ConsoleKey.D3 && boards.Length > 1) Cursor.MoveBoard(2, boards);

                if (key.Key == ConsoleKey.Enter)
                    selected = true;
                else
                    gameContext.DrawBoards(); // cursor draw
            }
            else
            {
                ConsoleHelper.WriteLine("Double check the controls and try navigating with either the arrow keys or n&m.");
            }
        } while (!selected);

        int currentBoard = Cursor.BoardID;
        Square sq = gameContext.GetBoard(currentBoard).Squares.FirstOrDefault(x => x.Row == Cursor.Row && x.Col == Cursor.Col);
        piece.Place(sq);

        //remove cursor from the board
        Cursor.SetLocation(-1, -1, -1);

        return new Move { PieceID = piece.PieceID, SquareID = sq.SquareID };
    }
}
