using System;
using Xunit;

namespace LiuYueSoft.Utilities.Test
{
    public class ByteArrayUtilityTest
    {
        [Fact]
        public void ToHexStringTest()
        {
            var bytes = new byte[]
            {
                0x00, 0x01, 0x02, 0x03, 0x10, 0x11, 0x12, 0x13,
                0xaa, 0xbb, 0xcc, 0xdd, 0xaf, 0xbe, 0xcc, 0xcb
            };

            Assert.Equal("0001020310111213AABBCCDDAFBECCCB", bytes.ToUpperHexString());
            Assert.Equal("0001020310111213aabbccddafbecccb", bytes.ToLowerHexString());
        }

        [Fact]
        public void WriteWithEndiannessTest()
        {
            const int intNumber = 0x12345678;
            var bytesPreserved = new byte[4];
            var bytesReversed = new byte[4];
            var bytesBigEndian = new byte[4];
            var bytesLittleEndian = new byte[4];
            unsafe
            {
                fixed (byte* pBytesPreserved = &bytesPreserved[0])
                fixed (byte* pBytesReversed = &bytesReversed[0])
                fixed (byte* pBytesBigEndian = &bytesBigEndian[0])
                fixed (byte* pBytesLittleEndian = &bytesLittleEndian[0])
                {
                    ByteArrayUtility.WriteWithEndiannessPreserved(pBytesPreserved, intNumber);
                    ByteArrayUtility.WriteWithEndiannessReversed(pBytesReversed, intNumber);
                    ByteArrayUtility.WriteInBigEndian(pBytesBigEndian, intNumber);
                    ByteArrayUtility.WriteInLittleEndian(pBytesLittleEndian, intNumber);
                }
            }

            if (BitConverter.IsLittleEndian)
            {
                Assert.Equal(new byte[] {0x78, 0x56, 0x34, 0x12}, bytesPreserved);
                Assert.Equal(new byte[] {0x12, 0x34, 0x56, 0x78}, bytesReversed);
            }
            else
            {
                Assert.Equal(new byte[] {0x12, 0x34, 0x56, 0x78}, bytesPreserved);
                Assert.Equal(new byte[] {0x78, 0x56, 0x34, 0x12}, bytesReversed);
            }

            Assert.Equal(new byte[] {0x12, 0x34, 0x56, 0x78}, bytesBigEndian);
            Assert.Equal(new byte[] {0x78, 0x56, 0x34, 0x12}, bytesLittleEndian);
        }
    }
}
