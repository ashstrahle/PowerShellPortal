using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PowerShellPortal.Controllers
{
    public class PowerShellController : Controller
    {
        private readonly IHubContext<OutputHub> _hubContext;

        public PowerShellController(IHubContext<OutputHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void RunScript(PowerShell shell, bool varwidth)
        {
            // Sleep a few secs to allow enough time for Results window to open and establish connection to OutputHub
            // Without this, output may not show
            System.Threading.Thread.Sleep(3000);

            string hubGroup = Environment.UserName;

            if (shell == null)
            {
                _hubContext.Clients.Group(hubGroup).SendAsync("show", "Shell empty - nothing to execute.");
                return;
            }

            string fontstr = "";
            if (varwidth != true)
            {
                fontstr = "face='monospace' size=3";
            }

            _hubContext.Clients.Group(hubGroup).SendAsync("show", "<b>Executing: </b>" + shell.Commands.Commands[0].ToString());
            string prevmsg = "";
            string msg = "";

            _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br><b>BEGIN</b>");
            _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br>_________________________________________________________________________");

            // Collect powershell OUTPUT and send to OutputHub
            var output = new PSDataCollection<PSObject>();

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            output.DataAdded += delegate (object sender, DataAddedEventArgs e)
            {
                msg = output[e.Index].ToString();

                if (msg != prevmsg)
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br><span><font color=black " + fontstr + ">" + msg + "</font></span>");
                }
                else
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", ".");
                }
                prevmsg = msg;
                var psoutput = (PSDataCollection<PSObject>)sender;
                Collection<PSObject> results = psoutput.ReadAll();
            };
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            prevmsg = "";
            // Collect powershell PROGRESS output and send to OutHub
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            shell.Streams.Progress.DataAdded += delegate (object sender, DataAddedEventArgs e)
            {
                msg = shell.Streams.Progress[e.Index].Activity.ToString();
                if (msg != prevmsg)
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br><span><font color=green " + fontstr + ">" + msg + "</font></span>");
                }
                else
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", ".");
                }
                prevmsg = msg;
                var psprogress = (PSDataCollection<ProgressRecord>)sender;
                Collection<ProgressRecord> results = psprogress.ReadAll();
            };
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            prevmsg = "";
            // Collect powershell WARNING output and send to OutHub
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            shell.Streams.Warning.DataAdded += delegate (object sender, DataAddedEventArgs e)
            {
                msg = shell.Streams.Warning[e.Index].ToString();
                if (msg != prevmsg)
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br><span><font color=orange " + fontstr + "><b>***WARNING***:</b> " + msg + "</font></span>");
                }
                else
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", ".");
                }
                prevmsg = msg;
                var pswarning = (PSDataCollection<WarningRecord>)sender;
                Collection<WarningRecord> results = pswarning.ReadAll();
            };
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            prevmsg = "";
            // Collect powershell ERROR output and send to OutHub
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            shell.Streams.Error.DataAdded += delegate (object sender, DataAddedEventArgs e)
            {
                msg = shell.Streams.Error[e.Index].ToString();
                if (msg != prevmsg)
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br><span><font color=red " + fontstr + "><b>***ERROR***:</b> " + msg + "</font></span>");
                }
                else
                {
                    _hubContext.Clients.Group(hubGroup).SendAsync(".");
                }
                prevmsg = msg;
                var pserror = (PSDataCollection<ErrorRecord>)sender;
                Collection<ErrorRecord> results = pserror.ReadAll();
            };
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // Execute powershell command
            IAsyncResult IasyncResult = shell.BeginInvoke<PSObject, PSObject>(null, output);

            // Wait for powershell command to finish
            IasyncResult.AsyncWaitHandle.WaitOne();


            _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br>_________________________________________________________________________");
            _hubContext.Clients.Group(hubGroup).SendAsync("show", "<br><b>EXECUTION COMPLETE</b>. Check above results for any errors.");
            return;
        }
    }
}