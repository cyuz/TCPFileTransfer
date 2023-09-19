using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModel
{
    class DisplayViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Running
        {
            get => _Running;
            set
            {
                if (_Running != value)
                {
                    _Running = value;
                    NotifyPropertyChanged();
                }
            }
        }
        bool _Running;

        public string Status
        {
            get => _Status;
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string _Status;

        public string FileName
        {
            get => _FileName;
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string _FileName;
        public string IP
        {
            get => _IP;
            set
            {
                if (_IP != value)
                {
                    _IP = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string _IP;
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
