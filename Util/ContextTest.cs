using System.Threading;
using System.Collections.Generic;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class ContextTest {
    private const int ThreadCount = 10;
    private static int lock_;
    private static int checkCount_;

    [OneTimeSetUp]
    public void Init() {
      lock_ = 0;
      checkCount_ = 0;
    }

    [Test]
    public void TestGlobalContext() {
      GlobalContext.Set("1", 1);
      GlobalContext.Set("2", 2);

      object value = GlobalContext.Get("1");
      Assert.AreEqual(value, 1, "value should be 1");

      value = GlobalContext.PropertySet["2"];
      Assert.AreEqual(value, 2, "value should be 2");

      bool result = GlobalContext.Contains("1");
      Assert.IsTrue(result, "resutl should be true");

      result = GlobalContext.Contains("2");
      Assert.IsTrue(result, "resutl should be true");

      result = GlobalContext.Contains("3");
      Assert.IsFalse(result, "resutl should be false");

      value = GlobalContext.Get("3");
      Assert.IsNull(value, "value should be null"); ;

      GlobalContext.Remove("1");

      result = GlobalContext.Contains("1");
      Assert.IsFalse(result, "resutl should be false");

      GlobalContext.Clear();

      result = GlobalContext.Contains("2");
      Assert.IsFalse(result, "resutl should be false");
    }

    private static void ThreadFunc(object state) {
      int index = (int)state;
      ThreadContext.Set((index + ThreadCount).ToString(), index);
      Atomic.Inc(ref lock_);

      while (Atomic.Cas(ref lock_, ThreadCount, ThreadCount) != ThreadCount) { Soyo.Base.Thread.Sleep(1); }

      bool result = ThreadContext.Contains("1");
      Assert.IsFalse(result, "value should be false"); ;

      result = ThreadContext.Contains("2");
      Assert.IsFalse(result, "value should be false"); ;

      for (int i = 0; i < ThreadCount; i++) {
        object value = ThreadContext.Get((i + ThreadCount).ToString());
        if (i == index) {
          Assert.AreEqual(value, index, "value should be index");
        } else {
          Assert.IsNull(value, "value should be null"); ;
        }
        Atomic.Inc(ref checkCount_);
      }
    }


    [Test]
    public void TestThreadContext() {
      ThreadContext.Set("1", 1);

      var set = ThreadContext.PropertySet;
      set["2"] = 2;

      object value = ThreadContext.Get("1");
      Assert.AreEqual(value, 1, "value should be 1");

      value = set["2"];
      Assert.AreEqual(value, 2, "value should be 2");

      bool result = ThreadContext.Contains("1");
      Assert.IsTrue(result, "resutl should be true");

      result = ThreadContext.Contains("2");
      Assert.IsTrue(result, "resutl should be true");

      result = ThreadContext.Contains("3");
      Assert.IsFalse(result, "resutl should be false");

      value = ThreadContext.Get("3");
      Assert.IsNull(value, "value should be null");

      ThreadContext.Remove("1");

      result = ThreadContext.Contains("1");
      Assert.IsFalse(result, "resutl should be false");

      ThreadContext.Clear();

      result = ThreadContext.Contains("2");
      Assert.IsFalse(result, "resutl should be false");

      ThreadContext.Set("1", 1);
      ThreadContext.Set("2", 2);
      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();

      for (int i = 0; i < ThreadCount; i++) {
        var thread = new System.Threading.Thread(ThreadFunc);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(i);
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }

      for (int i = 0; i < ThreadCount; i++) {
        value = ThreadContext.Get((i + ThreadCount).ToString());
        Assert.IsNull(value, "value should be null"); ;
      }

      Assert.AreEqual(checkCount_, ThreadCount * ThreadCount, "value should be ThreadCount * ThreadCount");
    }
  }
}
