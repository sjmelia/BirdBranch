namespace ArrayOfBytes.BirdBranch
{
    using System;
    using ArrayOfBytes.TeensyTwitter;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    /// <summary>
    /// Logger for logging messages to Twitter statuses or direct messages.
    /// </summary>
    /// <remarks>
    /// With thanks to https://wildermuth.com/2016/04/22/Implementing-an-ASP-NET-Core-RC1-Logging-Provider
    /// </remarks>
    public class TwitterLogger : ILogger
    {
        private readonly string categoryName;

        private readonly Func<string, LogLevel, bool> filter;

        private readonly TwitterClient twitter;

        private readonly string screenName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterLogger"/> class.
        /// </summary>
        /// <param name="categoryName">Category to log to.</param>
        /// <param name="twitter">Twitter client to log to.</param>
        /// <param name="filter">Filter for log messages.</param>
        /// <param name="screenName">Screen name to send direct messages to.</param>
        public TwitterLogger(
            string categoryName,
            TwitterClient twitter,
            Func<string, LogLevel, bool> filter = null,
            string screenName = null)
        {
            this.categoryName = categoryName;
            this.filter = filter;
            this.twitter = twitter;
            this.screenName = screenName;
        }

        /// <summary>
        /// Return disposable for controlling scope. (Unused)
        /// </summary>
        /// <typeparam name="TState">State indicator type.</typeparam>
        /// <param name="state">State indicator.</param>
        /// <returns>Disposable for controlling scope.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// Determine whether the logger will log this LogLevel.
        /// </summary>
        /// <param name="logLevel">LogLevel to check.</param>
        /// <returns>Whether the logger will log that level.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.filter == null || this.filter(this.categoryName, logLevel);
        }

        /// <summary>
        /// Log a message to Twitter.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="logLevel">Log level of message.</param>
        /// <param name="eventId">Event Id of message.</param>
        /// <param name="state">State of message.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="formatter">Formatter for message.</param>
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

            // Ensure no deadlock due to captured continuation context.
            Task t;
            if (!string.IsNullOrEmpty(this.screenName))
            {
                t = Task.Run(async () => await this.twitter.NewDirectMessage(this.screenName, message).ConfigureAwait(false));
            }
            else
            {
                t = Task.Run(async () => await this.twitter.UpdateStatus(message).ConfigureAwait(false));
            }

            t.Wait();
        }
    }
}
