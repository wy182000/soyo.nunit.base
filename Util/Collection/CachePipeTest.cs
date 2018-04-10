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

    [Test]
    public void Test() {
      Init(1);
      ypipe.LockEnable = false;
      WriteThreadFunc(0);
      ReadThreadFunc(0);
      Assert.AreEqual(value, Result, "value should be equal to result.");

      Init(1);
      ypipe.LockEnable = false;
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
      ypipe.LockEnable = false;
      ThreadCheck();
      Assert.AreEqual(value, Result, "value should be equal to result.");
    }

    [Test]
    public void TestThreadSafe() {
      Init(10);
      ypipe.LockEnable = true;
      ThreadCheck();

      Assert.AreEqual(value, Result, "value should be equal to result.");
    }
  }
}
