using Soyo.Base.Log;

using Soyo.Base.Text;

using NUnit.Framework;


namespace UnitTest.Base.Log.Appender {
  /// <summary>
  /// Used for internal unit testing the <see cref="BufferingAppenderBase"/> class.
  /// </summary>
  /// <remarks>
  /// Used for internal unit testing the <see cref="BufferingAppenderBase"/> class.
  /// </remarks>
  [TestFixture]
  public class BufferingAppenderTest {
    private AppenderLoggerBufferForward m_bufferingForwardingAppender;
    private CountingAppender m_countingAppender;
    private Soyo.Base.Log.Hierarchy m_hierarchy;


    private void SetupRepository() {
      m_hierarchy = new Soyo.Base.Log.Hierarchy();

      m_countingAppender = new CountingAppender();
      m_countingAppender.Activate();

      m_bufferingForwardingAppender = new AppenderLoggerBufferForward();
      m_bufferingForwardingAppender.AddAppender(m_countingAppender);

      m_bufferingForwardingAppender.Size = 0;
      m_bufferingForwardingAppender.ClearFilter();
      m_bufferingForwardingAppender.Trigger = null;
      //m_bufferingForwardingAppender.Fix = FixFlags.Partial;
      m_bufferingForwardingAppender.Lossy = false;
      m_bufferingForwardingAppender.TriggerLossy = null;
      m_bufferingForwardingAppender.Threshold = Level.All;

      m_bufferingForwardingAppender.Activate();

      BasicConfigurator.Config(m_hierarchy, m_bufferingForwardingAppender);
    }

    /// <summary>
    /// </summary>
    [Test]
    public void TestSetupAppender() {
      SetupRepository();

      Assert.AreEqual(0, m_countingAppender.Counter, "Test empty appender");

      ILogger logger = m_hierarchy.GetLogger("test");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message logged", null);

      Assert.AreEqual(1, m_countingAppender.Counter, "Test 1 event logged");
    }

    /// <summary>
    /// </summary>
    [Test]
    public void TestBufferSize5() {
      SetupRepository();

      m_bufferingForwardingAppender.Size = 5;
      m_bufferingForwardingAppender.Activate();

      Assert.AreEqual(m_countingAppender.Counter, 0);

      ILogger logger = m_hierarchy.GetLogger("test");

      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 1", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 1 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 2", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 2 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 3", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 3 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 4", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 4 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 5", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 5 event in buffer");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 6", null);
      Assert.AreEqual(6, m_countingAppender.Counter, "Test 0 event in buffer. 6 event sent");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 7", null);
      Assert.AreEqual(6, m_countingAppender.Counter, "Test 1 event in buffer. 6 event sent");
      logger.Log(typeof(BufferingAppenderTest), Level.Warn, "Message 8", null);
      Assert.AreEqual(6, m_countingAppender.Counter, "Test 2 event in buffer. 6 event sent");
    }
  }
}
