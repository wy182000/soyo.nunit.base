using Soyo.Base;
using Soyo.Base.Math;
using NUnit.Framework;

namespace UnitTest.Base.Maths {
  [TestFixture]
  [Category("Soyo.Base.Math")]
  internal class TestMatrix4x4 {
    [Test]
    public void TestConstructor() {
      Matrix4x4 m = new Matrix4x4();
      for (int x = 0; x < 4; x++) {
        for (int y = 0; y < 4; y++) {
          Assert.AreEqual(m[x, y], 0);
        }
      }

      m = new Matrix4x4(new Vector3(1, 2, 3), new Vector3(2, 3, 4), new Vector3(3, 4, 5));
      Assert.AreEqual(m[0, 0], 1);
      Assert.AreEqual(m[0, 1], 2);
      Assert.AreEqual(m[0, 2], 3);
      Assert.AreEqual(m[0, 3], 0);
      Assert.AreEqual(m[1, 0], 2);
      Assert.AreEqual(m[1, 1], 3);
      Assert.AreEqual(m[1, 2], 4);
      Assert.AreEqual(m[1, 3], 0);
      Assert.AreEqual(m[2, 0], 3);
      Assert.AreEqual(m[2, 1], 4);
      Assert.AreEqual(m[2, 2], 5);
      Assert.AreEqual(m[2, 3], 0);
      Assert.AreEqual(m[3, 0], 0);
      Assert.AreEqual(m[3, 1], 0);
      Assert.AreEqual(m[3, 2], 0);
      Assert.AreEqual(m[3, 3], 1);

      m = new Matrix4x4(new Vector3(1, 2, 3), new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6));
      Assert.AreEqual(m[0, 0], 1);
      Assert.AreEqual(m[0, 1], 2);
      Assert.AreEqual(m[0, 2], 3);
      Assert.AreEqual(m[0, 3], 0);
      Assert.AreEqual(m[1, 0], 2);
      Assert.AreEqual(m[1, 1], 3);
      Assert.AreEqual(m[1, 2], 4);
      Assert.AreEqual(m[1, 3], 0);
      Assert.AreEqual(m[2, 0], 3);
      Assert.AreEqual(m[2, 1], 4);
      Assert.AreEqual(m[2, 2], 5);
      Assert.AreEqual(m[2, 3], 0);
      Assert.AreEqual(m[3, 0], 4);
      Assert.AreEqual(m[3, 1], 5);
      Assert.AreEqual(m[3, 2], 6);
      Assert.AreEqual(m[3, 3], 1);
    }

    [Test]
    public void TestProperties() {
      Matrix4x4 m = new Matrix4x4(new Vector3(-1, -2, -3), new Vector3(-2, -3, -4), new Vector3(-3, -4, -5), new Vector3(-4, -5, -6));
      Assert.AreEqual(m[0, 0], -1);
      Assert.AreEqual(m[0, 1], -2);
      Assert.AreEqual(m[0, 2], -3);
      Assert.AreEqual(m[0, 3], 0);
      Assert.AreEqual(m[1, 0], -2);
      Assert.AreEqual(m[1, 1], -3);
      Assert.AreEqual(m[1, 2], -4);
      Assert.AreEqual(m[1, 3], 0);
      Assert.AreEqual(m[2, 0], -3);
      Assert.AreEqual(m[2, 1], -4);
      Assert.AreEqual(m[2, 2], -5);
      Assert.AreEqual(m[2, 3], 0);
      Assert.AreEqual(m[3, 0], -4);
      Assert.AreEqual(m[3, 1], -5);
      Assert.AreEqual(m[3, 2], -6);
      Assert.AreEqual(m[3, 3], 1);

      Assert.AreEqual(m[0, 0], m.m00);
      Assert.AreEqual(m[0, 1], m.m01);
      Assert.AreEqual(m[0, 2], m.m02);
      Assert.AreEqual(m[0, 3], m.m03);
      Assert.AreEqual(m[1, 0], m.m10);
      Assert.AreEqual(m[1, 1], m.m11);
      Assert.AreEqual(m[1, 2], m.m12);
      Assert.AreEqual(m[1, 3], m.m13);
      Assert.AreEqual(m[2, 0], m.m20);
      Assert.AreEqual(m[2, 1], m.m21);
      Assert.AreEqual(m[2, 2], m.m22);
      Assert.AreEqual(m[2, 3], m.m23);
      Assert.AreEqual(m[3, 0], m.m30);
      Assert.AreEqual(m[3, 1], m.m31);
      Assert.AreEqual(m[3, 2], m.m32);
      Assert.AreEqual(m[3, 3], m.m33);

      Assert.AreEqual(m[0], m.m00);
      Assert.AreEqual(m[1], m.m10);
      Assert.AreEqual(m[2], m.m20);
      Assert.AreEqual(m[3], m.m30);
      Assert.AreEqual(m[4], m.m01);
      Assert.AreEqual(m[5], m.m11);
      Assert.AreEqual(m[6], m.m21);
      Assert.AreEqual(m[7], m.m31);
      Assert.AreEqual(m[8], m.m02);
      Assert.AreEqual(m[9], m.m12);
      Assert.AreEqual(m[10], m.m22);
      Assert.AreEqual(m[11], m.m32);
      Assert.AreEqual(m[12], m.m03);
      Assert.AreEqual(m[13], m.m13);
      Assert.AreEqual(m[14], m.m23);
      Assert.AreEqual(m[15], m.m33);

      m[0, 0] = 1;
      m[0, 1] = 2;
      m[0, 2] = 3;
      m[1, 0] = 2;
      m[1, 1] = 3;
      m[1, 2] = 4;
      m[2, 0] = 3;
      m[2, 1] = 4;
      m[2, 2] = 5;
      m[3, 0] = 4;
      m[3, 1] = 5;
      m[3, 2] = 6;

      Assert.AreEqual(m[0, 0], 1);
      Assert.AreEqual(m[0, 1], 2);
      Assert.AreEqual(m[0, 2], 3);
      Assert.AreEqual(m[1, 0], 2);
      Assert.AreEqual(m[1, 1], 3);
      Assert.AreEqual(m[1, 2], 4);
      Assert.AreEqual(m[2, 0], 3);
      Assert.AreEqual(m[2, 1], 4);
      Assert.AreEqual(m[2, 2], 5);
      Assert.AreEqual(m[3, 0], 4);
      Assert.AreEqual(m[3, 1], 5);
      Assert.AreEqual(m[3, 2], 6);

      Assert.AreEqual(m.determinant, 0);
      Assert.AreEqual(m.rotDeterminant, 0);

      Matrix4x4 transpose = m.transpose;
      Assert.AreEqual(transpose[0, 0], 1);
      Assert.AreEqual(transpose[0, 1], 2);
      Assert.AreEqual(transpose[0, 2], 3);
      Assert.AreEqual(transpose[0, 3], 4);
      Assert.AreEqual(transpose[1, 0], 2);
      Assert.AreEqual(transpose[1, 1], 3);
      Assert.AreEqual(transpose[1, 2], 4);
      Assert.AreEqual(transpose[1, 3], 5);
      Assert.AreEqual(transpose[2, 0], 3);
      Assert.AreEqual(transpose[2, 1], 4);
      Assert.AreEqual(transpose[2, 2], 5);
      Assert.AreEqual(transpose[2, 3], 6);
      Assert.AreEqual(transpose[3, 0], 0);
      Assert.AreEqual(transpose[3, 1], 0);
      Assert.AreEqual(transpose[3, 2], 0);
      Assert.AreEqual(transpose[3, 3], 1);

      Assert.AreEqual(m.x, new Vector3(1, 2, 3));
      Assert.AreEqual(m.y, new Vector3(2, 3, 4));
      Assert.AreEqual(m.z, new Vector3(3, 4, 5));
    }

