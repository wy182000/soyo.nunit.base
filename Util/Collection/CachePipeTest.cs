using System.Collections.Generic;
using System.Threading;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class CachePipeTest {
    private const long CheckCount = 10000;
    private static int ThreadCount;
    private static long Result;
    private static long value;
    private static CachePipe<int> ypipe;

    private void Init(int threadCount) {
      value = 0;
      ThreadCount = threadCount;
      ypipe = new CachePipe<int>(128);
      Result = CheckCount * ThreadCount * (CheckCount * ThreadCount - 1) / 2;
    }

    private static void WriteThreadFunc(object state) {
      int index = (int)state;
      for (int i = 0; i < CheckCount; i++) {
        int value = (int)CheckCount * index + i;
        ypipe.Write(ref value);
      }
    }

    private static void WriteThreadSafeFunc(object state) {
      int index = (int)state;
      for (int i = 0; i < CheckCount; i++) {
        int value = (int)CheckCount * index + i;
        lock (ypipe.WriteRoot) {
          ypipe.Write(ref value);
        }
      }
    }

    private static void ReadThreadFunc(object state) {
      int count = 0;
      while (value < Result && count < CheckCount) {
        int element;
        while (ypipe.Read(out element)) {
          Atomic.Add(ref value, element);
        }
        Soyo.Base.Thread.Sleep(1);
        count++;
      }
    }

    private static void ReadThreadSafeFunc(object state) {
      int count = 0;
      while (value < Result && count < CheckCount) {
        int element;
        bool ret = false;
        lock (ypipe.ReadRoot) {
          ret = ypipe.Read(out element);
        }
        while (ret) {
          Atomic.Add(ref value, element);
          lock (ypipe.ReadRoot) {
            ret = ypipe.Read(out element);
          }
        }
        Soyo.Base.Thread.Sleep(1);
        count++;
      }
    }

    private static void ThreadCheck() {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(WriteThreadFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(i);
      }

      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(ReadThreadFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start();
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }
    }

    private static void ThreadSafeCheck() {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(WriteThreadSafeFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(i);
      }

      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(ReadThreadSafeFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start();
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }
    }

    [Test]
    public void TestApi() {
      ypipe = new CachePipe<int>();

      int valueResult;
      valueResult = ypipe.Peek();
      Assert.AreEqual(valueResult, default(int));

      int value = 1;
      var rc = ypipe.Write(ref value);
      Assert.IsTrue(rc);
      Assert.AreEqual(ypipe.Count, 1);

      valueResult = ypipe.Peek();
      Assert.AreEqual(valueResult, value);

      rc = ypipe.CheckRead();
      Assert.IsTrue(rc);

      rc = ypipe.Unwrite(out valueResult);
      Assert.IsFalse(rc);
      Assert.AreEqual(valueResult, default(int));

      rc = ypipe.Read(out valueResult);
      Assert.IsTrue(rc);
      Assert.AreEqual(valueResult, value);

      rc = ypipe.CheckRead();
      Assert.IsFalse(rc);

      rc = ypipe.Read(out valueResult);
      Assert.IsFalse(rc);
      Assert.AreEqual(valueResult, default(int));

      ypipe.WriteBegin(ref value);
      rc = ypipe.CheckRead();
      Assert.IsFalse(rc);
      rc = ypipe.Read(out valueResult);
      Assert.IsFalse(rc);

      rc = ypipe.Write(ref value);
      Assert.IsFalse(rc);
      rc = ypipe.CheckRead();
      Assert.IsFalse(rc);
      rc = ypipe.Read(out valueResult);
      Assert.IsFalse(rc);

      rc = ypipe.Unwrite(out valueResult);
      Assert.IsTrue(rc);
      Assert.AreEqual(valueResult, value);

      rc = ypipe.WriteEnd(ref value);
      Assert.IsTrue(rc);
      rc = ypipe.CheckRead();
      Assert.IsTrue(rc);

      rc = ypipe.Unwrite(out valueResult);
      Assert.IsFalse(rc);
      Assert.AreEqual(valueResult, default(int));

      ypipe.Clear();
      Assert.AreEqual(ypipe.Count, 0);

      rc = ypipe.Write(ref value);
      Assert.IsTrue(rc);
      rc = ypipe.Write(ref value);
      Assert.IsTrue(rc);

      var array = ypipe.ReadAll();
      Assert.AreEqual(array.Length, 2);
      for (int i = 0; i < array.Length; i++) {
        Assert.AreEqual(array[i], value);
      }

      Assert.AreEqual(ypipe.Count, 0);
    }

    [Test]
    public void Test() {
      Init(1);
      WriteThreadFunc(0);
      ReadThreadFunc(0);
      Assert.AreEqual(value, Result, "value should be equal to result.");

      Init(1);
      WriteThreadFunc(0);
      var result = ypipe.ReadAll();
      Assert.IsNotNull(result, "result should not be null.");
      foreach (var i in result) {
        value += i;
      }
      Assert.AreEqual(value, Result, "value should be equal to result.");
    }

    [Test]
    public void TestNoLockThreadSafe() {
      Init(1);
      ThreadCheck();
      Assert.AreEqual(value, Result, "value should be equal to result.");
    }

    [Test]
    public void TestThreadSafe() {
      Init(10);
      ThreadSafeCheck();

      Assert.AreEqual(value, Result, "value should be equal to result.");
    }
  }
}
