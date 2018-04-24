using System;

using Soyo.Base.Log;

using NUnit.Framework;

using System.Linq.Expressions;
using System.Reflection;

namespace UnitTest.Base.Log.Util {
  /// <summary>
  /// Used for internal unit testing the <see cref="SystemInfo"/> class.
  /// </summary>
  [TestFixture]
  public class SystemInfoTest {

    /// <summary>
    /// It's "does not throw not supported exception" NOT
    /// "returns 'Dynamic Assembly' string for dynamic assemblies" by purpose.
    /// <see cref="Assembly.GetCallingAssembly"/> can be JITted and inlined in different release configurations,
    /// thus we cannot determine what the exact result of this test will be.
    /// In 'Debug' GetCallingAssembly should return dynamic assembly named: 'Anonymously Hosted DynamicMethods Assembly'
    /// whereas in 'Release' this will be inlined and the result will be something like 'X:\Y\Z\UnitTest.Base.Log.dll'.
    /// Therefore simple check against dynamic assembly
    /// in <see cref="Soyo.Base.Log.SystemInfo.AssemblyLocationInfo"/> to avoid <see cref="NotSupportedException"/> 'Debug' release.
    /// </summary>
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
      return Soyo.Base.Log.SystemInfo.AssemblyLocationInfo(Assembly.GetCallingAssembly());
    }

    [Test]
    public void TestGetTypeFromStringFullyQualified() {
      Type t;

      t = GetTypeFromString("UnitTest.Base.Log.Util.SystemInfoTest", false, false);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case sensitive type load");

      t = GetTypeFromString("UnitTest.Base.Log.Util.SYSTEMINFOTEST", false, true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load caps");

      t = GetTypeFromString("UnitTest.Base.Log.Util.systeminfotest", false, true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load lower");
    }

    [Test]
    public void TestGetTypeFromStringRelative() {
      Type t;

      t = GetTypeFromString("UnitTest.Base.Log.Util.SystemInfoTest", false, false);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case sensitive type load");

      t = GetTypeFromString("UnitTest.Base.Log.Util.SYSTEMINFOTEST", false, true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load caps");

      t = GetTypeFromString("UnitTest.Base.Log.Util.systeminfotest", false, true);
      Assert.AreSame(typeof(SystemInfoTest), t, "Test explicit case in-sensitive type load lower");
    }

    [Test]
    public void TestGetTypeFromStringSearch() {
      Type t;

      t = GetTypeFromString("Soyo.Base.Log.SystemInfo", false, false);
      Assert.AreSame(typeof(SystemInfo), t,
                                       string.Format("Test explicit case sensitive type load found {0} rather than {1}",
                                                     t.AssemblyQualifiedName, typeof(SystemInfo).AssemblyQualifiedName));

      t = GetTypeFromString("Soyo.Base.Log.SYSTEMINFO", false, true);
      Assert.AreSame(typeof(SystemInfo), t, "Test explicit case in-sensitive type load caps");

      t = GetTypeFromString("Soyo.Base.Log.systeminfo", false, true);
      Assert.AreSame(typeof(SystemInfo), t, "Test explicit case in-sensitive type load lower");
    }

    // Wraps Soyo.Base.Log.SystemInfo.GetTypeFromString because the method relies on GetCallingAssembly, which is
    // unavailable in CoreFX. As a workaround, only overloads which explicitly take a Type or Assembly
    // are exposed for NETSTANDARD1_3.
    private Type GetTypeFromString(string typeName, bool throwOnError, bool ignoreCase) {
      return Soyo.Base.Log.SystemInfo.GetTypeFromString(typeName, throwOnError, ignoreCase);
    }

    [Test]
    public void EqualsIgnoringCase_BothNull_true() {
      Assert.True(Soyo.Base.Log.SystemInfo.EqualsIgnoringCase(null, null));
    }

    [Test]
    public void EqualsIgnoringCase_LeftNull_false() {
      Assert.False(Soyo.Base.Log.SystemInfo.EqualsIgnoringCase(null, "foo"));
    }

    [Test]
    public void EqualsIgnoringCase_RightNull_false() {
      Assert.False(Soyo.Base.Log.SystemInfo.EqualsIgnoringCase("foo", null));
    }

    [Test]
    public void EqualsIgnoringCase_SameStringsSameCase_true() {
      Assert.True(Soyo.Base.Log.SystemInfo.EqualsIgnoringCase("foo", "foo"));
    }

    [Test]
    public void EqualsIgnoringCase_SameStringsDifferentCase_true() {
      Assert.True(Soyo.Base.Log.SystemInfo.EqualsIgnoringCase("foo", "FOO"));
    }

    [Test]
    public void EqualsIgnoringCase_DifferentStrings_false() {
      Assert.False(Soyo.Base.Log.SystemInfo.EqualsIgnoringCase("foo", "foobar"));
    }
  }
}
