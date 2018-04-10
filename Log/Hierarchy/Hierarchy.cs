﻿using System;
using System.Xml;

using Soyo.Base.Log;
using Soyo.Base.Log.Config;
using Soyo.Base.Log.Core;
using Soyo.Base.Log.Repository;
using Soyo.Base.Log.Repository.Hierarchy;
using Soyo.Base.Text;
using UnitTest.Base.Log.Appender;
using NUnit.Framework;

namespace UnitTest.Base.Log.Hierarchy {
  [TestFixture]
  public class Hierarchy {
    [Test]
    public void SetRepositoryPropertiesInConfigFile() {
      // LOG4NET-53: Allow repository properties to be set in the config file
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <property>
                    <key value=""two-plus-two"" />
                    <value value=""4"" />
                  </property>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.Appender.StringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                </Soyo.Base.Log>");

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      XmlConfigurator.Configure(rep, log4netConfig["Soyo.Base.Log"]);

      Assert.AreEqual("4", rep.Properties["two-plus-two"]);
      Assert.IsNull(rep.Properties["one-plus-one"]);
    }

    [Test]
    public void AddingMultipleAppenders() {
      CountingAppender alpha = new CountingAppender();
      CountingAppender beta = new CountingAppender();

      Soyo.Base.Log.Repository.Hierarchy.Hierarchy hierarchy =
          (Soyo.Base.Log.Repository.Hierarchy.Hierarchy)Utils.GetRepository();

      hierarchy.Root.AddAppender(alpha);
      hierarchy.Root.AddAppender(beta);
      hierarchy.Configured = true;

      ILog log = LogManager.GetLogger(GetType());
      log.Debug("Hello World");

      Assert.AreEqual(1, alpha.Counter);
      Assert.AreEqual(1, beta.Counter);
    }

    [Test]
    public void AddingMultipleAppenders2() {
      CountingAppender alpha = new CountingAppender();
      CountingAppender beta = new CountingAppender();

      BasicConfigurator.Configure(alpha, beta);

      ILog log = LogManager.GetLogger(GetType());
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
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.Appender.StringAppender"">
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

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      XmlConfigurator.Configure(rep, log4netConfig["Soyo.Base.Log"]);
    }

    [Test]
    public void LoggerNameCanConsistOfASingleNonDot() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.Appender.StringAppender"">
                    <layout type=""Soyo.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                  <logger name=""L"">
                    <level value=""WARN"" />
                  </logger>
                </Soyo.Base.Log>");

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      XmlConfigurator.Configure(rep, log4netConfig["Soyo.Base.Log"]);
    }

    [Test]
    public void LoggerNameCanContainSequenceOfDots() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.Appender.StringAppender"">
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

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      XmlConfigurator.Configure(rep, log4netConfig["Soyo.Base.Log"]);
    }
  }
}