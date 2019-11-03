using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Buildings.Large
{
    public class FortressTest
    {
        private readonly LargeBuilding _fortress;
        
        public FortressTest() {
            _fortress = new Fortress();
        }

        [Fact]
        public void ComputeVPs_Exact() {
            IPlayer player = new Player();
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());

            var vp = _fortress.ComputeVictoryPoints(player);
            
            Assert.Equal(1, vp);
        }
        
        [Fact]
        public void ComputeVPs_NoPoints() {
            IPlayer player = new Player();
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());

            var vp = _fortress.ComputeVictoryPoints(player);
            
            Assert.Equal(0, vp);
        }
        
        [Fact]
        public void ComputeVPs_WithRest() {
            IPlayer player = new Player();
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());

            var vp = _fortress.ComputeVictoryPoints(player);
            
            Assert.Equal(1, vp);
        }
        
        [Fact]
        public void ComputeVPs_OnBuildings() {
            IPlayer player = new Player();
            player.AddColonist(new Colonist());
            player.AddColonist(new Colonist());
            var smallMarket = new SmallMarket();
            smallMarket.AddWorker(new Colonist());
            player.Build(smallMarket);
            _fortress.AddWorker(new Colonist());
            player.Build(_fortress);

            var vp = _fortress.ComputeVictoryPoints(player);
            
            Assert.Equal(1, vp);
        }
    }
}