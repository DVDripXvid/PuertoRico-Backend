using System.Threading.Tasks;
using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.SignalR
{
    public interface IGameHub
    {
        Task CreateGame(string name);
        Task JoinGame(string gameId);
        Task LeaveGame(string gameId);
        Task StartGame(string gameId);
        Task SelectRole(string gameId, SelectRole selectRole);
        Task BonusProduction(string gameId, BonusProduction bonusProduction);
        Task Build(string gameId, Build build);
        Task DeliverGoods(string gameId, DeliverGoods deliverGoods);
        Task EndPhase(string gameId, EndPhase endPhase);
        Task EndRole(string gameId, EndRole endRole);
        Task MoveColonist(string gameId, MoveColonist moveColonist);
        Task SellGood(string gameId, SellGood sellGood);
        Task StoreGoods(string gameId, StoreGoods storeGoods);
        Task TakePlantation(string gameId, TakePlantation takePlantation);
        Task TakeQuarry(string gameId, TakeQuarry takeQuarry);
        Task TakeRandomPlantation(string gameId, TakeRandomPlantation takeRandomPlantation);
        Task UseWharf(string gameId, UseWharf useWharf);
    }
}