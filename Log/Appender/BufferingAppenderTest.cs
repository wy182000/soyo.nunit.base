using Soyo.Base.Log;
using Soyo.Base.Text;

using NUnit.Framework;


namespace UnitTest.Base.Log {
  [TestFixture]
  public class BufferingAppenderTest {
    private AppenderLoggerBufferForward bufferingForwardingAppender_;
    private CountingAppender countingAppender_;
    private ILoggerController controller_;


    private void SetupController() {
      controller_ = new LoggerController();

      countingAppender_ = new CountingAppender();
      countingAppender_.Activate();

      bufferingForwardingAppender_ = new AppenderLoggerBufferForward();
      bufferingForwardingAppender_.AddAppender(countingAppender_);

      bufferingForwardingAppender_.Size = 0;
      bufferingForwardingAppender_.ClearFilter();
      bufferingForwardingAppender_.Trigger = null;
      //m_bufferingForwardingAppender.Fix = FixFlags.Partial;
      bufferingForwardingAppender_.Lossy = false;
      bufferingForwardingAppender_.TriggerLossy = null;
      bufferingForwardingAppender_.Threshold = Level.All;

      bufferingForwardingAppender_.Activate();

      BasicConfig.Config(controller_, bufferingForwardingAppender_);
    }

    [Test]
    public void TestSetupAppender() {
      SetupController();

      Assert.AreEqual(0, countingAppender_.Counter, "Test empty appender");

      ILogger logger = controller_.Get("test");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message logged", null);

      Assert.AreEqual(1, countingAppender_.Counter, "Test 1 event logged");
    }

    /// <summary>
    /// </summary>
    [Test]
    public void TestBufferSize5() {
      SetupController();

      bufferingForwardingAppender_.Size = 5;
      bufferingForwardingAppender_.Activate();

      Assert.AreEqual(countingAppender_.Counter, 0);

      ILogger logger = controller_.Get("test");

      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 1", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 1 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 2", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 2 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 3", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 3 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 4", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 4 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 5", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 5 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 6", null);
      Assert.AreEqual(6, countingAppender_.Counter, "Test 0 event in buffer. 6 event sent");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 7", null);
      Assert.AreEqual(6, countingAppender_.Counter, "Test 1 event in buffer. 6 event sent");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 8", null);
      Assert.AreEqual(6, countingAppender_.Counter, "Test 2 event in buffer. 6 event sent");
    }
  }
}
