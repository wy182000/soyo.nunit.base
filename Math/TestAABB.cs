using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestAABB {
    private AABB aabbOne;
    private AABB aabbHalf;
    private AABB aabbZeroTwo;
    private AABB aabbOneTwo;

    [OneTimeSetUp]
    public void Init() {
      aabbOne = new AABB(-Vector3.one, Vector3.one);
      aabbHalf = new AABB(-Vector3.half, Vector3.half);
      aabbZeroTwo = new AABB(Vector3.zero, Vector3.two);
      aabbOneTwo = new AABB(Vector3.one, Vector3.two);
    }

    [Test]
    public void TestConstructor() {
      AABB aabb = new AABB();
      Assert.AreEqual(aabb.min, Vector3.zero);
      Assert.AreEqual(aabb.max, Vector3.zero);

      aabb = new AABB(Vector3.one, Vector3.two);
      Assert.AreEqual(aabb.min, Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.two);
    }

    [Test]
    public void TestProperties() {
      AABB aabb = new AABB(Vector3.one, Vector3.two);

      Assert.AreEqual(aabb.center, 0.5f * (Vector3.one + Vector3.two));
      Assert.AreEqual(aabb.size, Vector3.one);
      Assert.AreEqual(aabb.extents, Vector3.half);
      Assert.AreEqual(aabb.min, Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.two);

      aabb.center = Vector3.zero;
      Assert.AreEqual(aabb.center, Vector3.zero);
      Assert.AreEqual(aabb.size, Vector3.one);
      Assert.AreEqual(aabb.extents, Vector3.half);
      Assert.AreEqual(aabb.min, -Vector3.half);
      Assert.AreEqual(aabb.max, Vector3.half);

      aabb.extents = Vector3.one;
      Assert.AreEqual(aabb.center, Vector3.zero);
      Assert.AreEqual(aabb.size, Vector3.two);
      Assert.AreEqual(aabb.extents, Vector3.one);
      Assert.AreEqual(aabb.min, -Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.one);
      
      aabb.size = Vector3.one;
      Assert.AreEqual(aabb.center, Vector3.zero);
      Assert.AreEqual(aabb.size, Vector3.one);
      Assert.AreEqual(aabb.extents, Vector3.half);
      Assert.AreEqual(aabb.min, -Vector3.half);
      Assert.AreEqual(aabb.max, Vector3.half);

      aabb.min = Vector3.zero;
      aabb.max = Vector3.one;
      Assert.AreEqual(aabb.center, Vector3.half);
      Assert.AreEqual(aabb.size, Vector3.one);
      Assert.AreEqual(aabb.extents, Vector3.half);
      Assert.AreEqual(aabb.min, Vector3.zero);
      Assert.AreEqual(aabb.max, Vector3.one);
    }

    [Test]
    public void TestEmpty() {
      Assert.IsFalse(aabbOne.IsEmpty());
      Assert.IsFalse(aabbHalf.IsEmpty());
      Assert.IsFalse(aabbZeroTwo.IsEmpty());
      Assert.IsFalse(aabbOneTwo.IsEmpty());

      AABB aabb = new AABB(new Vector3(1f, 2f, 3f), new Vector3(-1f, -2f, -3f));
      Assert.IsTrue(aabb.IsEmpty());
      aabb.Valid();
      Assert.IsFalse(aabb.IsEmpty());

      Assert.AreEqual(aabb.min, new Vector3(-1f, -2f, -3f));
      Assert.AreEqual(aabb.max, new Vector3(1f, 2f, 3f));
    }

    [Test]
    public void TestContain() {
      Assert.IsTrue(aabbOne.IsContain(Vector3.zero));
      Assert.IsTrue(aabbOne.IsContain(Vector3.one));
      Assert.IsTrue(aabbOne.IsContain(-Vector3.one));
      Assert.IsTrue(aabbOne.IsContain(Vector3.half));
      Assert.IsTrue(aabbOne.IsContain(-Vector3.half));
      Assert.IsFalse(aabbOne.IsContain(Vector3.two));
      Assert.IsFalse(aabbOne.IsContain(-Vector3.two));
      Assert.IsTrue(aabbOne.IsContain(aabbHalf));
      Assert.IsFalse(aabbOne.IsContain(aabbZeroTwo));
      Assert.IsFalse(aabbOne.IsContain(aabbOneTwo));

      Assert.IsTrue(aabbHalf.IsContain(Vector3.zero));
      Assert.IsFalse(aabbHalf.IsContain(Vector3.one));
      Assert.IsFalse(aabbHalf.IsContain(-Vector3.one));
      Assert.IsTrue(aabbHalf.IsContain(Vector3.half));
      Assert.IsTrue(aabbHalf.IsContain(-Vector3.half));
      Assert.IsFalse(aabbHalf.IsContain(Vector3.two));
      Assert.IsFalse(aabbHalf.IsContain(-Vector3.two));
      Assert.IsFalse(aabbHalf.IsContain(aabbOne));
      Assert.IsFalse(aabbHalf.IsContain(aabbZeroTwo));
      Assert.IsFalse(aabbHalf.IsContain(aabbOneTwo));

      Assert.IsTrue(aabbZeroTwo.IsContain(Vector3.zero));
      Assert.IsTrue(aabbZeroTwo.IsContain(Vector3.one));
      Assert.IsFalse(aabbZeroTwo.IsContain(-Vector3.one));
      Assert.IsTrue(aabbZeroTwo.IsContain(Vector3.half));
      Assert.IsFalse(aabbZeroTwo.IsContain(-Vector3.half));
      Assert.IsTrue(aabbZeroTwo.IsContain(Vector3.two));
      Assert.IsFalse(aabbZeroTwo.IsContain(-Vector3.two));
      Assert.IsFalse(aabbZeroTwo.IsContain(aabbOne));
      Assert.IsFalse(aabbZeroTwo.IsContain(aabbHalf));
      Assert.IsTrue(aabbZeroTwo.IsContain(aabbOneTwo));

      Assert.IsFalse(aabbOneTwo.IsContain(Vector3.zero));
      Assert.IsTrue(aabbOneTwo.IsContain(Vector3.one));
      Assert.IsFalse(aabbOneTwo.IsContain(-Vector3.one));
      Assert.IsFalse(aabbOneTwo.IsContain(Vector3.half));
      Assert.IsFalse(aabbOneTwo.IsContain(-Vector3.half));
      Assert.IsTrue(aabbOneTwo.IsContain(Vector3.two));
      Assert.IsFalse(aabbOneTwo.IsContain(-Vector3.two));
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

      AABB result;

      result = AABB.Intersect(aabbOne, aabbHalf);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbHalf.min);
      Assert.AreEqual(result.max, aabbHalf.max);
      result = AABB.Intersect(aabbOne, aabbZeroTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbZeroTwo.min);
      Assert.AreEqual(result.max, aabbOne.max);
      result = AABB.Intersect(aabbOne, aabbOneTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbOneTwo.min);
      Assert.AreEqual(result.max, aabbOne.max);
      result = AABB.Intersect(aabbHalf, aabbZeroTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbZeroTwo.min);
      Assert.AreEqual(result.max, aabbHalf.max);
      result = AABB.Intersect(aabbHalf, aabbOneTwo);
      Assert.IsNotNull(result);
      Assert.IsTrue(result.IsEmpty());
      result = AABB.Intersect(aabbZeroTwo, aabbOneTwo);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbOneTwo.min);
      Assert.AreEqual(result.max, aabbOneTwo.max);

      result = AABB.Intersect(aabbZeroTwo, aabbOneTwo.min, aabbOneTwo.max);
      Assert.IsNotNull(result);
      Assert.AreEqual(result.min, aabbOneTwo.min);
      Assert.AreEqual(result.max, aabbOneTwo.max);

      float distance = 0;
      bool ret = aabbOneTwo.Raycast(new Ray(new Vector3(-1f, 1f, 1f), new Vector3(1f, 0f, 0f)), out distance);
      Assert.IsTrue(ret);
      Assert.AreEqual(distance, 2f);
    }

    [Test]
    public void TestSet() {
      AABB aabb = new AABB(-Vector3.one, Vector3.one);

      aabb.Reset();
      Assert.IsTrue(aabb.IsEmpty());
      Assert.AreEqual(aabb.min, new Vector3(99999.0f, 99999.0f, 99999.0f));
      Assert.AreEqual(aabb.max, new Vector3(-99999.0f, -99999.0f, -99999.0f));

      aabb.Set(Vector3.one);
      Assert.AreEqual(aabb.min, Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.one);
    }

    [Test]
    public void TestCombine() {
      AABB aabb = new AABB();

      aabb.Combine(Vector3.one);
      Assert.AreEqual(aabb.min, Vector3.zero);
      Assert.AreEqual(aabb.max, Vector3.one);

      aabb.Combine(-Vector3.one);
      Assert.AreEqual(aabb.min, -Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.one);

      aabb.Reset();

      aabb.Combine(aabbOne);
      Assert.AreEqual(aabb.min, -Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.one);

      aabb.Combine(aabbOneTwo);
      Assert.AreEqual(aabb.min, -Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.two);

      aabb = AABB.Combine(Vector3.one, -Vector3.one);
      Assert.AreEqual(aabb.min, -Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.one);

      aabb = AABB.Combine(aabbOne, aabbOneTwo);
      Assert.AreEqual(aabb.min, -Vector3.one);
      Assert.AreEqual(aabb.max, Vector3.two);
    }
  }
}
