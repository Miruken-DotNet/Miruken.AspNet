namespace Miruken.Mediator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Concurrency;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Miruken.Mediator;
    using Validate;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class HandlerMediatorTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            _handler = new TeamHandler()
                     + new MiddlewareProvider()
                     + new DataAnnotationsValidator()
                     + new FluentValidationValidator()
                     + new ValidationHandler();
        }

        [TestMethod]
        public async Task Should_Send_Request_With_Response()
        {
            var team = await _handler.Send(new CreateTeam
            {
                Team = new Team
                { 
                    Name = "Liverpool Owen"
                }
            });
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);
        }

        [TestMethod]
        public async Task Should_Send_Request_With_Response_Dynamic()
        {
            var team = await _handler.Send<Team>((object)new CreateTeam
            {
                Team = new Team
                {
                    Name = "Liverpool Owen"
                }
            });
            Assert.AreEqual(1, team.Id);
            Assert.IsTrue(team.Active);
        }

        [TestMethod]
        public async Task Should_Send_Request_Without_Response()
        {
            var team = new Team
            {
                Id     = 1,
                Name   = "Liverpool Owen",
                Active = true
            };

            await _handler.Send(new RemoveTeam { Team = team });
            Assert.IsFalse(team.Active);
        }

        [TestMethod]
        public async Task Should_Publish_Notifiations()
        {
            var teams = new TeamHandler();
            var team  = await teams.Send(new CreateTeam
            {
                Team = new Team
                {
                    Name = "Liverpool Owen"
                }
            });
            var notifications = teams.Notifications;
            Assert.AreEqual(1, notifications.Count);
            var teamCreated = notifications.First() as TeamCreated;
            Assert.IsNotNull(teamCreated);
            Assert.AreEqual(team.Id, teamCreated.Team.Id);
        }

        [TestMethod]
        public async Task Should_Reject_Invalid_Request()
        {
            try
            {
                await _handler.Send(new CreateTeam());
                Assert.Fail("Should have rejected request");
            }
            catch (Validate.ValidationException vex)
            {
                var outcome = vex.Outcome;
                Assert.IsNotNull(outcome);
                CollectionAssert.AreEqual(new [] {"Team"}, outcome.Culprits);
                Assert.AreEqual("'Team' should not be empty.", outcome["Team"]);
            }
        }

        public class TeamHandler : Mediator
        {
            public int _teamId;
            private readonly List<object> _notifications = new List<object>();

            public ICollection<object> Notifications => _notifications;

            [Mediates]
            public Promise<Team> Create(CreateTeam create, IHandler composer)
            {
                var team = create.Team;
                team.Id     = ++_teamId;
                team.Active = true;

                composer.Publish(new TeamCreated {Team = team});
                return Promise.Resolved(team);
            }

            [Mediates]
            public void Remove(RemoveTeam remove, IHandler composer)
            {
                var team = remove.Team;
                team.Active = false;
                composer.Publish(new TeamRemoved {Team = team});
            }

            [Mediates]
            public void Notify(object notification)
            {
                _notifications.Add(notification);
            }
        }

        private class MiddlewareProvider : Handler
        {
            [Provides]
            public IMiddleware<TReq, TResp>[] GetMiddleware<TReq, TResp>()
            {
                 return new IMiddleware<TReq, TResp>[]
                 {
                    new ValidationMiddleware<TReq, TResp>()
                 };
            }

            [Provides]
            public IValidator<TeamAction>[] TeamActionValidators()
            {
                return new[] {new TeamActionIntegrity()};
            }
        }
    }
}
