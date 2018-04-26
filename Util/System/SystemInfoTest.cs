using System;
using System.Linq.Expressions;
using System.Reflection;

using Soyo.Base;
using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  public class SystemInfoTest {
    [Test]
    public void TestAssemblyLocationInfoDoesNotThrowNotSupportedExceptionForDynamicAssembly() {
      var systemInfoAssemblyLocationMethod = GetAssemblyLocationInfoMethodCall();
      Assert.DoesNotThrow(() => systemInfoAssemblyLocationMethod());
    }

    private static Func<string> GetAssemblyLocationInfoMethodCall() {
      var method = typeof(SystemInfoTest).GetMethod("TestAssemblyLocationInfoMethod", new Type[0]);
      var methodCall = Expression.Call(null, method, new Expression[0]);
      return Expression.Lambda<Func<string>>(methodCall, new ParameterExpression[0]).Compile();
    }

    public static string TestAssemblyLocationInfoMethod() {
      return SystemInfo.LocationInfo(Assembly.GetCallingAssembly());
    }

    [Test]
    public void TestGetTypeFromStringFullyQualified() {
      Type t;

      t = GetTypeFromString("UnitTest.Base.Util.SystemInfoTest", false);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case sensitive type load");

      t = GetTypeFromString("UnitTest.Base.Util.SYSTEMINFOTEST", true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load caps");

      t = GetTypeFromString("UnitTest.Base.Util.systeminfotest", true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load lower");
    }

    [Test]
    public void TestGetTypeFromStringRelative() {
      Type t;

      t = GetTypeFromString("UnitTest.Base.Util.SystemInfoTest", false);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case sensitive type load");

      t = GetTypeFromString("UnitTest.Base.Util.SYSTEMINFOTEST", true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load caps");

      t = GetTypeFromString("UnitTest.Base.Util.systeminfotest", true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load lower");
    }

    [Test]
    public void TestGetTypeFromStringSearch() {
      Type t;

      t = GetTypeFromString("Soyo.Base.SystemInfo", false);
      Assert.AreSame(typeof(SystemInfo), t,
                                       string.Format("Test explicit case sensitive type load found {0} rather than {1}",
                                                     t.AssemblyQualifiedName, typeof(SystemInfo).AssemblyQualifiedName));

      t = GetTypeFromString("SOYO.BASE.SYSTEMINFO", true);
      Assert.AreSame(typeof(SystemInfo), t, "Test explicit case in-sensitive type load caps");

      t = GetTypeFromString("soyo.base.systeminfo", true);
      Assert.AreSame(typeof(SystemInfo), t, "Test explicit case in-sensitive type load lower");
    }

    // Wraps SystemInfo.GetTypeFromString because the method relies on GetCallingAssembly
    private Type GetTypeFromString(string typeName, bool ignoreCase) {
      return SystemInfo.GetType(typeName, ignoreCase);
    }

    [Test]
    public void EqualsIgnoringCase_BothNull_true() {
      Assert.True(SystemInfo.Equals(null, null));
    }

    [Test]
    public void EqualsIgnoringCase_LeftNull_false() {
      Assert.False(SystemInfo.Equals(null, "foo"));
    }

    [Test]
    public void EqualsIgnoringCase_RightNull_false() {
      Assert.False(SystemInfo.Equals("foo", null));
    }

    [Test]
    public void EqualsIgnoringCase_SameStringsSameCase_true() {
      Assert.True(SystemInfo.Equals("foo", "foo"));
    }

    [Test]
    public void EqualsIgnoringCase_SameStringsDifferentCase_true() {
      Assert.True(SystemInfo.Equals("foo", "FOO"));
    }

    [Test]
    public void EqualsIgnoringCase_DifferentStrings_false() {
      Assert.False(SystemInfo.Equals("foo", "foobar"));
    }
  }
}
