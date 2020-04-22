using System.Collections.Generic;
using System.Threading.Tasks;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Services
{
    public interface IReplayableGameService : IGameService
    {
        Task<IEnumerable<PlayerResultDto>> HandleFinishedGame(Game game);
    }
}