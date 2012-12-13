using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MuliclientAPI;

namespace MultiClientRunner
{
	public class AppItem
	{
		Process Process;

        public string Name { get; private set; }
        public string ClientId { get; set; }

        public void Run(MulticlientData multiclientData, string clientId)
		{
            ClientId = clientId;
            Name = multiclientData.Name;
            var commandLineArguments = "regime='multiclient' " +
                "ClientId='" + ClientId +
                "' RemoteAddress='" + multiclientData.RemoteAddress +
                "' RemotePort='" + multiclientData.RemotePort.ToString() +
                "' login='" + multiclientData.Login +
                "' password='" + multiclientData.Password + "'";

			var processStartInfo = new ProcessStartInfo()
			{
				FileName = @"E:/Projects/Projects/FireMonitor/bin/Debug/FireMonitor.exe",
				Arguments = commandLineArguments
			};
			Process = System.Diagnostics.Process.Start(processStartInfo);
		}

        public void Kill()
        {
            Process.Kill();
        }

        public void Hide()
        {
            MulticlientServer.Muliclient.Show(ClientId);
        }

        public void Show(WindowSize windowSize)
        {
            if (windowSize != null)
            {
                MulticlientServer.Muliclient.SetWindowSize(ClientId, windowSize);
            }
            MulticlientServer.Muliclient.Hide(ClientId);
        }

        public WindowSize GetWindowSize()
        {
            return MulticlientServer.Muliclient.GetWindowSize(ClientId);
        }
	}
}