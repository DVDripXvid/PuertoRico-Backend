using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Misc;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Domain.Tiles;
using PuertoRico.Engine.Domain.Tiles.Plantations;

namespace PuertoRico.Engine.Domain
{
    public class Game
    {
        public string Id { get; }
        public string Name { get; }
        public string Endpoint { get; set; }
        public PlantationDeck PlantationDeck { get; private set; }
        public List<IBuilding> Buildings { get; private set; }
        public Stack<Colonist> Colonists { get; private set; }
        public Stack<Quarry> Quarries { get; private set; }
        public List<IPlayer> Players { get; }
        public IPlayer CurrentRoleOwnerPlayer { get; private set; }
        public ColonistShip ColonistShip { get; private set; }
        public TradeHouse TradeHouse { get; } = new TradeHouse();
        public List<IGood> Goods { get; private set; }
        public List<CargoShip> CargoShips { get; private set; }
        public List<VictoryPointChip> VictoryPointChips { get; private set; }
        public List<IRole> Roles { get; private set; }
        public int RandomSeed { get; private set; }
        public int PlayerCount => Players.Count;
        public GameStatus Status { get; set; } = GameStatus.INITIAL;
        public bool IsFull => Status != GameStatus.INITIAL || PlayerCount == GameConfig.MaxPlayer;
        private bool _isLastYear;
        private IPlayer _governor;
        private readonly object _syncRoot = new object();
        
        public Game(string name = "DefaultGame") {
            Name = name;
            Players = new List<IPlayer>();
            Id = Guid.NewGuid().ToString();
            RandomSeed = new Random().Next();
        }
        
        public Game(string id, string name, int randomSeed) {
            Name = name;
            Players = new List<IPlayer>();
            Id = id;
            RandomSeed = randomSeed;
        }

        public void Start() {
            if (PlayerCount < GameConfig.MinPlayer || PlayerCount > GameConfig.MaxPlayer) {
                throw new InvalidOperationException("Invalid player count");
            }
            
            PlantationDeck = new PlantationDeck(PlayerCount, RandomSeed);

            var colonistCount = GameConfig.ColonistCount[PlayerCount];
            Colonists = new Stack<Colonist>(colonistCount);
            for (var i = 0; i < colonistCount; i++) {
                Colonists.Push(new Colonist());
            }

            Quarries = new Stack<Quarry>(GameConfig.QuarryCount);
            for (var i = 0; i < GameConfig.QuarryCount; i++) {
                Quarries.Push(new Quarry());
            }

            Buildings = InitializeBuildings();
            _governor = Players.First();
            CurrentRoleOwnerPlayer = _governor;
            ColonistShip = new ColonistShip(this);
            Goods = InitializeGoods();
            CargoShips = InitializeCargoShips(PlayerCount);
            VictoryPointChips = InitializeVictoryPointChips(PlayerCount);
            Roles = InitializeRoles(PlayerCount, this);
            
            InitializePlayerDoubloons(Players);
            InitializePlayerPlantations(Players, PlantationDeck);

            Status = GameStatus.RUNNING;
        }

        public void Join(IPlayer player) {
            lock (_syncRoot) {
                if (IsFull) {
                    throw new InvalidOperationException("This game is full");
                }

                if (Players.Any(p => p.UserId == player.UserId)) {
                    throw new InvalidOperationException("Player already joined to this game");
                }

                Players.Add(player);
            }
        }

        public void Leave(IPlayer player) {
            lock (_syncRoot) {
                if (Status != GameStatus.INITIAL) {
                    throw new InvalidOperationException("Game already started");
                }

                if (!Players.Remove(player)) {
                    throw new InvalidOperationException("Player not in game");
                }
            }
        }

        public void MoveToNextPlayer() {
            CurrentRoleOwnerPlayer.Role.CleanUp();
            CurrentRoleOwnerPlayer = GetNextPlayerTo(CurrentRoleOwnerPlayer);
            if (CurrentRoleOwnerPlayer == _governor) {
                StartNewYear();
            }
        }

