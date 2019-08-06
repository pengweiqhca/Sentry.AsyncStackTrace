using System;

namespace Sentry.AsyncStackTrace
{
    public static class SentryOptionsExtensions
    {
        public static SentryOptions UseAsyncStackTraceFactory(this SentryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return options.UseStackTraceFactory(new AsyncStackTraceFactory(options));
        }
    }
}
