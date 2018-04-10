using System.Collections.Generic;
using System.Threading;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class CacheQueueTest {
    private const long CheckCount = 10000;
    private static int ThreadCount;
    private static long Result;
    private static long value;
    private static CacheQueue<int> yqueue;

    private void Init(int threadCount) {
      value = 0;
      ThreadCount = threadCount;
      yqueue = new CacheQueue<int>(128);
      Result = CheckCount * ThreadCount * (CheckCount * ThreadCount - 1) / 2;
    }

    private static void PushBackThreadFunc(object state) {
      int index = (int)state;
      for (int i = 0; i < CheckCount; i++) {
        int value = (int)CheckCount * index + i;
        yqueue.PushBack(ref value);
      }
    }

    private static void PopFrontThreadFunc(object state) {
      int count = 0;
      while (value < Result && count < CheckCount) {
        int element;
        while (yqueue.PopFront(out element)) {
          Atomic.Add(ref value, element);
        }
        Soyo.Base.Thread.Sleep(1);
        count++;
      }
    }

    private static void PopBackThreadFunc(object state) {
      int count = 0;
      while (value < Result && count < CheckCount) {
        int element;
        while (yqueue.PopBack(out element)) {
          Atomic.Add(ref value, element);
        }
        Soyo.Base.Thread.Sleep(1);
        count++;
      }
    }

    private static void ThreadCheck(bool popFront, bool popBack) {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(PushBackThreadFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(i);
      }

      if (popFront) {
        for (int i = 0; i < ThreadCount; i++) {
          var thread = new System.Threading.Thread(PopFrontThreadFunc);
          Assert.IsNotNull(thread, "value should not be null");
          threadList.Add(thread);
          thread.Start();
        }
      }

      if (popBack) {
        for (int i = 0; i < ThreadCount; i++) {
          var thread = new System.Threading.Thread(PopBackThreadFunc);
          Assert.IsNotNull(thread, "value should not be null");
          threadList.Add(thread);
          thread.Start();
        }
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }
    }

    [Test]
    public void Test() {
      Init(1);
      yqueue.LockEnable = false;
      PushBackThreadFunc(0);
      PopFrontThreadFunc(0);
      Assert.AreEqual(value, Result, "value should be equal to result.");

      Init(1);
      yqueue.LockEnable = false;
      PushBackThreadFunc(0);
      PopBackThreadFunc(0);
      Assert.AreEqual(value, Result, "value should be equal to result.");

      Init(1);
      yqueue.LockEnable = false;
      PushBackThreadFunc(0);
      var result = yqueue.PopAll();
      Assert.IsNotNull(result, "result should not be null.");
      foreach (var i in result) {
        value += i;
      }
      Assert.AreEqual(value, Result, "value should be equal to result.");
    }


    [Test]
    public void TestNoLockThreadSafe() {
      Init(1);
      yqueue.LockEnable = false;
      ThreadCheck(true, false);
      Assert.AreEqual(value, Result, "value should be equal to result.");
    }

    [Test]
    public void TestThreadSafe() {
      Init(10);
      yqueue.LockEnable = true;
      ThreadCheck(true, true);

      Assert.AreEqual(value, Result, "value should be equal to result.");
    }
  }
}
