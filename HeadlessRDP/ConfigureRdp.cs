using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMSTSCLib;
using Ookii.CommandLine;

namespace HeadlessRDP
{
    class ConfigureRdp
    {
        // Returns error code from function back to console
        private int LogonErrorCode { get; set; }
        public bool IsBackground { get; private set; }
        public string Server { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        // Create RDP connectin with specified parameters passed from function 
        public void CreateRdpConnection()
        {
            // Creats new form with RDP connection
            void ProcessTaskThread()
            {
                // Create new form
                var form = new Form();
                form.Load += (sender, args) =>
                {
                    // Set RDP connection parameters
                    var rdpConnection = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
                    form.Controls.Add(rdpConnection);
                    rdpConnection.Server = Server;
                    rdpConnection.Domain = Domain;
                    rdpConnection.UserName = UserName;
                    // Quotes are added in case there are contained quotes within the string
                    // These quotes are then stripped when making the connection 
                    rdpConnection.AdvancedSettings9.ClearTextPassword = Password;                                                                                                                                                            
                    rdpConnection.AdvancedSettings9.EnableCredSspSupport = true;
                    rdpConnection.Width = 1920;
                    rdpConnection.Height = 1080;

                    // If an error occurs then report error to the logon error code variable
                    // and do associate function handler
                    if (true)
                    {
                        rdpConnection.OnDisconnected += RdpConnectionOnOnDisconnected;
                        rdpConnection.OnLoginComplete += RdpConnectionOnOnLoginComplete;
                        rdpConnection.OnLogonError += RdpConnectionOnOnLogonError;
                    }

                    // connect
                    rdpConnection.Connect();
                    rdpConnection.Enabled = false;
                    rdpConnection.Dock = DockStyle.Fill;
                    Application.Run(form);
                };

                // show form
                form.Show();
            }


            // Create new thread after connection is created
            var rdpClientThread = new Thread(ProcessTaskThread) { IsBackground = true };
            rdpClientThread.SetApartmentState(ApartmentState.STA);
            rdpClientThread.Start();

            // end thread after connection is created
            while (rdpClientThread.IsAlive)
            {
                Task.Delay(1000).GetAwaiter().GetResult();
            }
            
            
        }

        // event handler for logon error
        private void RdpConnectionOnOnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            LogonErrorCode = e.lError;
        }

        // event handler on logon complete
        private void RdpConnectionOnOnLoginComplete(object sender, EventArgs e)
        {
            if (LogonErrorCode == -2)
            {
                Debug.WriteLine($"    ## New Session Detected ##");
                Task.Delay(10000).GetAwaiter().GetResult();
            }
            var rdpSession = (AxMsRdpClient9NotSafeForScripting)sender;
            rdpSession.Disconnect();            
        }

        // on disconnect event handler
        private void RdpConnectionOnOnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            CreateWmiProcess wmi = new CreateWmiProcess();
            wmi.Username = UserName;
            wmi.Domain = Domain;
            wmi.Password = Password;
            wmi.WmiTimeout = "60";
            wmi.ComputerName = Server;
            wmi.InvokeCommand("cmd /c gpupdate /force /wait:0");
            Environment.Exit(0);                        
        }
    }
}
