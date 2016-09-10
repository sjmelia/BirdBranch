using Microsoft.Extensions.Logging;
using System;

namespace ArrayOfBytes.BirdBranch
{
    public class TwitterLoggingProvider : ILoggerProvider
    {
        private readonly TinyTwitter twitter;
        private readonly Func<string, LogLevel, bool> filter;
        private readonly string screenName;

        public TwitterLoggingProvider(TinyTwitter twitter,
            Func<string, LogLevel, bool> filter = null,
            string screenName = null)
        {
            this.filter = filter;
            this.twitter = twitter;
            this.screenName = screenName;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TwitterLogger(categoryName, twitter, filter, screenName);
        }

        public void Dispose()
        {
        }
    }
}
