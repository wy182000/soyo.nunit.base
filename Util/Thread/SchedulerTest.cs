using System;
using System.Threading.Tasks;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class SchedulerTest {
    private static int checkCount = 0;

    private void updateTimer(object sender, Timer timer) {
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        var scheduler = sender as Scheduler;
        Assert.IsNotNull(scheduler);
        scheduler.Stop();
      }
    }

    [Test]
    public void TestSyncTimer() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      Timer timer = new Timer(1);
      timer.Once = false;
      timer.Action += updateTimer;
      scheduler.AddTimer(timer);
      timer = new Timer(10);
      timer.Once = false;
      timer.Action += (sender, t) => checkCount++;
      scheduler.AddTimer(timer);
      timer = new Timer(1000);
      timer.Once = true;
      timer.Action += (sender, t) => Assert.Fail();
      scheduler.AddTimer(timer);
      scheduler.StartAndLoop();
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();
    }

    [Test]
    public void TestASyncTimer() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      Timer timer = new Timer(1);
      timer.Once = false;
      timer.Action += updateTimer;
      scheduler.AddTimer(timer);
      timer = new Timer(10);
      timer.Once = false;
      timer.Action += (sender, t) => checkCount++;
      scheduler.AddTimer(timer);
      timer = new Timer(1000);
      timer.Once = true;
      timer.Action += (sender, t) => Assert.Fail();
      scheduler.AddTimer(timer);
      scheduler.StartAsync();
      scheduler.Join();
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();
    }

    void postUpdateFunc(object state) {
      var scheduler = state as Scheduler;
      Assert.IsNotNull(scheduler);
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        scheduler.Stop(true);
      } else {
        scheduler.Post(postUpdateFunc, scheduler, true);
      }
    }

    void postAddFunc(object state) {
      checkCount++;
      var scheduler = state as Scheduler;
      Assert.IsNotNull(scheduler);
      scheduler.Post(postAddFunc, scheduler, 10);
    }

    [Test]
    public void TestSyncPost() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      scheduler.Post(postUpdateFunc, scheduler);
      scheduler.Post(postAddFunc, scheduler);

      var failed = false;
      scheduler.Post(() => { failed = true;  scheduler.Stop(); }, 1000);

      scheduler.StartAndLoop();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();
    }

    [Test]
    public void TestASyncPost() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      scheduler.Post(postUpdateFunc, scheduler);
      scheduler.Post(postAddFunc, scheduler);

      var failed = false;
      scheduler.Post(() => { failed = true; scheduler.Stop(); }, 1000);

      scheduler.StartAsync();
      scheduler.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();
    }

    [Test]
    public void TestASyncSend() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      var failed = false;
      scheduler.Post(() => { failed = true; scheduler.Stop(); }, 1000);
      scheduler.StartAsync();
      for (int i = 0; i < 10; i++) {
        scheduler.Send(() => checkCount++);
        Assert.AreEqual(checkCount, i + 1);
      }
      scheduler.Send(() => scheduler.Stop());
      Assert.IsFalse(scheduler.IsRunning);
      scheduler.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();
    }
  }
}
