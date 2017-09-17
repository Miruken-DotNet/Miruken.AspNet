namespace Miruken.AspNet.Castle.Tests
{
    using Mediate;

    public class Player
    {
        public int    Id   { get; set; }
        public string Name { get; set; }
    }

    public class RegisterPlayer : IRequest<PlayerResponse>
    {
        public Player Player { get; set; }
    }

    public class PlayerResponse
    {
        public Player Player { get; set; }
    }
}
