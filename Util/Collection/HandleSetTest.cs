using System.Threading;
using System.Collections.Generic;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class HandleSetTest {
    private const int CheckCount = 1000;
    private const int CheckMax = 1000;
    private const int MaskBegin = 0x10000;

    private static int Capacity(int size) {
      int capacity = HandleSet<int>.DEFAULT_SLOT_SIZE;
      while (size >= capacity) {
        capacity *= 2;
        if (capacity > HandleSet<int>.MAX_SLOT_SIZE) {
          Assert.IsFalse(true, "Capacity: error: size large than max slot size");
        }
      }
      return capacity;
    }

    [Test]
    public void TestCheck() {
      HandleSet<int> table = new HandleSet<int>();

      // unused id
      int id = table.GetUnusedId();
      Assert.AreEqual(id, 0, "id is not 0");

      bool used = false;
      int value = 0;
      bool result = false;
      int checkCount = CheckCount;

      // add
      for (int i = 0; i < CheckCount; i++) {
        id = table.Add(i);
        Assert.AreEqual(id, i, "id is not index");

        used = table.IsUsed(id);
        Assert.IsTrue(used, "used is not true");

        Assert.AreEqual(table.Count, i + 1, "Count is not i + 1");
      }

      Assert.AreEqual(table.Count, CheckCount, "Count is not 1000");
      Assert.AreEqual(table.Capacity, Capacity(CheckCount), "Capacity is not 4096");
      Assert.AreEqual(table.IdMask, Capacity(CheckCount) - 1, "IdMask is not 0xfff");

      table.Check();

      // unused id
      id = table.GetUnusedId();
      Assert.AreEqual(id, CheckCount, "id is not CheckCount");

      // get
      for (int i = 0; i < CheckCount; i++) {
        value = table.Get(i);
        Assert.AreEqual(value, i, "value is not index");
      }

      value = table.Get(CheckCount);
      Assert.AreEqual(value, 0, "value is not 0");

      table.Check();

      // try get
      for (int i = 0; i < CheckCount; i++) {
        result = table.TryGet(i, out value);
        Assert.IsTrue(result, "result is not true");
        Assert.AreEqual(value, i, "value is not index");
      }

      result = table.TryGet(CheckCount, out value);
      Assert.IsFalse(result, "result is not false");
      Assert.AreEqual(value, 0, "value is not 0");

      table.Check();

      // enumerator
      var t = table.GetEnumerator();
      while (t.MoveNext()) {
        checkCount--;
        Assert.AreEqual(t.Current, checkCount, "current is not check count");
      } 

      Assert.AreEqual(checkCount, 0, "check count is not 0");

      table.Check();

      // id mask
      for (int i = MaskBegin; i < MaskBegin + CheckCount; i++) {
        result = table.TryGet(i, out value);
        Assert.IsFalse(result, "result is not false");
        Assert.AreEqual(value, 0, "value is not 0");
      }

      for (int i = MaskBegin; i < MaskBegin + CheckCount; i++) {
        result = table.TryGet(table.MaskId(i), out value);
        Assert.IsTrue(result, "result is not true");
        Assert.AreEqual(value, i - MaskBegin, "value is not index");
      }

      // []
      for (int i = 0; i < CheckCount; i++) {
        value = table[i];
        Assert.AreEqual(value, i, "value is not index");
      }

      value = table[CheckCount];
      Assert.AreEqual(value, 0, "value is not 0");

      for (int i = 0; i < CheckCount; i++) {
        table[i] = CheckCount - i;

        value = table[i];
        Assert.AreEqual(value, CheckCount - i, "value is not CheckCount - i");
      }

      table[CheckCount] = CheckCount;
      value = table[CheckCount];
      Assert.AreEqual(value, 0, "value is not 0");

      used = table.IsUsed(CheckCount);
      Assert.IsFalse(used, "used is not false");

      table.Check();

      // remove
      for (int i = 0; i < CheckCount; i++) {
        result = table.Remove(i);
        Assert.IsTrue(result, "result is not true");

        used = table.IsUsed(i);
        Assert.IsFalse(used, "used is not false");

        value = table.Get(i);
        Assert.AreEqual(value, 0, "value is not 0");

        Assert.AreEqual(table.Count, CheckCount - i - 1, "Count is not CheckCount - i - 1");
      }

      Assert.AreEqual(table.Count, 0, "Count is not 0");
      Assert.AreEqual(table.Capacity, Capacity(CheckCount), "Capacity is not 4096");
      Assert.AreEqual(table.IdMask, Capacity(CheckCount) - 1, "IdMask is not 0xfff");

      table.Check();

      // unused id
      id = table.GetUnusedId();
      Assert.AreEqual(id, CheckCount - 1, "id is not CheckCount - 1");

      // set
      for (int i = 0; i < CheckCount; i++) {
        result = table.Set(i, i);
        Assert.IsTrue(result, "result is not true");

        used = table.IsUsed(i);
        Assert.IsTrue(used, "used is not true");

        value = table.Get(i);
        Assert.AreEqual(value, i, "value is not index");

        Assert.AreEqual(table.Count, i + 1, "Count is not i + 1");
      }
      Assert.AreEqual(table.Count, CheckCount, "Count is not 1000");
      Assert.AreEqual(table.Capacity, Capacity(CheckCount), "Capacity is not 4096");
      Assert.AreEqual(table.IdMask, Capacity(CheckCount) - 1, "IdMask is not 0xfff");

      table.Check();

      // shuffle
      table.Shuffle(CheckCount);
      table.Check();

      // clear
      table.Clear();
      Assert.AreEqual(table.Count, 0, "Count is not 0");
      Assert.AreEqual(table.Capacity, Capacity(CheckCount), "Capacity is not 4096");
      Assert.AreEqual(table.IdMask, Capacity(CheckCount) - 1, "IdMask is not 0xfff");

      table.Check();

    }
  }
}
