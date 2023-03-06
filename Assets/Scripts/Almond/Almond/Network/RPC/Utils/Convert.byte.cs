using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.Text;
using Almond.RPC;

namespace Almond.RPC
{
    public partial class Convert
    {
        #region Encode
        /// <summary>
        /// 인코딩
        /// 데이터를 바이터배열 로 변환
        /// </summary>
        /// <typeparam name="T">변환할 타입</typeparam>
        /// <param name="value">데이터</param>
        /// <param name="param">추가 매개 변수</param>
        /// <returns></returns>
        public static byte[] Encode<T>(object value, params int[] param)
        {
            return ConvertDataToByte<T>(value, param);
        }

        public static void Encode<T>(ref byte[] data, object value, ref int length, params object[] param)
        {
            int typeLength = GetTypeLength<T>();
            var tBuffer = Encode<T>(value);
            Buffer.BlockCopy(tBuffer, 0, data, length, typeLength);
            length += typeLength;
        }
        #endregion

        #region Decode
        public static T Decode<T>(byte[] data, ref int index, params int[] param)
        {
            int typeLength = param.Length == 0 ? GetTypeLength<T>() : param[0];
            byte[] result = new byte[typeLength];

            Buffer.BlockCopy(data, index, result, 0, typeLength);

            index += typeLength;
            return (T)ConvertByteToData<T>(result);
        }

        #endregion

        #region Convert
        public static int GetTypeLength<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }
        private static byte[] ConvertDataToByte<T>(object value, params int[] param)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:  return BitConverter.GetBytes(System.Convert.ToBoolean(value));
                case TypeCode.Char:     return BitConverter.GetBytes(System.Convert.ToChar(value));
                case TypeCode.SByte:    return new byte[1] { (byte)System.Convert.ToSByte(value) };
                case TypeCode.Byte:     return new byte[1] {       System.Convert.ToByte(value) };
                case TypeCode.Int16:    return BitConverter.GetBytes(System.Convert.ToInt16(value));
                case TypeCode.UInt16:   return BitConverter.GetBytes(System.Convert.ToUInt16(value));
                case TypeCode.Int32:    return BitConverter.GetBytes(System.Convert.ToInt32(value));
                case TypeCode.UInt32:   return BitConverter.GetBytes(System.Convert.ToUInt32(value));
                case TypeCode.Int64:    return BitConverter.GetBytes(System.Convert.ToInt64(value));
                case TypeCode.UInt64:   return BitConverter.GetBytes(System.Convert.ToUInt64(value));
                case TypeCode.Single:   return BitConverter.GetBytes(System.Convert.ToSingle(value));
                case TypeCode.Double:   return BitConverter.GetBytes(System.Convert.ToDouble(value));
                case TypeCode.Decimal:  return BitConverter.GetBytes(System.Convert.ToDouble(value));
                case TypeCode.String:   return StringToByte((string)value, param);
            }

            return null;
        }

        private static object ConvertByteToData<T>(byte[] value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: return BitConverter.ToBoolean(value, 0);
                case TypeCode.Char: return BitConverter.ToChar(value, 0);
                case TypeCode.SByte: return System.Convert.ToSByte(value[0]);
                case TypeCode.Byte: return System.Convert.ToByte(value[0]);
                case TypeCode.Int16: return BitConverter.ToInt16(value, 0);
                case TypeCode.UInt16: return BitConverter.ToUInt16(value, 0);
                case TypeCode.Int32: return BitConverter.ToInt32(value, 0);
                case TypeCode.UInt32: return BitConverter.ToUInt32(value, 0);
                case TypeCode.Int64: return BitConverter.ToInt64(value, 0);
                case TypeCode.UInt64: return BitConverter.ToUInt64(value, 0);
                case TypeCode.Single: return BitConverter.ToSingle(value, 0);
                case TypeCode.Double: return BitConverter.ToDouble(value, 0);
                case TypeCode.Decimal: return ByteToDecimal(value);
                case TypeCode.String:
                    {
                        var str = Encoding.UTF8.GetString(value);
                        int index = str.IndexOf('\0');
                        if(index != 0)
                        {
                            str = str.Remove(index);
                        }

                        return str;
                    }
                    
            }

            return default;
        }


        private static byte[] StringToByte(string value, params int[] param)
        {
            if(param.Length == 0)
            {
                Encoder ec = Encoding.UTF8.GetEncoder();

                char[] charArray = value.ToCharArray();
                int length = ec.GetByteCount(charArray, 0, charArray.Length, false);
                byte[] encodeValues = new byte[length];
                ec.GetBytes(charArray, 0, charArray.Length, encodeValues, 0, true);
                
                // return Utils.FillLengthHead(encodeValues);
                return encodeValues;
            }
            else
            {
                var dataArray = Encoding.Default.GetBytes(value);
                var buffer = new byte[param[0]];
                Buffer.BlockCopy(dataArray, 0, buffer, 0, dataArray.Length);

                return buffer;
            }
        }
        
        private static object ByteToDecimal(byte[] data)
        {
            int[] bits = new int[4];
            for(int i = 0; i < sizeof(decimal); i++)
            {
                bits[i / 4] = BitConverter.ToInt32(data, i);
            }
            return new decimal(bits);
        }

        #endregion

        #region <Debug>
        public static void PrintHexaDecimal(byte[] data)
        {
            Debug.LogError(BitConverter.ToString(data));
        }
        #endregion
    }
}

