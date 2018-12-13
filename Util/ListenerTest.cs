using System;
using Soyo.Base;
using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class ListenerTest {
    [Test]
    public void TestListenerGeneric() {
      IListenerGeneric listener = new ListenerGeneric();
      int check = 0;
      IListenGeneric listen;

      Action action;
      action = () => {
        check += 1;
      };

      var rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1);
      Assert.IsFalse(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 1);

      listener.Remove(listen.Id);
      Assert.AreEqual(listener.Count, 0);

      check = 0;

      Action<int> func;
      func = (data) => {
        check += data;
      };

      rc = listener.On(func, out listen);
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

      rc = listener.On(func, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, func);
      Assert.AreEqual(listener.Count, 4);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 12);
      Assert.AreEqual(listener.Count, 3);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
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

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 18);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Remove(action);
      Assert.IsTrue(rc);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 18);
      Assert.AreEqual(listener.Count, 0);
    }

    [Test]
    public void TestListener() {
      IListener listener = new Listener();
      int check = 0;
      IListen listen;

      Action action;
      action = () => {
        check += 1;
      };

      var rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 1);

      listener.Remove(listen.Id);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 2);
      Assert.AreEqual(listener.Count, 1);

      listener.Clear();
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 2);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Once(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 3);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 3);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 4);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Remove(action);
      Assert.IsTrue(rc);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit();
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 4);
      Assert.AreEqual(listener.Count, 0);
    }

    [Test]
    public void TestListenerT() {
      IListener<int> listener = new Listener<int>();
      int check = 0;
      IListen<int> listen;

      Action<int> action;
      action = (t) => {
        check += t;
      };

      var rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 1);

      listener.Remove(listen.Id);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 1);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 2);
      Assert.AreEqual(listener.Count, 1);

      listener.Clear();
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 2);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Once(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 3);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 3);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 4);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Remove(action);
      Assert.IsTrue(rc);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 4);
      Assert.AreEqual(listener.Count, 0);
    }

    [Test]
    public void TestListenerT1T2() {
      IListener<int, int> listener = new Listener<int, int>();
      int check = 0;
      IListen<int, int> listen;

      Action<int, int> action;
      action = (t1, t2) => {
        check += t1;
        check += t2;
      };

      var rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 2);
      Assert.AreEqual(listener.Count, 1);

      listener.Remove(listen.Id);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 2);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 4);
      Assert.AreEqual(listener.Count, 1);

      listener.Clear();
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 4);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Once(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 6);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 6);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.On(action, out listen);
      Assert.IsTrue(rc);
      Assert.Greater(listen.Id, -1);
      Assert.AreEqual(listen.Callback, action);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Emit(1, 1);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 8);
      Assert.AreEqual(listener.Count, 1);

      rc = listener.Remove(action);
      Assert.IsTrue(rc);
      Assert.AreEqual(listener.Count, 0);

      rc = listener.Emit(1, 2);
      Assert.IsTrue(rc);
      Assert.AreEqual(check, 8);
      Assert.AreEqual(listener.Count, 0);
    }
  }
}
