using System;
using System.Collections;
using System.Xml;

using Soyo.Base.Log;
using Soyo.Base.Text;
using Soyo.Base.Log.Config;
using Soyo.Base.Log.Core;
using Soyo.Base.Log.Repository;
using Soyo.Base.Log.Util;
using NUnit.Framework;

namespace UnitTest.Base.Log.LoggerRepository {
  [TestFixture]
  public class ConfigurationMessages {
    [Test]
    public void ConfigurationMessagesTest() {
      try {
        LogLog.EmitInternalMessages = false;
        LogLog.InternalDebugging = true;

        XmlDocument log4netConfig = new XmlDocument();
        log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""LogLogAppender"" type=""UnitTest.Base.Log.LoggerRepository.LogLogAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <appender name=""AppenderMemory"" type=""Soyo.Base.Text.AppenderMemory"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""LogLogAppender"" />
                    <appender-ref ref=""AppenderMemory"" />
                  </root>  
                </Soyo.Base.Log>");

        ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
        rep.ConfigurationChanged += new LoggerRepositoryConfigurationChangedEventHandler(rep_ConfigurationChanged);

        ICollection configurationMessages = XmlConfigurator.Configure(rep, log4netConfig["Soyo.Base.Log"]);

        Assert.IsTrue(configurationMessages.Count > 0);
      } finally {
        LogLog.EmitInternalMessages = true;
        LogLog.InternalDebugging = false;
      }
    }

    static void rep_ConfigurationChanged(object sender, EventArgs e) {
      ConfigurationChangedEventArgs configChanged = (ConfigurationChangedEventArgs)e;

      Assert.IsTrue(configChanged.ConfigurationMessages.Count > 0);
    }
  }

  public class LogLogAppender : AppenderBase {
    private readonly static Type declaringType = typeof(LogLogAppender);

    public override void Activate() {
      LogLog.Debug(declaringType, "Debug - Activating options...");
      LogLog.Warn(declaringType, "Warn - Activating options...");
      LogLog.Error(declaringType, "Error - Activating options...");

      base.Activate();
    }

    protected override void append(IRender render, object loggingEvent) {
      LogLog.Debug(declaringType, "Debug - Appending...");
      LogLog.Warn(declaringType, "Warn - Appending...");
      LogLog.Error(declaringType, "Error - Appending...");
    }
  }
}
