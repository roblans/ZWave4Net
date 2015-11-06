using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWaveWPFDiscoverySample.ViewModels
{
    public class NodeValue : ViewModel
    {

        public string Type { get; private set; }
        public object Value { get; private set; }

        public NodeValue(string type, object value)
        {
            Type = type;
            SetValue(value);
        }

        public void SetValue(object value)
        {
            Value = value;
            Notify(nameof(Value));
        }

    }
}
