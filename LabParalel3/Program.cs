
using System.Diagnostics;

namespace LabParalel3
{
    class Program
    {
        static void Main()
        {
            var threadPool = new ThreadPool();
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
            Console.WriteLine("7 - Run test");
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
                        case 7:
                            Console.Write("Enter sleep time in milliseconds: ");
                            if (int.TryParse(Console.ReadLine(), out int sleepTime) && sleepTime > 0)
                            {
                                Test(sleepTime);
                            }
                            else
                            {
                                Console.WriteLine("[ERROR] Invalid sleep time. Please enter a positive number.");
                            }
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
        public static void Test(int sleepTime)
        {
            float averageQueueLength = 0;
            Stopwatch timer = new();
            ThreadPool threadPool = new(3, 2);
            TaskAdder taskAdder = new(threadPool, 6, 12, 2, 3);

            taskAdder.InitiateThreads();
            threadPool.InitiateThreads();

            Console.WriteLine("\n[TEST] Starting test...");
            timer.Start();

            taskAdder.StartAdding();
            threadPool.StartExecuting();

            Thread.Sleep(sleepTime); 

            taskAdder.StopAdding();
            threadPool.StopExecuting();

            timer.Stop();

            foreach (var queue in threadPool.QueueList)
            {
                averageQueueLength += queue.Count;
            }

            averageQueueLength /= threadPool.QueueListCount;

            Console.WriteLine("\n=== Test Results ===");
            Console.WriteLine($"Total working time:          {timer.ElapsedMilliseconds} ms");
            Console.WriteLine($"Average queue length:        {averageQueueLength}");
            Console.WriteLine($"Average task execution time: {threadPool.AverageExecutingTime} ms");
            Console.WriteLine($"Executed task count:         {threadPool.ExecutingTaskCounter}");
            Console.WriteLine($"Average task wait time:      {threadPool.AverageWaitingTime} ms");
            Console.WriteLine($"Waited task count:           {threadPool.WaitTaskCounter}");
        }
    }
}