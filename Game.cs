

class Game
{
    public Player Player1;
    public Player Player2;
    public Board? Board;
    public int TargetNumber => this.Board.Size*(this.Board.Size*this.Board.Size + 1) / 2;
    public Piece[] Pieces;
    public int TurnNumber = 1;
    public Player WhoseTurn => this.TurnNumber % 2 == 0 ? this.Player2 : this.Player1;
    public bool Finished = false;

    public Game()
    {
        //TODO make setup data entry code nicer
        //TODO error handling correctly
        //TODO fix gameplay instructions to be more descriptive

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
        int boardInput;
        while (true)
        {

            Console.WriteLine($"How big dat board?");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out boardInput))
            {
                Console.WriteLine("Failed - Not even a number. Try Again.");
                continue;
            }

            if (boardInput < 2 || boardInput > 10)
            {
                Console.WriteLine("That board is too big or too small");
                continue;
            }

            break; 
        }

        int BoardSize = boardInput;

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
        

        this.Turn();

    }

    public void Turn()
    {           
        Console.WriteLine($"Starting Turn {this.TurnNumber}. It's Player {this.WhoseTurn.Position}'s Turn");
        
        //Show Board
        this.Board.Draw();
        //Allow current player to move
        WhoseTurn.DoMove();
        //Check if move just won
        this.ResolveTurn();

        if(!this.Finished){
            this.TurnNumber++;
            this.Turn();
        }
    }

    public void ResolveTurn()
    {   
        this.Board.Draw();
        foreach (Square[] line in this.Board.Lines)
        {
            bool isFull = Array.TrueForAll(line, el => el.Value != null);
            if(!isFull) continue;

            int lineSum = line.Aggregate(0, (acc, el) => el.Value.Value + acc);
            // Console.WriteLine($"a line is full with sum {lineSum}");
            
            if(lineSum == this.TargetNumber) {
                this.Finished = true;
                Console.WriteLine($"Player {this.WhoseTurn.Position} Wins!");
                return; //prevents looping through further lines in the case of a win
                //also deals with a winning move on the final turn, preventing progression to tie check
            }
        }
        //check if no squares left after a victory can't be found
        if(this.Board.SquaresAvailable.Length <= 0)
        {
            this.Finished = true;
            Console.WriteLine($"No winner, its a tie!");
        }
    }

    

    //TODO new class for game menu inc controls & stuff
}