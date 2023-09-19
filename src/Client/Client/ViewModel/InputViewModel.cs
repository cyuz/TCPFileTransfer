using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public class InputViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
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

        public string FilePath
        {
            get {
                return Path.Combine(this._FileFolder, this._FileName);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
