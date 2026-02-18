

class Game
{
    public Player Player1 = new Human(1);
    public Player? Player2;
    public Board? Board;
    public Game()
    {
        //TODO game setup logic here

        string mode;
        do
        {
            Console.WriteLine("How would you like to play? 1:Human v Human or 2:Human v Computer");
            mode = Console.ReadLine();

            if (mode != "1" && mode != "2")
                Console.WriteLine("You Cunted it, try again.");

        } while (mode != "1" && mode != "2");

        if (mode == "1")
            this.Player2 = new Human(2);
        else
            this.Player2 = new Computer(2);

        this.Player1.Write();
        this.Player2.Write();

        Console.WriteLine($"How big dat board?");
        int BoardSize = Convert.ToInt32(Console.ReadLine());

        this.Board = new Board(BoardSize);

        this.Turn();

    }

    public void Turn()
    {
        //TODO process single turn logic here
        this.Board.Show();
    }
}