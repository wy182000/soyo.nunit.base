using System.Threading;
using System.Collections.Generic;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class TerminateMenagerTest {
    [Test]
    public void TestTerminateMenager() {
      int check = 0;
      using (var terminate = new TerminateManager()) {
        TerminateManager.Register(() => Assert.AreEqual(check++, 3));
        TerminateManager.Register(() => Assert.AreEqual(check++, 2));
        TerminateManager.Register(() => Assert.AreEqual(check++, 1));
        TerminateManager.Register(() => Assert.AreEqual(check++, 0));
        Assert.AreEqual(check, 0);
      }
      Assert.AreEqual(check, 4);

      check = 0;
      using (var t = new TerminateManager()) {
        TerminateManager.Register(() => Assert.AreEqual(check++, 3));
        using (var t1 = new TerminateManager()) {
          TerminateManager.Register(() => Assert.AreEqual(check++, 2));
          using (var t2 = new TerminateManager()) {
            TerminateManager.Register(() => Assert.AreEqual(check++, 1));
            using (var t3 = new TerminateManager()) {
              TerminateManager.Register(() => Assert.AreEqual(check++, 0));
              Assert.AreEqual(check, 0);
            }
            Assert.AreEqual(check, 1);
          }
          Assert.AreEqual(check, 2);
        }
        Assert.AreEqual(check, 3);
      }
      Assert.AreEqual(check, 4);
    }
  }
}
