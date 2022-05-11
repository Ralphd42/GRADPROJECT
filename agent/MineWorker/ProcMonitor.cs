using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MineWorker
{

    public class ProcMonitor
    {
        public static async Task<double> GetCpuUsage()
        {
            DateTime Start = DateTime.UtcNow;
            var startUsage =Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);
            DateTime end = DateTime.UtcNow;
            var enduseage = Process.GetCurrentProcess().TotalProcessorTime;
            var used = enduseage -startUsage;
            double usedms = used.TotalMilliseconds;
            var timeelap = end -Start;
            double timeElapD = timeelap.TotalMilliseconds;
            double CPUUsage = usedms/(  Environment.ProcessorCount*timeElapD *100  );
            return CPUUsage;
        }

    }
}