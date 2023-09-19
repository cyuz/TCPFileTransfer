using NetworkDLL;
using ServerLib.Core;
using Server.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        private FolderBrowserDialog _folderBrowserDialog1;

        private IFileServerProxy _server;

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

            _server = new FileServerProxy();
            _server.OnReceiveRequest += _server_OnReceiveRequest;
            _server.OnStart += _server_OnStart;
            _server.OnStop += _server_OnStop;
            _server.OnStartFailed += _server_OnStartFailed;

            SetInitlValue();
        }

        private void BindModel()
        {
            textBoxIP.DataBindings.Add(nameof(textBoxIP.Text), _DisplayViewModel, nameof(_DisplayViewModel.IP));
            textBoxPort.DataBindings.Add(nameof(textBoxPort.Text), _DisplayViewModel, nameof(_DisplayViewModel.Port));
            textBoxFileName.DataBindings.Add(nameof(textBoxFileName.Text), _DisplayViewModel, nameof(_DisplayViewModel.FileName));

            textBoxLocalPort.DataBindings.Add(nameof(textBoxLocalPort.Text), _InputViewModel, nameof(_InputViewModel.Port));
            textBoxFileFolder.DataBindings.Add(nameof(textBoxFileFolder.Text), _InputViewModel, nameof(_InputViewModel.FileFolder));

            List<Control> controls = new List<Control>() { this.textBoxLocalPort, this.buttonStart };
            foreach (Control control in controls)
            {
                Binding bind = new Binding(nameof(control.Enabled), _DisplayViewModel, nameof(_DisplayViewModel.Running));
                bind.Format += Reverse;
                bind.Parse += Reverse;

                control.DataBindings.Add(bind);
            }

            controls = new List<Control>() { this.buttonStop };
            foreach (Control control in controls)
            {
                Binding bind = new Binding(nameof(control.Enabled), _DisplayViewModel, nameof(_DisplayViewModel.Running));

                control.DataBindings.Add(bind);
            }
        }

        private void SetInitlValue()
        {
            this._InputViewModel.Port = Consts.DEFAULT_PORT.ToString();
            _InputViewModel.FileFolder = System.IO.Directory.GetCurrentDirectory();
        }

        private void UpdateRequestInfo(string ip, int port, string fileName)
        {
            if (this.textBoxIP.InvokeRequired)
            {
                Action safeWrite = () => {
                    this._DisplayViewModel.IP = ip;
                    this._DisplayViewModel.Port = port.ToString();
                    this._DisplayViewModel.FileName = fileName;
                };
                this.textBoxIP.Invoke(safeWrite);
            }
            else
            {
                this._DisplayViewModel.IP = ip;
                this._DisplayViewModel.Port = port.ToString();
                this._DisplayViewModel.FileName = fileName;
            }
        }

        private void UpdateRunning(bool running)
        {
            if (this.textBoxIP.InvokeRequired)
            {
                Action safeWrite = () => {
                    this._DisplayViewModel.Running = running;
                };
                this.textBoxIP.Invoke(safeWrite);
            }
            else
            {
                this._DisplayViewModel.Running = running;
            }
        }

        private void _server_OnStop()
        {
            this.UpdateRunning(false);
        }

        private void _server_OnStart()
        {
            this.UpdateRunning(true);
        }

        private void _server_OnStartFailed(StartFailedCode errorCode)
        {
            string errorReason = string.Empty;
            switch(errorCode)
            {
                case StartFailedCode.PORT_OCCUPIED:
                    errorReason = "port occupied";
                    break;
                case StartFailedCode.SOCKET_ERROR:
                    errorReason = "socket error";
                    break;
                case StartFailedCode.UNKNONW_REASON:
                    errorReason = "unknown error";
                    break;
            }
            this.UpdateRunning(false);

            MessageBox.Show($"Start Failed:{errorReason}");
        }

        private void _server_OnReceiveRequest(string ip, int port, string fileName, DateTime datetime, bool success)
        {
            this.UpdateRequestInfo(ip, port, fileName);
        }

        public static void Reverse(object sender, ConvertEventArgs e)
        {
            e.Value = !((bool)e.Value);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_InputViewModel.FileFolder))
            {
                MessageBox.Show("Please select folder");
                return;
            }

            int port = 0;
            
            if(!int.TryParse(this._InputViewModel.Port, out port) || port > 65565 || port < 0)
            {
                MessageBox.Show("Please input port between 1~65535");
                return;
            }

            this._server.Start(port, this._InputViewModel.FileFolder);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this._server.Stop();
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            var result = this._folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this._InputViewModel.FileFolder = this._folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
