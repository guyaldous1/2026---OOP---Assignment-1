

class Game
{
    public Player Player1;
    public Player Player2;
    public Board Board;
    public int TargetNumber => this.Board!.Size*(this.Board.Size*this.Board.Size + 1) / 2;
    public Piece[] Pieces;
    public int TurnNumber = 1;
    public Player WhoseTurn => this.TurnNumber % 2 == 0 ? this.Player2 : this.Player1;
    public bool Finished = false;

    public Game()
    {
        Console.Clear();

        // Step 1 setup game mode
        int mode = 0;
        bool success = false;

        while (!success)
        {
            try
            {
                Console.WriteLine("-- How would you like to play:");
                Console.WriteLine("Press 1 for Human v Human");
                Console.WriteLine("Press 2 for Human v Computer");

                mode = int.Parse(Console.ReadLine()!);

                if (mode == 1 || mode == 2)
                {
                    success = true;
                }
                else
                {
                    Console.WriteLine("Choice must be 1 or 2.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("That's not even a number.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("That number is too big.");
            }
        }

        Console.Clear();
        //Player 1 always human
       this.Player1 = new Human(1, this);

        // Player 2 selected by mode
        if (mode == 1)
        {
            this.Player2 = new Human(2, this);
            Console.WriteLine("-- Human v Human --");
        }
        else
        {
            this.Player2 = new Computer(2, this);
            Console.WriteLine("-- Human v Computer --");
        }
            
        //Step 2 Setup Board size and pieces
        int boardInput;
        while (true)
        {

            Console.WriteLine("-- Enter a number for the board size:");
            string input = Console.ReadLine()!;
            //if input can be parsed to an integer, proceed
            if (!int.TryParse(input, out boardInput))
            {
                Console.WriteLine("Failed - Not even a number. Try Again.");
                continue;
            }

            if (boardInput < 2 || boardInput > 10)
            {
                Console.WriteLine("That board is too small");
                continue;
            }

            break; 
        }

        //setup board size
        int BoardSize = boardInput;
        this.Board = new Board(BoardSize, this);

        //Create pieces and assign players
        int pieceCount = BoardSize * BoardSize;
        this.Pieces = new Piece[pieceCount];

        for(int i = 0; i < pieceCount; i++)
        {
            int val = i+1;
            Player Owner = (i%2 == 0) ? this.Player1 : this.Player2;
            this.Pieces[i] = new Piece(val, Owner);
        }
        
        //Initiate recursive turn logic
        this.Turn();

    }

    public void Turn()
    {           
        //Show Board
        this.Board.Draw();
        //Allow current player to move
        WhoseTurn.DoMove();
        //Check if move just won
        this.ResolveTurn();

        //If the game isn't over, increment and loop
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
}