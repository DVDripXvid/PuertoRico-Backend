using StackExchange.Redis;
using System;

namespace PuertoRico.Engine.Extensions
{
    public static class RedisExtensions
    {
        public static ConfigurationOptions ParseUrl(this ConfigurationOptions options, string redisUrl)
        {
            var parts = redisUrl
                .Replace("redis://:", "")
                .Replace("redis://", "").Split("@");
            if (parts.Length > 2)
            {
                throw new ArgumentException("Failed to parse redis url: " + redisUrl);
            }
            options.Password = parts.Length == 2 ? parts[0] : null;
            var hostAndPort = (parts.Length == 2 ? parts[1] : parts[0]);
            options.EndPoints.Add(hostAndPort);
            options.AllowAdmin = false;
            options.Ssl = false;
            return options;
        }
    }
}