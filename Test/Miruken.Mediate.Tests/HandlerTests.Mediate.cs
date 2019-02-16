namespace Miruken.Mediate.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Callback;
    using Callback.Policy;
    using Concurrency;
    using FluentValidation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mediate;
    using Validate;
    using Validate.DataAnnotations;
    using Validate.FluentValidation;

    [TestClass]
    public class HandlerMediateTests
    {
        private IHandler _handler;

        [TestInitialize]
        public void TestInitialize()
        {
            HandlerDescriptor.ResetDescriptors();
            HandlerDescriptor.GetDescriptor<TeamHandler>();

            _handler = new TeamHandler()
                     + new FilterProvider()
                     + new DataAnnotationsValidator()
                     + new FluentValidationValidator();
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
        public async Task Should_Publish_Notifications()
        {
            var teams = new TeamHandler();
            var handler = teams + new FilterProvider();
            var team  = await handler.Send(new CreateTeam
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

        [Filter(typeof(MetricsFilter<,>)),
         Filter(typeof(ValidateFilter<,>))]
        public class TeamHandler : Handler
        {
            public int _teamId;
            private readonly List<object> _notifications = new List<object>();

            public ICollection<object> Notifications => _notifications;

            [Handles]
            public Promise<Team> Create(CreateTeam create, IHandler composer)
            {
                var team = create.Team;
                team.Id     = ++_teamId;
                team.Active = true;

                composer.Publish(new TeamCreated {Team = team});
                return Promise.Resolved(team);
            }

            [Handles]
            public void Remove(RemoveTeam remove, Command command, IHandler composer)
            {
                var team = remove.Team;
                team.Active = false;
                composer.Publish(new TeamRemoved {Team = team});
            }

            [Handles]
            public void Notify(TeamCreated teamCreated)
            {
                _notifications.Add(teamCreated);
            }

            [Handles]
            public void Notify(TeamRemoved teamRemoved)
            {
                _notifications.Add(teamRemoved);
            }
        }

        public class MetricsFilter<TReq, TResp> 
            : DynamicFilter<TReq, TResp>, IFilter<TReq, TResp>
        {
            public Task<TResp> Next(TReq request, Next<TResp> next,
                                    [Proxy]IStash stash)
            {
                stash.Put("Hello");
                return next();
            }
        }

        private class FilterProvider : Handler
        {
            [Provides]
            public ValidateFilter<TReq, TResp>[] GetValidateFilter<TReq, TResp>()
            {
                 return new []
                 {
                    new ValidateFilter<TReq, TResp>()
                 };
            }

            [Provides]
            public MetricsFilter<TReq, TResp>[] GetMetricsFilter<TReq, TResp>()
            {
                return new []
                {
                    new MetricsFilter<TReq, TResp>()
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
