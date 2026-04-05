class PlayerFactory(IGameContext gameContext)
{
    private IGameContext GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));

    public Player CreateHumanPlayer(int pos) => new Human(pos, GameContext);
    public Player CreateComputerPlayer(int pos) => new Computer(pos, GameContext);

    public Player CreateFromState(string type, int pos)
    {
        return type == "human" ? new Human(pos, GameContext) : new Computer(pos, GameContext);
    }
}
