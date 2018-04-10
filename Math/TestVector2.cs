using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestVector2 {
    [Test]
    public void TestConstructor() {
      Vector2 v = new Vector2();
      Assert.AreEqual(v.x, 0);
      Assert.AreEqual(v.y, 0);

      v = new Vector2(1, 2);
      Assert.AreEqual(v.x, 1);
      Assert.AreEqual(v.y, 2);
    }

    [Test]
    public void TestProperties() {
      Vector2 v = new Vector2(1, 2);

      Assert.AreEqual(v.sqrMagnitude, 5);
      Assert.AreEqual(v.magnitude, Mathf.Sqrt(5));
      Assert.AreEqual(v.normalized, new Vector2(1 / v.magnitude, 2 / v.magnitude));
      Assert.AreEqual(v[0], 1);
      Assert.AreEqual(v[1], 2);
      v[0] = 2;
      v[1] = 1;
      Assert.AreEqual(v[0], 2);
      Assert.AreEqual(v[1], 1);

      Assert.AreEqual(Vector2.zero, new Vector2(0, 0));
      Assert.AreEqual(Vector2.half, new Vector2(0.5f, 0.5f));
      Assert.AreEqual(Vector2.one, new Vector2(1, 1));
      Assert.AreEqual(Vector2.two, new Vector2(2, 2));
      Assert.AreEqual(Vector2.right, new Vector2(1, 0));
      Assert.AreEqual(Vector2.left, new Vector2(-1, 0));
      Assert.AreEqual(Vector2.forward, new Vector2(0, 1));
      Assert.AreEqual(Vector2.back, new Vector2(0, -1));
    }

    [Test]
    public void TestOperator() {
      Vector2 v1 = new Vector2(1, 2);
      Vector2 v2 = new Vector2(2, 3);

      Assert.AreEqual(-v1, new Vector2(-1, -2));

      Assert.AreEqual(v1 - v2, new Vector2(-1, -1));

      Assert.IsFalse(v1 != new Vector2(1, 2));
      Assert.IsTrue(v1 != new Vector2(2, 2));
      Assert.IsTrue(v1 != new Vector2(1, 1));

      Assert.IsTrue(v1 == new Vector2(1, 2));
      Assert.IsFalse(v1 == new Vector2(2, 2));
      Assert.IsFalse(v1 == new Vector2(1, 1));

      Assert.AreEqual(v1 * 2, new Vector2(2, 4));
      Assert.AreEqual(2 * v1, new Vector2(2, 4));

      Assert.AreEqual(v1 / 2, new Vector2(0.5f, 1));

      Assert.AreEqual(v1 + v2, new Vector2(3, 5));

#if UNITY_5
      UnityEngine.Vector2 unityV = v1;
      Assert.AreEqual(unityV, new UnityEngine.Vector2(1, 2));
      Vector2 v = unityV;
      Assert.AreEqual(v, v1);
#endif
    }

    [Test]
    public void TestFunctions() {
      // member
      Vector2 v = new Vector2(1, 2);
      float l = v.magnitude;

      v.Normalize();
      Assert.AreEqual(v, new Vector2(1 / l, 2 / l));

      v.Set(1, 0);
      Assert.AreEqual(v, new Vector2(1, 0));

      // staitc
      Vector2 v1 = new Vector2(1, 2);
      Vector2 v2 = new Vector2(2, 3);

      Assert.IsTrue(Mathf.Equals(v1, new Vector2(1.000009f, 2.000009f)));
      Assert.IsFalse(Mathf.Equals(v1, new Vector2(1.00001f, 2.000009f)));
      Assert.IsFalse(Mathf.Equals(v1, new Vector2(1.000009f, 2.00001f)));

      Assert.AreEqual(Vector2.Angle(v1, v2), 0.124354646f);

      Assert.AreEqual(Vector2.Cross(v1, v2), 1f);

      Assert.AreEqual(Vector2.Distance(v1, v2), Mathf.Sqrt(2));

      Assert.AreEqual(Vector2.Dot(v1, v2), 8f);

      Assert.AreEqual(Vector2.Lerp(v1, v2, 0f), v1);
      Assert.AreEqual(Vector2.Lerp(v1, v2, 0.5f), new Vector2(1.5f, 2.5f));
      Assert.AreEqual(Vector2.Lerp(v1, v2, 1f), v2);

      Assert.AreEqual(Vector2.SqrMagnitude(v1), 5);
      Assert.AreEqual(Vector2.Magnitude(v1), Mathf.Sqrt(5));
      Assert.AreEqual(Vector2.Normalize(v1), new Vector2(1 / v1.magnitude, 2 / v1.magnitude));

      Assert.AreEqual(Vector2.Max(v1, v2), v2);
      Assert.AreEqual(Vector2.Min(v1, v2), v1);
    }

    [Test]
    public void TestObject() {
      Vector2 v1 = new Vector2(1, 2);
      Vector2 v2 = new Vector2(2, 3);

      Assert.AreEqual(v1.GetHashCode(), (new Vector2(1, 2)).GetHashCode());
      Assert.AreNotEqual(v1.GetHashCode(), v2.GetHashCode());

      object o = new object();
      Assert.AreNotEqual(v1, o);
      o = v1;
      Assert.AreEqual(v1, o);
      o = v2;
      Assert.AreNotEqual(v1, o);

      StringAssert.AreEqualIgnoringCase(v1.ToString(), "(1, 2)");
      StringAssert.AreNotEqualIgnoringCase(v1.ToString(), v2.ToString());
    }
  }
}