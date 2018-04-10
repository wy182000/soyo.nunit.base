using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestQuaternion2D {
    [Test]
    public void TestConstructor() {
      Quaternion2D q = new Quaternion2D();
      Assert.AreEqual(q.y, 0);
      Assert.AreEqual(q.w, 0);
    }

    [Test]
    public void TestProperties() {
      Quaternion2D q = new Quaternion2D(2, 4);

      Assert.AreEqual(q.length, Mathf.Sqrt(20));
      q.Normalize();

      Assert.AreEqual(q.angle, 0.927295268f);
    }

    [Test]
    public void TestFunctions() {
      Quaternion2D q = new Quaternion2D();

      q.SetIdentity();
      Assert.AreEqual(q, new Quaternion2D(0, 1));

      q = new Quaternion2D(2, 4);
      float l = q.length;

      q.Normalize();
      Assert.AreEqual(q, new Quaternion2D(2 / l, 4 / l));

      q = new Quaternion2D(2, 4);

      q.Inverse();
      Assert.AreEqual(q, new Quaternion2D(-2, 4));

      q.SetIdentity();
      q.angle = 0.927295268f;
      Assert.AreEqual(q, Quaternion2D.Normalize(new Quaternion2D(2, 4)));

      q.SetIdentity();
      q.SetFromTo(new Vector2(-1, -3), new Vector2(2, 4));
      Assert.AreEqual(q, new Quaternion2D(-0.9974842f, 0.07088902f));

      q = Quaternion2D.LookRotation(new Vector2(1, 3));
      Assert.AreEqual(q, new Quaternion2D(0.1601822f, 0.9870874f));
    }

    [Test]
    public void TestOperator() {
      Quaternion2D q1 = new Quaternion2D(2, 4);
      Quaternion2D q2 = new Quaternion2D(3, 5);

      Assert.IsTrue(q1 == (new Quaternion2D(2, 4)));
      Assert.IsFalse(q1 != (new Quaternion2D(2, 4)));
      Assert.IsTrue(q1 == q2);
      Assert.IsFalse(q1 != q2);
      Assert.IsTrue(q1.normalized == q1.normalized);
      Assert.IsFalse(q1.normalized != q1.normalized);
      Assert.IsTrue(q1.normalized != q2.normalized);
      Assert.IsFalse(q1.normalized == q2.normalized);

      Assert.AreEqual(q1 * q2, new Quaternion2D( 22, 14));
      Assert.AreEqual(q1 / 2, new Quaternion2D(1, 2));
      
      Vector2 v = new Vector2(1, 4);

      Assert.AreEqual((q1.normalized * v), new Vector2(3.8f, 1.6f));
    }

    [Test]
    public void TestStaticProperties() {
      Assert.AreEqual(Quaternion2D.identity, new Quaternion2D(0, 1));
    }

    [Test]
    public void TestStaticFunctions() {
      Quaternion2D q1 = new Quaternion2D(2, 4);
      Quaternion2D q2 = new Quaternion2D(3, 5);

      Assert.IsTrue(Mathf.Equals(q1, new Quaternion2D(2.000009f, 4)));
      Assert.IsFalse(Mathf.Equals(q1, new Quaternion2D(2.00001f, 4)));
      Assert.IsFalse(Mathf.Equals(q1, new Quaternion2D(2.000009f, 4.00001f)));

      Assert.AreEqual(Quaternion2D.Length(q1), Mathf.Sqrt(20));

      float l = q1.length;
      Assert.AreEqual(Quaternion2D.Normalize(q1), new Quaternion2D(2 / l, 4 / l));

      Assert.AreEqual(Quaternion2D.Angle(q1, q2), 0f);
      Assert.AreEqual(Quaternion2D.Angle(q1.normalized, q2.normalized), 0.153542951f);

      Assert.AreEqual(Quaternion2D.Dot(q1, q2), 26);

      Assert.AreEqual(Quaternion2D.Inverse(q1), new Quaternion2D(-2, 4));

      Assert.AreEqual(Quaternion2D.Slerp(q1, q2, 0), q1);
      Assert.AreEqual(Quaternion2D.Slerp(q1, q2, 0.5f), new Quaternion2D(2.5f, 4.5f));
      Assert.AreEqual(Quaternion2D.Slerp(q1, q2, 1), q2);

      Assert.AreEqual(Quaternion2D.FromAngle(0.927295268f),
        Quaternion2D.Normalize(new Quaternion2D(2, 4)));

      Assert.AreEqual(Quaternion2D.FromToRotation(new Vector3(-1, -3), new Vector3(2, 4)),
        new Quaternion2D(-0.9974842f, 0.07088902f));
    }

    [Test]
    public void TestObject() {
      Quaternion2D q1 = new Quaternion2D(2, 4);
      Quaternion2D q2 = new Quaternion2D(3, 5);

      Assert.AreEqual(q1.GetHashCode(), (new Quaternion2D(2, 4)).GetHashCode());
      Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());

      object o = new object();
      Assert.AreNotEqual(q1, o);
      o = q1;
      Assert.AreEqual(q1, o);
      o = q2;
      Assert.AreNotEqual(q1, o);

      StringAssert.AreEqualIgnoringCase(q1.ToString(), "(2, 4)");
      StringAssert.AreNotEqualIgnoringCase(q1.ToString(), q2.ToString());

      Quaternion2D q = new Quaternion2D();
      q.angle = Mathf.PI * 0.3f;
      Rotation2D rot = new Rotation2D(Mathf.PI * 0.3f);
      Assert.AreEqual(q * Vector2.right, rot.x);
    }
  }
}