using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace AglonaReader.Mp3Player
{

    class AudioPlayer
    {
        private Mp3FileReader mp3Stream;
        private WaveOut m_WavePlayer;
        
        private string m_FileName = "";
        
        public void Open(string mp3FileName)
        {
            if (!File.Exists(mp3FileName)) return;
            if (mp3FileName == m_FileName) return;

            Close();
            
            mp3Stream = new Mp3FileReader(mp3FileName);
            
            m_WavePlayer = new WaveOut();
            m_WavePlayer.Init(mp3Stream);

            m_FileName = mp3FileName;

        }

        public void PlayFromTo(uint startMs, uint finishMs)
        {
            if (m_WavePlayer == null)
                return;
            
            if (startMs >= finishMs && finishMs != 0)
                return;

            Stop(true);

            mp3Stream.TrimToPlay(startMs, finishMs);

            m_WavePlayer.Play();
        }

        public void Stop(bool reInit)
        {
            if (m_WavePlayer == null)
                return;

            if (m_WavePlayer.PlaybackState == PlaybackState.Stopped)
                return;

            bool needToReInit = reInit && m_WavePlayer.PlaybackState == PlaybackState.Playing;

            if (needToReInit)
            {
                m_WavePlayer.Dispose();

                m_WavePlayer = new WaveOut();
                m_WavePlayer.Init(mp3Stream);
            }
            else
                m_WavePlayer.Stop();

        }

        public void Close()
        {
            if (m_WavePlayer == null)
                return;

            m_FileName = "";
            m_WavePlayer.Dispose();
            m_WavePlayer = null;
            mp3Stream.Dispose();
            mp3Stream = null;
        }


        internal string FileName()
        {
            return m_FileName;
        }


        // returns approximate time based by the current frame in the Mp3FileReader
        public uint CurrentTime
        {
            get
            {
                return mp3Stream.CurrentTimeMs;
            }
        }

        public bool Playing
        {
            get
            {
                if (m_WavePlayer == null)
                    return false;
                return m_WavePlayer.PlaybackState == PlaybackState.Playing;
            } 
        }
    }
}
