using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THD.Core.Api.Private
{
    public interface ILogger
    {
        void Info(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception ex);
    }
}