        public void SendShouldFinishSignal() {
            _isLastYear = true;
        }

        private void StartNewYear() {
            if (_isLastYear) {
                Status = GameStatus.ENDED;
                Players.ForEach(p => p.PutBackRole(this));
                return;
            }

            Roles.ForEach(role => role.AddOneDoubloon());
            Players.ForEach(p => p.PutBackRole(this));
            _governor = GetNextPlayerTo(_governor);
            CurrentRoleOwnerPlayer = _governor;
        }

        public HashSet<ActionType> GetAvailableActionTypes(IPlayer player) {
            if (CurrentRoleOwnerPlayer.Role == null) {
                return player == CurrentRoleOwnerPlayer 
                    ? new HashSet<ActionType> {ActionType.SelectRole} 
                    : new HashSet<ActionType>();
            }

            return CurrentRoleOwnerPlayer.Role.GetAvailableActionTypes(player);
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

        public IPlayer GetCurrentPlayer() {
            return CurrentRoleOwnerPlayer.Role == null
                ? CurrentRoleOwnerPlayer
                : CurrentRoleOwnerPlayer.Role.CurrentPlayer;
        }

        private static void InitializePlayerDoubloons(List<IPlayer> players) {
            var doubloonCount = players.Count - 1;
            players.ForEach(p => p.Doubloons = doubloonCount);
        }

        private static void InitializePlayerPlantations(IReadOnlyList<IPlayer> players, PlantationDeck deck) {
            players[0].Plant(deck.DrawForType<Indigo>());
            players[1].Plant(deck.DrawForType<Indigo>());
            switch (players.Count) {
                case 3:
                    players[2].Plant(deck.DrawForType<Corn>());
                    break;
                case 4:
                    players[2].Plant(deck.DrawForType<Corn>());
                    players[3].Plant(deck.DrawForType<Corn>());
                    break;
                case 5:
                    players[2].Plant(deck.DrawForType<Indigo>());
                    players[3].Plant(deck.DrawForType<Corn>());
                    players[4].Plant(deck.DrawForType<Corn>());
                    break;
                default:
                    throw new InvalidOperationException("Invalid player count: " + players.Count);
            }
        }

        private static List<IRole> InitializeRoles(int playerCount, Game game) {
            var roles = new List<IRole> {
                new Builder(game),
                new Captain(game),
                new Craftsman(game),
                new Mayor(game),
                new Settler(game),
                new Trader(game),
            };
            if (playerCount > 3) {
                roles.Add(new Prospector(game));
            }

            if (playerCount > 4) {
                roles.Add(new Prospector(game));
            }

            return roles;
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
                new ConstructionHut(),
                new ConstructionHut(),
                new Factory(),
                new Factory(),
                new Hacienda(),
                new Hacienda(),
                new Harbor(),
                new Harbor(),
                new Hospice(),
                new Hospice(),
                new LargeMarket(),
                new LargeMarket(),
                new LargeWarehouse(),
                new LargeWarehouse(),
                new Office(),
                new Office(),
                new SmallMarket(),
                new SmallMarket(),
                new SmallWarehouse(),
                new SmallWarehouse(),
                new University(),
                new University(),
                new Wharf(),
                new Wharf(),
                // Small production buildings
                new SmallIndigoPlant(),
                new SmallIndigoPlant(),
                new SmallIndigoPlant(),
                new SmallIndigoPlant(),
                new SmallSugarMill(),
                new SmallSugarMill(),
                new SmallSugarMill(),
                new SmallSugarMill(),
                // Large production buildings
                new IndigoPlant(),
                new IndigoPlant(),
                new IndigoPlant(),
                new SugarMill(),
                new SugarMill(),
                new SugarMill(),
                new TobaccoStorage(),
                new TobaccoStorage(),
                new TobaccoStorage(),
                new CoffeeRoaster(),
                new CoffeeRoaster(),
                new CoffeeRoaster(),
            };
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