using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PuertoRico.Engine.Services
{
    public interface IUserService
    {
        string GetUserId(HubCallerContext context);
        string GetUsername(HubCallerContext context);
        string GetPictureUrl(HubCallerContext context);
    }
}