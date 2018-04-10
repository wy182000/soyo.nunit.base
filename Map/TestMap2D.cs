using Soyo.Base;
using Soyo.Base.Math;
using Soyo.Base.Map;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnitTest.Base.Map {
  [TestFixture]
  [Category("Soyo.Base.Map")]
  internal class TestMap2D {
    private Map2D map;
    private Vector2 scope = new Vector2(32, 128);

    [SetUp]
    public void Setup() {
      map = new Map2D();
      map.data.Create(32, 64, 1, 2);
    }

    [Test]
    public void TestCreate() {
      Assert.AreEqual(map.aabb.min, Vector2.zero);
      Assert.AreEqual(map.aabb.max, scope);
      Assert.AreEqual(map.unitX, 1);
      Assert.AreEqual(map.unitY, 2);
      Assert.AreEqual(map.sizeX, 32);
      Assert.AreEqual(map.sizeY, 64);
    }

    private void TestIntersect(List<int> list, AABB2D aabb) {
      for (int i = 0; i < list.Count; i++) {
        Assert.IsTrue(map.IsValidBlock(list[i]));
        Assert.IsTrue(map.BlockAABB(list[i]).IsIntersect(aabb));
      }
    }

    [Test]
    public void TestBlock() {
      Map2DBlock block;

      block = new Map2DBlock(1, 1);

      Assert.AreEqual(map.BlockCenter(block), new Vector2(1.5f, 3f));
      Assert.AreEqual(map.BlockMin(block), new Vector2(1f, 2f));
      Assert.AreEqual(map.BlockMax(block), new Vector2(2f, 4f));
      Assert.AreEqual(map.BlockAABB(block), new AABB2D(new Vector2(1f, 2f), new Vector2(2f, 4f)));
      Assert.AreEqual(map.BlockVertex(block, 0), new Vector2(1f, 2f));
      Assert.AreEqual(map.BlockVertex(block, 1), new Vector2(1f, 4f));
      Assert.AreEqual(map.BlockVertex(block, 2), new Vector2(2f, 4f));
      Assert.AreEqual(map.BlockVertex(block, 3), new Vector2(2f, 2f));
      Assert.AreEqual(map.BlockVertex(block, 4), Vector2.zero);

      Assert.IsTrue(map.IsValidBlock(block));
      Assert.IsTrue(map.IsValidBlock(new Map2DBlock((int)map.sizeX - 1, (int)map.sizeY - 1)));
      Assert.IsFalse(map.IsValidBlock(new Map2DBlock((int)map.sizeX, 0)));
      Assert.IsFalse(map.IsValidBlock(new Map2DBlock(0, (int)map.sizeY)));
      Assert.IsFalse(map.IsValidBlock(new Map2DBlock(-1, 0)));
      Assert.IsFalse(map.IsValidBlock(new Map2DBlock(0, -1)));

      Assert.AreEqual(map.ToValidBlock(new Map2DBlock((int)map.sizeX, 0)), new Map2DBlock((int)map.sizeX - 1, 0));

      Assert.AreEqual(map.BlockIndex(block), 33);
      Assert.AreEqual(map.IndexToBlock(35), new Map2DBlock(3, 1));

      Assert.IsTrue(map.IsValidHeight((int)map.sizeX, (int)map.sizeY));
      Assert.IsFalse(map.IsValidHeight((int)map.sizeX + 1, 0));
      Assert.IsFalse(map.IsValidHeight(0, (int)map.sizeY + 1));
      Assert.IsFalse(map.IsValidHeight(-1, 0));
      Assert.IsFalse(map.IsValidHeight(0, -1));

      Assert.AreEqual(map.HeightIndex(1, 1), 34);

      map.SetBlockData(block, 0x12);
      Assert.AreEqual(map.BlockData(block), 0x12);

      map.SetBlockBit(block, 0xf0);
      Assert.AreEqual(map.BlockData(block), 0xf2);

      Assert.AreEqual(map.BlockData(new Map2DBlock(-1, 0)), 0xff);

      map.SetHeight(0, 0, 1f);
      map.SetHeight(0, 1, 2f);
      map.SetHeight(1, 0, 4f);
      map.SetHeight(1, 1, 8f);

      Assert.AreEqual(map.GetHeight(0, 1), 2f);
      Assert.AreEqual(map.Find(new Vector2(0.2f, 1)), new Map2DBlock(0, 0));
      Assert.AreEqual(map.GetHeight(new Vector2(0.25f, 1f)), 2.625f);


      Assert.AreEqual(map.GetHeight(33, 0), 0f);
      Assert.AreEqual(map.GetHeight(new Vector2(33, 1)), 0f);

      block = map.Find(Vector2.zero);
      Assert.IsNotNull(block);
      Assert.IsTrue(map.IsValidBlock(block));
      Assert.AreEqual(map.BlockCenter(block), new Vector2(0.5f, 1));
      Assert.AreEqual(map.BlockMin(block), new Vector2(0, 0));
      Assert.AreEqual(map.BlockMax(block), new Vector2(1, 2));

      block = map.Find(new Vector2(19, 45));
      Assert.IsNotNull(block);
      Assert.IsTrue(map.IsValidBlock(block));
      Assert.AreEqual(map.BlockCenter(block), new Vector2(19.5f, 45));
      Assert.AreEqual(map.BlockMin(block), new Vector2(19, 44));
      Assert.AreEqual(map.BlockMax(block), new Vector2(20, 46));

      block = map.Find(-Vector2.one);
      Assert.IsNotNull(block);
      Assert.IsFalse(map.IsValidBlock(block));

      // profile
      for (int i = 0; i < 10000; i++) {
        block = map.Find(new Vector2(Rand.Default.Range(0, scope.x), Rand.Default.Range(0, scope.y)));
        Assert.IsNotNull(block);
        Assert.IsTrue(map.IsValidBlock(block));
      }

      AABB2D aabb = new AABB2D(Vector2.zero, Vector2.one);
      List<int> list = null;
      bool ret = map.Find(aabb, ref list);
      Assert.IsTrue(ret);
      Assert.AreEqual(list.Count, 1);
      Assert.AreEqual(map.BlockMin(list[0]), new Vector2(0, 0));
      TestIntersect(list, aabb);

      aabb = new AABB2D(new Vector2(4, 4), new Vector2(8, 8));
      ret = map.Find(aabb, ref list);
      Assert.IsTrue(ret);
      Assert.AreEqual(list.Count, 4 * 2);
      TestIntersect(list, aabb);

      aabb = new AABB2D(new Vector2(4, 4), new Vector2(12, 12));
      ret = map.Find(aabb, ref list);
      Assert.IsTrue(ret);
      Assert.AreEqual(list.Count, 8 * 4);
      TestIntersect(list, aabb);

      aabb = new AABB2D(new Vector2(32, 129), new Vector2(64, 200));
      ret = map.Find(aabb, ref list);
      Assert.IsFalse(ret);

      // profile
      for (int i = 0; i < 1000; i++) {
        aabb = new AABB2D(Vector2.zero, new Vector2(Rand.Default.Range(0, scope.x), Rand.Default.Range(0, scope.y)));
        ret = map.Find(aabb, ref list);
        Assert.IsTrue(ret);
        Assert.IsNotEmpty(list);
        //TestIntersect(list, aabb);
      }

      Vector2 p = Vector2.zero;
      int x, y;
      map.Find(p, out x, out y);
      Vector2 dir = Vector2.Normalize(new Vector2(2, 1));

      bool flag = true;
      int count = 1;
      float distance = 0;
      int checkX = x;
      int checkY = y;

      while (map.IsValidBlock(x, y)) {
        float t = 0;
        map.NextBlock(ref x, ref y, p, dir, ref t);
        checkX++;

        if ((checkX % 4) == 1) flag = true;

        if (flag) distance += Mathf.Sqrt(5f) / 2f;

        Assert.IsTrue(Mathf.Equals(t, distance));

        if (flag && (checkX % 4) == 0) { checkX--; checkY++; flag = false; }
        Assert.AreEqual(x, checkX);
        Assert.AreEqual(y, checkY);
        count++;
      }
      Assert.AreEqual(count, 41);

      Map2DHit hit;
      ret = map.Raycast(p, dir, 0x01, 100, out hit);
      Assert.IsTrue(ret);
      Assert.AreEqual(hit.x, 32);
      Assert.AreEqual(hit.y, 8);
      Assert.AreEqual(hit.distance, 32 / dir.x);

      map.SetBlockData(10, 2, 0x1);

      ret = map.Raycast(p, dir, 0x01, 100, out hit);
      Assert.IsTrue(ret);
      Assert.AreEqual(hit.x, 10);
      Assert.AreEqual(hit.y, 2);
      Assert.AreEqual(hit.distance, 10 / dir.x);
    }
  }
}
