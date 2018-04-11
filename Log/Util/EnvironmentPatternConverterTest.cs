using System;
using System.IO;
using System.Text;
using NUnit.Framework;

using Soyo.Base.Text;

namespace UnitTest.Base.Log.Util {
  [TestFixture]
  public class EnvironmentPatternConverterTest {
    private const string ENVIRONMENT_VARIABLE_NAME = "LOG4NET_TEST_TEMP";
    const string SYSTEM_LEVEL_VALUE = "SystemLevelEnvironmentValue";
    const string USER_LEVEL_VALUE = "UserLevelEnvironmentValue";
    const string PROCESS_LEVEL_VALUE = "ProcessLevelEnvironmentValue";

    [Test, Ignore("failed build release on .net core on linux")]
    public void SystemLevelEnvironmentVariable() {
      EnvironmentPatternConverter converter = new EnvironmentPatternConverter();
      try {
        Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, SYSTEM_LEVEL_VALUE, EnvironmentVariableTarget.Machine);
      } catch (System.Security.SecurityException) {
        Assert.Ignore("Test skipped as current user must not set system level environment variables");
      }

      converter.Option = ENVIRONMENT_VARIABLE_NAME;

      StringWriter sw = new StringWriter();
      converter.Convert(sw, null, null);

      Assert.AreEqual(SYSTEM_LEVEL_VALUE, sw.ToString(), "System level environment variable not expended correctly.");

      Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, null, EnvironmentVariableTarget.Machine);
    }

    [Test, Ignore("Unity can not set environment variables")]
    public void UserLevelEnvironmentVariable() {
      EnvironmentPatternConverter converter = new EnvironmentPatternConverter();
      Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, USER_LEVEL_VALUE, EnvironmentVariableTarget.User);

      converter.Option = ENVIRONMENT_VARIABLE_NAME;

      StringWriter sw = new StringWriter();
      converter.Convert(sw, null, null);

      Assert.AreEqual(USER_LEVEL_VALUE, sw.ToString(), "User level environment variable not expended correctly.");

      Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, null, EnvironmentVariableTarget.User);
    }

    [Test]
    public void ProcessLevelEnvironmentVariable() {
      EnvironmentPatternConverter converter = new EnvironmentPatternConverter();
      Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, PROCESS_LEVEL_VALUE);

      converter.Option = ENVIRONMENT_VARIABLE_NAME;

      StringWriter sw = new StringWriter();
      converter.Convert(sw, null, null);

      Assert.AreEqual(PROCESS_LEVEL_VALUE, sw.ToString(), "Process level environment variable not expended correctly.");

      Environment.SetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME, null);
    }

    private class EnvironmentPatternConverter {
      private object target = null;

      public EnvironmentPatternConverter() {
        target = Utils.CreateInstance("Soyo.Base.Text.EnvironmentPatternConverter");
      }

      public string Option {
        get { return Utils.GetProperty(target, "Option") as string; }
        set { Utils.SetProperty(target, "Option", value); }
      }

      public void Convert(TextWriter writer, IRender render, object state) {
        Type[] types = { typeof(TextWriter), typeof(IRender), typeof(object) };
        Utils.InvokeMethod(target, "format", types, writer, render, state);
      }
    }
  }
}