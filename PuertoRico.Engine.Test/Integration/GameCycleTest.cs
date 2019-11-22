using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Test.Integration.Helpers;
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
            await s1.SelectRole(gameId, action);
            Assert.DoesNotContain(s1.GameState.SelectableRoles, r => r.Name == role.Name);
            var governorRole = s1.GameState.Players.Single(p => p.UserId == s1.UserId).Role;
            Assert.NotNull(governorRole);
            Assert.Equal(role.Name, governorRole.Name);
        }

        [Fact]
        public async Task RoleCanBeFinished() {
            var (gameId, s1, s2, s3) = await Create3PlayerGame();
            await SelectRole(s1, "Settler");
            await s1.TakeQuarry(gameId, new TakeQuarry());
            var t1 = s1.GameState.VisiblePlantations.First().Index;
            await s2.TakePlantation(gameId, new TakePlantation {TileIndex = t1});
            await s3.TakePlantation(gameId, new TakePlantation {TileIndex = t1 + 1});
            Assert.Equal(s2.UserId, s1.GameState.CurrentPlayer.UserId);
            Assert.Null(s1.GameState.CurrentRole);
        }

        [Fact]
        public async Task YearCanBeFinished() {
            var (gameId, s1, s2, s3) = await Create3PlayerGame();
            Assert.Equal(6, s1.GameState.SelectableRoles.Count());
            
            await SelectRole(s1, "Settler");
            await s1.TakeQuarry(gameId, new TakeQuarry());
            var t1 = s1.GameState.VisiblePlantations.First().Index;
            await s2.TakePlantation(gameId, new TakePlantation {TileIndex = t1});
            await s3.TakePlantation(gameId, new TakePlantation {TileIndex = t1 + 1});
            Assert.Equal(s2.UserId, s1.GameState.CurrentPlayer.UserId);
            Assert.Null(s1.GameState.CurrentRole);
            Assert.DoesNotContain(s1.GameState.SelectableRoles, r => r.Name == "Settler");

            await SelectRole(s2, "Builder");
            var bIdx = s1.GameState.Buildings.IndexOf("SmallIndigoPlant");
            await s2.Build(gameId, new Build {BuildingIndex = bIdx});
            bIdx = s1.GameState.Buildings.IndexOf("SmallIndigoPlant");
            await s3.Build(gameId, new Build {BuildingIndex = bIdx});
            bIdx = s1.GameState.Buildings.IndexOf("SmallIndigoPlant");
            await s1.Build(gameId, new Build {BuildingIndex = bIdx});
            Assert.Equal(s3.UserId, s1.GameState.CurrentPlayer.UserId);
            Assert.Null(s1.GameState.CurrentRole);
            Assert.DoesNotContain(s1.GameState.SelectableRoles, r => r.Name == "Settler");
            Assert.DoesNotContain(s1.GameState.SelectableRoles, r => r.Name == "Builder");
            
            await SelectRole(s3, "Mayor");
            await s3.EndPhase(gameId, new EndPhase());
            await s1.EndPhase(gameId, new EndPhase());
            await s2.EndPhase(gameId, new EndPhase());
            
            Assert.Equal(s2.UserId, s1.GameState.CurrentPlayer.UserId);
            Assert.Null(s1.GameState.CurrentRole);
            Assert.Contains(s1.GameState.SelectableRoles, r => r.Name == "Settler");
            Assert.Contains(s1.GameState.SelectableRoles, r => r.Name == "Builder");
            Assert.Contains(s1.GameState.SelectableRoles, r => r.Name == "Mayor");
            Assert.Equal(0, s1.GameState.SelectableRoles.Single(r => r.Name == "Settler").Doubloons);
            Assert.Equal(0, s1.GameState.SelectableRoles.Single(r => r.Name == "Builder").Doubloons);
            Assert.Equal(0, s1.GameState.SelectableRoles.Single(r => r.Name == "Mayor").Doubloons);

            Assert.Equal(3, s1.GameState.SelectableRoles.Count(r => r.Doubloons == 1));
        }
    }
}