using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    public class Deferral
    {
        private readonly Action _onComplete;

        public Deferral(Action onComplete)
        {
            _onComplete = onComplete;
        }

        public void SetComplete()
        {
            _onComplete();
        }
    }
}
