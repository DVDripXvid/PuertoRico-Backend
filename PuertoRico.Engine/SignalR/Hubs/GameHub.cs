using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.Services;
using PuertoRico.Engine.SignalR.Commands;
using PuertoRico.Engine.SignalR.Events;
using PuertoRico.Engine.Stores;

namespace PuertoRico.Engine.SignalR.Hubs
{
    [Authorize]
    public class GameHub : Hub<IGameClient>, IGameHub
    {
        private readonly IReplayableGameService _gameService;
        private readonly IInProgressGameStore _inProgressGameStore;
        private readonly IUserService _userService;

        public GameHub(IReplayableGameService gameService, IInProgressGameStore inProgressGameStore,
            IUserService userService) {
            _gameService = gameService;
            _inProgressGameStore = inProgressGameStore;
            _userService = userService;
        }

        public override async Task OnConnectedAsync() {
            if (Context.GetHttpContext().Request.Query.TryGetValue("gameId", out var gameId)) {
                var game = _inProgressGameStore.FindById(gameId);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                var changedEvent = new GameChangedEvent {
                    Game = GameDto.Create(game)
                };
                await Clients.Caller.GameChanged(changedEvent);
                await SendAvailableActionTypes(game, GetUserId());
            }
            else {
                await Clients.Caller.Error(new GameErrorEvent {
                    ErrorMessage = "Not found gameId query param"
                });
            }
        }

        public async Task SelectRole(GameCommand<SelectRole> cmd) {
            var game = _inProgressGameStore.FindById(cmd.GameId);
            try {
                await _gameService.ExecuteSelectRole(game, GetUserId(), cmd.Action);
            }
            catch (Exception e) {
                await Clients.Caller.Error(new GameErrorEvent {ErrorMessage = e.Message});
            }

            var changedEvent = new GameChangedEvent {
                Game = GameDto.Create(game)
            };
            await Clients.Group(cmd.GameId).GameChanged(changedEvent);

            await SendAvailableActionTypes(game, GetUserId());
        }

        public Task BonusProduction(GameCommand<BonusProduction> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task Build(GameCommand<Build> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task DeliverGoods(GameCommand<DeliverGoods> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task EndPhase(GameCommand<EndPhase> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task EndRole(GameCommand<EndRole> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task MoveColonist(GameCommand<MoveColonist> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task PlaceColonist(GameCommand<PlaceColonist> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task SellGood(GameCommand<SellGood> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task StoreGoods(GameCommand<StoreGoods> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task TakePlantation(GameCommand<TakePlantation> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task TakeQuarry(GameCommand<TakeQuarry> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task TakeRandomPlantation(GameCommand<TakeRandomPlantation> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task UseWharf(GameCommand<UseWharf> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        private async Task ExecuteRoleAction(string gameId, IAction build) {
            var game = _inProgressGameStore.FindById(gameId);
            try {
                await _gameService.ExecuteRoleAction(game, GetUserId(), build);
            }
            catch (Exception e) {
                await Clients.Caller.Error(new GameErrorEvent {ErrorMessage = e.Message});
            }

            await Clients.Groups(gameId).GameChanged(new GameChangedEvent {Game = GameDto.Create(game)});

            var tasks = game.Players.Select(p => SendAvailableActionTypes(game, p.UserId));
            await Task.WhenAll(tasks);

            if (game.Status == GameStatus.ENDED) {
                await AfterGameEnded(game);
            }
        }

        private async Task SendAvailableActionTypes(Game game, string userId) {
            var availableActions = await _gameService.GetAvailableActionTypeForUser(game, userId);

            await Clients.Users(userId).AvailableActionTypesChanged(new AvailableActionTypesChangedEvent {
                GameId = game.Id,
                ActionTypes = availableActions,
            });
        }

        private async Task AfterGameEnded(Game game) {
            var results = await _gameService.HandleFinishedGame(game);
            var endedEvent = new GameEndedEvent {
                GameId = game.Id,
                Results = results
            };
            await Clients.Groups(game.Id).GameEnded(endedEvent);
            _inProgressGameStore.Remove(game.Id);
        }

        private string GetUserId() {
            return _userService.GetUserId(Context);
        }
    }
}