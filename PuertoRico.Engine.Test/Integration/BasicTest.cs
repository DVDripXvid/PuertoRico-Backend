using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.SignalR.Commands;
using PuertoRico.Engine.SignalR.Events;
using Xunit;

namespace PuertoRico.Engine.Test.Integration
{
    public class BasicTest : TestBase
    {
        [Fact]
        public async Task CreateGame() {
            var session = await CreateSession("user1");
            GameCreatedEvent ev = null;
            session.GameCreated += e => ev = e;
            await session.CreateGame(new CreateGameCmd {Name = "game1"});
            Assert.NotNull(ev);
            Assert.Equal("game1", ev.GameName);
            Assert.Single(ev.Players);
            Assert.Equal("user1", ev.Players.First().UserId);
        }
    }
}