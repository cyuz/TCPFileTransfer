using NetworkDLL.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Client.ViewModel;
using NetworkDLL;
using ClientLib.Core;

namespace Client
{
    public partial class Form1 : Form
    {
        private IFileClientProxy _client;

        private FolderBrowserDialog _folderBrowserDialog1;

        InputViewModel _InputViewModel { get; }
             = new InputViewModel();

        DisplayViewModel _DisplayViewModel { get; }
             = new DisplayViewModel();


        public Form1()
        {
            InitializeComponent();

            this._folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this._folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;

            BindModel();

            _client = new FileClientProxy();
            _client.OnStart += _client_OnStart;
            _client.OnStop += _client_OnStop;
            _client.OnDownloadFailed += _client_OnDownloadFailed;
            _client.OnDownloadComplete += _client_OnDownloadComplete;
            _client.OnConnectFailed += _client_OnConnectFailed;
            _client.OnConnectSuccess += _client_OnConnectSuccess;

            SetInitlValue();
        }

        private void _client_OnConnectSuccess()
        {
            this.UpdateStatus("connected");
        }

        private void BindModel()
        {
            textBoxIP.DataBindings.Add(nameof(textBoxIP.Text), _InputViewModel, nameof(_InputViewModel.IP));
            textBoxPort.DataBindings.Add(nameof(textBoxPort.Text), _InputViewModel, nameof(_InputViewModel.Port));
            textBoxFileFolder.DataBindings.Add(nameof(textBoxFileFolder.Text), _InputViewModel, nameof(_InputViewModel.FileFolder));
            textBoxFileName.DataBindings.Add(nameof(textBoxFileName.Text), _InputViewModel, nameof(_InputViewModel.FileName));

            textBoxStatus.DataBindings.Add(nameof(textBoxStatus.Text), _DisplayViewModel, nameof(_DisplayViewModel.Status));

            List<Control> controls = new List<Control>() { this.buttonDownlaod, this.textBoxIP, this.textBoxPort, this.textBoxFileName };
            foreach (Control control in controls)
            {
                Binding bind = new Binding(nameof(control.Enabled), _DisplayViewModel, nameof(_DisplayViewModel.Running));
                bind.Format += Reverse;
                bind.Parse += Reverse;

                control.DataBindings.Add(bind);
            }
        }

        private void SetInitlValue()
        {
            _InputViewModel.IP = Consts.DEFAULT_IP;
            _InputViewModel.Port = Consts.DEFAULT_PORT.ToString();
            _InputViewModel.FileFolder = System.IO.Directory.GetCurrentDirectory();
        }

        public void UpdateStatus(string status)
        {
            if (this.textBoxFileFolder.InvokeRequired)
            {
                Action safeWrite = () => { this._DisplayViewModel.Status = status; };
                this.textBoxFileFolder.Invoke(safeWrite);
            }
            else
            {
                this._DisplayViewModel.Status = status;
            }
        }

        public void UpdateRunning(bool running)
        {
            if (this.textBoxFileFolder.InvokeRequired)
            {
                Action safeWrite = () => { this._DisplayViewModel.Running = running; };
                this.textBoxFileFolder.Invoke(safeWrite);
            }
            else
            {
                this._DisplayViewModel.Running = running;
            }
        }

        private void _client_OnConnectFailed(ConnectFailErrorCode errorCode)
        {
            string status = string.Empty;
            switch (errorCode)
            {
                case ConnectFailErrorCode.ARGUMENT_ERROR:
                    status = "Argument Error";
                    break;
                case ConnectFailErrorCode.SOCKET_ERROR:
                    status = "Connection Error";
                    break;
                case ConnectFailErrorCode.CONNECTION_REFUSE:
                    status = "Refuse conenction";
                    break;
                case ConnectFailErrorCode.HOST_NOT_FOUND:
                    status = "Host not found";
                    break;
                default:
                    status = "Unknown Error";
                    break;
            }
            this.UpdateStatus(status);
            this.UpdateRunning(false);
        }

        private void _client_OnDownloadComplete(string fileName)
        {
            this.UpdateStatus($"Download {fileName} Complete");
        }

        private void _client_OnDownloadFailed(DownloadErrorCode errorCode, string desc)
        {
            string status = string.Empty;
            switch (errorCode)
            {
                case DownloadErrorCode.FILE_NOT_FOUND:
                    status = $"File {desc} not found";
                    break;
                case DownloadErrorCode.CANNOT_READ:
                    status = $"Cannot read file {desc}";
                    break;
                case DownloadErrorCode.CANNOT_WRITE:
                    status = $"Cannot write file {desc}";
                    break;
                case DownloadErrorCode.COMMUNICATE_ERROR:
                    status = "Communation error";
                    break;
                case DownloadErrorCode.DONWLOAD_ERROR:
                    status = "Error occuring during downloading";
                    break;
                case DownloadErrorCode.UNKNONW_ERRO:
                default:
                    status = "unknown error";
                    break;
            }

            this.UpdateStatus(status);
        }

        private void _client_OnStop()
        {
            this.UpdateRunning(false);
        }

        private void _client_OnStart()
        {
            this.UpdateRunning(true);
        }

        public static void Reverse(object sender, ConvertEventArgs e)
        {
            e.Value = !((bool)e.Value);
        }

        private void buttonDownlaod_Click(object sender, EventArgs e)
        {
            if(this._DisplayViewModel.Running)
            {
                return;
            }

            this.UpdateStatus(string.Empty);

            if(string.IsNullOrEmpty(_InputViewModel.FileName))
            {
                MessageBox.Show("Please input FileName");
                return;
            }

            if (string.IsNullOrEmpty(_InputViewModel.IP))
            {
                MessageBox.Show("Please input IP");
                return;
            }

            if (string.IsNullOrEmpty(_InputViewModel.FileFolder))
            {
                MessageBox.Show("Please select folder");
                return;                
            }

            int port = 0;

            if (!int.TryParse(this._InputViewModel.Port, out port) || port > 65565 || port < 0)
            {
                MessageBox.Show("Please input port between 1~65535");
                return;
            }

            try
            {
                if(!Directory.Exists(_InputViewModel.FileFolder))
                {
                    Directory.CreateDirectory(_InputViewModel.FileFolder);
                }

                _client.RequestFile(_InputViewModel.IP, port, _InputViewModel.FileFolder, _InputViewModel.FileName);
            }
            catch (Exception)
            {
                this.UpdateStatus("Unknown Error");
                this.UpdateRunning(false);
                return;
            }
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            var result = this._folderBrowserDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                this._InputViewModel.FileFolder = this._folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
