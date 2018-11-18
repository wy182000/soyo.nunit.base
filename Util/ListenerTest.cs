using System;
using Soyo.Base;
using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class ListenerTest {
    [Test]
    public void TestListener() {
      IListener listener = new Listener();
      int check = 0;
      Func<IListen, object, bool> func;
      func = (l, data) => {
        check += (int)data;
        return true;
      };
      IListen listen;
      var rc = listener.On(func, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, func);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.On(func, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, func);
      Assert.AreEqual(listener.Count, 2);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 3);
      Assert.AreEqual(listener.Count, 2);

      rc = listener.Once(func, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, func);
      Assert.AreEqual(listener.Count, 3);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 6);
      Assert.AreEqual(listener.Count, 2);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 8);
      Assert.AreEqual(listener.Count, 2);

      rc = listener.Once(func, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, func);
      Assert.AreEqual(listener.Count, 3);

      func = (l, data) => {
        check += (int)data;
        return false;
      };

      rc = listener.On(func, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, func);
      Assert.AreEqual(listener.Count, 4);

      rc = listener.Emit(1);
      Assert.IsFalse(rc);
      Assert.AreEqual(check, 12);
      Assert.AreEqual(listener.Count, 3);

      rc = listener.Emit(1);
      Assert.IsFalse(rc);
      Assert.AreEqual(check, 15);
      Assert.AreEqual(listener.Count, 3);

      listener.Remove(listen.Id);
      Assert.AreEqual(listener.Count, 2);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 17);
      Assert.AreEqual(listener.Count, 2);

      listener.Clear();
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 17);
      Assert.AreEqual(listener.Count, 0);
    }
  }
}
