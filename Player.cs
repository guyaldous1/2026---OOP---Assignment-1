
using System.IO.Pipelines;

abstract class Player(int pos, IGameContext gameContext)
{
    public int Position = pos;
    public Piece[] Pieces => Array.FindAll(GameContext.GetPieces() ?? [], p => p.OwnerPosition == this.Position);
    public Piece[] PiecesAvailable => Array.FindAll(this.Pieces, p => p.LocationSquareID == -1);
    public IGameContext GameContext => gameContext;
    public abstract Move DoMove();
    public string CaptureState() => this is Human ? "human" : "computer";
    public ConsoleColor Colour => this.Position % 2 == 0 ? ConsoleColor.Red : ConsoleColor.Green;

    protected void PlacePiece(Piece piece, Square square)
    {
        piece.Place(square.SquareID);
        square.IsOccupied = true;
    }
}

class Human : Player
{
    public Cursor Cursor;

    public Human(int pos, IGameContext gameContext) : base (pos, gameContext)
    {
        Cursor = new Cursor("0", gameContext, Position);
    }

    public override Move DoMove()
    {
        Console.ResetColor();
        //Select A Piece or force one for non neumerical games
        Piece piece = GameContext.GameType != "tictactoe" ? PiecesAvailable.FirstOrDefault() : null;
        while (piece == null)
        {
            Console.WriteLine($"Player {this.Position}, enter the number of the piece you'd like to use and press enter to confirm:");
            string input = Console.ReadLine();
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
            Console.Write(GameContext.PlayerMoveInstructions());

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
                if(key.Key == ConsoleKey.D2 && GameContext.GetBoards().Length > 1) this.Cursor.MoveBoard(1);
                if(key.Key == ConsoleKey.D3 && GameContext.GetBoards().Length > 1) this.Cursor.MoveBoard(2);

                if(key.Key == ConsoleKey.Enter)      selected = true;

                GameContext.DrawBoards();
            }
            else
            {
                Console.WriteLine("Double check the controls and try navigating with either the arrow keys or n&m.");
            }
        } while (!selected);

        var currentBoard = this.Cursor.Location.BoardID;    
        Square sq = GameContext.GetBoard(currentBoard).Squares.FirstOrDefault(x => x.Row == this.Cursor.Location.Row && x.Col == this.Cursor.Location.Col);
        PlacePiece(piece, sq);

        //remove cursor from the board
        this.Cursor.Location = null;

        return new Move { PieceID = piece.PieceID, SquareID = sq.SquareID };
    }
}

class Computer(int pos, IGameContext gameContext) : Player(pos, gameContext)
{
    public override Move DoMove()
    {
        var availSquares = GameContext.AllAvailableSquares;

        Move[] strategicMoves = GameContext.GetStrategicMoves()
            .Where(move => PiecesAvailable.Any(piece => piece.PieceID == move.PieceID))
            .ToArray();
        if (strategicMoves.Length > 0)
        {
            Random strategicMoveRng = new Random();
            int moveNum = strategicMoveRng.Next(0, strategicMoves.Length);

            Move strategicMove = strategicMoves[moveNum];
            Piece strategicPiece = PiecesAvailable.FirstOrDefault(piece => piece.PieceID == strategicMove.PieceID);
            Square strategicSquare = availSquares.FirstOrDefault(square => square.SquareID == strategicMove.SquareID);
            if (strategicPiece is null || strategicSquare is null)
            {
                throw new GamePlayException("Error in strategic move creation: pieces and squares state error.");
            }

            PlacePiece(strategicPiece, strategicSquare);

            return strategicMoves[moveNum];
        }

        // no strategic move is found, randomly pick a piece to place in a random square 
        Random randomMoveRng = new Random();
        int sqrnum = randomMoveRng.Next(0, availSquares.Length);
        int piecenum = randomMoveRng.Next(0, PiecesAvailable.Length);

        Square sq = availSquares[sqrnum];
        Piece p = PiecesAvailable[piecenum];
        PlacePiece(p, sq);

        return new Move { PieceID = p.PieceID, SquareID = sq.SquareID };
    }
}