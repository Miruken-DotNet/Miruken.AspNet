namespace Miruken.AspNet.Castle.Tests
{
    using FluentValidation;
    using Mediate.Api;

    public class Player
    {
        public int    Id   { get; set; }
        public string Name { get; set; }
    }

    public class CreatePlayer : IRequest<PlayerResponse>
    {
        public Player Player { get; set; }
    }

    public class PlayerCreated
    {
        public Player Player { get; set; }
    }

    public class UpdatePlayer : IRequest<PlayerResponse>
    {
        public Player Player { get; set; }
    }

    public class PlayerUpdated
    {
        public Player Player { get; set; }
    }

    public class PlayerResponse
    {
        public Player Player { get; set; }
    }

    public class CreatePlayerIntegrity : AbstractValidator<CreatePlayer>
    {
        public CreatePlayerIntegrity()
        {
            RuleFor(p => p.Player).NotNull();
            When(p => p.Player != null, () =>
            {
                RuleFor(p => p.Player.Id).Equal(0);
                RuleFor(p => p.Player.Name).NotEmpty();
            });
        }
    }
}
