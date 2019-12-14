using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.SignalR.Commands;
using Xunit;

namespace PuertoRico.Engine.Test.Integration
{
    public class GameCycleTest : TestBase
    {
        [Fact]
        public async Task SelectRole_With_Governor() {
            var (gameId, s1, s2, s3) = await Create3PlayerGame();
            var role = s1.GameState.SelectableRoles.First();
            var action = new SelectRole {
                RoleIndex = role.Index
            };
            await s1.SelectRole(new GameCommand<SelectRole> {
                Action = action,
                GameId = gameId
            });
            Assert.DoesNotContain(s1.GameState.SelectableRoles, r => r.Name == role.Name);
            var governorRole = s1.GameState.Players.Single(p => p.UserId == s1.UserId).Role;
            Assert.NotNull(governorRole);
            Assert.Equal(role.Name, governorRole.Name);
        }

        [Fact]
        public async Task RoleCanBeFinished() {
            var (gameId, s1, s2, s3) = await Create3PlayerGame();
            await SelectRole(s1, "Settler");
            await s1.TakeQuarry(new GameCommand<TakeQuarry> {
                Action = new TakeQuarry(),
                GameId = gameId
            });
            var t1 = s1.GameState.VisiblePlantations.First().Index;
            await s2.TakePlantation(new GameCommand<TakePlantation> {
                Action = new TakePlantation {TileIndex = t1},
                GameId = gameId
            });
            await s3.TakePlantation(new GameCommand<TakePlantation> {
                Action = new TakePlantation {TileIndex = t1 + 1},
                GameId = gameId
            });
            Assert.Equal(s2.UserId, s1.GameState.CurrentPlayer.UserId);
            Assert.Null(s1.GameState.CurrentRole);
        }
    }
}