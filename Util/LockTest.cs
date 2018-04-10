using System.Collections.Generic;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class LockTest {
    private static int RWLockCount;
    private static int SpinLockCount;
    private static RWLock rwLocker;
    private static SpinLock spinLocker;
    private const int ThreadCount = 10;

    [OneTimeSetUp]
    public void Init() {
      RWLockCount = 0;
      SpinLockCount = 0;

      rwLocker = new RWLock();
      spinLocker = new SpinLock();
    }

    private static void RWLockFunc() {
      for (int i = 0; i < 1000; i++) {
        rwLocker.WLock();
        RWLockCount++;
        rwLocker.WUnlock();
      }
    }

    private static void RWLockWriterFunc() {
      for (int i = 0; i < 1000; i++) {
        using (rwLocker.Writer()) {
          RWLockCount++;
        }
      }
    }

    private static void SpinLockFunc() {
      for (int i = 0; i < 100000; i++) {
        spinLocker.Lock();
        SpinLockCount++;
        spinLocker.Unlock();
      }
    }

    [Test]
    public void TestRWLock() {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(RWLockFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start();
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }

      Assert.AreEqual(10000, RWLockCount, "lock count should be 10000");
    }

    public void TestRWLockWriter() {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(RWLockWriterFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start();
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }

      Assert.AreEqual(10000, RWLockCount, "lock count should be 10000");
    }

    [Test]
    public void TestSpinLock() {
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(SpinLockFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start();
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }

      Assert.AreEqual(1000000, SpinLockCount, "lock count should be 1000000");
    }
  }
}
