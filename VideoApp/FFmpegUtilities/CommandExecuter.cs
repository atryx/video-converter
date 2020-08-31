using Newtonsoft.Json;
using System;
using System.Diagnostics;
using VideoApp.FFmpegUtilities.Models;

namespace FFmpegUtilities
{
    public class CommandExecuter : ICommandExecuter
    {
        private Process process;
        private string output = string.Empty;
        private ProcessStartInfo processInfo = new ProcessStartInfo()
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
    };

        public bool ExecuteCommand(ProcessStartParameters parameters)
        {
            
            using (process = new Process())
            {
                try
                {
                    process.StartInfo.FileName = parameters.Command;
                    process.StartInfo.Arguments = parameters.Arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();                 

                    process.WaitForExit();
                    var processExecutedSuccesfully = process.ExitCode == 0;
                    return processExecutedSuccesfully;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("===========  ERROR ===========" + ex.Message);
                    throw;
                }
            }
        }

        public VideoInformation GetVideo(string fullFileName)
        {
            try
            {
                processInfo.FileName = "ffprobe";
                processInfo.Arguments = $"-v quiet -print_format json -show_format -show_streams {fullFileName}";

                using (process = Process.Start(processInfo))
                {
                    output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                    var processExecutedSuccesfully = process.ExitCode == 0;
                    if (!processExecutedSuccesfully)
                    {
                        throw new InvalidOperationException("reading file failed");
                    }

                    return JsonConvert.DeserializeObject<VideoInformation>(output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("===========  ERROR ===========" + ex.Message);
                throw;
            }
        }
    }
}
