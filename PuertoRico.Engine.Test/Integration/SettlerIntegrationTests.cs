using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using Xunit;

namespace PuertoRico.Engine.Test.Integration
{
    public class SettlerIntegrationTests : TestBase
    {
        [Fact]
        public async Task CanFillTilePlaces() {
            var (gameId, s, _, _) = await Create3PlayerGame();

            for (var i = 1; i <= 11; i++) {
                var current = GetCurrentSession(s.GameState);
                await SelectRole(current, "Settler");
                var t1 = s.GameState.VisiblePlantations.First().Index;
                await current.TakePlantation(gameId, new TakePlantation {TileIndex = t1});
                current = GetCurrentSession(s.GameState);
                await current.TakePlantation(gameId, new TakePlantation {TileIndex = t1 + 1});
                current = GetCurrentSession(s.GameState);
                await current.TakePlantation(gameId, new TakePlantation {TileIndex = t1 + 2});
                Assert.Null(s.GameState.CurrentRole);
                Assert.DoesNotContain(s.GameState.SelectableRoles, r => r.Name == "Settler");

                current = GetCurrentSession(s.GameState);
                await SelectRole(current, "Craftsman");
                await current.EndPhase(gameId, new EndPhase());
                Assert.Null(s.GameState.CurrentRole);
                Assert.DoesNotContain(s.GameState.SelectableRoles, r => r.Name == "Settler");
                Assert.DoesNotContain(s.GameState.SelectableRoles, r => r.Name == "Craftsman");
            
                current = GetCurrentSession(s.GameState);
                await SelectRole(current, "Mayor");
                await current.EndPhase(gameId, new EndPhase());
                current = GetCurrentSession(s.GameState);
                await current.EndPhase(gameId, new EndPhase());
                current = GetCurrentSession(s.GameState);
                await current.EndPhase(gameId, new EndPhase());
                
                Assert.Null(s.GameState.CurrentRole);
                Assert.Contains(s.GameState.SelectableRoles, r => r.Name == "Settler");
                Assert.Contains(s.GameState.SelectableRoles, r => r.Name == "Builder");
                Assert.Contains(s.GameState.SelectableRoles, r => r.Name == "Mayor");
            }
            
            Assert.All(s.GameState.Players.Select(p => p.Tiles.Count), tileCount => Assert.Equal(12, tileCount));
        }
    }
}