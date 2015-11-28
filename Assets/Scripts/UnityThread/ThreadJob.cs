using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UnityThread
{
    public abstract class ThreadJob
    {
        public delegate void ThreadHandler(ThreadJob threadJob);
        public event ThreadHandler OnThreadComplete = delegate { };
        public event ThreadHandler OnThreadAbort = delegate { };
        private bool _isRunning = false;

        public Thread _thread;

        public virtual void Start()
        {
            _thread = new Thread(ThreadFunction);
            _thread.Start();
            _isRunning = true;
        }

        public virtual void Abort()
        {
            _thread.Abort();
            OnThreadAbort(this);
            _isRunning = false;
        }

        protected virtual void ThreadFunction()
        {
            if (_isRunning)
            {
                OnThreadComplete(this);
                _isRunning = false;
            }
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public void ResetEventSubscriptions()
        {
            OnThreadComplete = delegate { };
            OnThreadAbort = delegate { };
        }
    }
}
