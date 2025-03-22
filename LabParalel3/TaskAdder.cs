using System;
using System.Collections.Generic;
using System.Threading;

namespace LabParalel3
{
    public class TaskAdder
    {
        private bool _isContinueAdding = true;
        private int _taskCounter = 0;
        private int _threadsAmount;
        private int _adderSleepTimeInSeconds;
        private float _minTaskTime;
        private float _maxTaskTime;
        private List<Thread> _threadsList = new();
        private object _locker = new();
        private ThreadPool _threadPool;
        public bool IsContinueAdding => _isContinueAdding;
        public TaskAdder(ThreadPool threadPool, int minTaskTime = 6, int maxTaskTime = 12, int threadsAmount = 2, int adderSleepTimeInSeconds = 3)
        {
            _minTaskTime = minTaskTime;
            _maxTaskTime = maxTaskTime;
            _threadsAmount = threadsAmount;
            _adderSleepTimeInSeconds = adderSleepTimeInSeconds;
            _threadPool = threadPool;
        }
        public void InitiateThreads()
        {
            if (_threadsAmount <= 0)
                throw new ArgumentException();
            for (int i = 0; i < _threadsAmount; i++)
                _threadsList.Add(new Thread(() =>
                {
                    while (_isContinueAdding)
                        AddTask(CreateTask());

                }));

        }
        public void StartAdding()
        {
            if (_threadsList.Count == 0)
                throw new Exception("Threads list is empty");
            foreach (var thread in _threadsList)
                thread.Start();

        }
        public void StopAdding()
        {
            lock (_locker)
            {
                _isContinueAdding = false;
                Monitor.PulseAll(_locker);
            }
            foreach (var thread in _threadsList)
                thread.Join();

        }
        private MyTask CreateTask()
        {
            lock (_locker)
            {
                Random random = new();
                _taskCounter++;
                return new MyTask(_taskCounter, _minTaskTime + random.NextDouble() * (_maxTaskTime - _minTaskTime));
            }
        }
        private void AddTask(MyTask task)
        {
            Thread.Sleep(_adderSleepTimeInSeconds * 1000);
            int queueNumber = new Random().Next(0,

            _threadPool.QueueListCount);

            _threadPool.AddTask(queueNumber, task.Id, () =>
            {
                Thread.Sleep((int)(task.SleepTime * 1000));
                Console.WriteLine($"Task {task.Id} has been processed");
            });
        }
    }
}
