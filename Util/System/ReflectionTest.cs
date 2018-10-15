using System;
using System.Linq.Expressions;
using System.Reflection;

using Soyo.Base;
using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  public class ReflectionTest {
    private class TestClass {
      public static int SValue;
      public int Value;
      public int Property { get; set; }
      public void Func() => Value = 1;
      public static void SFunc() => SValue = 2;
      public int RFunc() => Value;
      public static int SRFunc() => SValue;
      public int PFunc(int value) { Value = value; return Value; }
      public static int SPFunc(int value) { SValue = value; return SValue; }
    }

    private struct TestStruct {
      public int Value;
      public int Property { get; set; }
    }

    [Test]
    public void TestMethod() {
      var testClass = new TestClass();
      var testClassType = typeof(TestClass);

      Action<object> func;
      var rt = Reflection.GetMethod(testClassType, "Func", out func);
      Assert.IsTrue(rt);

      func.Invoke(testClass);
      Assert.AreEqual(testClass.Value, 1);

      Action sfunc;
      rt = Reflection.GetMethod(testClassType, "SFunc", out sfunc);
      Assert.IsTrue(rt);

      sfunc.Invoke();
      Assert.AreEqual(TestClass.SValue, 2);

      Func<object, object> rfunc;
      rt = Reflection.GetMethod(testClassType, "RFunc", out rfunc);
      Assert.IsTrue(rt);

      testClass.Value = Rand.Default.RandInt();
      int value = (int)rfunc.Invoke(testClass);
      Assert.AreEqual(testClass.Value, value);

      Func<object> srfunc;
      rt = Reflection.GetMethod(testClassType, "SRFunc", out srfunc);
      Assert.IsTrue(rt);

      TestClass.SValue = Rand.Default.RandInt();
      value = (int)srfunc.Invoke();
      Assert.AreEqual(TestClass.SValue, value);

      dynamic pfunc;
      rt = Reflection.GetMethod(testClassType, "PFunc", out pfunc);
      Assert.IsTrue(rt);

      int pvalue = Rand.Default.RandInt();
      value = ((Func<object, int, int>)pfunc).Invoke(testClass, pvalue);
      Assert.AreEqual(testClass.Value, pvalue);
      Assert.AreEqual(value, pvalue);

      dynamic spfunc;
      rt = Reflection.GetMethod(testClassType, "SPFunc", out spfunc);
      Assert.IsTrue(rt);

      pvalue = Rand.Default.RandInt();
      value = ((Func<int, int>)spfunc).Invoke(pvalue);
      Assert.AreEqual(TestClass.SValue, pvalue);
      Assert.AreEqual(value, pvalue);

      Delegate pd;
      rt = Reflection.GetMethod(testClassType, "PFunc", out pd);
      Assert.IsTrue(rt);

      pvalue = Rand.Default.RandInt();
      value = (int)pd.DynamicInvoke(testClass, pvalue);
      Assert.AreEqual(testClass.Value, pvalue);
      Assert.AreEqual(value, pvalue);

      pvalue = Rand.Default.RandInt();
      value = (int)((Func<object, object, object>)pd).Invoke(testClass, pvalue);
      Assert.AreEqual(testClass.Value, pvalue);
      Assert.AreEqual(value, pvalue);

      Delegate spd;
      rt = Reflection.GetMethod(testClassType, "SPFunc", out spd);
      Assert.IsTrue(rt);

      pvalue = Rand.Default.RandInt();
      value = (int)spd.DynamicInvoke(pvalue);
      Assert.AreEqual(TestClass.SValue, pvalue);
      Assert.AreEqual(value, pvalue);

      pvalue = Rand.Default.RandInt();
      value = (int)((Func<object, object>)spd).Invoke(pvalue);
      Assert.AreEqual(TestClass.SValue, pvalue);
      Assert.AreEqual(value, pvalue);

      pvalue = Rand.Default.RandInt();
      value = (int)Reflection.Invoke(testClassType, "PFunc", testClass, pvalue);
      Assert.AreEqual(testClass.Value, pvalue);
      Assert.AreEqual(value, pvalue);

      pvalue = Rand.Default.RandInt();
      value = (int)Reflection.Invoke(testClassType, "SPFunc", null, pvalue);
      Assert.AreEqual(TestClass.SValue, pvalue);
      Assert.AreEqual(value, pvalue);
    }

    [Test]
    public void TestClassPropertyAndField() {
      var testClass = new TestClass();
      var testClassType = typeof(TestClass);

      var fieldInfo = testClassType.GetField("Value");
      Assert.IsNotNull(fieldInfo);

      var getter = Reflection.GetGetter(fieldInfo);
      testClass.Value = Rand.Default.RandInt();
      var value = getter(testClass);
      Assert.AreEqual(value, testClass.Value);

      var setter = Reflection.GetSetterClass(fieldInfo);
      value = Rand.Default.RandInt();
      setter(testClass, value);
      Assert.AreEqual(value, testClass.Value);

      testClass.Value = Rand.Default.RandInt();
      value = Reflection.Get(testClass, fieldInfo);
      Assert.AreEqual(value, testClass.Value);

      value = Rand.Default.RandInt();
      Reflection.Set(testClass, fieldInfo, value);
      Assert.AreEqual(value, testClass.Value);

      var propertyInfo = testClassType.GetProperty("Property");
      Assert.IsNotNull(fieldInfo);

      getter = Reflection.GetGetter(propertyInfo);
      testClass.Value = Rand.Default.RandInt();
      value = getter(testClass);
      Assert.AreEqual(value, testClass.Property);

      setter = Reflection.GetSetterClass(propertyInfo);
      value = Rand.Default.RandInt();
      setter(testClass, value);
      Assert.AreEqual(value, testClass.Property);

      testClass.Property = Rand.Default.RandInt();
      value = Reflection.Get(testClass, propertyInfo);
      Assert.AreEqual(value, testClass.Property);

      value = Rand.Default.RandInt();
      Reflection.Set(testClass, propertyInfo, value);
      Assert.AreEqual(value, testClass.Property);
    }

    [Test]
    public void TestStructPropertyAndField() {
      var testStruct = new TestStruct();
      var testStructType = typeof(TestStruct);

      var fieldInfo = testStructType.GetField("Value");
      Assert.IsNotNull(fieldInfo);

      var getter = Reflection.GetGetter(fieldInfo);
      testStruct.Value = Rand.Default.RandInt();
      var value = getter(testStruct);
      Assert.AreEqual(value, testStruct.Value);

      var setter = Reflection.GetSetterStruct(fieldInfo);
      value = Rand.Default.RandInt();
      object obj = testStruct;
      setter(obj, value);
      Assert.AreEqual(value, ((TestStruct)obj).Value);

      testStruct.Value = Rand.Default.RandInt();
      value = Reflection.Get(testStruct, fieldInfo);
      Assert.AreEqual(value, testStruct.Value);

      value = Rand.Default.RandInt();
      obj = testStruct;
      Reflection.Set(obj, fieldInfo, value);
      Assert.AreEqual(value, ((TestStruct)obj).Value);

      var propertyInfo = testStructType.GetProperty("Property");
      Assert.IsNotNull(fieldInfo);

      getter = Reflection.GetGetter(propertyInfo);
      testStruct.Value = Rand.Default.RandInt();
      value = getter(testStruct);
      Assert.AreEqual(value, testStruct.Property);

      setter = Reflection.GetSetterStruct(propertyInfo);
      value = Rand.Default.RandInt();
      obj = testStruct;
      setter(obj, value);
      Assert.AreEqual(value, ((TestStruct)obj).Property);

      testStruct.Property = Rand.Default.RandInt();
      value = Reflection.Get(testStruct, propertyInfo);
      Assert.AreEqual(value, testStruct.Property);

      value = Rand.Default.RandInt();
      obj = testStruct;
      Reflection.Set(obj, propertyInfo, value);
      Assert.AreEqual(value, ((TestStruct)obj).Property);
    }
  }
}
