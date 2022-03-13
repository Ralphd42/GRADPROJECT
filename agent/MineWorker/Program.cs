using System;

namespace MineWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            JobListner jl = new JobListner();
            jl.ListenToController();
            
            if (false)
            {
                MineOperations mi = new MineOperations();
                mi.joinNetwork("AAA");
            }
        }
    }
}
