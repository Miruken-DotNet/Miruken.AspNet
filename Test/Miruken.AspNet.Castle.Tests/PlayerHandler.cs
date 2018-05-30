namespace Miruken.AspNet.Castle.Tests
{
    using System.Security.Principal;
    using System.Threading;
    using Callback;

    public class PlayerHandler : Handler
    {
        private static int _id;

        [Handles]
        public PlayerResponse Create(
            CreatePlayer create, IPrincipal principal)
        {
            var player = create.Player;
            player.Id  = Interlocked.Increment(ref _id);
            return new PlayerResponse { Player = player };
        }

        [Handles]
        public void Created(PlayerCreated created)
        {           
        }

        [Handles]
        public void Updated(PlayerUpdated updated)
        {
        }
    }
}
