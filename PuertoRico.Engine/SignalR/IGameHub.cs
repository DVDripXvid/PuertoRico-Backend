using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.SignalR.Commands;

namespace PuertoRico.Engine.SignalR
{
    public interface IGameHub
    {
        Task CreateGame(CreateGameCmd cmd);
        Task JoinGame(GenericGameCmd cmd);
        Task LeaveGame(GenericGameCmd cmd);
        Task StartGame(GenericGameCmd cmd);
        Task SelectRole(GameCommand<SelectRole> cmd);
        Task BonusProduction(GameCommand<BonusProduction> cmd);
        Task Build(GameCommand<Build> cmd);
        Task DeliverGoods(GameCommand<DeliverGoods> cmd);
        Task EndPhase(GameCommand<EndPhase> cmd);
        Task EndRole(GameCommand<EndRole> cmd);
        Task MoveColonist(GameCommand<MoveColonist> cmd);
        Task PlaceColonist(GameCommand<PlaceColonist> cmd);
        Task SellGood(GameCommand<SellGood> cmd);
        Task StoreGoods(GameCommand<StoreGoods> cmd);
        Task TakePlantation(GameCommand<TakePlantation> cmd);
        Task TakeQuarry(GameCommand<TakeQuarry> cmd);
        Task TakeRandomPlantation(GameCommand<TakeRandomPlantation> cmd);
        Task UseWharf(GameCommand<UseWharf> cmd);
    }
}