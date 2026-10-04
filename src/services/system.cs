using System.Diagnostics;

namespace konrevise.Remote.Services;

public static class SystemService
{
	public static void ExecuteCommand(string command)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            if (process == null)
                throw new Exception("failed to start a process");
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            Console.WriteLine(output);
        }
	}
}
