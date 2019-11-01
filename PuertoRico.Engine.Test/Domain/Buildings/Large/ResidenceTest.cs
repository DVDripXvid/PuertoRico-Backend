using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Tiles;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Buildings.Large
{
    public class ResidenceTest
    {
        private readonly LargeBuilding _residence;

        public ResidenceTest() {
            _residence = new Residence();
        }

        [Fact]
        public void ComputeVPs_LessThen9Plantation() {
            IPlayer player = new Player();
            for (var i = 1; i <= 7; i++) {
                player.Plant(new Quarry());
            }

            var vp = _residence.ComputeVictoryPoints(player);
            
            Assert.Equal(4, vp);
        }
        
        [Fact]
        public void ComputeVPs_9Plantation() {
            IPlayer player = new Player();
            for (var i = 1; i <= 9; i++) {
                player.Plant(new Quarry());
            }

            var vp = _residence.ComputeVictoryPoints(player);
            
            Assert.Equal(4, vp);
        }
        
        [Fact]
        public void ComputeVPs_12Plantation() {
            IPlayer player = new Player();
            for (var i = 1; i <= 12; i++) {
                player.Plant(new Quarry());
            }

            var vp = _residence.ComputeVictoryPoints(player);
            
            Assert.Equal(7, vp);
        }
    }
}