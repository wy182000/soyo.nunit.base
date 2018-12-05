using System;
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

      var failed = false;
      scheduler.Post(() => { failed = true;  scheduler.Stop(); }, 1000);

      var task1 = scheduler.Post(postUpdateFunc, scheduler);
      var task2 = scheduler.Post(postAddFunc, scheduler);

      Assert.IsNotNull(task1);
      Assert.IsNotNull(task2);
      Assert.IsFalse(task1.IsCompleted);
      Assert.IsFalse(task1.IsFaulted);
      Assert.IsFalse(task2.IsCompleted);
      Assert.IsFalse(task2.IsFaulted);

      scheduler.StartAndLoop();

      Assert.IsTrue(task1.IsCompleted);
      Assert.IsFalse(task1.IsFaulted);
      Assert.IsTrue(task2.IsCompleted);
      Assert.IsFalse(task2.IsFaulted);

      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();


      scheduler = new Scheduler();
      scheduler.Start();

      int count = 0;
      Action action = () => count++;
      ITask task = scheduler.Post(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      task = scheduler.Post(action, true);
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 2);

      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Post(actionObject, 2);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 4);

      task = scheduler.Post(actionObject, 2, true);
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 4);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 6);

      action = () => throw new Exception("Test Send");
      task = scheduler.Post(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);


      action = () => throw new Exception("Test Send");
      task = scheduler.Post(action, true);
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);

      Func<int> func = () => count++;
      ITask<int> taskInt = scheduler.Post(func);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 6);
      Assert.AreEqual(count, 7);

      taskInt = scheduler.Post(func, true);
      Assert.IsNotNull(task);
      Assert.IsFalse(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 7);

      scheduler.Step();

      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 7);
      Assert.AreEqual(count, 8);

      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Post(funcObject, 2);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 10);
      Assert.AreEqual(count, 10);

      taskInt = scheduler.Post(funcObject, 2,  true);
      Assert.IsNotNull(task);
      Assert.IsFalse(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 10);

      scheduler.Step();

      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 12);
      Assert.AreEqual(count, 12);

      scheduler.Dispose();
    }

    [Test]
    public void TestASyncPost() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      var failed = false;
      scheduler.Post(() => { failed = true; scheduler.Stop(); }, 1000);

      var task1 = scheduler.Post(postUpdateFunc, scheduler);
      var task2 = scheduler.Post(postAddFunc, scheduler);

      Assert.IsNotNull(task1);
      Assert.IsNotNull(task2);
      Assert.IsFalse(task1.IsCompleted);
      Assert.IsFalse(task1.IsFaulted);
      Assert.IsFalse(task2.IsCompleted);
      Assert.IsFalse(task2.IsFaulted);

      scheduler.StartAsync();

      int count = 0;
      Action action = () => count++;
      ITask task = scheduler.Post(action);
      var rc = task.Wait(1000);
      Assert.IsTrue(rc);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Post(actionObject, 2);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 3);

      action = () => throw new Exception("Test Send");
      task = scheduler.Post(action);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);

      Func<int> func = () => count++;
      ITask<int> taskInt = scheduler.Post(func);
      rc = taskInt.Wait(1000);
      Assert.IsTrue(rc);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 3);
      Assert.AreEqual(count, 4);

      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Post(funcObject, 2);
      rc = taskInt.Wait(1000);
      Assert.IsTrue(rc);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 6);
      Assert.AreEqual(count, 6);

      scheduler.Join();

      Assert.IsTrue(task1.IsCompleted);
      Assert.IsFalse(task1.IsFaulted);
      Assert.IsTrue(task2.IsCompleted);
      Assert.IsFalse(task2.IsFaulted);

      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);
      scheduler.Dispose();
    }

    [Test]
    public void TestSyncSend() {
      Scheduler scheduler = new Scheduler();
      scheduler.Start();
      int count = 0;
      Action action = () => count++;
      ITask task = scheduler.Send(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Send(actionObject, 2);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 3);

      action = () => throw new Exception("Test Send");
      task = scheduler.Send(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);

      Func<int> func = () => count++;
      ITask<int> taskInt = scheduler.Send(func);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 3);
      Assert.AreEqual(count, 4);

      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Send(funcObject, 2);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 6);
      Assert.AreEqual(count, 6);

      scheduler.Dispose();
    }

    [Test]
    public void TestASyncSend() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      var failed = false;

      scheduler.Post(() => { failed = true; scheduler.Stop(); }, 1000);
      scheduler.StartAsync();

      int count = 0;
      Action action = () => count++;
      ITask task = scheduler.Send(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Send(actionObject, 2);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 3);

      action = () => throw new Exception("Test Send");
      task = scheduler.Send(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);

      Func<int> func = () => count++;
      ITask<int> taskInt = scheduler.Send(func);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 3);
      Assert.AreEqual(count, 4);

      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Send(funcObject, 2);
      Assert.IsNotNull(task);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 6);
      Assert.AreEqual(count, 6);

      for (int i = 0; i < 10; i++) {
        var t = scheduler.Send(() => checkCount++);
        Assert.IsNotNull(t);
        Assert.IsTrue(t.IsCompleted);
        Assert.IsFalse(t.IsFaulted);
        Assert.AreEqual(checkCount, i + 1);
      }
      task = scheduler.Send(() => scheduler.Stop());
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.IsFalse(scheduler.IsRunning);
      scheduler.Join();
      Assert.IsFalse(failed);
      Assert.AreEqual(checkCount, 10);

      scheduler.Dispose();
    }
  }
}
