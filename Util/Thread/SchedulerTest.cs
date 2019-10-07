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
        scheduler.Post(postUpdateFunc, scheduler);
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
      IAction task = scheduler.Post(action);
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      count = 0;
      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Post(actionObject, 2);
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 2);

#if !UNITY_2017_1_OR_NEWER
      action = () => throw new Exception("Test Send");
      task = scheduler.Post(action);
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);
#endif // !UNITY_2017_1_OR_NEWER

      count = 0;
      Func<int> func = () => count++;
      ITask<int> taskInt = scheduler.Post(func);
      Assert.IsNotNull(taskInt);
      Assert.IsFalse(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 1);

      count = 0;
      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Post(funcObject, 2);
      Assert.IsNotNull(taskInt);
      Assert.IsFalse(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 2);
      Assert.AreEqual(count, 2);

      scheduler.Dispose();
    }

    void postTaskUpdateFunc(object state) {
      var scheduler = state as Scheduler;
      Assert.IsNotNull(scheduler);
      if (checkCount >= 10) {
        Assert.AreEqual(checkCount, 10);
        scheduler.Stop(true);
      } else {
        scheduler.Post(new Task(postTaskUpdateFunc, scheduler));
      }
    }

    void postTaskAddFunc(object state) {
      checkCount++;
      var scheduler = state as Scheduler;
      Assert.IsNotNull(scheduler);
      scheduler.Post(new Task(postTaskAddFunc, scheduler));
    }

    [Test]
    public void TestTaskSyncPost() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;

      var failed = false;
      scheduler.Post(() => { failed = true;  scheduler.Stop(); }, 1000);

      var task1 = scheduler.Post(new Task(postTaskUpdateFunc, scheduler));
      var task2 = scheduler.Post(new Task(postTaskAddFunc, scheduler));

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
      var task = scheduler.Post(new Task(action));
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      count = 0;
      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Post(new Task(actionObject, 2));
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 2);

#if !UNITY_2017_1_OR_NEWER
      action = () => throw new Exception("Test Send");
      task = scheduler.Post(new Task(action));
      Assert.IsNotNull(task);
      Assert.IsFalse(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);

      scheduler.Step();

      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);
#endif // !UNITY_2017_1_OR_NEWER

      count = 0;
      Func<int> func = () => count++;
      var taskInt = scheduler.Post(new Task<int>(func));
      Assert.IsNotNull(taskInt);
      Assert.IsFalse(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      // task.Result will run task immediately
      // Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 1);

      count = 0;
      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Post(new Task<int>(funcObject, 2));
      Assert.IsNotNull(taskInt);
      Assert.IsFalse(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      // task.Result will run task immediately
      // Assert.AreEqual(taskInt.Result, 0);
      Assert.AreEqual(count, 0);

      scheduler.Step();

      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 2);
      Assert.AreEqual(count, 2);

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
      IAction task = scheduler.Post(action);
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

#if !UNITY_2017_1_OR_NEWER
      action = () => throw new Exception("Test Send");
      task = scheduler.Post(action);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);
#endif // !UNITY_2017_1_OR_NEWER

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
      IAction task = scheduler.Send(action);
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

#if !UNITY_2017_1_OR_NEWER
      action = () => throw new Exception("Test Send");
      task = scheduler.Send(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);
#endif // !UNITY_2017_1_OR_NEWER

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
    public void TestAsyncSend() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      var failed = false;

      scheduler.Post(() => { failed = true; scheduler.Stop(); }, 1000);
      scheduler.StartAsync();

      int count = 0;
      Action action = () => count++;
      IAction task = scheduler.Send(action);
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


#if !UNITY_2017_1_OR_NEWER
      action = () => throw new Exception("Test Send");
      task = scheduler.Send(action);
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);
#endif // !UNITY_2017_1_OR_NEWER

      Func<int> func = () => count++;
      ITask<int> taskInt = scheduler.Send(func);
      Assert.IsNotNull(taskInt);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 3);
      Assert.AreEqual(count, 4);

      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Send(funcObject, 2);
      Assert.IsNotNull(taskInt);
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

    [Test]
    public void TestTaskAsyncSend() {
      Scheduler scheduler = new Scheduler();
      checkCount = 0;
      var failed = false;

      scheduler.Post(() => { failed = true; scheduler.Stop(); }, 1000);
      scheduler.StartAsync();

      int count = 0;
      Action action = () => count++;
      var task = scheduler.Send(new Task(action));
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 1);

      Action<object> actionObject = (value) => count += (int)value;
      task = scheduler.Send(new Task(actionObject, 2));
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsFalse(task.IsFaulted);
      Assert.AreEqual(count, 3);


#if !UNITY_2017_1_OR_NEWER
      action = () => throw new Exception("Test Send");
      task = scheduler.Send(new Task(action));
      Assert.IsNotNull(task);
      Assert.IsTrue(task.IsCompleted);
      Assert.IsTrue(task.IsFaulted);
#endif // !UNITY_2017_1_OR_NEWER

      Func<int> func = () => count++;
      var taskInt = scheduler.Send(new Task<int>(func));
      Assert.IsNotNull(taskInt);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 3);
      Assert.AreEqual(count, 4);

      Func<object, int> funcObject = (value) => count += (int)value;
      taskInt = scheduler.Send(new Task<int>(funcObject, 2));
      Assert.IsNotNull(taskInt);
      Assert.IsTrue(taskInt.IsCompleted);
      Assert.IsFalse(taskInt.IsFaulted);
      Assert.AreEqual(taskInt.Result, 6);
      Assert.AreEqual(count, 6);

      for (int i = 0; i < 10; i++) {
        var t = scheduler.Send(new Task(() => checkCount++));
        Assert.IsNotNull(t);
        Assert.IsTrue(t.IsCompleted);
        Assert.IsFalse(t.IsFaulted);
        Assert.AreEqual(checkCount, i + 1);
      }
      task = scheduler.Send(new Task(() => scheduler.Stop()));
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
