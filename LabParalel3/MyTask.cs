using System;

namespace LabParalel3
{
    struct MyTask
    {
        private double _sleepTime;
        private int _id;
        public int Id => _id;
        public double SleepTime => _sleepTime;
        public MyTask(int id, double sleepTime)
        {
            _id = id;
            _sleepTime = sleepTime;
        }
    }
}
