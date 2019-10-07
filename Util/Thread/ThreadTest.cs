using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class ThreadTest {
    private static int checkCount = 0;

    [OneTimeSetUp]
    public void Init() {
      Thread.Initialize();
    }

    [OneTimeTearDown]
    public void Terminate() {
      Thread.Terminate();
    }

    private void updateTimer(Thread thread) {
      Assert.IsNotNull(thread);
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        Thread.StopThread(thread);
      }
    }

    [Test]
    public void TesTimer() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      checkCount = 0;
      Timer timer = new Timer(1);
      timer.Once = false;
      timer.Action += (sender, t) => updateTimer(thread);
      thread.AddTimer(timer);
      timer = new Timer(10);
      timer.Once = false;
      timer.Action += (sender, t) => checkCount++;
      thread.AddTimer(timer);
      timer = new Timer(1000);
      timer.Once = true;
      timer.Action += (sender, t) => Assert.Fail();
      thread.AddTimer(timer);
      thread.Join();
      Assert.AreEqual(checkCount, 10);
      thread.Dispose();
    }

    void postUpdateFunc(object state) {
      var thread = state as Thread;
      Assert.IsNotNull(thread);
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        Thread.StopThread(thread);
      } else {
        thread.Post(postUpdateFunc, thread);
      }
    }

    void postAddFunc(object state) {
      checkCount++;
      var thread = state as Thread;
      Assert.IsNotNull(thread);
      thread.Post(postAddFunc, thread, 10);
    }

    [Test]
    public void TestPost() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      checkCount = 0;
      thread.Post(postUpdateFunc, thread);
      thread.Post(postAddFunc, thread);

      var failed = false;
      thread.Post(() => { failed = true; Thread.StopThread(thread); }, 1000);

      thread.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
    }

    void postTaskUpdateFunc(object state) {
      var thread = state as Thread;
      Assert.IsNotNull(thread);
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        Thread.StopThread(thread);
      } else {
        thread.Post(new Task(postUpdateFunc, thread));
      }
    }

    void postTaskAddFunc(object state) {
      checkCount++;
      var thread = state as Thread;
      Assert.IsNotNull(thread);
      if (checkCount < 10) {
        thread.Post(new Task(postAddFunc, thread));
      }
    }

    [Test]
    public void TestTaskPost() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      checkCount = 0;
      thread.Post(new Task(postTaskUpdateFunc, thread));
      thread.Post(new Task(postTaskAddFunc, thread));

      var failed = false;
      thread.Post(() => { failed = true; Thread.StopThread(thread); }, 1000);

      thread.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
    }

    [Test]
    public void TestSend() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      checkCount = 0;
      var failed = false;
      thread.Post(() => { failed = true; Thread.StopThread(thread); }, 1000);
      for (int i = 0; i < 10; i++) {
        thread.Send(() => checkCount++);
        Assert.AreEqual(checkCount, i + 1);
      }
      thread.Send(() => thread.Stop());
      Assert.IsFalse(thread.IsRunning);
      thread.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
    }

    [Test]
    public void TestTaskSend() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      checkCount = 0;
      var failed = false;
      thread.Post(() => { failed = true; Thread.StopThread(thread); }, 1000);
      for (int i = 0; i < 10; i++) {
        thread.Send(new Task(() => checkCount++));
        Assert.AreEqual(checkCount, i + 1);
      }
      thread.Send(() => thread.Stop());
      Assert.IsFalse(thread.IsRunning);
      thread.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
    }

    [Test]
    public void TestProperty() {
      var threadCount = 4;
      List<Thread> threadSet = new List<Thread>(threadCount);
      for (int i = 0; i < threadCount; i++) {
        var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
        thread.Send(() => Thread.SetProperty("ID", thread.Id));
        thread.Send(() => Assert.AreEqual(Thread.GetProperty("ID"), thread.Id));
        threadSet.Add(thread);
      }

      for (int i = 0; i < threadCount; i++) {
        Thread.StopThread(threadSet[i]);
        threadSet[i].Join();
      }
    }

    [Test]
    public void TestWait() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      bool flag = false;
      thread.Post(() => { Thread.Sleep(10); flag = true; });
      var rc = Thread.Wait(() => flag, 100);
      Assert.IsTrue(flag);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestMonitor() {
      var thread = Thread.CreateThread();
      Assert.IsNotNull(thread);
      thread.Post(() => Thread.Sleep(100));
      Thread.InitializeMonitor();
      bool endless = false;
      Thread.OnEndless = (s) => endless = true;
      Thread.OnRecover = (s) => endless = false;
      var rc = Thread.Wait(() => endless, 1000);
      Assert.IsTrue(endless);
      Assert.IsTrue(rc);
      rc = Thread.Wait(() => endless == false, 1000);
      Assert.IsFalse(endless);
      Assert.IsTrue(rc);
      Thread.StopThread(thread);

      Thread.OnEndless = null;
      Thread.OnRecover = null;
    }
  }
}
