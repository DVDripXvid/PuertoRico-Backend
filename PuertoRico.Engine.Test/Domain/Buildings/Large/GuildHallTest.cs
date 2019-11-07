﻿using System.Linq;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Production;
using PuertoRico.Engine.Domain.Buildings.Production.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Buildings.Large
{
    public class GuildHallTest
    {
        private readonly LargeBuilding _guildHall;
        
        public GuildHallTest() {
            _guildHall = new GuildHall();
        }

        [Fact]
        public void ComputeVPs_NoBuildings() {
            IPlayer player = new Player();
            player.Build(_guildHall);
            
            var vp = _guildHall.ComputeVictoryPoints(player);
            
            Assert.Equal(0, vp);
        }
        
        [Fact]
        public void ComputeVPs_LargeAndSmallBuildings() {
            IPlayer player = new Player();
            var buildings = new IBuilding[] {new SmallSugarMill(), new SmallSugarMill(), new LargeSugarMill()};
            var expectedVp = buildings.Select(b => b.VictoryPoint).Sum();
            buildings.ToList().ForEach(b => player.Build(b));
            player.Build(_guildHall);

            var vp = _guildHall.ComputeVictoryPoints(player);
            
            Assert.Equal(expectedVp, vp);
        }
    }
}