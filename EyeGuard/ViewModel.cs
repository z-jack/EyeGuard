using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EyeGuard
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _timer = 50;
        private bool _lockTimer = true;
        private bool _deamon = true;
        private bool _startup = true;

        public string timer
        {
            get { return _timer.ToString(); }
            set
            {
                int tmp;
                if (!((!int.TryParse(value, out tmp)) || tmp < 1 || tmp > 300))
                {
                    _timer = tmp;
                }
            }
        }

        public bool lockTimer
        {
            get { return _lockTimer; }
            set { _lockTimer = (bool)value; }
        }

        public bool deamon
        {
            get { return _deamon; }
            set { _deamon = (bool)value; }
        }

        public bool startup
        {
            get { return _startup; }
            set { _startup = (bool)value; }
        }

        public bool Set(string name, Object data)
        {
            if (this.GetType().GetProperty(name) == null)
            {
                return false;
            }
            try
            {
                this.GetType().GetProperty(name).SetValue(this, data, null);
                RaisePropertyChangedEvent(name);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Apply(Core tmr)
        {
            try
            {
                if(Settings.Default.timer != timer)
                {
                    tmr.SetInterval(int.Parse(timer) * 60 * 1000);
                }
                foreach (string name in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(field => field.Name).ToList())
                {
                    Settings.Default[name] = this.GetType().GetProperty(name).GetValue(this, null);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Restore()
        {
            try
            {
                foreach (string name in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(field => field.Name).ToList())
                {
                    this.Set(name, Settings.Default[name]);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
