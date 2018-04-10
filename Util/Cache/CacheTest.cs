using System;
using System.Collections.Generic;
using System.Threading;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Cache {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class CacheTest {
    private const int cache_data_count = 1000;
    private const int get_data_count = 10000;
    private const int thread_count = 10;
    private const string check_data =
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" + // size 78
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
      "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"; // line 32

    private static readonly int check_data_size = check_data.Length; // size = 2496 + 1

    [SetUp]
    public void Init() {
      Rand.Default.Seed = Clock.CurrentSeconds;
      expire_count = 0;
    }

    private static int expire_count = 0; 
    private static void expire_callback(Cache<string, string> cache, string key, string value) {
      expire_count++;
      Assert.AreEqual(key, "Key");
    }

    [Test]
    public void TestCachAPI() {
      var cache = new Cache<string, string>();
      Assert.IsNotNull(cache);

      cache.ExpireHandler += expire_callback;

      string key = "Key";
      int data_size = Rand.Default.Range(1, check_data_size);
      int cas = 0;

      bool ret = cache.Add(key, check_data.Substring(0, data_size), 0);
      Assert.IsTrue(ret);

      ret = cache.Add(key, check_data.Substring(0, data_size), 0);
      Assert.IsFalse(ret);

      string data;
      ret = cache.Get(key, out data, out cas);
      Assert.IsTrue(ret);
      Assert.IsNotNull(data);

      Assert.AreEqual(data.ToString(), check_data.Substring(0, data_size));
      Assert.AreEqual(cas, 1);

      data_size = Rand.Default.Range(1, check_data_size);

      int old_cas = cas;
      ret = cache.Set(key, check_data.Substring(0, data_size), ref cas);

      Assert.IsTrue(ret);
      Assert.Less(old_cas, cas);

      ret = cache.Set(key, check_data.Substring(0, data_size), ref old_cas);
      Assert.IsFalse(ret);

      ret = cache.Get(key, out data);
      Assert.IsTrue(ret);
      Assert.IsNotNull(data);
      Assert.AreEqual(data.ToString(), check_data.Substring(0, data_size));
      Assert.Less(old_cas, cas);

      ret = cache.Set(key, check_data.Substring(0, data_size), ref cas);
      Assert.IsTrue(ret);


      ret = cache.Touch(key, 0, ref cas);
      Assert.IsTrue(ret);

      ret = cache.Touch(key, 0, ref old_cas);
      Assert.IsFalse(ret);

      ret = cache.Touch(key, 1, ref cas);
      Assert.IsTrue(ret);

      Clock.Sleep(1200);

      ret = cache.Get(key, out data);
      Assert.IsFalse(ret);
      Assert.IsNull(data);

      Assert.AreEqual(expire_count, 1);

      ret = cache.Add(key, check_data.Substring(0, data_size));
      Assert.IsTrue(ret);

      ret = cache.Get(key, out data);
      Assert.IsTrue(ret);
      Assert.IsNotNull(data);

      ret = cache.Remove(key);
      Assert.IsTrue(ret);

      ret = cache.Get(key, out data);
      Assert.IsFalse(ret);
      Assert.IsNull(data);

      ret = cache.Add(key, check_data.Substring(0, data_size));
      Assert.IsTrue(ret);

      ret = cache.Get(key, out data);
      Assert.IsTrue(ret);
      Assert.IsNotNull(data);

      cache.Clear();

      ret = cache.Get(key, out data);
      Assert.IsFalse(ret);
      Assert.IsNull(data);

      // stats
      cache.StatsPrint();
    }

    [Test]
    public void TestCacheGetSet() {
      var cache = new Cache<string, string>();
      Assert.IsNotNull(cache);

      int data_size;
      int[] data_size_set = new int[cache_data_count];

      for (int i = 0; i < cache_data_count; i++) {
        data_size = Rand.Default.Range(1, check_data_size);
        data_size_set[i] = data_size;
        int cas = 0;
        bool ret = cache.Set(i.ToString(), check_data.Substring(0, data_size), ref cas);
        Assert.IsTrue(ret);
        Assert.Less(0, cas);
      }

      for (int i = 0; i < get_data_count; i++) {
        string value;
        int index = Rand.Default.RandInt(cache_data_count);
        bool ret = cache.Get(index.ToString(), out value);
        Assert.IsTrue(ret);
        Assert.AreEqual(value, check_data.Substring(0, data_size_set[index]));
      }

      cache.StatsPrint();
    }

    volatile static int[] data_size_set = new int[cache_data_count * thread_count];

    // thread add data
    void thread_add_data(object state) {
      var param = state as object[];
      Assert.IsNotNull(param);

      var cache = param[0] as Cache<int, string>;
      Assert.IsNotNull(cache);

      int index = (int)param[1];

      int data_size;

      for (int i = 0; i < cache_data_count; i++) {
        data_size = Rand.Default.Range(1, check_data_size);

        bool ret = cache.Add(cache_data_count * index + i, check_data.Substring(0, data_size));
        if (ret) {
          data_size_set[cache_data_count * index + i] = data_size;
        }
      }
    }

    // thread cas data
    void thread_cas_data(object state) {
      var param = state as object[];
      Assert.IsNotNull(param);

      var cache = param[0] as Cache<int, string>;
      Assert.IsNotNull(cache);

      int index = (int)param[1];

      string data;
      int cas;

      for (int i = 0; i < cache_data_count; i++) {
        bool ret = cache.Get(cache_data_count * index + i, out data, out cas);
        Assert.IsTrue(ret);
        Assert.AreEqual(data, check_data.Substring(0, data_size_set[cache_data_count * index + i]));

        ret = cache.Set(cache_data_count * index + i, check_data.Substring(0, data_size_set[cache_data_count * index + i]), ref cas);
        Assert.IsTrue(ret);
      }
    }

    // thread get data
    void thread_get_data(object state) {
      var param = state as object[];
      Assert.IsNotNull(param);

      var cache = param[0] as Cache<int, string>;
      Assert.IsNotNull(cache);

      int checkCount = (int)param[1];

      string data;

      for (int i = 0; i < get_data_count; i++) {
        int index = Rand.Default.RandInt(cache_data_count * checkCount);

        bool ret = cache.Get(index, out data);
        Assert.IsTrue(ret);
        Assert.AreEqual(data, check_data.Substring(0, data_size_set[index]));
      }
    }

    [Test]
    public void TestCacheSingeThread() {
      var cache = new Cache<int, string>();
      Assert.IsNotNull(cache);

      cache.LockFree = true;

      thread_add_data(new object[] { cache, 0 });
      thread_cas_data(new object[] { cache, 0 });
      thread_get_data(new object[] { cache, 1 });

      // stats
      cache.StatsPrint();
    }

    [Test]
    public void TestCacheMultiThread() {
      var cache = new Cache<int, string>();
      Assert.IsNotNull(cache);

      List<System.Threading.Thread> threadList = new List<System.Threading.Thread>();
      for (int i = 0; i < thread_count; i++) {
        var thread = new System.Threading.Thread(this.thread_add_data);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(new object[] { cache, i });
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }

      threadList.Clear();

      for (int i = 0; i < thread_count; i++) {
        var thread = new System.Threading.Thread(this.thread_cas_data);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(new object[] { cache, i });
      }

      for (int i = 0; i < thread_count; i++) {
        var thread = new System.Threading.Thread(this.thread_get_data);
        Assert.IsNotNull(thread, "value should not be null");
        threadList.Add(thread);
        thread.Start(new object[] { cache, thread_count });
      }

      for (int i = 0; i < threadList.Count; i++) {
        threadList[i].Join();
      }

      // stats
      cache.StatsPrint();
    }
  }
}
