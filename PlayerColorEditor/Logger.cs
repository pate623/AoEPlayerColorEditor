using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace PlayerColorEditor
{
    internal class Logger(Type loggerType)
    {
        private readonly ILogger NlogLogger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger(loggerType.ToString());

#pragma warning disable CA2254 // This program doesn't use structured logging
        public void Trace(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                NlogLogger.LogTrace(message);
            }
            else
            {
                NlogLogger.LogTrace(message, ex);
            }
            VisualStudioLog(message, ex);
        }

        public void Debug(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                NlogLogger.LogDebug(message);
            }
            else
            {
                NlogLogger.LogDebug(message, ex);
            }
            VisualStudioLog(message, ex);
        }

        public void Info(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                NlogLogger.LogInformation(message);
            }
            else
            {
                NlogLogger.LogInformation(message, ex);
            }
            VisualStudioLog(message, ex);
        }

        public void Warn(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                NlogLogger.LogWarning(message);
            }
            else
            {
                NlogLogger.LogWarning(message, ex);
            }
            VisualStudioLog(message, ex);
        }

        public void Error(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                NlogLogger.LogError(message);
            }
            else
            {
                NlogLogger.LogError(message, ex);
            }
            VisualStudioLog(message, ex);
        }

        public void Fatal(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                NlogLogger.LogCritical(message);
            }
            else
            {
                NlogLogger.LogCritical(message, ex);
            }
            VisualStudioLog(message, ex);
        }
#pragma warning restore CA2254

        /// <summary>WPF applications don't have normal console output, so have to use this for Visual Studio debugging.</summary>
        private static void VisualStudioLog(string message, Exception? ex = null)
        {
            System.Diagnostics.Debug.WriteLine($"{message} {(ex == null ? "" : ex.ToString())}");
        }
    }
}
