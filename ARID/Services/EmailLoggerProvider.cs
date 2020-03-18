using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ARID.Services
{
    public class EmailLoggerProvider : IEmailLoggerProvider
    {
        private readonly IEmailLogger _emailLogger;

        public EmailLoggerProvider(IEmailLogger emailLogger)
        {
            _emailLogger = emailLogger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _emailLogger;
        }

        public void Dispose()
        {
        }

        ILogger ILoggerProvider.CreateLogger(string categoryName)
        {
            throw new NotImplementedException();
        }
    }
}
