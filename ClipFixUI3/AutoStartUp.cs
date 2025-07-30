using ClipFixUI3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace ClipFixUI3
{
    class AutoStartUp
    {
        StartupTask startupTask = null;

        public async Task<Boolean> EnableStartup()
        {
            if (startupTask == null)
                startupTask = await StartupTask.GetAsync("ClipFixUI3");

            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    var result = await startupTask.RequestEnableAsync();
                    if (result == StartupTaskState.Enabled)
                    {
                        return true;
                    }
                    break;

                case StartupTaskState.DisabledByUser:
                    break;

                case StartupTaskState.Enabled:
                    return true;
            }
            return false;
        }

        public async void DisableStartup()
        {
            if (startupTask == null)
                startupTask = await StartupTask.GetAsync("ClipFixUI3");
            MessageDialog dialog;
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    break;
                case StartupTaskState.DisabledByUser:
                    
                    break;
                case StartupTaskState.Enabled:
                    startupTask.Disable();
                    break;
            }
        }
        
        public async Task<Boolean> IsEnabled()
        {
            if (startupTask == null)
                startupTask = await StartupTask.GetAsync("ClipFixUI3");
            return startupTask.State == StartupTaskState.Enabled;
        }
    }
}
