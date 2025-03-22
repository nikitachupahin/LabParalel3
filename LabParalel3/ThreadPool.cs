using System;
using System.Collections.Generic;
using System.Threading;

namespace LabParalel3
{
   public class ThreadPool
    {
        private List<BlockingQueue<Action>> _queueList = new();
        private List<Thread> _threadsList = new();
        private int _threadsPerQueue;
        private int _amountOfQueue;

        public ThreadPool(int amountOfQueue = 3, int threadsPerQueue = 2)
        {
            _threadsPerQueue = threadsPerQueue;
            _amountOfQueue = amountOfQueue;

            if (amountOfQueue <= 0 || threadsPerQueue <= 0)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < amountOfQueue; i++)
                _queueList.Add(new BlockingQueue<Action>());
        }
    }

}