    [Test]
    public void TestFunctions() {
      Matrix4x4 m = new Matrix4x4();

      m.SetIdentity();

      Assert.AreEqual(m[0, 0], 1);
      Assert.AreEqual(m[0, 1], 0);
      Assert.AreEqual(m[0, 2], 0);
      Assert.AreEqual(m[0, 3], 0);
      Assert.AreEqual(m[1, 0], 0);
      Assert.AreEqual(m[1, 1], 1);
      Assert.AreEqual(m[1, 2], 0);
      Assert.AreEqual(m[1, 3], 0);
      Assert.AreEqual(m[2, 0], 0);
      Assert.AreEqual(m[2, 1], 0);
      Assert.AreEqual(m[2, 2], 1);
      Assert.AreEqual(m[2, 3], 0);
      Assert.AreEqual(m[3, 0], 0);
      Assert.AreEqual(m[3, 1], 0);
      Assert.AreEqual(m[3, 2], 0);
      Assert.AreEqual(m[3, 3], 1);
    }

    [Test]
    public void TestOperator() {
      Matrix4x4 m1 = new Matrix4x4(new Vector3(1, 2, 3), new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6));
      Matrix4x4 m2 = new Matrix4x4(new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6), new Vector3(5, 6, 7));

      Assert.IsTrue(m1 != m2);
      Assert.IsTrue(m2 != m1);

      Assert.IsFalse(m1 == m2);
      Assert.IsFalse(m2 == m1);
    }

    [Test]
    public void TestStaticProperties() {
      Matrix4x4 m = Matrix4x4.identity;

      Assert.AreEqual(m[0, 0], 1);
      Assert.AreEqual(m[0, 1], 0);
      Assert.AreEqual(m[0, 2], 0);
      Assert.AreEqual(m[0, 3], 0);
      Assert.AreEqual(m[1, 0], 0);
      Assert.AreEqual(m[1, 1], 1);
      Assert.AreEqual(m[1, 2], 0);
      Assert.AreEqual(m[1, 3], 0);
      Assert.AreEqual(m[2, 0], 0);
      Assert.AreEqual(m[2, 1], 0);
      Assert.AreEqual(m[2, 2], 1);
      Assert.AreEqual(m[2, 3], 0);
      Assert.AreEqual(m[3, 0], 0);
      Assert.AreEqual(m[3, 1], 0);
      Assert.AreEqual(m[3, 2], 0);
      Assert.AreEqual(m[3, 3], 1);
    }

    [Test]
    public void TestStaticFunctions() {
    }

    [Test]
    public void TestObject() {
      Matrix4x4 m1 = new Matrix4x4(new Vector3(1, 2, 3), new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6));
      Matrix4x4 m2 = new Matrix4x4(new Vector3(2, 3, 4), new Vector3(3, 4, 5), new Vector3(4, 5, 6), new Vector3(5, 6, 7));

      Assert.AreNotEqual(m1.GetHashCode(), m2.GetHashCode());

      object o = new object();
      Assert.AreNotEqual(m1, o);
      o = m1;
      Assert.AreEqual(m1, o);
      o = m2;
      Assert.AreNotEqual(m1, o);
    }
  }
}