using Soyo.Base;
using Soyo.Base.Math;
using Soyo.Base.Map;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnitTest.Base.Map {
  [TestFixture]
  [Category("Soyo.Base.Map")]
  internal class TestMap {
    private Soyo.Base.Map.Map map;
    private Vector3 scope = new Vector3(32, 64, 128);

    [SetUp]
    public void Setup() {
      map = new Soyo.Base.Map.Map();
      map.data.Create(32, 32, 32, 1, 2, 4);
    }

    [Test]
    public void TestCreate() {
      Assert.AreEqual(map.aabb.min, Vector3.zero);
      Assert.AreEqual(map.aabb.max, scope);
      Assert.AreEqual(map.unitX, 1);
      Assert.AreEqual(map.unitY, 2);
      Assert.AreEqual(map.unitZ, 4);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, false);
      Assert.AreEqual(map.ignoreZ, false);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 32);
      Assert.AreEqual(map.sizeZ, 32);
    }

    [Test]
    public void TestIgnore() {
      map.data.Create(1, 32, 32);
      Assert.AreEqual(map.ignoreX, true);
      Assert.AreEqual(map.ignoreY, false);
      Assert.AreEqual(map.ignoreZ, false);
      Assert.AreEqual(map.sizeX, 1);
      Assert.AreEqual(map.sizeY, 32);
      Assert.AreEqual(map.sizeZ, 32);

      map.data.Create(32, 32, 32, 0, 4, 4);
      Assert.AreEqual(map.ignoreX, true);
      Assert.AreEqual(map.ignoreY, false);
      Assert.AreEqual(map.ignoreZ, false);
      Assert.AreEqual(map.sizeX, 1);
      Assert.AreEqual(map.sizeY, 32);
      Assert.AreEqual(map.sizeZ, 32);

      map.data.Create(32, 1, 32);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, true);
      Assert.AreEqual(map.ignoreZ, false);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 1);
      Assert.AreEqual(map.sizeZ, 32);

      map.data.Create(32, 32, 32, 4, 0, 4);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, true);
      Assert.AreEqual(map.ignoreZ, false);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 1);
      Assert.AreEqual(map.sizeZ, 32);

      map.data.Create(32, 32, 1);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, false);
      Assert.AreEqual(map.ignoreZ, true);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 32);
      Assert.AreEqual(map.sizeZ, 1);

      map.data.Create(32, 32, 32, 4, 4, 0);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, false);
      Assert.AreEqual(map.ignoreZ, true);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 32);
      Assert.AreEqual(map.sizeZ, 1);

      map.data.Create(32, 1, 1);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, true);
      Assert.AreEqual(map.ignoreZ, true);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 1);
      Assert.AreEqual(map.sizeZ, 1);

      map.data.Create(32, 32, 32, 4, 0, 0);
      Assert.AreEqual(map.ignoreX, false);
      Assert.AreEqual(map.ignoreY, true);
      Assert.AreEqual(map.ignoreZ, true);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 1);
      Assert.AreEqual(map.sizeZ, 1);
    }

    private void TestIntersect(List<MapBlock> list, AABB aabb) {
      for (int i = 0; i < list.Count; i++) {
        Assert.IsTrue(map.IsValidBlock(list[i]));
        Assert.IsTrue(map.BlockAABB(list[i]).IsIntersect(aabb));
      }
    }

    [Test]
    public void TestBlock() {
      MapBlock block;

      block = new MapBlock(1, 1, 1);

      Assert.AreEqual(map.BlockCenter(block), new Vector3(1.5f, 3f, 6f));
      Assert.AreEqual(map.BlockMin(block), new Vector3(1f, 2f, 4f));
      Assert.AreEqual(map.BlockMax(block), new Vector3(2f, 4f, 8f));
      Assert.AreEqual(map.BlockAABB(block), new AABB(new Vector3(1f, 2f, 4f), new Vector3(2f, 4f, 8f)));
      Assert.IsTrue(map.IsValidBlock(block));
      Assert.IsFalse(map.IsValidBlock(new MapBlock((int)map.sizeX, 0, 0)));
      Assert.IsFalse(map.IsValidBlock(new MapBlock(0, (int)map.sizeY, 0)));
      Assert.IsFalse(map.IsValidBlock(new MapBlock(0, 0, (int)map.sizeZ)));
      Assert.IsFalse(map.IsValidBlock(new MapBlock(-1, 0, 0)));
      Assert.IsFalse(map.IsValidBlock(new MapBlock(0, -1, 0)));
      Assert.IsFalse(map.IsValidBlock(new MapBlock(0, 0, -1)));

      map.SetBlockData(block, 0x12);
      Assert.AreEqual(map.BlockData(block), 0x12);

      map.SetBlockBit(block, 0xf0);
      Assert.AreEqual(map.BlockData(block), 0xf2);

      Assert.AreEqual(map.BlockData(new MapBlock(-1, 0, 0)), 0xff);

      block = map.Find(Vector3.zero);
      Assert.IsNotNull(block);
      Assert.IsTrue(map.IsValidBlock(block));
      Assert.AreEqual(map.BlockCenter(block), new Vector3(0.5f, 1, 2));
      Assert.AreEqual(map.BlockMin(block), new Vector3(0, 0, 0));
      Assert.AreEqual(map.BlockMax(block), new Vector3(1, 2, 4));

      block = map.Find(new Vector3(19, 45, 51));
      Assert.IsNotNull(block);
      Assert.IsTrue(map.IsValidBlock(block));
      Assert.AreEqual(map.BlockCenter(block), new Vector3(19.5f, 45, 50));
      Assert.AreEqual(map.BlockMin(block), new Vector3(19, 44, 48));
      Assert.AreEqual(map.BlockMax(block), new Vector3(20, 46, 52));

      block = map.Find(-Vector3.one);
      Assert.IsNotNull(block);
      Assert.IsFalse(map.IsValidBlock(block));

      // profile
      for (int i = 0; i < 10000; i++) {
        block = map.Find(new Vector3(Rand.Default.Range(0, scope.x), Rand.Default.Range(0, scope.y), Rand.Default.Range(0, scope.z)));
        Assert.IsNotNull(block);
      Assert.IsTrue(map.IsValidBlock(block));
      }

      AABB aabb = new AABB(Vector3.zero, Vector3.one);
      List<MapBlock> list;
      list = map.Find(aabb);
      Assert.IsNotNull(list);
      Assert.AreEqual(list.Count, 1);
      Assert.AreEqual(map.BlockMin(list[0]), new Vector3(0, 0, 0));
      TestIntersect(list, aabb);

      aabb = new AABB(new Vector3(4, 4, 4), new Vector3(8, 8, 8));
      list = map.Find(aabb);
      Assert.IsNotNull(list);
      Assert.AreEqual(list.Count, 4 * 2 * 1);
      TestIntersect(list, aabb);

      aabb = new AABB(new Vector3(4, 4, 4), new Vector3(12, 12, 12));
      list = map.Find(aabb);
      Assert.IsNotNull(list);
      Assert.AreEqual(list.Count, 8 * 4 * 2);
      TestIntersect(list, aabb);

      aabb = new AABB(new Vector3(32, 64, 128), new Vector3(32, 64, 128));
      list = map.Find(aabb);
      Assert.IsNotNull(list);
      Assert.AreEqual(list.Count, 0);

      // profile
      for (int i = 0; i < 1000; i++) {
        aabb = new AABB(Vector3.zero, new Vector3(Rand.Default.Range(0, scope.x), Rand.Default.Range(0, scope.y), Rand.Default.Range(0, scope.z)));
        list =  map.Find(aabb);
        Assert.IsNotNull(list);
        Assert.IsNotEmpty(list);
        //TestIntersect(list, aabb);
      }
    }
  }
}
