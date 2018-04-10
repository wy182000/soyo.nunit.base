using Soyo.Base.Profiler;

using NUnit.Framework;

namespace UnitTest.Base.Profiler {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class ProfilerTest {
    private static bool enableDefault;
    [SetUp]
    public void Init() {
      enableDefault = ProfilerManger.Enable;
      ProfilerManger.Enable = true;
    }

    [TearDown]
    public void Term() {
      ProfilerManger.Enable = enableDefault;
    }

    [Test]
    public void TestProfiler() {
#if !PROFILER
      return;
#endif
      Assert.IsTrue(ProfilerManger.Enable, "profiler should be enable.");
      Assert.IsNotNull(ProfilerManger.Storage, "profiler storage should not be null.");

      ISession session = null;
      ProfilerManger.Start(ref session, "session test");
      Assert.IsNotNull(session, "session should not be null");

      var parentId = ProfilerManger.Uuid.Current - 1;

      IStep step = null;
      ProfilerManger.Start(ref step, session, "step test");
      Assert.IsNotNull(step, "step should not be null");

      ITiming timing = step as ITiming;
      Assert.AreEqual("step test", timing.Name, "step name should be step test");
      Assert.AreEqual(ProfilerManger.Uuid.Current, timing.Id, "step id should be current uuid");
      Assert.AreEqual(parentId, timing.ParentId.Value, "step parent id should be current uuid");
      Assert.IsFalse(timing.IsStopped, "step is stopped should be false");
      Assert.IsFalse(timing.IsDiscarded, "step is discared should be false");
      Assert.Less(0, timing.StartTicks, "step start should be more than 0");
      Assert.AreEqual(0, timing.DurationTicks, "step duration should be 0");
      Assert.AreEqual(0, timing.TotalCount, "step total count should be 0");
      Assert.AreEqual(0, timing.TotalDuration, "step total durationi should be 0");

      ProfilerManger.Stop(step);
      Assert.IsTrue(timing.IsStopped, "step is stopped should be true");
      Assert.Less(0, timing.StartTicks, "step start should be more than 0");
      Assert.Less(0, timing.DurationTicks, "step duration should more than be 0");
      Assert.AreEqual(1, timing.TotalCount, "step total count should more than be 0");
      Assert.Less(0, timing.TotalDuration, "step total durationi should more than be 0");

      ProfilerManger.Start(ref step, session, "step test");
      Assert.AreEqual("step test", timing.Name, "step name should be step test");
      Assert.AreEqual(ProfilerManger.Uuid.Current, timing.Id, "step id should be current uuid");
      Assert.AreEqual(parentId, timing.ParentId.Value, "step parent id should be current uuid");
      Assert.IsFalse(timing.IsStopped, "step is stopped should be false");
      Assert.IsFalse(timing.IsDiscarded, "step is discared should be false");
      Assert.Less(0, timing.StartTicks, "step start should be more than 0");
      Assert.AreEqual(0, timing.DurationTicks, "step duration should more than be 0");
      Assert.AreEqual(1, timing.TotalCount, "step total count should more than be 0");
      Assert.Less(0, timing.TotalDuration, "step total durationi should more than be 0");

      ProfilerManger.Stop(step);
      Assert.IsTrue(timing.IsStopped, "step is stopped should be true");
      Assert.Less(0, timing.StartTicks, "step start should be more than 0");
      Assert.Less(0, timing.DurationTicks, "step duration should more than be 0");
      Assert.AreEqual(2, timing.TotalCount, "step total count should more than be 0");
      Assert.Less(0, timing.TotalDuration, "step total durationi should more than be 0");

      using (var disposable = ProfilerManger.Step(session, "step test")) {
        timing = disposable as ITiming;
        Assert.AreEqual("step test", timing.Name, "step name should be step test");
        Assert.AreEqual(ProfilerManger.Uuid.Current, timing.Id, "step id should be current uuid");
        Assert.AreEqual(parentId, timing.ParentId.Value, "step parent id should be current uuid");
        Assert.IsFalse(timing.IsStopped, "step is stopped should be false");
        Assert.IsFalse(timing.IsDiscarded, "step is discared should be false");
        Assert.Less(0, timing.StartTicks, "step start should be more than 0");
        Assert.AreEqual(0, timing.DurationTicks, "step duration should be 0");
        Assert.AreEqual(0, timing.TotalCount, "step total count should be 0");
        Assert.AreEqual(0, timing.TotalDuration, "step total durationi should be 0");
      }

      Assert.IsTrue(timing.IsStopped, "step is stopped should be true");
      Assert.Less(0, timing.StartTicks, "step start should be more than 0");
      Assert.Less(0, timing.DurationTicks, "step duration should more than be 0");
      Assert.AreEqual(1, timing.TotalCount, "step total count should more than be 0");
      Assert.Less(0, timing.TotalDuration, "step total durationi should more than be 0");
    }
  }
}
