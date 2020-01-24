using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Large;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using Xunit;

namespace PuertoRico.Engine.UnitTest.Domain.Buildings.Large
{
    public class CityHallTest
    {
        private readonly LargeBuilding _cityHall;
        
        public CityHallTest() {
            _cityHall = new CityHall();
        }
        
        [Fact]
        public void ComputeVPs() {
            IPlayer player = new Player();
            player.Build(new Fortress());
            player.Build(_cityHall);
            
            player.Build(new SmallMarket());
            player.Build(new Hacienda());
            
            player.Build(new SugarMill());

            var vp = _cityHall.ComputeVictoryPoints(player);
            
            Assert.Equal(4, vp);
        }
    }
}