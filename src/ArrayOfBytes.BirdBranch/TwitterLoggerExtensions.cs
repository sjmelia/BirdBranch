namespace ArrayOfBytes.BirdBranch
{
    using System;
    using ArrayOfBytes.OAuth.Client;
    using ArrayOfBytes.TeensyTwitter;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extensions for adding Twitter logging.
    /// </summary>
    public static class TwitterLoggerExtensions
    {
        /// <summary>
        /// Add a logger provider for logging to Twitter timelines.
        /// </summary>
        /// <param name="factory">LoggerFactory to add provider to.</param>
        /// <param name="twitterOauthInfo">OAuth credentials for the Twitter account.</param>
        /// <param name="filter">Filter for log messages.</param>
        /// <returns>LoggerFactory for a Twitter logger</returns>
        public static ILoggerFactory AddTwitterStatus(
            this ILoggerFactory factory,
            OAuthConfig twitterOauthInfo,
            Func<string, LogLevel, bool> filter = null)
        {
            var provider = new TwitterLoggingProvider(
                new TwitterClient(twitterOauthInfo),
                filter);
            factory.AddProvider(provider);
            return factory;
        }

        /// <summary>
        /// Add a logger provider for logging with Twitter direct messaging.
        /// </summary>
        /// <param name="factory">LoggerFactory to add provider to.</param>
        /// <param name="twitterOauthInfo">OAuth credentials for the Twitter account.</param>
        /// <param name="screenName">Screen name of the user to tweet to.</param>
        /// <param name="filter">Filter for log messages.</param>
        /// <returns>LoggerFactory for a Twitter logger</returns>
        public static ILoggerFactory AddTwitterDirectMessage(
            this ILoggerFactory factory,
            OAuthConfig twitterOauthInfo,
            string screenName,
            Func<string, LogLevel, bool> filter = null)
        {
            var provider = new TwitterLoggingProvider(
                new TwitterClient(twitterOauthInfo),
                filter,
                screenName);
            factory.AddProvider(provider);
            return factory;
        }
    }
}
