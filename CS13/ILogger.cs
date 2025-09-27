using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS13
{
    internal interface ILogger
    {
        void Log(string message);
    }

    internal class ConsoleLogger: ILogger
    {
        private readonly bool canLog;

        public ConsoleLogger(bool canLog)
        {
            this.canLog = canLog;
        }
        public void Log(string message)
        {
            if (canLog)
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        }
    }
}
