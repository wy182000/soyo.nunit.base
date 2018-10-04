using System.Collections.Generic;
using System.Threading;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class ObjectPoolTest {
    private class ObjectPoolable {
      public int value = 0;
    }

    private const int CheckCount = 1000;
    [Test]
    public void Test() {
      int createCount = 0;
      List<ObjectPoolable> objectList = new List<ObjectPoolable>();
      var pool = new ObjectPool<ObjectPoolable>();
      pool.OnCreate = (o) => createCount++;
      pool.OnDestroy = (o) => createCount--;

      for (int i = 0; i < CheckCount; i++) {
        pool.OnEnable = (o) => o.value = i;
        var value = pool.Allocate();
        Assert.AreEqual(value.value, i, "value should be " + i);

        objectList.Add(value);
      }

      Assert.AreEqual(pool.AllocateCount, CheckCount, "allocate count should be equal to check count.");
      Assert.AreEqual(pool.CacheCountMax, 0, "cache count max default is 0.");
      Assert.AreEqual(pool.CacheCount, 0, "cache count should be 0.");
      Assert.AreEqual(createCount, CheckCount, "create count should be equal to check count.");

      for (int i = 0; i < CheckCount / 2; i++) {
        var value = objectList[i];
        pool.OnDisable = (o) => o.value = 0;
        pool.Free(value);
        Assert.AreEqual(value.value, 0);
        objectList[i] = null;
      }

      Assert.AreEqual(pool.AllocateCount, CheckCount / 2, "allocate count should be equal to half check count.");
      Assert.AreEqual(pool.CacheCount, CheckCount / 2, "cache count should be equal to half check count.");
      Assert.AreEqual(createCount, CheckCount, "create count should be equal to check count.");

      pool.CacheCountMax = CheckCount / 4;
      Assert.AreEqual(pool.CacheCountMax, CheckCount / 4, "cache count max should be equal to quarter check count.");
      Assert.AreEqual(pool.CacheCount, CheckCount / 4, "cache count should be equal to quarter check count.");
      Assert.AreEqual(createCount, CheckCount / 2 + CheckCount / 4, "create count should be equal to 3 quarter check count.");

      pool.Clear();
      Assert.AreEqual(pool.CacheCount, 0, "cache count should be 0.");
      Assert.AreEqual(createCount, CheckCount / 2, "create count should be equal to half check count.");
    }
  }
}
