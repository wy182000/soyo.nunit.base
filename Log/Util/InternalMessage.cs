using System;
using System.Collections.Generic;
using System.Xml;

using Soyo.Base.Log;
using Soyo.Base.Text;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class InternalMessage {
    [Test]
    public void InternalMessageTest() {
      try {
        Soyo.Base.Log.LogInternal.InternalMessage = false;
        Soyo.Base.Log.LogInternal.InternalDebug = true;

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

        ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
        rep.ChangeConfigEvent += new LoggerControllerEventHandler(rep_ConfigurationChanged);

        ICollection<LogInternal> configurationMessages = XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);

        Assert.IsTrue(configurationMessages.Count > 0);
      } finally {
        Soyo.Base.Log.LogInternal.InternalMessage = true;
        Soyo.Base.Log.LogInternal.InternalDebug = false;
      }
    }

    static void rep_ConfigurationChanged(object sender, EventArgs e) {
      LoggerControllerEventArgs configChanged = (LoggerControllerEventArgs)e;

      Assert.IsTrue(configChanged.Controller.InteralMessage.Count > 0);
    }
  }

  public class LogLogAppender : AppenderBase {
    private readonly static Type declaringType = typeof(LogLogAppender);

    public override void Activate() {
      Soyo.Base.Log.LogInternal.Debug(declaringType, "Debug - Activating options...");
      Soyo.Base.Log.LogInternal.Warn(declaringType, "Warn - Activating options...");
      Soyo.Base.Log.LogInternal.Error(declaringType, "Error - Activating options...");

      base.Activate();
    }

    protected override void append(IRender render, object loggingEvent) {
      Soyo.Base.Log.LogInternal.Debug(declaringType, "Debug - Appending...");
      Soyo.Base.Log.LogInternal.Warn(declaringType, "Warn - Appending...");
      Soyo.Base.Log.LogInternal.Error(declaringType, "Error - Appending...");
    }
  }
}
