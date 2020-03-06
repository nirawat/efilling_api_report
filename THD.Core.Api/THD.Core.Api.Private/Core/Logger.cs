using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THD.Core.Api.Private
{
    public class Logger : ILogger
    {
        private readonly NLog.Logger _logger = null;

        private Logger()
        {
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        private static class NLogLoggerHolder
        {
            public readonly static ILogger LoggerHolder = new Logger();
        }

        public static ILogger Instance
        {
            get
            {
                return NLogLoggerHolder.LoggerHolder;
            }
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception ex)
        {
            _logger.Error(ex);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }
    }
}
