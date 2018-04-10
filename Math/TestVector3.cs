using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestVector3 {
    [Test]
    public void TestConstructor() {
      Vector3 v = new Vector3();
      Assert.AreEqual(v.x, 0);
      Assert.AreEqual(v.y, 0);
      Assert.AreEqual(v.z, 0);

      v = new Vector3(1, 2);
      Assert.AreEqual(v.x, 1);
      Assert.AreEqual(v.y, 0);
      Assert.AreEqual(v.z, 2);

      v = new Vector3(1, 2, 3);
      Assert.AreEqual(v.x, 1);
      Assert.AreEqual(v.y, 2);
      Assert.AreEqual(v.z, 3);
    }

    [Test]
    public void TestProperties() {
      Vector3 v = new Vector3(1, 2, 3);

      Assert.AreEqual(v.sqrMagnitude, 14);
      Assert.AreEqual(v.magnitude, Mathf.Sqrt(14));
      Assert.AreEqual(v.normalized, new Vector3(1 / v.magnitude, 2 / v.magnitude, 3 / v.magnitude));
      Assert.AreEqual(v[0], 1);
      Assert.AreEqual(v[1], 2);
      Assert.AreEqual(v[2], 3);
      v[0] = 2;
      v[1] = 1;
      v[2] = 0;
      Assert.AreEqual(v[0], 2);
      Assert.AreEqual(v[1], 1);
      Assert.AreEqual(v[2], 0);

      Assert.AreEqual(Vector3.zero, new Vector3(0, 0, 0));
      Assert.AreEqual(Vector3.half, new Vector3(0.5f, 0.5f, 0.5f));
      Assert.AreEqual(Vector3.one, new Vector3(1, 1, 1));
      Assert.AreEqual(Vector3.two, new Vector3(2, 2, 2));
      Assert.AreEqual(Vector3.right, new Vector3(1, 0, 0));
      Assert.AreEqual(Vector3.left, new Vector3(-1, 0, 0));
      Assert.AreEqual(Vector3.forward, new Vector3(0, 0, 1));
      Assert.AreEqual(Vector3.back, new Vector3(0, 0, -1));
      Assert.AreEqual(Vector3.up, new Vector3(0, 1, 0));
      Assert.AreEqual(Vector3.down, new Vector3(0, -1, 0));
    }

    [Test]
    public void TestOperator() {
      Vector3 v1 = new Vector3(1, 2, 3);
      Vector3 v2 = new Vector3(2, 3, 4);

      Assert.AreEqual(-v1, new Vector3(-1, -2, -3));

      Assert.AreEqual(v1 - v2, new Vector3(-1, -1, -1));

      Assert.IsFalse(v1 != new Vector3(1, 2, 3));
      Assert.IsTrue(v1 != new Vector3(2, 2, 3));
      Assert.IsTrue(v1 != new Vector3(1, 1, 3));
      Assert.IsTrue(v1 != new Vector3(1, 2, 1));

      Assert.IsTrue(v1 == new Vector3(1, 2, 3));
      Assert.IsFalse(v1 == new Vector3(2, 2, 3));
      Assert.IsFalse(v1 == new Vector3(1, 1, 3));
      Assert.IsFalse(v1 == new Vector3(1, 2, 1));

      Assert.AreEqual(v1 * 2, new Vector3(2, 4, 6));
      Assert.AreEqual(2 * v1, new Vector3(2, 4, 6));

      Assert.AreEqual(v1 / 2, new Vector3(0.5f, 1, 1.5f));

      Assert.AreEqual(v1 + v2, new Vector3(3, 5, 7));

#if UNITY_5
      UnityEngine.Vector3 unityV = v1;
      Assert.AreEqual(unityV, new UnityEngine.Vector3(1, 2, 3));
      Vector3 v = unityV;
      Assert.AreEqual(v, v1);
#endif
    }

    [Test]
    public void TestFunctions() {
      // member
      Vector3 v = new Vector3(1, 2, 3);
      float l = v.magnitude;

      v.Normalize();
      Assert.AreEqual(v, new Vector3(1 / l, 2 / l, 3 / l));

      v.Set(1, 0, 2);
      Assert.AreEqual(v, new Vector3(1, 0, 2));

      Assert.AreEqual(v.Rotate(Vector3.up, Mathf.PI), new Vector3(-1f, 0f, -2f));
      Assert.IsTrue(v.Rotate(Vector3.up, Mathf.PI) == new Vector3(-1f, 0f, -2f));

      Assert.AreEqual(v, new Vector3(1, 0, 2));

      // staitc
      Vector3 v1 = new Vector3(1, 2, 3);
      Vector3 v2 = new Vector3(2, 3, 4);

      Assert.IsTrue(Mathf.Equals(v1, new Vector3(1.000009f, 2.000009f, 2.999991f)));
      Assert.IsFalse(Mathf.Equals(v1, new Vector3(1.00001f, 2.000009f, 2.999991f)));
      Assert.IsFalse(Mathf.Equals(v1, new Vector3(1.000009f, 2.00001f, 2.999991f)));
      Assert.IsFalse(Mathf.Equals(v1, new Vector3(1.000009f, 2.000009f, 2.99999f)));

      Assert.AreEqual(Vector3.Angle(v1, v2), 0.121868052f);

      Assert.IsTrue(Vector3.Cross(v1, v2) == new Vector3(-1, 2, -1));

      Assert.AreEqual(Vector3.Distance(v1, v2), Mathf.Sqrt(3));

      Assert.AreEqual(Vector3.Dot(v1, v2), 20);

      Assert.AreEqual(Vector3.Lerp(v1, v2, 0), v1);
      Assert.AreEqual(Vector3.Lerp(v1, v2, 0.5f), new Vector3(1.5f, 2.5f, 3.5f));
      Assert.AreEqual(Vector3.Lerp(v1, v2, 1), v2);

      Assert.AreEqual(Vector3.SqrMagnitude(v1), 14);
      Assert.AreEqual(Vector3.Magnitude(v1), Mathf.Sqrt(14));
      Assert.AreEqual(Vector3.Normalize(v1), new Vector3(1 / v1.magnitude, 2 / v1.magnitude, 3 / v1.magnitude));

      Assert.AreEqual(Vector3.Max(v1, v2), v2);
      Assert.AreEqual(Vector3.Min(v1, v2), v1);
    }

    [Test]
    public void TestObject() {
      Vector3 v1 = new Vector3(1, 2, 3);
      Vector3 v2 = new Vector3(2, 3, 4);

      Assert.AreEqual(v1.GetHashCode(), (new Vector3(1, 2, 3)).GetHashCode());
      Assert.AreNotEqual(v1.GetHashCode(), v2.GetHashCode());

      object o = new object();
      Assert.AreNotEqual(v1, o);
      o = v1;
      Assert.AreEqual(v1, o);
      o = v2;
      Assert.AreNotEqual(v1, o);

      StringAssert.AreEqualIgnoringCase(v1.ToString(), "(1, 2, 3)");
      StringAssert.AreNotEqualIgnoringCase(v1.ToString(), v2.ToString());
    }
  }
}