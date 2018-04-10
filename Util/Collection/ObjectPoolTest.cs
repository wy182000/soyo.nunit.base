using System.Collections.Generic;
using System.Threading;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class ObjectPoolTest {
    private class ObjectPoolable : IPoolable {
      public int value = 0;
      public void Reset() {
        value = 0;
      }
    }

    private const int CheckCount = 1000;
    [Test]
    public void Test() {
      List<ObjectPoolable> objectList = new List<ObjectPoolable>();
      var pool = new ObjectPool<ObjectPoolable>();

      for (int i = 0; i < CheckCount; i++) {
        var value = pool.Allocate();
        value.value = i;
        objectList.Add(value);
      }

      Assert.AreEqual(pool.AllocateCount, CheckCount, "allocate count should be equal to check count.");
      Assert.AreEqual(pool.CacheCount, 0, "cache count should be 0.");

      for (int i = 0; i < CheckCount / 2; i++) {
        var value = objectList[i];
        pool.Free(value);
        Assert.AreEqual(value.value, 0);
        objectList[i] = null;
      }

      Assert.AreEqual(pool.AllocateCount, CheckCount / 2, "allocate count should be equal to half check count.");
      Assert.AreEqual(pool.CacheCount, CheckCount / 2, "cache count should be equal to half check count.");

      pool.Clear();
      Assert.AreEqual(pool.CacheCount, 0, "cache count should be 0.");
    }
  }
}
