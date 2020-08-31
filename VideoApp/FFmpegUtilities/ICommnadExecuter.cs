using VideoApp.FFmpegUtilities.Models;

namespace FFmpegUtilities
{
    public interface ICommandExecuter
    {
        bool ExecuteCommand(ProcessStartParameters parameters);
        VideoInformation GetVideo(string fullFileName);
    }
}