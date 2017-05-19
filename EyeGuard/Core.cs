using System;
using System.Timers;

namespace EyeGuard
{
    class Core
    {
        private Timer timer = null;
        public delegate void Inter(object sender, EventArgs e);
        public Inter cbk = (object sender,EventArgs e)=> { };

        public Core()
        {
            timer = new Timer();
            timer.Elapsed += Handler;
        }

        public void SetInterval(int data)
        {
            timer.Stop();
            timer.Interval = data;
            timer.Start();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Restart()
        {
            timer.Stop();
            timer.Start();
        }

        private void Handler(object sender, EventArgs e)
        {
            cbk(sender, e);
        }
    }
}
