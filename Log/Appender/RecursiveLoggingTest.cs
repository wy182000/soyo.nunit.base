using System;
using Soyo.Base.LogBase;
using NUnit.Framework;
using System.Globalization;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class RecursiveLoggingTest {
    private EventRaisingAppender eventRaisingAppender_;
    private ILoggerController controller_;
    private int eventCount_;
    private ILogger logger_;
    private const int MaxRecursion = 3;

    private void SetupController() {
      controller_ = new LoggerController();

      eventRaisingAppender_ = new EventRaisingAppender();
      eventRaisingAppender_.LoggingEventAppended += eventRaisingAppender_LoggingEventAppended;

      controller_.Root.Level = Level.All;
      controller_.Root.AddAppender(eventRaisingAppender_);

      BasicConfig.Config(controller_, eventRaisingAppender_);
      logger_ = controller_.Get("test");
    }

    void eventRaisingAppender_LoggingEventAppended(object sender, LoggingEventEventArgs e) {
      if (eventCount_ < MaxRecursion && logger_ != null) {
        eventCount_++;
        string message = String.Format(CultureInfo.CurrentCulture, "Log event {0} from EventRaisingAppender", eventCount_);
        Console.WriteLine("Logging message: " + message);
        logger_.Log(typeof(RecursiveLoggingTest), Level.Warn, message, null);
      }
    }

    [Test]
    public void TestAllowRecursiveLoggingFromAppender() {
      SetupController();

      eventCount_ = 0;
      logger_.Log(typeof(RecursiveLoggingTest), Level.Warn, "Message logged", null);

      Assert.AreEqual(MaxRecursion, eventCount_, "Expected MaxRecursion recursive calls");
    }
  }
}