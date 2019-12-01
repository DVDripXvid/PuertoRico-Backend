using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace PuertoRico.Engine.Services
{
    public class UserService : IUserService
    {
        public string GetUserId(HubCallerContext context)
        {
            var fallbackUserId = context.GetHttpContext().Request.Query["user"].First();
            
            return context.UserIdentifier ??
                   fallbackUserId ??
                   throw new UnauthorizedAccessException("Cannot obtain user identifier");
        }

        public string GetUsername(HubCallerContext context) {
            return context.User.FindFirstValue(ClaimTypes.Name) ?? GetUserId(context);
        }
    }
}