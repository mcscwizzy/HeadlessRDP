using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AxMSTSCLib;
using MSTSCLib;
using Ookii.CommandLine;

namespace HeadlessRDP
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read switches from command line
            CommandLineParser parser = new CommandLineParser(typeof(SwitchParameters));
            SwitchParameters sp = (SwitchParameters)parser.Parse(args);

            // Verify required fields are not left blank
            if(sp.Server == null || sp.UserName == null || sp.Domain == null ||  sp.Password == null)
            {
                Console.Write("CLI Usage -- HeadlessRDP.exe -Server xxxx -UserName xxxx -Domain xxxx -Password xxxx");
                Console.ReadLine();
            }
            else
            {
                // Before we can initiate remote connection we need to remove the legal notice
                CreateWmiProcess wmi = new CreateWmiProcess();
                wmi.Username = sp.UserName;
                wmi.Domain = sp.Domain;
                wmi.Password = sp.Password;
                wmi.WmiTimeout = "60";
                wmi.ComputerName = sp.Server;
                wmi.InvokeCommand("C:\\Windows\\System32\\WindowsPowershell\\v1.0\\Powershell.exe -ExecutionPolicy Bypass -NoProfile -Command \"& {Remove-ItemProperty -Path \"HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System -Name LegalNoticeCaption, LegalNoticeText -Force}\"");

                // Now that legal notice is remove we can create remote connection
                var rdp = new ConfigureRdp();
                rdp.UserName = sp.UserName;
                rdp.Password = sp.Password;
                rdp.Domain = sp.Domain;
                rdp.Server = sp.Server;
                rdp.CreateRdpConnection();

                // destroy object
                wmi = null;
                rdp = null;
            }

        }
    }
}
