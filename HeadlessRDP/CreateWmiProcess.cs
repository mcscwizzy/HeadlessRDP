using System;
using System.Management;
using System.IO;
using System.Threading;

/// <summary>
/// Summary description for CreateWmiProcess
/// </summary>
public class CreateWmiProcess
{
    //username and password for wmi connection
    public string Username { get; set; }
    public string Password { get; set; }
    public string Domain { get; set; }
    public string ComputerName { get; set; }
    public string WmiTimeout { get; set; }

    //method to invoke command line on remote machine
    public void InvokeCommand(string command)
    {
        try
        {
            //connection options
            var connOptions = new System.Management.ConnectionOptions();
            connOptions.Authentication = AuthenticationLevel.PacketPrivacy;
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.EnablePrivileges = true;
            connOptions.Username = Domain + "\\" + Username;
            connOptions.Password = Password;

            //management scope
            var manScope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", ComputerName), connOptions);
            manScope.Connect();
            ObjectGetOptions objectGetOptions = new ObjectGetOptions();
            objectGetOptions.Timeout = TimeSpan.FromSeconds(20);
            ManagementPath managementPath = new ManagementPath("Win32_Process");
            ManagementClass processClass = new ManagementClass(manScope, managementPath, objectGetOptions);
            ManagementBaseObject inParams = processClass.GetMethodParameters("Create");

            //script to execute from passed parameter        
            inParams["CommandLine"] = command;

            //execute script
            ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);

            //loop through processes until process has quit
            SelectQuery CheckProcess = new SelectQuery("Select * from Win32_Process Where ProcessId = " + outParams["processId"]);

            //counter for loop
            int counter = 0;
            int loopcounter = Convert.ToInt32(WmiTimeout) / 10;

            //loop until process has died
            while (counter != (loopcounter + 1))
            {
                // create searcher object
                ManagementObjectSearcher ProcessSearcher = new ManagementObjectSearcher(manScope, CheckProcess);
                ProcessSearcher.Options.Timeout = TimeSpan.FromSeconds(20);

                // if it is not null there is a process running
                if (ProcessSearcher.Get().Count != 0)
                {
                    if (counter == loopcounter)
                    {
                        // kill process
                        inParams["CommandLine"] = "cmd /c \"taskkill /f /pid " + outParams["processId"].ToString() + " /t \"";
                        processClass.InvokeMethod("Create", inParams, null);

                        // throw new exception for process timeout
                        throw new Exception("WMI process timeout");
                    }
                    else
                    {
                        counter++;
                        Thread.Sleep(10000);
                    }
                }
                // if it is null then process is not running and break from loop
                else if (ProcessSearcher.Get().Count == 0)
                {
                    break;
                }
                
            }

        }
        catch (Exception e)
        {
            //catch exception
            throw new Exception(e.Message.ToString());
        }
    }
}
