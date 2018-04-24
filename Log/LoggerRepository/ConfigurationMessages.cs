using System;
using System.Collections;
using System.Xml;

using Soyo.Base.Log;
using Soyo.Base.Text;
using NUnit.Framework;

namespace UnitTest.Base.Log.LoggerRepository {
  [TestFixture]
  public class ConfigurationMessages {
    [Test]
    public void ConfigurationMessagesTest() {
      try {
        Soyo.Base.Log.Log.InternalMessage = false;
        Soyo.Base.Log.Log.InternalDebug = true;

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

        IRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
        rep.ChangeConfigEvent += new ChangeConfigEventHandler(rep_ConfigurationChanged);

        ICollection configurationMessages = XmlConfigurator.Configure(rep, log4netConfig["Soyo.Base.Log"]);

        Assert.IsTrue(configurationMessages.Count > 0);
      } finally {
        Soyo.Base.Log.Log.InternalMessage = true;
        Soyo.Base.Log.Log.InternalDebug = false;
      }
    }

    static void rep_ConfigurationChanged(object sender, EventArgs e) {
      ChangeConfigEventArgs configChanged = (ChangeConfigEventArgs)e;

      Assert.IsTrue(configChanged.Message.Count > 0);
    }
  }

  public class LogLogAppender : AppenderBase {
    private readonly static Type declaringType = typeof(LogLogAppender);

    public override void Activate() {
      Soyo.Base.Log.Log.Debug(declaringType, "Debug - Activating options...");
      Soyo.Base.Log.Log.Warn(declaringType, "Warn - Activating options...");
      Soyo.Base.Log.Log.Error(declaringType, "Error - Activating options...");

      base.Activate();
    }

    protected override void append(IRender render, object loggingEvent) {
      Soyo.Base.Log.Log.Debug(declaringType, "Debug - Appending...");
      Soyo.Base.Log.Log.Warn(declaringType, "Warn - Appending...");
      Soyo.Base.Log.Log.Error(declaringType, "Error - Appending...");
    }
  }
}
