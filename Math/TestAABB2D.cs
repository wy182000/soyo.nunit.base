using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestAABB2D {
    private AABB2D aabbOne;
    private AABB2D aabbHalf;
    private AABB2D aabbZeroTwo;
    private AABB2D aabbOneTwo;

    [OneTimeSetUp]
    public void Init() {
      aabbOne = new AABB2D(-Vector2.one, Vector2.one);
      aabbHalf = new AABB2D(-Vector2.half, Vector2.half);
      aabbZeroTwo = new AABB2D(Vector2.zero, Vector2.two);
      aabbOneTwo = new AABB2D(Vector2.one, Vector2.two);
    }

    [Test]
    public void TestConstructor() {
      AABB2D aabb = new AABB2D();
      Assert.AreEqual(aabb.min, Vector2.zero);
      Assert.AreEqual(aabb.max, Vector2.zero);

      aabb = new AABB2D(Vector2.one, Vector2.two);
      Assert.AreEqual(aabb.min, Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.two);
    }

    [Test]
    public void TestProperties() {
      AABB2D aabb = new AABB2D(Vector2.one, Vector2.two);

      Assert.AreEqual(aabb.center, 0.5f * (Vector2.one + Vector2.two));
      Assert.AreEqual(aabb.size, Vector2.one);
      Assert.AreEqual(aabb.extents, Vector2.half);
      Assert.AreEqual(aabb.min, Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.two);

      aabb.center = Vector2.zero;
      Assert.AreEqual(aabb.center, Vector2.zero);
      Assert.AreEqual(aabb.size, Vector2.one);
      Assert.AreEqual(aabb.extents, Vector2.half);
      Assert.AreEqual(aabb.min, -Vector2.half);
      Assert.AreEqual(aabb.max, Vector2.half);

      aabb.extents = Vector2.one;
      Assert.AreEqual(aabb.center, Vector2.zero);
      Assert.AreEqual(aabb.size, Vector2.two);
      Assert.AreEqual(aabb.extents, Vector2.one);
      Assert.AreEqual(aabb.min, -Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.one);
      
      aabb.size = Vector2.one;
      Assert.AreEqual(aabb.center, Vector2.zero);
      Assert.AreEqual(aabb.size, Vector2.one);
      Assert.AreEqual(aabb.extents, Vector2.half);
      Assert.AreEqual(aabb.min, -Vector2.half);
      Assert.AreEqual(aabb.max, Vector2.half);

      aabb.min = Vector2.zero;
      aabb.max = Vector2.one;
      Assert.AreEqual(aabb.center, Vector2.half);
      Assert.AreEqual(aabb.size, Vector2.one);
      Assert.AreEqual(aabb.extents, Vector2.half);
      Assert.AreEqual(aabb.min, Vector2.zero);
      Assert.AreEqual(aabb.max, Vector2.one);
    }

    [Test]
    public void TestEmpty() {
      Assert.IsFalse(aabbOne.IsEmpty());
      Assert.IsFalse(aabbHalf.IsEmpty());
      Assert.IsFalse(aabbZeroTwo.IsEmpty());
      Assert.IsFalse(aabbOneTwo.IsEmpty());

      AABB2D aabb = new AABB2D(new Vector2(1f, 2f), new Vector2(-1f, -2f));
      Assert.IsTrue(aabb.IsEmpty());
      aabb.Valid();
      Assert.IsFalse(aabb.IsEmpty());

      Assert.AreEqual(aabb.min, new Vector2(-1f, -2f));
      Assert.AreEqual(aabb.max, new Vector2(1f, 2f));
    }

    [Test]
    public void TestContain() {
      Assert.IsTrue(aabbOne.IsContain(Vector2.zero));
      Assert.IsTrue(aabbOne.IsContain(Vector2.one));
      Assert.IsTrue(aabbOne.IsContain(-Vector2.one));
      Assert.IsTrue(aabbOne.IsContain(Vector2.half));
      Assert.IsTrue(aabbOne.IsContain(-Vector2.half));
      Assert.IsFalse(aabbOne.IsContain(Vector2.two));
      Assert.IsFalse(aabbOne.IsContain(-Vector2.two));
      Assert.IsTrue(aabbOne.IsContain(aabbHalf));
      Assert.IsFalse(aabbOne.IsContain(aabbZeroTwo));
      Assert.IsFalse(aabbOne.IsContain(aabbOneTwo));

      Assert.IsTrue(aabbHalf.IsContain(Vector2.zero));
      Assert.IsFalse(aabbHalf.IsContain(Vector2.one));
      Assert.IsFalse(aabbHalf.IsContain(-Vector2.one));
      Assert.IsTrue(aabbHalf.IsContain(Vector2.half));
      Assert.IsTrue(aabbHalf.IsContain(-Vector2.half));
      Assert.IsFalse(aabbHalf.IsContain(Vector2.two));
      Assert.IsFalse(aabbHalf.IsContain(-Vector2.two));
      Assert.IsFalse(aabbHalf.IsContain(aabbOne));
      Assert.IsFalse(aabbHalf.IsContain(aabbZeroTwo));
      Assert.IsFalse(aabbHalf.IsContain(aabbOneTwo));

      Assert.IsTrue(aabbZeroTwo.IsContain(Vector2.zero));
      Assert.IsTrue(aabbZeroTwo.IsContain(Vector2.one));
      Assert.IsFalse(aabbZeroTwo.IsContain(-Vector2.one));
      Assert.IsTrue(aabbZeroTwo.IsContain(Vector2.half));
      Assert.IsFalse(aabbZeroTwo.IsContain(-Vector2.half));
      Assert.IsTrue(aabbZeroTwo.IsContain(Vector2.two));
      Assert.IsFalse(aabbZeroTwo.IsContain(-Vector2.two));
      Assert.IsFalse(aabbZeroTwo.IsContain(aabbOne));
      Assert.IsFalse(aabbZeroTwo.IsContain(aabbHalf));
      Assert.IsTrue(aabbZeroTwo.IsContain(aabbOneTwo));

      Assert.IsFalse(aabbOneTwo.IsContain(Vector2.zero));
      Assert.IsTrue(aabbOneTwo.IsContain(Vector2.one));
      Assert.IsFalse(aabbOneTwo.IsContain(-Vector2.one));
      Assert.IsFalse(aabbOneTwo.IsContain(Vector2.half));
      Assert.IsFalse(aabbOneTwo.IsContain(-Vector2.half));
      Assert.IsTrue(aabbOneTwo.IsContain(Vector2.two));
      Assert.IsFalse(aabbOneTwo.IsContain(-Vector2.two));
      Assert.IsFalse(aabbOneTwo.IsContain(aabbOne));
      Assert.IsFalse(aabbOneTwo.IsContain(aabbHalf));
      Assert.IsFalse(aabbOneTwo.IsContain(aabbZeroTwo));
    }

    [Test]
    public void TestIntersect() {
      Assert.IsTrue(aabbOne.IsIntersect(aabbHalf));
      Assert.IsTrue(aabbOne.IsIntersect(aabbZeroTwo));
      Assert.IsTrue(aabbOne.IsIntersect(aabbOneTwo));

      Assert.IsTrue(aabbHalf.IsIntersect(aabbOne));
      Assert.IsTrue(aabbHalf.IsIntersect(aabbZeroTwo));
      Assert.IsFalse(aabbHalf.IsIntersect(aabbOneTwo));

      Assert.IsTrue(aabbZeroTwo.IsIntersect(aabbOne));
      Assert.IsTrue(aabbZeroTwo.IsIntersect(aabbHalf));
      Assert.IsTrue(aabbZeroTwo.IsIntersect(aabbOneTwo));

      Assert.IsTrue(aabbOneTwo.IsIntersect(aabbOne));
      Assert.IsFalse(aabbOneTwo.IsIntersect(aabbHalf));
      Assert.IsTrue(aabbOneTwo.IsIntersect(aabbZeroTwo));

      AABB2D result;

      result = AABB2D.Intersect(aabbOne, aabbHalf);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbHalf.min);
      Assert.AreEqual(result.max, aabbHalf.max);
      result = AABB2D.Intersect(aabbOne, aabbZeroTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbZeroTwo.min);
      Assert.AreEqual(result.max, aabbOne.max);
      result = AABB2D.Intersect(aabbOne, aabbOneTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbOneTwo.min);
      Assert.AreEqual(result.max, aabbOne.max);
      result = AABB2D.Intersect(aabbHalf, aabbZeroTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbZeroTwo.min);
      Assert.AreEqual(result.max, aabbHalf.max);
      result = AABB2D.Intersect(aabbHalf, aabbOneTwo);
      Assert.IsNotNull(result);
      Assert.IsTrue(result.IsEmpty());
      result = AABB2D.Intersect(aabbZeroTwo, aabbOneTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbOneTwo.min);
      Assert.AreEqual(result.max, aabbOneTwo.max);

      result = AABB2D.Intersect(aabbZeroTwo, aabbOneTwo.min, aabbOneTwo.max);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbOneTwo.min);
      Assert.AreEqual(result.max, aabbOneTwo.max);

      float distance = 0;
      bool ret = aabbOneTwo.Raycast(new Ray2D(new Vector2(-1f, 1f), new Vector2(1f, 0f)), out distance);
      Assert.IsTrue(ret);
      Assert.AreEqual(distance, 2f);
    }

    [Test]
    public void TestSet() {
      AABB2D aabb = new AABB2D(-Vector2.one, Vector2.one);

      aabb.Reset();
      Assert.IsTrue(aabb.IsEmpty());
      Assert.AreEqual(aabb.min, new Vector2(99999f, 99999f));
      Assert.AreEqual(aabb.max, new Vector2(-99999f, -99999f));

      aabb.Set(Vector2.one);
      Assert.AreEqual(aabb.min, Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.one);
    }

    [Test]
    public void TestCombine() {
      AABB2D aabb = new AABB2D();

      aabb.Combine(Vector2.one);
      Assert.AreEqual(aabb.min, Vector2.zero);
      Assert.AreEqual(aabb.max, Vector2.one);

      aabb.Combine(-Vector2.one);
      Assert.AreEqual(aabb.min, -Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.one);

      aabb.Reset();

      aabb.Combine(aabbOne);
      Assert.AreEqual(aabb.min, -Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.one);

      aabb.Combine(aabbOneTwo);
      Assert.AreEqual(aabb.min, -Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.two);

      aabb = AABB2D.Combine(Vector2.one, -Vector2.one);
      Assert.AreEqual(aabb.min, -Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.one);

      aabb = AABB2D.Combine(aabbOne, aabbOneTwo);
      Assert.AreEqual(aabb.min, -Vector2.one);
      Assert.AreEqual(aabb.max, Vector2.two);
    }
  }
}
