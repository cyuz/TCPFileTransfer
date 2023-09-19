using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModel
{
    class InputViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FileFolder
        {
            get => _FileFolder;
            set
            {
                if (_FileFolder != value)
                {
                    _FileFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string _FileFolder;

        public string Port
        {
            get => _Port;
            set
            {
                if (_Port != value)
                {
                    _Port = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string _Port;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
