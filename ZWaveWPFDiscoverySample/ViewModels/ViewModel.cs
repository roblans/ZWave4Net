using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWaveWPFDiscoverySample.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }

    }
}
