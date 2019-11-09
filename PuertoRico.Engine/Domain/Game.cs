using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Misc;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Tiles;
using PuertoRico.Engine.Domain.Tiles.Plantations;

namespace PuertoRico.Engine.Domain
{
    public class Game
    {
        public PlantationDeck PlantationDeck { get; }
        public List<IBuilding> Buildings { get; }
        public int PlayerCount { get; }
        public Stack<Colonist> Colonists { get; }
        public Stack<Quarry> Quarries { get; }
        public List<IPlayer> Players { get; }
        public IPlayer CurrentPlayer { get; private set; }
        public ColonistsShip ColonistsShip { get; }
        public TradeHouse TradeHouse { get; } = new TradeHouse();
        public List<IGood> Goods { get; }
        public List<CargoShip> CargoShips { get; }
        public List<VictoryPointChip> VictoryPointChips { get; }

        public Game(int playerCount) {
            if (playerCount < GameConfig.MinPlayer || playerCount > GameConfig.MaxPlayer) {
                throw new InvalidOperationException("Invalid player count");
            }

            PlayerCount = playerCount;
            PlantationDeck = new PlantationDeck(playerCount);

            var colonistCount = GameConfig.ColonistCount[playerCount];
            Colonists = new Stack<Colonist>(colonistCount);
            for (var i = 0; i < colonistCount; i++) {
                Colonists.Push(new Colonist());
            }

            Quarries = new Stack<Quarry>(GameConfig.QuarryCount);
            for (var i = 0; i < GameConfig.QuarryCount; i++) {
                Quarries.Push(new Quarry());
            }

            Buildings = InitializeBuildings();
            Players = InitializePlayers(playerCount);
            CurrentPlayer = Players.First();
            ColonistsShip = new ColonistsShip(this);
            Goods = InitializeGoods();
            CargoShips = InitializeCargoShips(playerCount);
            VictoryPointChips = InitializeVictoryPointChips(playerCount);
        }

        public void MoveToNextPlayer() {
            CurrentPlayer.Role.CleanUp();
            CurrentPlayer = GetNextPlayerTo(CurrentPlayer);
        }

        public HashSet<ActionType> GetAvailableActionTypes(IPlayer player) {
            if (player == CurrentPlayer && player.Role == null) {
                return new HashSet<ActionType> {ActionType.SelectRole};
            }

            return CurrentPlayer.Role.GetAvailableActionTypes(player);
        }

        public void ForEachPlayerStartWith(IPlayer initPlayer, Action<IPlayer> action) {
            var currentPlayer = initPlayer;
            do {
                action(currentPlayer);
                currentPlayer = GetNextPlayerTo(currentPlayer);
            } while (initPlayer != currentPlayer);
        }

        public IPlayer GetNextPlayerTo(IPlayer currentPlayer) {
            var indexOfCurrentPlayer = Players.IndexOf(currentPlayer);
            var indexOfNextPlayer = indexOfCurrentPlayer == PlayerCount - 1
                ? 0
                : indexOfCurrentPlayer + 1;
            return Players[indexOfNextPlayer];
        }

        private static List<IBuilding> InitializeBuildings() {
            return new List<IBuilding> {
                // Large buildings
                new Fortress(),
                new Residence(),
                new CityHall(),
                new CustomsHouse(),
                new GuildHall(),
                // Small buildings
                // TODO: rest of the small buildings
                new Hacienda(),
                new Hacienda(),
                new Hospice(),
                new Hospice(),
                // TODO: production buildings
                // Small production buildings
                new SmallIndigoPlant(),
                new SmallIndigoPlant(),
                new SmallIndigoPlant()
            };
        }

        private static List<IPlayer> InitializePlayers(int playerCount) {
            var players = new List<IPlayer>();
            for (var i = 0; i < playerCount; i++) {
                players.Add(new Player.Player());
            }

            return players;
        }

        private static List<IGood> InitializeGoods() {
            var goods = new List<IGood>();
            for (var i = 0; i < GameConfig.IndigoCount; i++) {
                goods.Add(new Indigo());
            }

            for (var i = 0; i < GameConfig.SugarCount; i++) {
                goods.Add(new Sugar());
            }

            for (var i = 0; i < GameConfig.CornCount; i++) {
                goods.Add(new Corn());
            }

            for (var i = 0; i < GameConfig.TobaccoCount; i++) {
                goods.Add(new Tobacco());
            }

            for (var i = 0; i < GameConfig.CoffeeCount; i++) {
                goods.Add(new Coffee());
            }

            return goods;
        }

        private static List<CargoShip> InitializeCargoShips(int playerCount) {
            var cargos = new List<CargoShip>(3);
            var firstShipCapacity = playerCount + 1;
            for (var i = 0; i < 3; i++) {
                cargos.Add(new CargoShip(firstShipCapacity + i));
            }

            return cargos;
        }

        private static List<VictoryPointChip> InitializeVictoryPointChips(int playerCount) {
            var vpCount = playerCount * 25;
            var vpList = new List<VictoryPointChip>(vpCount);
            for (var i = 0; i < vpCount; i++) {
                vpList.Add(new VictoryPointChip());
            }

            return vpList;
        }
    }
}