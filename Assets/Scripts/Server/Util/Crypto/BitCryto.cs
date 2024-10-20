using System.Collections;

namespace Almond.Util
{
    public class BitCryto
    {

        private short[] crytoKey;
        private int offsetOfKey;

        public BitCryto(short[] sKey)
        {
            crytoKey = sKey;
        }

        public byte Encode(byte inputByte)
        {
            if (offsetOfKey >= crytoKey.Length)
            {
                offsetOfKey = 0;
            }
            ushort offset = (ushort)crytoKey[offsetOfKey];
            ++offsetOfKey;

            byte outputByte = (byte)((offset + inputByte) & 0xff);
            return outputByte;
        }

        public byte Decode(byte inputByte)
        {
            if (offsetOfKey >= crytoKey.Length)
            {
                offsetOfKey = 0;
            }
            short offset = crytoKey[offsetOfKey];
            ++offsetOfKey;

            short outputByte = (short)(inputByte - offset);
            if (outputByte < 0)
            {
                outputByte += 256;
            }
            return (byte)outputByte;
        }

        public void Reset()
        {
            offsetOfKey = 0;
        }

    }
}