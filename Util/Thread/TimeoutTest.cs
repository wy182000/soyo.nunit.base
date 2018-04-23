using System;
using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class TimoutTest {
    [OneTimeSetUp]
    public void Init() {
      Thread.Initialize();
    }

    [OneTimeTearDown]
    public void Terminate() {
      Thread.Terminate();
    }

    [Test]
    public void TesTimer() {
      bool ret = false;
      var timer = Timeout.Add(100, (id) => ret = true, (id) => Assert.Fail());
      var rc = Thread.Wait(() => ret, 1000);
      Assert.IsTrue(rc);
      Assert.IsTrue(ret);
      rc = Timeout.Remove(timer);
      Assert.IsFalse(rc);

      ret = false;
      timer = Timeout.Add(1000, (id) => Assert.Fail(), (id)=> ret = true);
      Thread.Sleep(10);
      rc = Timeout.Remove(timer);
      Assert.IsTrue(rc);
      Assert.IsTrue(ret);
    }
  }
}
