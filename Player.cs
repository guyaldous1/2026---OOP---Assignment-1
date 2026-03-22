
using System.Diagnostics;

abstract class Player(int pos, IGameContext gameContext)
{
    public int Position = pos;
    public Piece[] Pieces => Array.FindAll(GameContext.GetPieces() ?? [], p => p.OwnerPosition == this.Position);
    public Piece[] PiecesAvailable => Array.FindAll(this.Pieces, p => p.Location == null);
    public IGameContext GameContext => gameContext;
    public abstract void DoMove(Game game);

    public string CaptureState() => this is Human ? "human" : "computer";
}

class Human : Player
{
    public Cursor Cursor;

    public Human(int pos, IGameContext gameContext) : base (pos, gameContext)
    {
        Cursor = new Cursor("0", gameContext, Position);
    }

    public override void DoMove(Game game)
    {
        Console.ResetColor();
        //Select A Piece or force one for non neumerical games
        Piece? piece = game.gameMode != "tictactoe" ? PiecesAvailable.FirstOrDefault() : null;
        while (piece == null)
        {
            Console.WriteLine($"Player {this.Position}, enter the number of the piece you'd like to use and press enter to confirm:");
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) { piece = PiecesAvailable.FirstOrDefault(x => x.Value == input); }
            if (piece == null) Console.WriteLine($"That's not a valid piece, try again Player {this.Position}");
        }
        
        // Initialise Cursor
        this.Cursor.Location = GameContext.GetBoard(0).SquaresAvailable[0];
        this.Cursor.Value = piece.Value;
        GameContext.DrawBoards();
        //Only accept valid inputs based on the keystrokes in this array
        ConsoleKey[] validKeys = [ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.N, ConsoleKey.M, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter];
        ConsoleKeyInfo key;
        bool selected = false;
        do
        {
            Console.WriteLine($"Player {this.Position}, use the arrow keys to navigate the remaining spaces and press enter to select one");
            Console.WriteLine($"If the space you want to use is inaccessible with the arrow keys, use the n and m keys to cycle through available spaces");
            Console.WriteLine($"You can use the number keys 1,2 or 3 to navigate to alternate boards if the game type requires"); //TODO make this conditional on gametype

            key = Console.ReadKey(true);
            if (validKeys.Contains(key.Key)){

                if(key.Key == ConsoleKey.LeftArrow)  this.Cursor.MoveLocation("left");
                if(key.Key == ConsoleKey.RightArrow) this.Cursor.MoveLocation("right");
                if(key.Key == ConsoleKey.UpArrow)    this.Cursor.MoveLocation("up");
                if(key.Key == ConsoleKey.DownArrow)  this.Cursor.MoveLocation("down");
                if(key.Key == ConsoleKey.N) this.Cursor.MoveLocation("prev");
                if(key.Key == ConsoleKey.M) this.Cursor.MoveLocation("next");

                //move boards with numbers keys if possible
                if(key.Key == ConsoleKey.D1) this.Cursor.MoveBoard(0);
                if(key.Key == ConsoleKey.D2) this.Cursor.MoveBoard(1);
                if(key.Key == ConsoleKey.D3) this.Cursor.MoveBoard(2);

                if(key.Key == ConsoleKey.Enter)      selected = true;

                GameContext.DrawBoards();
            }
            else
            {
                Console.WriteLine("Double check the controls and try navigating with either the arrow keys or n&m.");
            }
        } while (!selected);

        var currentBoard = this.Cursor.Location.BoardID;    
        Square? sq = GameContext.GetBoard(currentBoard).Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col);
        //Place Piece
        piece.Location = sq;
        sq.IsOccupied = true;
        //remove cursor from the board
        this.Cursor.Location = null;
    }
}

class Computer(int pos, IGameContext Game) : Player(pos, Game)
{
    public override void DoMove(Game game) // TODO win logic needs to be owned by the game. We'll need to refactor this out of here and make Computer select from a list of Square provided by the game.
    {
        Square? sq = null;
        List<Square[]> AlmostFullLines = [];
        Piece? p = null;

        //Build a list of lines that have one space free/almost full
        foreach(Board board in GameContext.GetBoards())
        {
            foreach (Square[] line in board.Lines!)
            {
                int countOfFullLines = line.Count(el => el.IsOccupied);
                bool isAlmostFull = countOfFullLines+1 == board.Size;

                if(isAlmostFull) AlmostFullLines.Add(line);
            }
        }
        
        //check all available spots for a winning move
        //TODO need to make the winning move selection work for all boards
        // if (AlmostFullLines.Count > 0) commented this out for now
        if(false)
        {
            foreach (Square[] line in AlmostFullLines)
            {
                int lineSum = line.Aggregate(0, (acc, el) => el.IsOccupied ? this.GameContext.GetPieceValueForSquareAsInt(el) + acc : acc);
                int requires = ((TicTacToe)GameContext).targetNumber - lineSum; // TODO we can't leave this this way.

                Piece? requiredPiece = this.PiecesAvailable.FirstOrDefault(el => int.Parse(el.Value) == requires);

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
            var availSqurares = GameContext.AllAvailableSquares;
            var availPieces = this.PiecesAvailable;

            Random rng = new Random();
            int sqrnum = rng.Next(0, availSqurares.Length - 1);
            int piecenum = rng.Next(0, this.PiecesAvailable.Length - 1);

            sq = availSqurares[sqrnum];
            p = availPieces[piecenum];
        }
        //set a space after resolution of the above
        p.Location = sq;
        sq.IsOccupied = true;
    }
}