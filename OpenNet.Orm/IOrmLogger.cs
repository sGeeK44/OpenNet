using System;

namespace OpenNet.Orm
{
    public interface IOrmLogger
    {
        /// <summary>
        /// Write info log level in log file
        /// </summary>
        /// <param name="comment">Log to put</param>
        void Trace(string comment);

        /// <summary>
        /// Write info log level in log file
        /// </summary>
        /// <param name="comment">Log to put</param>
        void Info(string comment);

        /// <summary>
        /// Write warning log level in log file
        /// </summary>
        /// <param name="comment">Log to put</param>
        void Warning(string comment);

        /// <summary>
        /// Write warning log level in log file
        /// </summary>
        /// <param name="exception">Exception to log</param>
        void Warning(Exception exception);

        /// <summary>
        /// Write warning log level in log file
        /// </summary>
        /// <param name="comment">Log to put</param>
        /// <param name="exception">Exception to log</param>
        void Warning(string comment, Exception exception);

        /// <summary>
        /// Write error log level in log file
        /// </summary>
        /// <param name="comment">Log to put</param>
        void Error(string comment);

        /// <summary>
        /// Write error log level in log file
        /// </summary>
        /// <param name="exception">Exception to log</param>
        void Error(Exception exception);

        /// <summary>
        /// Write error log level in log file
        /// </summary>
        /// <param name="comment">Log to put</param>
        /// <param name="exception">Exception to log</param>
        void Error(string comment, Exception exception);

        /// <summary>
        /// Write fatal log level in log file
        /// </summary>
        /// <param name="exception">Exception to log</param>
        void Fatal(Exception exception);
    }
}
