using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LabParalel3;

namespace LabParalel3
{
    public class ThreadPool
    {
        private List<BlockingQueue<Action>> _queueList = new();
        private List<Thread> _threadsList = new();

        private int _threadsPerQueue;
        private int _amountOfQueue;

        private bool _isContinueExecuting = true;
        private bool _isRunningApp = true;

        private object _executeLocker = new();
        private object _addLocker = new();
        private object _appLocker = new();

        public int QueueListCount => _queueList.Count;
        public bool IsRunningApp => _isRunningApp;

        // Лічильники для статистики
        public int ExecutingTaskCounter { get; private set; }
        public int WaitTaskCounter { get; private set; }
        public float AverageExecutingTime { get; private set; }
        public float AverageWaitingTime { get; private set; }

        public List<BlockingQueue<Action>> QueueList => _queueList;

        public ThreadPool(int amountOfQueue = 3, int threadsPerQueue = 2)
        {
            _threadsPerQueue = threadsPerQueue;
            _amountOfQueue = amountOfQueue;

            if (amountOfQueue <= 0 || threadsPerQueue <= 0)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < amountOfQueue; i++)
                _queueList.Add(new BlockingQueue<Action>());
        }

        private void ExecuteTask(int queueNumber)
        {
            while (_isRunningApp)
            {
                Action? task = null;
                DateTime? taskEnqueueTime = null;

                if (queueNumber >= _queueList.Count)
                    throw new Exception("Queue Number was out of range");

                lock (_executeLocker)
                {
                    while (_queueList[queueNumber].Count == 0 && _isContinueExecuting)
                    {
                        try
                        {
                            Monitor.Wait(_executeLocker);
                        }
                        catch
                        {
                            throw new ThreadInterruptedException();
                        }
                    }

                    if (_isContinueExecuting)
                    {
                        taskEnqueueTime = DateTime.Now;
                        Monitor.Pulse(_executeLocker);
                        task = _queueList[queueNumber].Dequeue();
                    }
                }

                if (taskEnqueueTime != null)
                {
                    DateTime taskStartTime = DateTime.Now;
                    task?.Invoke();
                    TimeSpan taskExecutionTime = DateTime.Now - taskStartTime;

                    lock (_executeLocker)
                    {
                        ExecutingTaskCounter++;
                        AverageExecutingTime = ((AverageExecutingTime * (ExecutingTaskCounter - 1)) + (float)taskExecutionTime.TotalMilliseconds) / ExecutingTaskCounter;

                        if (taskEnqueueTime != null)
                        {
                            TimeSpan waitTime = DateTime.Now - taskEnqueueTime.Value;

                            WaitTaskCounter++;
                            AverageWaitingTime = ((AverageWaitingTime * (WaitTaskCounter - 1)) + (float)waitTime.TotalMilliseconds) / WaitTaskCounter;
                        }
                    }
                }

                if (_isRunningApp && !_isContinueExecuting)
                {
                    lock (_appLocker)
                    {
                        Console.WriteLine("Some thread paused");
                        Monitor.Wait(_appLocker);
                    }
                }
            }

            Console.WriteLine("Some thread stopped working");
        }

        public void InitiateThreads()
        {
            if (_amountOfQueue <= 0 || _threadsPerQueue <= 0)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < _amountOfQueue; i++)
            {
                int k = i;
                for (int j = 0; j < _threadsPerQueue; j++)
                    _threadsList.Add(new Thread(() => ExecuteTask(k)));
            }
        }

        public void AddTask(int queueNumber, int taskNumber, Action func)
        {
            lock (_executeLocker)
            {
                Console.WriteLine($"Add task {taskNumber} in {queueNumber + 1} queue");
                _queueList[queueNumber].Enqueue(func);
                Monitor.Pulse(_executeLocker);
            }
        }

        public void StartExecuting()
        {
            if (_threadsList.Count == 0)
                throw new Exception("Threads list is empty");

            foreach (var thread in _threadsList)
                thread.Start();
        }

        public void PauseExecuting()
        {
            lock (_executeLocker)
            {
                _isContinueExecuting = false;
                Monitor.PulseAll(_executeLocker);
            }
        }

        public void RestartExecuting()
        {
            lock (_appLocker)
            {
                _isContinueExecuting = true;
                Monitor.PulseAll(_appLocker);
            }
        }

        public void StopExecuting()
        {
            lock (_executeLocker)
            {
                lock (_appLocker)
                {
                    _isRunningApp = false;
                    _isContinueExecuting = false;
                    Monitor.PulseAll(_executeLocker);
                    Monitor.PulseAll(_appLocker);
                }
            }

            foreach (var thread in _threadsList)
                thread.Join();

            Console.WriteLine("All threads were stopped");
        }
    }

}
