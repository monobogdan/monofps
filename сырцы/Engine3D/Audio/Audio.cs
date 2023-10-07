using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.DirectSound;
using System.IO;

namespace Engine3D
{
    public sealed class AudioSource
    {

    }

    public struct WavHeader
    {
        public int chunkId;
        public int chunkSize;
        public int format;
        public int subchunkid;
        public int subchunksize;
        public short audioFormat;
        public short numChannels;
        public int sampleRate;
        public int byteRate;
        public short blockAlign;
        public short bitsPerSample;
        public int subchunk2Id;
        public int subchunk2Size;
    }

    public enum AudioType
    {
        Music,
        Effect
    }

    public sealed class AudioStream
    {
        private SecondarySoundBuffer buffer;

        public AudioType Type
        {
            get;
            set;
        }

        public float Volume
        {
            get
            {
                return buffer.Volume;
            }
            set
            {
                buffer.Volume = (int)(value * -2000.0f);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return buffer.Status == (int)BufferStatus.Playing;
            }
        }

        public AudioStream()
        {

        }

        public void Play()
        {
            buffer.Play(0, PlayFlags.None);
        }

        public void Stop()
        {
            buffer.Stop();
        }

        public void Upload(int sampleRate, int align, int channels, byte[] data, int size)
        {
            if(data != null)
            {
                if (buffer != null)
                    buffer.Dispose();

                SoundBufferDescription desc = new SoundBufferDescription();
                desc.Flags = BufferFlags.Software | BufferFlags.ControlVolume;
                desc.Format = new SharpDX.Multimedia.WaveFormat(sampleRate, align, channels);
                desc.BufferBytes = size;
                buffer = new SecondarySoundBuffer(Game.Current.AudioManager.ds, desc);

                DataStream ds2 = null;
                DataStream strm = buffer.Lock(0, size, LockFlags.EntireBuffer, out ds2);
                strm.WriteRange<byte>(data, 0, size);
                buffer.Unlock(strm, ds2);
                
            }
        }

        public unsafe static AudioStream LoadWav(System.IO.Stream strm)
        {
            BinaryReader reader = new BinaryReader(strm);
            WavHeader hdr = new WavHeader()
            {
                chunkId = reader.ReadInt32(),
                chunkSize = reader.ReadInt32(),
                format = reader.ReadInt32(),
                subchunkid = reader.ReadInt32(),
                subchunksize = reader.ReadInt32(),
                audioFormat = reader.ReadInt16(),
                numChannels = reader.ReadInt16(),
                sampleRate = reader.ReadInt32(),
                byteRate = reader.ReadInt32(),
                blockAlign = reader.ReadInt16(),
                bitsPerSample = reader.ReadInt16(),
                subchunk2Id = reader.ReadInt32(),
                subchunk2Size = reader.ReadInt32()
            };

            byte[] data = new byte[strm.Length - sizeof(WavHeader)];
            reader.Read(data, 0, data.Length);

            AudioStream stream = new AudioStream();
            stream.Upload(hdr.sampleRate, hdr.bitsPerSample, hdr.numChannels, data, data.Length);

            return stream;
        }

        public unsafe static AudioStream LoadVorbis(System.IO.Stream strm)
        {
            Vorbis.Vorbis vorbisStrm = new Vorbis.Vorbis(strm);

            return null;
        }
    }

    public sealed class AudioManager
    {
        internal DirectSound ds;
        private PrimarySoundBuffer buffer;

        internal AudioManager()
        {
            ds = new DirectSound();
            ds.SetCooperativeLevel(Game.Current.Form.Handle, CooperativeLevel.Normal);

            SoundBufferDescription desc = new SoundBufferDescription();
            desc.Flags = BufferFlags.PrimaryBuffer;
            desc.Format = null;
            buffer = new PrimarySoundBuffer(ds, desc);
        }


    }
}
