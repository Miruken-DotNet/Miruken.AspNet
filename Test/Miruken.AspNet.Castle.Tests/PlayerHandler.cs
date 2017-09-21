namespace Miruken.AspNet.Castle.Tests
{
    using System.Threading;
    using Mediate;

    /// <summary>
    /// Handles the Player Api.
    /// </summary>
    public class PlayerHandler : PipelineHandler
    {
        private static int _id;

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="create">Create request</param>
        /// <returns>Response containing new player.</returns>
        [Mediates]
        public PlayerResponse Create(CreatePlayer create)
        {
            var player = create.Player;
            player.Id  = Interlocked.Increment(ref _id);
            return new PlayerResponse { Player = player };
        }
    }
}
