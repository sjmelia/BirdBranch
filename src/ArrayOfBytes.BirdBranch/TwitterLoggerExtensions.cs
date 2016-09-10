using Microsoft.Extensions.Logging;
using System;

namespace ArrayOfBytes.BirdBranch
{
    public static class TwitterLoggerExtensions
    {
        public static ILoggerFactory AddTwitterStatus(this ILoggerFactory factory,
            OAuthInfo twitterOauthInfo,
            Func<string, LogLevel, bool> filter = null)
        {
            var provider = new TwitterLoggingProvider(
                new TinyTwitter(twitterOauthInfo),
                filter);
            factory.AddProvider(provider);
            return factory;
        }

        public static ILoggerFactory AddTwitterDirectMessage(this ILoggerFactory factory,
            OAuthInfo twitterOauthInfo,
            string screenName,
            Func<string, LogLevel, bool> filter = null)
        {
            var provider = new TwitterLoggingProvider(
                new TinyTwitter(twitterOauthInfo),
                filter,
                screenName);
            factory.AddProvider(provider);
            return factory;
        }
    }
}
