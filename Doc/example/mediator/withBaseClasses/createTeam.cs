namespace Example.Mediator.WithBaseClasses
{
    using Miruken.Mediator;

    public class CreateTeam : IRequest<TeamResult>
    {
        public Team Team { get; set; }
    }
}