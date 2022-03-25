using System;

namespace MineWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger lg = new Logger();
            try
            {
                MineOperations mi = new MineOperations(lg);
                bool joined = mi.joinNetwork(args[0]);
                if (joined)
                {
                    lg.LogMessage("Joined Nwetwork");
                    JobListner jl = new JobListner();
                    jl.ListenToController();
                }
                else
                {
                    lg.LogMessage("Failed to join NETWORK");
                }
            }catch (Exception exp)
            {
                lg.LogError(exp, "ERROR in agent");


            }
        }
    }
}
