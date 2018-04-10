using Soyo.Base;
using Soyo.Base.Crypto;
using NUnit.Framework;

namespace UnitTest.Base.Crypto {
  [TestFixture]
  [Category("Soyo.Base.Crypto")]
  internal class TestCryptoBool {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestCreate() {
      CryptoType<bool> value = false.crypto();
      Assert.AreEqual(value.value(), false);
    }

    [Test]
    public void TestFunctions() {
      CryptoType<bool> value = false.crypto();
      Assert.AreEqual(value.value(), false);
      Assert.AreEqual(value.value(), false.crypto().value());
      value = true.crypto();
      Assert.AreEqual(value.value(), true);
      Assert.AreEqual(value.value(), true.crypto().value());
    }
  }
}
