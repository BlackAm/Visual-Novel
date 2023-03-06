using System.Runtime.InteropServices;
using System;

namespace Almond.RPC
{
    public partial class Convert
    {
        public static byte[] ArrayToBytes<T>(T[] arrayData)
        {
            int structSize = GetTypeLength<T>();
            int len = arrayData.Length * structSize;
            byte[] array = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            for (int i = 0; i < arrayData.Length; i++)
            {
                Marshal.StructureToPtr(arrayData[i], ptr + i * structSize, true);
            }
            Marshal.Copy(ptr, array, 0, len);
            Marshal.FreeHGlobal(ptr);
            return array;
        }
        public static byte[] StructToByte(object obj)
        {
            if (obj == null)
            {
                return new byte[0];
            }
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;

        }
        public static byte[] StructToByte(object obj,byte[] data)
        {
            if (obj == null)
            {
                return new byte[0];
            }
            int dataArraySize = 0;
            if (data != null) dataArraySize = data.Length;

            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size + dataArraySize];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            if(data != null)
                Buffer.BlockCopy(data, 0, arr, size, dataArraySize);

            return arr;

        }
        public static T ByteToStruct<T>(byte[] buffer) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            if (size > buffer.Length)
            {
                throw new Exception();
            }
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, ptr, size);
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return obj;
        }
    }
}

