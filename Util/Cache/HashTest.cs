using System.Text;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Cache {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class HashTest {
    [Test]
    public void TestHashLookup() {
      uint b = 0, c = 0;

      var byteEmpty = new byte[0];

      HashLookup.Hash(byteEmpty, ref c, ref b);
      Assert.AreEqual(b, 0xdeadbeef);
      Assert.AreEqual(c, 0xdeadbeef);

      b = 0xdeadbeef;
      c = 0;
      HashLookup.Hash(new byte[0], ref c, ref b);
      Assert.AreEqual(b, 0xdeadbeef);
      Assert.AreEqual(c, 0xbd5b7dde);

      b = 0xdeadbeef;
      c = 0xdeadbeef;
      HashLookup.Hash(new byte[0], ref c, ref b);
      Assert.AreEqual(b, 0xbd5b7dde);
      Assert.AreEqual(c, 0x9c093ccd);

      byte[] byteData = Encoding.UTF8.GetBytes("Four score and seven years ago");

      b = 0;
      c = 0;
      HashLookup.Hash(byteData, ref c, ref b);
      Assert.AreEqual(b, 0xce7226e6);
      Assert.AreEqual(c, 0x17770551);

      b = 1;
      c = 0;
      HashLookup.Hash(byteData, ref c, ref b);
      Assert.AreEqual(b, 0xbd371de4);
      Assert.AreEqual(c, 0xe3607cae);

      b = 0;
      c = 1;
      HashLookup.Hash(byteData, ref c, ref b);
      Assert.AreEqual(b, 0x6cbea4b3);
      Assert.AreEqual(c, 0xcd628161);

      c = HashLookup.Hash(byteData, 0);
      Assert.AreEqual(c, 0x17770551);

      c = HashLookup.Hash(byteData, 1);
      Assert.AreEqual(c, 0xcd628161);
    }
  }
}
