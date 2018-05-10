using System;
using System.Text;
using Soyo.Base;
using Soyo.Base.IO;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class CompressTest {
    private const int checkCount = 100000;

    private byte[] generateData(int count) {
      var data = new byte[count];
      for (int i = 0; i < count; i++) {
        data[i] = (byte)Rand.Default.Range(0x61, 0x7B); // add lower char for test
      }
      return data;
    }

    private void checkData(byte[] src, byte[] dest) {
      Assert.IsNotNull(src);
      Assert.IsNotNull(dest);
      Assert.AreEqual(src.Length, dest.Length);

      for (int i = 0; i < src.Length; i++) {
        Assert.AreEqual(src[i], dest[i]);
      }
    }

    [Test]
    public void TestCompress() {
      var checkValue = generateData(checkCount);
      var compressSize = CompressUtil.CompressSize(checkValue);
      var compressbuffer = BufferManager.Take(compressSize);

      var ret = CompressUtil.Compress(checkValue, compressbuffer, checkCount);

      var decompressSize = CompressUtil.DecompressSize(compressbuffer);
      Assert.AreEqual(decompressSize, checkCount);

      byte[] decompressBuffer = new byte[decompressSize];

      ret = CompressUtil.Decompress(compressbuffer, decompressBuffer);
      Assert.AreEqual(ret, checkCount);

      checkData(checkValue, decompressBuffer);
    }
  }
}
