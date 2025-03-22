using LabParalel3;

namespace Program
{
    class Program
    {
        static void Main()
        {
            var threadPool = new LabParalel3.ThreadPool();
            var taskAdder = new TaskAdder(threadPool, 6, 12);
            taskAdder.InitiateThreads();
            threadPool.InitiateThreads();

            Console.WriteLine("=== Task Manager ===");
            Console.WriteLine("1 - Start adding tasks");
            Console.WriteLine("2 - Start executing tasks");
            Console.WriteLine("3 - Pause executing tasks");
            Console.WriteLine("4 - Restart executing tasks");
            Console.WriteLine("5 - Stop executing tasks");
            Console.WriteLine("6 - Stop adding tasks");
            Console.WriteLine("0 - Exit");

            while (threadPool.IsRunningApp || taskAdder.IsContinueAdding)
            {
                Console.Write("\n");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int result))
                {
                    switch (result)
                    {
                        case 1:
                            Console.WriteLine("[INFO] Starting to add tasks...");
                            taskAdder.StartAdding();
                            break;
                        case 2:
                            Console.WriteLine("[INFO] Starting to execute tasks...");
                            threadPool.StartExecuting();
                            break;
                        case 3:
                            Console.WriteLine("[INFO] Pausing task execution...");
                            threadPool.PauseExecuting();
                            break;
                        case 4:
                            Console.WriteLine("[INFO] Restarting task execution...");
                            threadPool.RestartExecuting();
                            break;
                        case 5:
                            Console.WriteLine("[INFO] Stopping task execution...");
                            threadPool.StopExecuting();
                            break;
                        case 6:
                            Console.WriteLine("[INFO] Stopping task adding...");
                            taskAdder.StopAdding();
                            break;
                        case 0:
                            Console.WriteLine("[INFO] Exiting program...");
                            taskAdder.StopAdding();
                            threadPool.StopExecuting();
                            return;
                        default:
                            Console.WriteLine("[ERROR] Invalid command. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("[ERROR] Invalid input. Please enter a number.");
                }
            }

            Console.WriteLine("[INFO] Program exited successfully.");
        }
    }
}