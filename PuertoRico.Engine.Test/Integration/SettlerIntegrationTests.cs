using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.SignalR.Commands;
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
                await current.TakePlantation(new GameCommand<TakePlantation> {
                    Action = new TakePlantation {TileIndex = t1},
                    GameId = gameId
                });
                current = GetCurrentSession(s.GameState);
                await current.TakePlantation(new GameCommand<TakePlantation> {
                    Action = new TakePlantation {TileIndex = t1 + 1},
                    GameId = gameId
                });
                current = GetCurrentSession(s.GameState);
                await current.TakePlantation(new GameCommand<TakePlantation> {
                    Action = new TakePlantation {TileIndex = t1 + 2},
                    GameId = gameId
                });
                Assert.Null(s.GameState.CurrentRole);
                Assert.DoesNotContain(s.GameState.SelectableRoles, r => r.Name == "Settler");

                current = GetCurrentSession(s.GameState);
                await SelectRole(current, "Craftsman");
                await current.EndPhase(new GameCommand<EndPhase> {
                    Action = new EndPhase(),
                    GameId = gameId
                });
                Assert.Null(s.GameState.CurrentRole);
                Assert.DoesNotContain(s.GameState.SelectableRoles, r => r.Name == "Settler");
                Assert.DoesNotContain(s.GameState.SelectableRoles, r => r.Name == "Craftsman");
            
                current = GetCurrentSession(s.GameState);
                await SelectRole(current, "Mayor");
                await current.EndPhase(new GameCommand<EndPhase> {
                    Action = new EndPhase(),
                    GameId = gameId
                });
                current = GetCurrentSession(s.GameState);
                await current.EndPhase(new GameCommand<EndPhase> {
                    Action = new EndPhase(),
                    GameId = gameId
                });
                current = GetCurrentSession(s.GameState);
                await current.EndPhase(new GameCommand<EndPhase> {
                    Action = new EndPhase(),
                    GameId = gameId
                });
                
                Assert.Null(s.GameState.CurrentRole);
                Assert.Contains(s.GameState.SelectableRoles, r => r.Name == "Settler");
                Assert.Contains(s.GameState.SelectableRoles, r => r.Name == "Builder");
                Assert.Contains(s.GameState.SelectableRoles, r => r.Name == "Mayor");
            }
            
            Assert.All(s.GameState.Players.Select(p => p.Tiles.Count), tileCount => Assert.Equal(12, tileCount));
        }
    }
}