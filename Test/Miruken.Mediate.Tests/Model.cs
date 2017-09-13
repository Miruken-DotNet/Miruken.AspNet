namespace Miruken.Mediate.Tests
{
    using FluentValidation;
    using Validate;
    using Validate.FluentValidation;

    public class Team : ValidationAware
    {
        public int    Id       { get; set; }
        public string Name     { get; set; }
        public bool   Active   { get; set; }
        public string Division { get; set; }
    }

    public class TeamAction
    {
        public Team Team { get; set; }
    }

    public class CreateTeam : TeamAction, IRequest<Team>
    {
    }

    public class TeamCreated : TeamAction
    {
    }

    public class RemoveTeam : TeamAction
    {
    }

    public class TeamRemoved : TeamAction
    {
    }

    public class TeamIntegrity : AbstractValidator<Team>
    {
        public TeamIntegrity()
        {
            RuleFor(t => t.Name).NotEmpty();
        }
    }

    public class TeamActionIntegrity : AbstractValidator<TeamAction>
    {
        public TeamActionIntegrity()
        {
            RuleFor(ta => ta.Team).NotEmpty().Valid();
        }
    }

    public class RemoveTeamIntegrity : AbstractValidator<RemoveTeam>
    {
        public RemoveTeamIntegrity()
        {
            RuleFor(ta => ta.Team.Id).GreaterThan(0);
        }
    }
}
