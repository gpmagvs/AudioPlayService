namespace AudioPlayService.Models
{
    public class AudioPlayerInformation
    {
        public bool IsAudioPlaying => PlayingAudiosInfoList.Count != 0;

        public List<PlayingAudioInfo> PlayingAudiosInfoList { get; set; } = new();
    }

    public class PlayingAudioInfo
    {
        public enum PLAYING_MODE
        {
            STAND_ALONE,
            QUEUEING
        }

        public string AudioFilePath { get; set; } = string.Empty;
        public string PlayingBy { get; set; } = string.Empty;

        public PLAYING_MODE PlayingMode { get; set; } = PLAYING_MODE.STAND_ALONE;
    }
}
