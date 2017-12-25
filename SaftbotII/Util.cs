using System;
using System.IO;

namespace SaftbotII
{
    public static class Util
    {
        /// <summary>
        /// Appends (0), (1), (2)... to a given File's filename until an unused variant is found.
        /// Will return the input path if it isn't used yet
        /// </summary>
        /// <param name="filepath">A full path to a file</param>
        public static string FindUnusedVariant(string filepath)
        {
            if (!File.Exists(filepath))
                return filepath;

            int i = 1;
            string currToTest, prefix, suffix;
            suffix = Path.GetExtension(filepath);
            prefix = filepath.Substring(0, filepath.Length - suffix.Length);

            do
            {
                currToTest = $"{prefix}({i}){suffix}";
                i++;
            } while (File.Exists(currToTest));

            return currToTest;
        }

        /// <summary>
        /// Returns new array with all elements from the input array starting from startIndex
        /// </summary>
        /// <param name="source">Source Array</param>
        /// <param name="startIndex">First index to take elements from</param>
        /// <typeparam name="T">Type of array elements</typeparam>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] source, int startIndex)
            => SubArray(source, startIndex, source.Length - startIndex);

        /// <summary>
        /// Returns new array with a certain amount of elements from the input array starting from startIndex
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="startIndex">First index to take elements from</param>
        /// <param name="length">Amount of elements</param>
        /// <typeparam name="T">Type of array elements</typeparam>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] source, int startIndex, int length)
        {
            T[] newArr = new T[length];

            Array.Copy(source, startIndex, newArr, 0, length);

            return newArr;
        }

        /// <summary>
        /// Concatenates all elements of the array as verbal list using commas and a final 'and'
        /// Example: [1, 2, 3, 4] will return "1, 2, 3 and 4"
        /// </summary>
        /// <param name="array">Elements to concatenate</param>
        /// <typeparam name="T">Type of array elements</typeparam>
        /// <returns></returns>
        public static string AndJoin<T>(this T[] array)
        {
            string msg = "";
            int i;

            for (i = 0; i < array.Length - 2; i++)
                msg += $"{array[i]}, ";

            if (array.Length > 0)
                msg += array[i].ToString();

            if (array.Length > 1)
                msg += $" and {array[i + 1]}";

            return msg;
        }
        
        #region Bitwise Manipulation
        /// <summary>
        /// Retrieves the bit at the specified position in the specified byte
        /// </summary>
        /// <param name="b">Byte to take the bit from</param>
        /// <param name="index">Left-based index of bit</param>
        public static bool ChopFromByte(byte b, int index)
        {
            return (((b << index) & Byte.MaxValue) >> 7) > 0;
        }

        /// <summary>
        /// Retrieves the bit at the specified position in the specified ushort
        /// </summary>
        /// <param name="s">Ushort to take a bit form</param>
        /// <param name="index">Left-based index of bit</param>
        public static bool ChopFromShort(ushort s, int index)
        {
            return (((s << index) & UInt16.MaxValue) >> 15) > 0;

        }

        /// <summary>
        /// Changes the bit at the specified position in the given byte to the given value
        /// </summary>
        /// <param name="value">New bit value</param>
        /// <param name="number">Old number</param>
        /// <param name="index">Left-based index of bit</param>
        /// <returns></returns>
        public static byte SetBit(bool value, byte number, int index)
        {
            if (value)
                return (byte)(number | 1 << (7 - index));
            else
                return (byte)(number & (~(1 << (7 - index))));
        }

        /// <summary>
        /// Changes the bit at the specified position in the given ushort to the given value
        /// </summary>
        /// <param name="value">New bit value</param>
        /// <param name="number">Old number</param>
        /// <param name="index">left-based index of bit</param>
        public static ushort SetBit(bool value, ushort number, int index)
        {
            if (value)
                return (ushort)(number | 1 << (15 - index));
            else
                return (ushort)(number & (~(1 << (15 - index))));
        }
        #endregion
    }
}
