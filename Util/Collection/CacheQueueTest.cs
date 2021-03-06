﻿using System.Collections.Generic;
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

    private static void PushBackThreadSafeFunc(object state) {
      int index = (int)state;
      for (int i = 0; i < CheckCount; i++) {
        int value = (int)CheckCount * index + i;
        lock (yqueue.BackRoot) {
          yqueue.PushBack(ref value);
        }
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

    private static void PopFrontThreadSafeFunc(object state) {
      int count = 0;
      while (value < Result && count < CheckCount) {
        int element;
        bool ret = false;
        lock (yqueue.FrontRoot) {
          ret = yqueue.PopFront(out element);
        }
        while (ret) {
          Atomic.Add(ref value, element);
          lock (yqueue.FrontRoot) {
            ret = yqueue.PopFront(out element);
          }
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

    private static void PopBackThreadSafeFunc(object state) {
      int count = 0;
      while (value < Result && count < CheckCount) {
        int element;
        bool ret = false;
        lock (yqueue.BackRoot) {
          ret = yqueue.PopBack(out element);
        }
        while (ret ) {
          Atomic.Add(ref value, element);
          lock (yqueue.BackRoot) {
            ret = yqueue.PopBack(out element);
          }
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

    private static void ThreadSafeCheck(bool popFront, bool popBack) {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(PushBackThreadSafeFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(i);
      }

      if (popFront) {
        for (int i = 0; i < ThreadCount; i++) {
          var thread = new System.Threading.Thread(PopFrontThreadSafeFunc);
          Assert.IsNotNull(thread, "value should not be null");
          threadList.Add(thread);
          thread.Start();
        }
      }

      if (popBack) {
        for (int i = 0; i < ThreadCount; i++) {
          var thread = new System.Threading.Thread(PopBackThreadSafeFunc);
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
    public void TestApi() {
      yqueue = new CacheQueue<int>();

      Assert.AreEqual(yqueue.FrontPos, 0);
      Assert.AreEqual(yqueue.BackPos, 0);
      Assert.AreEqual(yqueue.EndPos, 0);

      int valueResult;
      valueResult = yqueue.Front;
      Assert.AreEqual(valueResult, default(int));

      valueResult = yqueue.Back;
      Assert.AreEqual(valueResult, default(int));

      var rc = yqueue.PopFront(out valueResult);
      Assert.IsFalse(rc);

      rc = yqueue.PopBack(out valueResult);
      Assert.IsFalse(rc);

      int value = 1;
      yqueue.PushBack(ref value);
      Assert.AreEqual(yqueue.Count, 1);
      Assert.AreEqual(yqueue.FrontPos, 0);
      Assert.AreEqual(yqueue.BackPos, 0);
      Assert.AreEqual(yqueue.EndPos, 1);

      valueResult = yqueue.Front;
      Assert.AreEqual(valueResult, value);

      valueResult = yqueue.Back;
      Assert.AreEqual(valueResult, value);

      rc = yqueue.PopFront(out valueResult);
      Assert.IsTrue(rc);
      Assert.AreEqual(valueResult, value);
      Assert.AreEqual(yqueue.Count, 0);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 0);
      Assert.AreEqual(yqueue.EndPos, 1);

      rc = yqueue.PopBack(out valueResult);
      Assert.IsFalse(rc);
      Assert.AreEqual(yqueue.Count, 0);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 0);
      Assert.AreEqual(yqueue.EndPos, 1);

      yqueue.PushBack(ref value);
      Assert.AreEqual(yqueue.Count, 1);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 1);
      Assert.AreEqual(yqueue.EndPos, 2);

      rc = yqueue.PopBack(out valueResult);
      Assert.IsTrue(rc);
      Assert.AreEqual(valueResult, value);
      Assert.AreEqual(yqueue.Count, 0);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 0);
      Assert.AreEqual(yqueue.EndPos, 1);

      rc = yqueue.PopFront(out valueResult);
      Assert.IsFalse(rc);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 0);
      Assert.AreEqual(yqueue.EndPos, 1);

      yqueue.PushBack(ref value);
      Assert.AreEqual(yqueue.Count, 1);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 1);
      Assert.AreEqual(yqueue.EndPos, 2);

      yqueue.PushBack(ref value);
      Assert.AreEqual(yqueue.Count, 2);
      Assert.AreEqual(yqueue.FrontPos, 1);
      Assert.AreEqual(yqueue.BackPos, 2);
      Assert.AreEqual(yqueue.EndPos, 3);

      var array = yqueue.PopAll();
      Assert.AreEqual(array.Length, 2);
      for (int i = 0; i < array.Length; i++) {
        Assert.AreEqual(array[i], value);
      }
      Assert.AreEqual(yqueue.Count, 0);
      Assert.AreEqual(yqueue.FrontPos, 3);
      Assert.AreEqual(yqueue.BackPos, 2);
      Assert.AreEqual(yqueue.EndPos, 3);

      yqueue.PushBack(ref value);
      Assert.AreEqual(yqueue.Count, 1);
      Assert.AreEqual(yqueue.FrontPos, 3);
      Assert.AreEqual(yqueue.BackPos, 3);
      Assert.AreEqual(yqueue.EndPos, 4);

      yqueue.PushBack(ref value);
      Assert.AreEqual(yqueue.Count, 2);
      Assert.AreEqual(yqueue.FrontPos, 3);
      Assert.AreEqual(yqueue.BackPos, 4);
      Assert.AreEqual(yqueue.EndPos, 5);

      yqueue.Clear();
      Assert.AreEqual(yqueue.Count, 0);
      Assert.AreEqual(yqueue.FrontPos, 5);
      Assert.AreEqual(yqueue.BackPos, 4);
      Assert.AreEqual(yqueue.EndPos, 5);
    }

    [Test]
    public void Test() {
      Init(1);
      PushBackThreadFunc(0);
      PopFrontThreadFunc(0);
      Assert.AreEqual(value, Result, "value should be equal to result.");

      Init(1);
      PushBackThreadFunc(0);
      PopBackThreadFunc(0);
      Assert.AreEqual(value, Result, "value should be equal to result.");

      Init(1);
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
      ThreadCheck(true, false);
      Assert.AreEqual(value, Result, "value should be equal to result.");
    }

    [Test]
    public void TestThreadSafe() {
      Init(10);
      ThreadSafeCheck(true, true);

      Assert.AreEqual(value, Result, "value should be equal to result.");
    }
  }
}
