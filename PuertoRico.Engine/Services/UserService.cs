using System;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace PuertoRico.Engine.Services
{
    public class UserService : IUserService
    {
        public string GetUserId(HubCallerContext context) {
            return context.UserIdentifier ??
                   throw new UnauthorizedAccessException("Cannot obtain user identifier");
        }

        public string GetUsername(HubCallerContext context) {
            return context.User.FindFirstValue(ClaimTypes.Name) ?? GetUserId(context);
        }
    }
}