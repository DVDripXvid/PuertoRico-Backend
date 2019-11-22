using System.Threading.Tasks;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.Test.Integration.Infrastructure;
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
            await session.CreateGame("game1");
            Assert.NotNull(ev);
            Assert.Equal("game1", ev.GameName);
            Assert.Equal("user1", ev.CreatedBy.UserId);
        }
    }
}