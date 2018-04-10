﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class ThreadPoolTest {
    private static int checkCount = 0;

    [OneTimeSetUp]
    public void Init() {
      Thread.Initialize();
    }

    [OneTimeTearDown]
    public void Terminate() {
      Thread.Terminate();
    }

    void postUpdateFunc(object state) {
      var pool = state as ThreadPool;
      Assert.IsNotNull(pool);
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        pool.Stop();
      } else {
        pool.Post(postUpdateFunc, pool, true);
      }
    }

    void postAddFunc(object state) {
      checkCount++;
      var pool = state as ThreadPool;
      Assert.IsNotNull(pool);
      pool.Post(postAddFunc, pool, 10);
    }

    [Test]
    public void TestPost() {
      var pool = new ThreadPool();
      checkCount = 0;
      pool.Post(postUpdateFunc, pool);
      pool.Post(postAddFunc, pool);

      var failed = false;
      pool.Post(() => { failed = true; pool.Stop(); }, 1000);

      Thread.Wait(() => checkCount >= 10, 100);
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
    }

    [Test]
    public void TestChooseThread() {
      var pool = new ThreadPool();
      var threadCount = pool.ThreadCountMax;
      Assert.AreEqual(threadCount, ThreadPool.DefaultMaxThreadCount);
      Assert.AreEqual(pool.ThreadCount, 0);
      HashSet<int> idSet = new HashSet<int>();
      for (int i = 0; i < threadCount; i++) {
        var thread = pool.ChooseThread();
        idSet.Add(thread.Id);
      }
      Assert.AreEqual(idSet.Count, threadCount);
      Assert.AreEqual(pool.ThreadCount, threadCount);

      idSet.Clear();
      for (int i = 0; i < threadCount; i++) {
        var thread = pool.ChooseThreadFast();
        idSet.Add(thread.Id);
      }
      Assert.AreEqual(idSet.Count, threadCount);

      pool.Stop();
    }
  }
}
