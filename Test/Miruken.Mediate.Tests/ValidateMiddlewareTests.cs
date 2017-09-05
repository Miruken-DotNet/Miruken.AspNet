﻿namespace Miruken.Mediate.Tests
{
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Validate;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class ValidateMiddlewareTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.ResetDescriptors();
            HandlerDescriptor.GetDescriptor<TeamHandler>();

            _handler = new TeamHandler()
                     + new MiddlewareProvider()
                     + new DataAnnotationsValidator()
                     + new FluentValidationValidator()
                     + new ValidationHandler();
        }

        [TestMethod]
        public async Task Should_Validate_Request()
        {
            var team = await _handler.Send(new CreateTeam
            {
                Team = new Team
                {
                    Name = "Liverpool Owen"
                }
            });
            Assert.IsTrue(team.ValidationOutcome.IsValid);
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);
        }

        [TestMethod]
        public async Task Should_Match_All_Validators()
        {
            try
            {
                await _handler.Send(new RemoveTeam
                {
                    Team = new Team()
                });
                Assert.Fail("Should not succeed");
            }
            catch (Validate.ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                var team = outcome.GetOutcome("Team");
                Assert.IsNotNull(team);
                CollectionAssert.AreEqual(new[] { "Id" }, team.Culprits);
                Assert.AreEqual("'Team. Id' must be greater than '0'.", team["Id"]);
            }
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request()
        {
            try
            {
                await _handler.Send(new CreateTeam());
            }
            catch (Validate.ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new[] { "Team" }, outcome.Culprits);
                Assert.AreEqual("'Team' should not be empty.", outcome["Team"]);
            }
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request_Content()
        {
            try
            {
                await _handler.Send(new CreateTeam
                {
                    Team = new Team()
                });
                Assert.Fail("Should not succeed");
            }
            catch (Validate.ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                var team = outcome.GetOutcome("Team");
                Assert.IsNotNull(team);
                CollectionAssert.AreEqual(new[] { "Name" }, team.Culprits);
                Assert.AreEqual("'Name' should not be empty.", team["Name"]);
            }
        }

        public class TeamHandler : PipelineHandler
        {
            public int _teamId;

            [Mediates]
            public Promise<Team> Create(CreateTeam create, IHandler composer)
            {
                var team = create.Team;
                team.Id = ++_teamId;
                team.Active = true;

                composer.Publish(new TeamCreated { Team = team });
                return Promise.Resolved(team);
            }

            [Mediates]
            public void Remove(RemoveTeam remove, IHandler composer)
            {
                remove.Team.Active = false;

            }
        }

        private class MiddlewareProvider : Handler
        {
            [Provides]
            public IMiddleware<TReq, TResp>[] GetMiddleware<TReq, TResp>()
            {
                return new IMiddleware<TReq, TResp>[]
                {
                    new ValidateMiddleware<TReq, TResp>()
                };
            }

            [Provides]
            public IValidator<Team>[] TeamValidators()
            {
                return new[] { new TeamIntegrity() };
            }

            [Provides]
            public IValidator<TeamAction>[] TeamActionValidators()
            {
                return new [] { new TeamActionIntegrity() };
            }

            [Provides]
            public IValidator<RemoveTeam>[] RemoveTeamValidators()
            {
                return new IValidator<RemoveTeam>[] { new RemoveTeamIntegrity() };
            }
        }
    }
}