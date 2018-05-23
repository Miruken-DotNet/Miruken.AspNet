namespace Miruken.AspNet.Castle.Tests
{
    using System.Security.Principal;
    using System.Threading;
    using Mediate;

    public class PlayerHandler : PipelineHandler
    {
        private static int _id;

        [Mediates]
        public PlayerResponse Create(
            CreatePlayer create, IPrincipal principal)
        {
            var player = create.Player;
            player.Id  = Interlocked.Increment(ref _id);
            return new PlayerResponse { Player = player };
        }

        [Mediates]
        public void Created(PlayerCreated created)
        {           
        }

        [Mediates]
        public void Updated(PlayerUpdated updated)
        {
        }
    }
}
