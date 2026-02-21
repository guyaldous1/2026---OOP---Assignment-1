

class Game
{
    public Player Player1;
    public Player Player2;
    public Board? Board;
    public Piece[] Pieces;
    public int TurnNumber = 1;
    public Player WhoseTurn => this.TurnNumber % 2 == 0 ? this.Player2 : this.Player1;
    public bool Finished = false;

    public Game()
    {
        //TODO game setup logic here
        //TODO make data entry code nicer
        //TODO remove setup lines when moving to first turn

        Console.Clear();
        // Step 1 setup mode
        string mode;
        do
        {
            Console.WriteLine("How would you like to play? 1:Human v Human or 2:Human v Computer");
            mode = Console.ReadLine();

            if (mode != "1" && mode != "2")
                Console.WriteLine("You Cunted it, try again.");

        } while (mode != "1" && mode != "2");

        //Player 1 always human
       this.Player1 = new Human(1, this);

        // Console.Clear();
        // Player 2 selected by mode
        if (mode == "1")
        {
            this.Player2 = new Human(2, this);
            Console.WriteLine("-- Human v Human");
        }
        else
        {
            this.Player2 = new Computer(2, this);
            Console.WriteLine("-- Human v Computer");
        }
            
        //Step 2 Setup Board size and pieces
        
        Console.WriteLine($"How big dat board?");
        int BoardSize = Convert.ToInt32(Console.ReadLine());

        this.Board = new Board(BoardSize, this);

        int pieceCount = BoardSize * BoardSize;
        this.Pieces = new Piece[pieceCount];

        //assign pieces to players
        for(int i = 0; i < pieceCount; i++)
        {
            int val = i+1;
            Player Owner = (i%2 == 0) ? this.Player1 : this.Player2;
            this.Pieces[i] = new Piece(val, Owner);
        }
        
        this.Player1.Write();
        this.Player2.Write();

        this.Turn();

    }

    public void Turn()
    {           
        // Console.Clear();
        Console.WriteLine($"Starting Turn {this.TurnNumber}. It's Player {this.WhoseTurn.Position}'s Turn");
        
        //TODO process single turn logic here - increment turn counter
        this.Board.Draw();

        //TODO player position selects whose move should be allowed
        WhoseTurn.DoMove();

        this.ResolveTurn();

        if(!this.Finished){
            this.TurnNumber++;
            Console.WriteLine($"Next Turn is {this.TurnNumber}");
            this.Turn();
        }

    }

    public void ResolveTurn()
    {
        //TODO method for win calculation logic
    }

    

    //TODO new class for game menu inc controls & stuff
}