using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LiuYueSoft.Utilities
{
    /// <summary>
    /// References:
    /// - https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa/24343727#24343727
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    public static class ByteArrayUtility
    {
        private static readonly uint[] Lookup32Unsafe = CreateLookup32Unsafe();

        private static readonly unsafe uint* Lookup32UnsafeP =
            (uint*) GCHandle.Alloc(Lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject();

        private static uint[] CreateLookup32Unsafe()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");

                if (BitConverter.IsLittleEndian)
                {
                    result[i] = ((uint) s[0]) + ((uint) s[1] << 16);
                }
                else
                {
                    result[i] = ((uint) s[1]) + ((uint) s[0] << 16);
                }
            }

            return result;
        }

        public static string ToHexString(this byte[] bytes)
        {
            unsafe
            {
                var lookupP = Lookup32UnsafeP;
                var result = new string((char) 0, bytes.Length * 2);
                fixed (byte* bytesP = bytes)
                fixed (char* resultP = result)
                {
                    uint* resultP2 = (uint*) resultP;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        resultP2[i] = lookupP[bytesP[i]];
                    }
                }

                return result;
            }
        }

        public static unsafe string ToHexString(byte* bytesP, int length)
        {
            var lookupP = Lookup32UnsafeP;
            var result = new string((char) 0, length * 2);
            fixed (char* resultP = result)
            {
                uint* resultP2 = (uint*) resultP;
                for (int i = 0; i < length; i++)
                {
                    resultP2[i] = lookupP[bytesP[i]];
                }
            }

            return result;
        }

        public static unsafe void WriteWithEndiannessPreserved<T>(byte* bytes, T n) where T : unmanaged
        {
            Unsafe.Copy(bytes, ref n);
        }

        public static unsafe void WriteWithEndiannessSwapped<T>(byte* bytes, T n) where T : unmanaged
        {
            var source = (byte*) &n;
            for (var i = 0; i < sizeof(T); ++i)
            {
                var j = sizeof(T) - i - 1;
                bytes[i] = source[j];
            }
        }

        private unsafe delegate void WriteInEndiannessDelegate<in T>(byte* bytes, T n) where T : unmanaged;

        private static unsafe WriteInEndiannessDelegate<T> GetWriteInBigEndianFunction<T>() where T : unmanaged
        {
            if (BitConverter.IsLittleEndian)
            {
                return WriteWithEndiannessSwapped<T>;
            }

            return WriteWithEndiannessPreserved<T>;
        }

        private static unsafe WriteInEndiannessDelegate<T> GetWriteInLittleEndianFunction<T>() where T : unmanaged
        {
            if (BitConverter.IsLittleEndian)
            {
                return WriteWithEndiannessPreserved<T>;
            }

            return WriteWithEndiannessSwapped<T>;
        }

        private static class WriteInBigEndianClass<T> where T : unmanaged
        {
            public static readonly WriteInEndiannessDelegate<T> Function = GetWriteInBigEndianFunction<T>();
        }

        private static class WriteInLittleEndianClass<T> where T : unmanaged
        {
            public static readonly WriteInEndiannessDelegate<T> Function = GetWriteInLittleEndianFunction<T>();
        }

        public static unsafe void WriteInBigEndian<T>(byte* bytes, T n) where T : unmanaged
        {
            WriteInBigEndianClass<T>.Function(bytes, n);
        }

        public static unsafe void WriteInLittleEndian<T>(byte* bytes, T n) where T : unmanaged
        {
            WriteInLittleEndianClass<T>.Function(bytes, n);
        }
    }
}
