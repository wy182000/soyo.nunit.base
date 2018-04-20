using Soyo.Base;
using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class HookTest {
    private int checkValue_ = 0;

    private void add(int value) {
      checkValue_ += value;
    }

    [Test]
    public void TestHook() {
      var hook = new Hook<int>();
      var item1 = hook.Add(add);
      var item2 = hook.Add(add, true);
      Assert.AreEqual(hook.Count, 2);
      hook.Invoke(1);
      Assert.AreEqual(checkValue_, 2);
      Assert.AreEqual(hook.Count, 1);
      hook.Invoke(1);
      Assert.AreEqual(checkValue_, 3);
      hook.Remove(item1);
      Assert.AreEqual(hook.Count, 0);
      hook.Invoke(1);
      Assert.AreEqual(checkValue_, 3);
    }
  }
}
