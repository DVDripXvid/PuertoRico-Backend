using System.Collections.Generic;
using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources;
using Xunit;

namespace PuertoRico.Engine.UnitTest.Domain.Buildings.Large
{
    public class CustomsHouseTest
    {
        private readonly LargeBuilding _customsHouse;
        
        public CustomsHouseTest() {
            _customsHouse = new CustomsHouse();
        }
        
        [Fact]
        public void ComputeVPs_Exact() {
            IPlayer player = new Player();
            var chips = new List<VictoryPointChip> {
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
            };
            player.AddVictoryPointChips(chips);

            var vp = _customsHouse.ComputeVictoryPoints(player);
            
            Assert.Equal(1, vp);
        }
        
        [Fact]
        public void ComputeVPs_NoPoints() {
            IPlayer player = new Player();
            var chips = new List<VictoryPointChip> {
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
            };
            player.AddVictoryPointChips(chips);

            var vp = _customsHouse.ComputeVictoryPoints(player);
            
            Assert.Equal(0, vp);
        }
        
        [Fact]
        public void ComputeVPs_WithRest() {
            IPlayer player = new Player();
            var chips = new List<VictoryPointChip> {
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
                new VictoryPointChip(),
            };
            player.AddVictoryPointChips(chips);

            var vp = _customsHouse.ComputeVictoryPoints(player);
            
            Assert.Equal(1, vp);
        }
    }
}