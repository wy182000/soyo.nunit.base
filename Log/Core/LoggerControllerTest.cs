using System;
using System.Xml;

using Soyo.Base.Log;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class LoggerControllerTest {
    [Test]
    public void SetControllerPropertiesInXmlFile() {
      // LOG4NET-53: Allow repository properties to be set in the config file
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <property>
                    <key value=""two-plus-two"" />
                    <value value=""4"" />
                  </property>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.StringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                </Soyo.Base.Log>");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);

      Assert.AreEqual("4", rep.PropertySet["two-plus-two"]);
      Assert.IsNull(rep.PropertySet["one-plus-one"]);
    }

    [Test]
    public void AddingMultipleAppenders() {
      CountingAppender alpha = new CountingAppender();
      CountingAppender beta = new CountingAppender();

      ILoggerController hierarchy =
          Utils.GetController();

      hierarchy.Root.AddAppender(alpha);
      hierarchy.Root.AddAppender(beta);
      hierarchy.Initialize();

      ILog log = LogManager.Get(GetType());
      log.Debug("Hello World");

      Assert.AreEqual(1, alpha.Counter);
      Assert.AreEqual(1, beta.Counter);
    }

    [Test]
    public void AddingMultipleAppenders2() {
      CountingAppender alpha = new CountingAppender();
      CountingAppender beta = new CountingAppender();

      BasicConfig.Config(alpha, beta);

      ILog log = LogManager.Get(GetType());
      log.Debug("Hello World");

      Assert.AreEqual(1, alpha.Counter);
      Assert.AreEqual(1, beta.Counter);
    }

    [Test]
    // LOG4NET-343
    public void LoggerNameCanConsistOfASingleDot() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.StringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                  <logger name=""."">
                    <level value=""WARN"" />
                  </logger>
                </Soyo.Base.Log>");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);
    }

    [Test]
    public void LoggerNameCanConsistOfASingleNonDot() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.StringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                  <logger name=""L"">
                    <level value=""WARN"" />
                  </logger>
                </Soyo.Base.Log>");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);
    }

    [Test]
    public void LoggerNameCanContainSequenceOfDots() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.StringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                  <logger name=""L..M"">
                    <level value=""WARN"" />
                  </logger>
                </Soyo.Base.Log>");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);
    }
  }
}
