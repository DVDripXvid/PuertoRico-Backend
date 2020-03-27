using Microsoft.AspNetCore.SignalR;
using PuertoRico.Engine.Services;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class FakeUserService : IUserService
    {
        public volatile string UserId;
        
        public string GetUserId(HubCallerContext context) {
            return UserId;
        }

        public string GetUsername(HubCallerContext context) {
            return UserId;
        }

        public string GetPictureUrl(HubCallerContext context) {
            return "http://nopic.com";
        }
    }
}