using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Almond.RPC
{

    public class Utils
    {
        private Utils()
        {
        }

        /// <summary>
        /// 데이터 헤더 채우기。
        /// </summary>
        public static byte[] FillLengthHead(byte[] srcData)
        {
            return FillLengthHead(srcData, (ushort)srcData.Length);
        }

        /// <summary>
            /// 데이터 헤더 채우기。
        /// </summary>
        public static byte[] FillLengthHead(byte[] srcData, ushort length)
        {
            byte[] lengthByteArray = BitConverter.GetBytes(length);//문자열 길이를 이진수로 변환
            //Array.Reverse(lengthByteArray);
            byte[] result = new byte[length + lengthByteArray.Length];
            Buffer.BlockCopy(lengthByteArray, 0, result, 0, lengthByteArray.Length);
            Buffer.BlockCopy(srcData, 0, result, lengthByteArray.Length, length);
            return result;
        }
    }
}