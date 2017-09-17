namespace Miruken.AspNet.Castle.Tests
{
    using System.Threading;
    using Callback;
    using Mediate;

    public class PlayerHandler : Handler
    {
        private static int _id;

        [Mediates]
        public PlayerResponse Register(RegisterPlayer register)
        {
            var player = register.Player;
            player.Id  = Interlocked.Increment(ref _id);
            return new PlayerResponse { Player = player };
        }
    }
}
