using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace PuertoRico.Engine.Services
{
    public class UserService : IUserService
    {
        public string GetUserId(HubCallerContext context)
        {
            return context.UserIdentifier ??
                   throw new UnauthorizedAccessException("Cannot obtain user identifier");
        }

        public string GetUsername(HubCallerContext context) {
            return context.User.FindFirstValue("name") ?? 
                   context.User.FindFirstValue(ClaimTypes.Name) ??
                   GetUserId(context);
        }
    }
}