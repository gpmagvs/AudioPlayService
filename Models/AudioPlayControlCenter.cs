

using NAudio.Wave;
using System.Collections.Concurrent;

namespace AudioPlayService.Models
{
    public class AudioPlayControlCenter : BackgroundService
    {
        private ConcurrentDictionary<string, WaveOutEvent> _PlayingQueue = new ConcurrentDictionary<string, WaveOutEvent>();
        public AudioPlayControlCenter()
        {
            ExecuteAsync(CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task QueuePlayTask = QueuePlay();
        }
        private async Task QueuePlay()
        {
            await Task.Delay(10).ContinueWith(async tk =>
            {
                while (true)
                {
                    if (_PlayingQueue.Count > 0)
                    {
                        foreach (var item in _PlayingQueue)
                        {
                            WaveOutEvent _waveOut = item.Value;
                            _waveOut.Play();
                            Console.WriteLine($"Now playing: {item.Key}");
                            while (_waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                await Task.Delay(1);
                            }
                            await Task.Delay(200);
                            if (_PlayingQueue.ContainsKey(item.Key))
                            {
                                _waveOut.Dispose();
                                var _newWaveOut = CreateWaveOutInstance(item.Key);
                                _PlayingQueue[item.Key] = _newWaveOut;
                            }
                        }
                    }
                    await Task.Delay(1000);
                }
            });
        }
        private ConcurrentDictionary<string, WaveOutEvent> playingStandaloneWavesList = new();


        internal async Task StopAll()
        {
            foreach (var path in playingStandaloneWavesList.Keys)
            {
                await StopAudio(path);
            }

            foreach (var item in _PlayingQueue.Keys)
            {
                await RemoveAudioFromPlayQueue(item);
            }
        }

        internal async Task PlayAudioStandalone(string audioFilePath)
        {
            _ = Task.Run(async () =>
            {

                WaveOutEvent waveout = await PlayAudioOnce(audioFilePath);
                if (playingStandaloneWavesList.TryAdd(audioFilePath, waveout))
                {
                    await Task.Delay(100);
                    while (waveout != null)
                    {
                        while (waveout.PlaybackState == PlaybackState.Playing)
                        {
                            await Task.Delay(10);
                        }
                        waveout.Dispose();
                        await Task.Delay(10);
                        if (!playingStandaloneWavesList.ContainsKey(audioFilePath))
                            return;

                        await Task.Delay(500);
                        waveout = await PlayAudioOnce(audioFilePath);
                        playingStandaloneWavesList[audioFilePath] = waveout;
                        waveout.Play();
                    }
                }

            });
        }

        internal async Task StopAudio(string audioFilePath)
        {
            if (playingStandaloneWavesList.TryRemove(audioFilePath, out WaveOutEvent _waveOut))
            {
                _waveOut.Dispose();
            }
        }


        private async Task<WaveOutEvent> PlayAudioOnce(string audioFilePath)
        {
            WaveOutEvent waveOut = CreateWaveOutInstance(audioFilePath);
            if (waveOut == null)
                return null;
            Task.Run(() => waveOut.Play());
            return waveOut;
        }


        internal async Task<(bool confirm, string message)> AddAudioToPlayQueue(string audioFilePath)
        {
            WaveOutEvent _waveOut = CreateWaveOutInstance(audioFilePath);

            if (_PlayingQueue.TryAdd(audioFilePath, _waveOut))
            {

                return (true, $"Add {audioFilePath} to play queue success.");
            }
            else
            {
                _waveOut.Dispose();
                return (false, $"{audioFilePath} already in play queue.");

            }
        }

        internal async Task<(bool confirm, string message)> RemoveAudioFromPlayQueue(string audioFilePath)
        {
            if (_PlayingQueue.TryRemove(audioFilePath, out WaveOutEvent _waveOut))
            {
                _waveOut.Dispose();
                return (true, $"Remove {audioFilePath} from play queue success.");
            }
            else
            {
                return (false, $"{audioFilePath} not found in play queue.");
            }
        }

        private WaveOutEvent CreateWaveOutInstance(string audioFilePath)
        {
            WaveOutEvent outputDevice = null;
            var audioFile = new AudioFileReader(audioFilePath);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            return outputDevice;
        }

        internal AudioPlayerInformation GetInformation()
        {
            AudioPlayerInformation infoWrapper = new AudioPlayerInformation();

            //_PlayingQueue
            infoWrapper.PlayingAudiosInfoList.AddRange(_PlayingQueue.Select(pair => new PlayingAudioInfo
            {
                AudioFilePath = pair.Key,
                PlayingMode = PlayingAudioInfo.PLAYING_MODE.QUEUEING
            }));
            //playingStandaloneWavesList
            infoWrapper.PlayingAudiosInfoList.AddRange(playingStandaloneWavesList.Select(pair => new PlayingAudioInfo
            {
                AudioFilePath = pair.Key,
                PlayingMode = PlayingAudioInfo.PLAYING_MODE.STAND_ALONE
            }));
            return infoWrapper;
        }

    }
}
