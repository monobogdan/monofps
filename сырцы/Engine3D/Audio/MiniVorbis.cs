using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Engine3D.Vorbis
{
    internal class Vorbis
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct ov_callbacks
        {
            public IntPtr read_func;
            public IntPtr seek_func;
            public IntPtr close_func;
            public IntPtr tell_func;
        }

        private struct vorbis_info
        {
            int version;
            int channels;
            long rate;

            long bitrate_upper;
            long bitrate_nominal;
            long bitrate_lower;
            long bitrate_window;

            IntPtr codec_setup;
        }

        [DllImport("vorbisfile", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_fopen(StringBuilder path, IntPtr vf);

        [DllImport("vorbisfile", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_open_callbacks(IntPtr datasource, IntPtr vf, IntPtr initial, long ibytes, ov_callbacks callbacks);
        [DllImport("vorbisfile", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_test_callbacks(IntPtr datasource, IntPtr vf, IntPtr initial, long ibytes, ov_callbacks callbacks);
        [DllImport("vorbisfile", CallingConvention = CallingConvention.Cdecl)]
        private static extern vorbis_info ov_info(IntPtr vf, int file);
        [DllImport("vorbisfile", CallingConvention = CallingConvention.Cdecl)]
        private static extern long ov_read(IntPtr vf, IntPtr buffer, int length, int bigendianp, int word, int sgned, IntPtr bitstream);
        [DllImport("vorbisfile", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_clear(IntPtr vf);

        const int vfSize = 16384;

        private IntPtr vf;
        private Stream stream;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_cb_read_func(IntPtr ptr, int size, int nmemb, IntPtr datasource);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_cb_close_func (IntPtr datasource);

        public int cb_read(IntPtr ptr, int size, int nmemb, IntPtr datasource)
        {
            byte[] data = new byte[size];

            Console.WriteLine("Hi");

            stream.Read(data, 0, data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);

            return -1;
        }

        public int cb_close(IntPtr datasource)
        {
            stream.Close();

            return 0;
        }

        public Vorbis(Stream stream)
        {
            this.stream = stream;

            vf = Marshal.AllocHGlobal(vfSize);

            ov_callbacks callbacks = new ov_callbacks();
            d_cb_read_func cbr = cb_read;
            d_cb_close_func cbc = cb_close;

            callbacks.read_func = Marshal.GetFunctionPointerForDelegate(cbr);
            callbacks.seek_func = IntPtr.Zero;
            callbacks.tell_func = IntPtr.Zero;
            callbacks.close_func = Marshal.GetFunctionPointerForDelegate(cbc);

            int res = ov_open_callbacks(IntPtr.Zero, vf, IntPtr.Zero, 0, callbacks);

            if(res < 0)
                throw new FormatException("Failed to open vorbis file: " + res);
        }
        
    }
}
