using Microsoft.Extensions.Logging;
using System;

namespace ArrayOfBytes.BirdBranch
{
    // https://wildermuth.com/2016/04/22/Implementing-an-ASP-NET-Core-RC1-Logging-Provider
    public class TwitterLogger : ILogger
    {
        private readonly string categoryName;
        private readonly Func<string, LogLevel, bool> filter;
        private readonly TinyTwitter twitter;
        private readonly string screenName;

        public TwitterLogger(string categoryName,  
            TinyTwitter twitter,
            Func<string, LogLevel, bool> filter = null,
            string screenName = null)
        {
            this.categoryName = categoryName;
            this.filter = filter;
            this.twitter = twitter;
            this.screenName = screenName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (this.filter == null || this.filter(this.categoryName, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = message.Substring(0, Math.Min(140, message.Length));

            if (!string.IsNullOrEmpty(screenName))
            {
                this.twitter.DirectMessageNew(this.screenName, message);
            }
            else
            {
                this.twitter.UpdateStatus(message);
            }
        }
    }
}
