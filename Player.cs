
abstract class Player(int pos, IGameContext gameContext)
{
    public int Position = pos;
    public Piece[] Pieces => Array.FindAll(GameContext.GetPieces() ?? [], p => p.OwnerPosition == this.Position);
    public Piece[] PiecesAvailable => Array.FindAll(this.Pieces, p => p.Location == null);
    public IGameContext GameContext => gameContext;
    public abstract void DoMove();

    public string CaptureState() => this is Human ? "human" : "computer";
}

class Human : Player
{
    public Cursor Cursor;

    public Human(int pos, IGameContext gameContext) : base (pos, gameContext)
    {
        Cursor = new Cursor(0, gameContext, Position);
    }

    public override void DoMove()
    {
        Console.ResetColor();
        //Select A Piece
        Piece? piece = null;
        while (piece == null)
        {
            Console.WriteLine($"Player {this.Position}, enter the number of the piece you'd like to use and press enter to confirm:");
            if (int.TryParse(Console.ReadLine(), out int val)) piece = PiecesAvailable.FirstOrDefault(x => x.Value == val);
            if (piece == null) Console.WriteLine($"That's not a valid piece, try again Player {this.Position}");
        }
        
        // Initialise Cursor
        this.Cursor.Location = GameContext.GetBoard(0).SquaresAvailable[0];
        this.Cursor.Value = piece.Value;
        GameContext.DrawBoards();
        //Only accept valid inputs based on the keystrokes in this array
        ConsoleKey[] validKeys = [ConsoleKey.N, ConsoleKey.M, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];
        ConsoleKeyInfo key;
        bool selected = false;
        do
        {
            Console.WriteLine($"Player {this.Position}, use the arrow keys to navigate the remaining spaces and press enter to select one");
            Console.WriteLine($"If the space you want to use is inaccessible with the arrow keys, use the n and m keys to cycle through available spaces");

            key = Console.ReadKey(true);
            if (validKeys.Contains(key.Key)){

                if(key.Key == ConsoleKey.LeftArrow)  this.Cursor.MoveLocation("left");
                if(key.Key == ConsoleKey.RightArrow) this.Cursor.MoveLocation("right");
                if(key.Key == ConsoleKey.UpArrow)    this.Cursor.MoveLocation("up");
                if(key.Key == ConsoleKey.DownArrow)  this.Cursor.MoveLocation("down");
                if(key.Key == ConsoleKey.N) this.Cursor.MoveLocation("prev");
                if(key.Key == ConsoleKey.M) this.Cursor.MoveLocation("next");

                if(key.Key == ConsoleKey.Enter)      selected = true;

                GameContext.DrawBoards();
            }
            else
            {
                Console.WriteLine("Double check the controls and try navigating with either the arrow keys or n&m.");
            }
        } while (!selected);
        Square? sq = GameContext.GetBoard(0).Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col); // TODO cater for n boards
        //Place Piece
        piece.Location = sq;
        sq.IsOccupied = true;
        //remove cursor from the board
        this.Cursor.Location = null;
    }
}

class Computer(int pos, IGameContext Game) : Player(pos, Game)
{
    public override void DoMove() // TODO win logic needs to be owned by the game. We'll need to refactor this out of here and make Computer select from a list of Square provided by the game.
    {
        Square? sq = null;
        List<Square[]> AlmostFullLines = [];
        Piece? p = null;

        //Build a list of lines that have one space free/almost full
        foreach (Square[] line in GameContext.GetBoard(0).Lines!)
        {
            int countOfFullLines = line.Count(el => el.IsOccupied);
            bool isAlmostFull = countOfFullLines+1 == GameContext.GetBoard(0).Size;

            if(isAlmostFull) AlmostFullLines.Add(line);
        }
        //check all available spots for a winning move
        if (AlmostFullLines.Count > 0)
        {
            foreach (Square[] line in AlmostFullLines)
            {
                int lineSum = line.Aggregate(0, (acc, el) => el.IsOccupied ? this.GameContext.GetPieceValueForSquare(el) + acc : acc);
                int requires = ((TicTacToe)GameContext).targetNumber - lineSum; // TODO we can't leave this this way.

                Piece? requiredPiece = this.PiecesAvailable.FirstOrDefault(el => el.Value == requires);

                if(requiredPiece != null)
                {
                    Square emptySpace = line.First(el => !el.IsOccupied);

                    sq = emptySpace;
                    p = requiredPiece;
                    // don't check lines if winning move is found
                    break;
                }
            }
        }
        //if no winning move is found, get available squares, and randomly pick a piece to place in a random square 
        if(sq == null)
        {
            var availSqurares = GameContext.GetBoard(0).SquaresAvailable;
            var availPieces = this.PiecesAvailable;

            Random rng = new Random();
            int sqrnum = rng.Next(0, GameContext.GetBoard(0).SquaresAvailable.Length - 1);
            int piecenum = rng.Next(0, this.PiecesAvailable.Length - 1);

            sq = availSqurares[sqrnum];
            p = availPieces[piecenum];
        }
        //set a space after resolution of the above
        p.Location = sq;
        sq.IsOccupied = true;
    }
}