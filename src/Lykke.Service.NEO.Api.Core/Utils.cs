using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.Service.NEO.Api.Core
{
    public static class Utils
    {

        private static readonly Dictionary<byte, string> Hex0 = new Dictionary<byte, string>
        {
            {0, "0"},
            {1, "1"},
            {2, "2"},
            {3, "3"},
            {4, "4"},
            {5, "5"},
            {6, "6"},
            {7, "7"},
            {8, "8"},
            {9, "9"},
            {10, "A"},
            {11, "B"},
            {12, "C"},
            {13, "D"},
            {14, "E"},
            {15, "F"},
        };


        private static readonly Dictionary<char, byte> Decimal1 = new Dictionary<char, byte>
                                                    {
                                                        {'0',0},
                                                        {'1',1},
                                                        {'2',2},
                                                        {'3',3},
                                                        {'4',4},
                                                        {'5',5},
                                                        {'6',6},
                                                        {'7',7},
                                                        {'8',8},
                                                        {'9',9},
                                                        {'A',10},
                                                        {'B',11},
                                                        {'C',12},
                                                        {'D',13},
                                                        {'E',14},
                                                        {'F',15},
                                                        {'a',10},
                                                        {'b',11},
                                                        {'c',12},
                                                        {'d',13},
                                                        {'e',14},
                                                        {'f',15},
                                                    };

        private static readonly Dictionary<char, byte> Decimal0 = new Dictionary<char, byte>
                                                    {
                                                        {'0',0},
                                                        {'1',16},
                                                        {'2',32},
                                                        {'3',48},
                                                        {'4',64},
                                                        {'5',80},
                                                        {'6',96},
                                                        {'7',112},
                                                        {'8',128},
                                                        {'9',144},
                                                        {'A',160},
                                                        {'B',176},
                                                        {'C',192},
                                                        {'D',208},
                                                        {'E',224},
                                                        {'F',240},
                                                        {'a',160},
                                                        {'b',176},
                                                        {'c',192},
                                                        {'d',208},
                                                        {'e',224},
                                                        {'f',240},
                                                    };
         

        public static string ToHexString(this ICollection<byte> src)
        {
            var sb = new StringBuilder(src.Count * 2);

            foreach (var b in src)
                sb.Append(ByteToHex(b));

            return sb.ToString();
        }


        public static string ByteToHex(byte src)
        {
            var d2 = (byte)(src * 0.0625);
            src = (byte)(src - d2 * 16);

            return Hex0[d2] + Hex0[src];

        }

        public static string HexToString(string value)
        {
            var array = HexToArray(value);
            return Encoding.UTF8.GetString(array, 0, array.Length);
        }

        public static byte[] HexToArray(string src)
        {
            if (src.Length % 2 != 0)
                throw new Exception("Invalid length of [" + src + "] must equals to 2");

            var result = new byte[src.Length / 2];
            int ri = 0;

            for (var i = 0; i < src.Length; i += 2)
                result[ri++] = HexToByte(src.Substring(i, 2));

            return result;

        }


        public static byte HexToByte(string src)
        {
            if (src.Length == 0)
                throw new Exception("Can not convert empty string to byte");

            if (src.Any(b => !Decimal0.ContainsKey(b)))
                throw new Exception("Inapropriate hex string [" + src + "]");

            var d0 = src.Length == 1 ? '0' : src[0];
            var d1 = src.Length == 1 ? src[0] : src[1];


            return (byte)(Decimal0[d0] + Decimal1[d1]);

        }
    }
}
