using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestQuaternion {
    [Test]
    public void TestConstructor() {
      Quaternion q = new Quaternion();
      Assert.AreEqual(q.x, 0);
      Assert.AreEqual(q.y, 0);
      Assert.AreEqual(q.z, 0);
      Assert.AreEqual(q.w, 0);

      q = new Quaternion(1, 2, 3);
      Assert.AreEqual(q, new Quaternion(0.7549338f, -0.2061492f, 0.4444351f, 0.4359528f));

      q = new Quaternion(1, new Vector3(1, 2, 3));
      Assert.AreEqual(q, new Quaternion(0.1281319f, 0.2562637f, 0.3843956f, 0.8775826f));

      q = new Quaternion(new Vector3(-1, 2, -3), new Vector3(2, 3, 4));
      Assert.AreEqual(q, new Quaternion(0.7682884f, -0.09038687f, -0.316354f, 0.5490749f));
    }

    [Test]
    public void TestProperties() {
      Quaternion q = new Quaternion(1, 2, 3, 4);

      Assert.AreEqual(q.length, Mathf.Sqrt(30));

      q.Normalize();
      Assert.AreEqual(q.euler, new Vector3(-0.1337316f, 0.8329812f, 1.227772f));

      Assert.AreEqual(q.angle, 1.50408018f);

      Assert.AreEqual(q.axis, new Vector3(0.2672612f, 0.5345224f, 0.8017837f));

      Assert.AreEqual(q.right, new Vector3(0.1333333f, 0.9333333f, -0.3333333f));
      Assert.AreEqual(q.forward, new Vector3(0.7333333f, 0.1333334f, 0.6666667f));
      Assert.AreEqual(q.up, new Vector3(-0.6666667f, 0.3333333f, 0.6666667f));
    }

    [Test]
    public void TestFunctions() {
      Quaternion q = new Quaternion();

      q.SetIdentity();
      Assert.AreEqual(q, new Quaternion(0, 0, 0, 1));

      q = new Quaternion(1, 2, 3, 4);
      float l = q.length;

      q.Normalize();
      Assert.AreEqual(q, new Quaternion(1 / l, 2 / l, 3 / l, 4 / l));

      q = new Quaternion(1, 2, 3, 4);

      q.Inverse();
      Assert.AreEqual(q, new Quaternion(-1, -2, -3, 4));

      q.SetIdentity();
      q.SetEuler(-0.1337316f, 0.8329812f, 1.227772f);
      Assert.AreEqual(q, Quaternion.Normalize(new Quaternion(1, 2, 3, 4)));

      q.SetIdentity();
      q.SetAngleAxis(1.50408018f, new Vector3(0.2672612f, 0.5345224f, 0.8017837f));
      Assert.AreEqual(q, Quaternion.Normalize(new Quaternion(1, 2, 3, 4)));

      float angle;
      Vector3 axis = new Vector3();
      q.ToAngleAxis(out angle, out axis);
      Assert.AreEqual(angle, 1.50408018f);
      Assert.AreEqual(axis, new Vector3(0.2672612f, 0.5345224f, 0.8017837f));

      q.SetIdentity();
      q.SetFromTo(new Vector3(-1, 2, -3), new Vector3(2, 3, 4));
      Assert.AreEqual(q, new Quaternion(0.7682884f, -0.09038687f, -0.316354f, 0.5490749f));

      q.SetLookRotation(new Vector3(1, 2, 3));
      Assert.AreEqual(q, new Quaternion(-0.2746567f, 0.1538565f, 0.04457066f, 0.9481062f));
    }

    [Test]
    public void TestOperator() {
      Quaternion q1 = new Quaternion(1, 2, 3, 4);
      Quaternion q2 = new Quaternion(2, 3, 4, 5);

      Assert.IsTrue(q1 == (new Quaternion(1, 2, 3, 4)));
      Assert.IsFalse(q1 != (new Quaternion(1, 2, 3, 4)));
      Assert.IsTrue(q1 == q2);
      Assert.IsFalse(q1 != q2);
      Assert.IsTrue(q1.normalized == q1.normalized);
      Assert.IsFalse(q1.normalized != q1.normalized);
      Assert.IsTrue(q1.normalized != q2.normalized);
      Assert.IsFalse(q1.normalized == q2.normalized);

      Assert.AreEqual(q1 * q2, new Quaternion(12, 24, 30, 0));
      Assert.AreEqual(q1 / 2, new Quaternion(0.5f, 1, 1.5f, 2));
      
      Vector3 v = new Vector3(1, 2, 4);

      Assert.AreEqual((q1.normalized * v), new Vector3(1.733333f, 2.133333f, 3.666667f));

#if UNITY_5
      q1 = new Quaternion(1, 2, 3, 4);
      UnityEngine.Quaternion unityQ = q1;
      Assert.AreEqual(unityQ, new UnityEngine.Quaternion(1, 2, 3, 4));
      Quaternion q = unityQ;
      Assert.AreEqual(q, q1);
#endif
    }

    [Test]
    public void TestStaticProperties() {
      Assert.AreEqual(Quaternion.identity, new Quaternion(0, 0, 0, 1));
    }

    [Test]
    public void TestStaticFunctions() {
      Quaternion q1 = new Quaternion(1, 2, 3, 4);
      Quaternion q2 = new Quaternion(2, 3, 4, 5);

      Assert.IsTrue(Mathf.Equals(q1, new Quaternion(1.000009f, 2.000009f, 2.999991f, 4)));
      Assert.IsFalse(Mathf.Equals(q1, new Quaternion(1.00001f, 2.000009f, 2.999991f, 4)));
      Assert.IsFalse(Mathf.Equals(q1, new Quaternion(1.000009f, 2.00001f, 2.999991f, 4)));
      Assert.IsFalse(Mathf.Equals(q1, new Quaternion(1.000009f, 2.000009f, 2.99999f, 4)));
      Assert.IsFalse(Mathf.Equals(q1, new Quaternion(1.000009f, 2.000009f, 2.999991f, 4.00001f)));

      Assert.AreEqual(Quaternion.Length(q1), Mathf.Sqrt(30));

      float l = q1.length;
      Assert.AreEqual(Quaternion.Normalize(q1), new Quaternion(1 / l, 2 / l, 3 / l, 4 / l));

      Assert.AreEqual(Quaternion.Angle(q1, q2), 0f);
      Assert.AreEqual(Quaternion.Angle(q1.normalized, q2.normalized), 0.222681284f);

      Assert.AreEqual(Quaternion.Dot(q1, q2), 40);

      Assert.AreEqual(Quaternion.Inverse(q1), new Quaternion(-1, -2, -3, 4));

      Assert.AreEqual(Quaternion.Lerp(q1, q2, 0), new Quaternion(0.1825742f, 0.3651484f, 0.5477226f, 0.7302967f));
      Assert.AreEqual(Quaternion.Lerp(q1, q2, 0.5f), new Quaternion(0.2342606f, 0.3904344f, 0.5466082f, 0.7027819f));
      Assert.AreEqual(Quaternion.Lerp(q1, q2, 1f), new Quaternion(0.2721655f, 0.4082483f, 0.5443311f, 0.6804138f));

      Assert.AreEqual(Quaternion.Slerp(q1, q2, 0), q1);
      Assert.AreEqual(Quaternion.Slerp(q1, q2, 0.5f), new Quaternion(1.502327f, 2.503879f, 3.505431f, 4.506982f));
      Assert.AreEqual(Quaternion.Slerp(q1, q2, 1), q2);

      Assert.AreEqual(Quaternion.Euler(-0.1337316f, 0.8329812f, 1.227772f),
        Quaternion.Normalize(q1));

      Assert.AreEqual(Quaternion.Euler(new Vector3(-0.1337316f, 0.8329812f, 1.227772f)),
        Quaternion.Normalize(q1));

      Assert.AreEqual(Quaternion.AngleAxis(1.50408018f, new Vector3(0.2672612f, 0.5345224f, 0.8017837f)),
        Quaternion.Normalize(new Quaternion(1, 2, 3, 4)));

      Assert.AreEqual(Quaternion.FromToRotation(new Vector3(-1, 2, -3), new Vector3(2, 3, 4)),
        new Quaternion(0.7682884f, -0.09038687f, -0.316354f, 0.5490749f));

    /*
    public static Quaternion LookRotation(Vector3 forward);
    public static Quaternion LookRotation(Vector3 forward, Vector3 upwards);
    */
    }

    [Test]
    public void TestObject() {
      Quaternion q1 = new Quaternion(1, 2, 3, 4);
      Quaternion q2 = new Quaternion(2, 3, 4, 5);

      Assert.AreEqual(q1.GetHashCode(), (new Quaternion(1, 2, 3, 4)).GetHashCode());
      Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());

      object o = new object();
      Assert.AreNotEqual(q1, o);
      o = q1;
      Assert.AreEqual(q1, o);
      o = q2;
      Assert.AreNotEqual(q1, o);

      StringAssert.AreEqualIgnoringCase(q1.ToString(), "(1, 2, 3, 4)");
      StringAssert.AreNotEqualIgnoringCase(q1.ToString(), q2.ToString());
    }
  }
}