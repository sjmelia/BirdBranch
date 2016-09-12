namespace ArrayOfBytes.BirdBranch
{
    using System;
    using ArrayOfBytes.TeensyTwitter;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logging Provider for logging to Twitter.
    /// </summary>
    public class TwitterLoggingProvider : ILoggerProvider
    {
        private readonly TwitterClient twitter;

        private readonly Func<string, LogLevel, bool> filter;

        private readonly string screenName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterLoggingProvider"/> class.
        /// </summary>
        /// <param name="twitter">Twitter client to log to.</param>
        /// <param name="filter">Filter for log messages.</param>
        /// <param name="screenName">Screen name to log to for direct messages.</param>
        public TwitterLoggingProvider(
            TwitterClient twitter,
            Func<string, LogLevel, bool> filter = null,
            string screenName = null)
        {
            this.filter = filter;
            this.twitter = twitter;
            this.screenName = screenName;
        }

        /// <summary>
        /// Creates a Twitter logger for the given category.
        /// </summary>
        /// <param name="categoryName">The category to log to.</param>
        /// <returns>A Twitter logger.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new TwitterLogger(categoryName, this.twitter, this.filter, this.screenName);
        }

        /// <summary>
        /// Dispose of resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
