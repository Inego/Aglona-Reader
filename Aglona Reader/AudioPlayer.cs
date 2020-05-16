using System;
using NAudio.Wave;
using System.IO;

namespace AglonaReader.Mp3Player
{
    internal class AudioPlayer : IDisposable
        
    {
        private Mp3FileReader mp3Stream;
        private WaveOut mWavePlayer;
        
        private string mFileName = "";
        
        public void Open(string mp3FileName)
        {
            if (!File.Exists(mp3FileName)) return;
            if (mp3FileName == mFileName) return;

            Close();
            
            mp3Stream = new Mp3FileReader(mp3FileName);
            
            mWavePlayer = new WaveOut();
            mWavePlayer.Init(mp3Stream);

            mFileName = mp3FileName;

        }

        public void PlayFromTo(uint startMs, uint finishMs)
        {
            if (mWavePlayer == null)
                return;
            
            if (startMs >= finishMs && finishMs != 0)
                return;

            Stop(true);

            mp3Stream.TrimToPlay(startMs, finishMs);

            mWavePlayer.Play();
        }

        public void Stop(bool reInit)
        {
            if (mWavePlayer == null)
                return;

            if (mWavePlayer.PlaybackState == PlaybackState.Stopped)
                return;

            var needToReInit = reInit && mWavePlayer.PlaybackState == PlaybackState.Playing;

            if (needToReInit)
            {
                mWavePlayer.Dispose();

                mWavePlayer = new WaveOut();
                mWavePlayer.Init(mp3Stream);
            }
            else
                mWavePlayer.Stop();

        }

        public void Close()
        {
            if (mWavePlayer == null)
                return;

            mFileName = "";
            mWavePlayer.Dispose();
            mWavePlayer = null;
            mp3Stream.Dispose();
            mp3Stream = null;
        }


        // returns approximate time based by the current frame in the Mp3FileReader
        public uint CurrentTime => mp3Stream.CurrentTimeMs;

        public bool Playing
        {
            get
            {
                if (mWavePlayer == null)
                    return false;
                return mWavePlayer.PlaybackState == PlaybackState.Playing;
            } 
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mWavePlayer.Dispose();
                    mp3Stream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AudioPlayer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
