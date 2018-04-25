using System;
using Soyo.Base.Text;
using Soyo.Base.Log;
using UnitTest.Base.Log.Appender;
using NUnit.Framework;

namespace UnitTest.Base.Log.Core {
  [TestFixture]
  public class EvaluatorTest {
    private AppenderLoggerBufferForward m_bufferingForwardingAppender;
    private CountingAppender m_countingAppender;
    private LoggerController controller;

    [SetUp]
    public void SetupRepository() {
      controller = new Soyo.Base.Log.LoggerController();

      m_countingAppender = new CountingAppender();
      m_countingAppender.Activate();

      m_bufferingForwardingAppender = new AppenderLoggerBufferForward();
      m_bufferingForwardingAppender.AddAppender(m_countingAppender);

      m_bufferingForwardingAppender.Size = 5;
      m_bufferingForwardingAppender.ClearFilter();
      //m_bufferingForwardingAppender.Fix = FixFlags.Partial;
      m_bufferingForwardingAppender.Lossy = false;
      m_bufferingForwardingAppender.TriggerLossy = null;
      m_bufferingForwardingAppender.Threshold = Level.All;
    }

    [Test]
    public void TestLevelEvaluator() {
      m_bufferingForwardingAppender.Trigger = new TriggerLoggerLevel(Level.Info);
      m_bufferingForwardingAppender.Activate();
      Soyo.Base.Log.BasicConfigurator.Config(controller, m_bufferingForwardingAppender);

      ILogger logger = controller.Get("TestLevelEvaluator");

      logger.Log(typeof(EvaluatorTest), Level.Debug, "Debug message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Debug, "Debug message logged", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Info, "Info message logged", null);
      Assert.AreEqual(3, m_countingAppender.Counter, "Test 3 events flushed on Info message.");
    }

    [Test]
    public void TestExceptionEvaluator() {
      m_bufferingForwardingAppender.Trigger = new TriggerLoggerException(typeof(ApplicationException), true);
      m_bufferingForwardingAppender.Activate();
      Soyo.Base.Log.BasicConfigurator.Config(controller, m_bufferingForwardingAppender);

      ILogger logger = controller.Get("TestExceptionEvaluator");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(3, m_countingAppender.Counter, "Test 3 events flushed on ApplicationException message.");
    }

    [Test]
    public void TestExceptionEvaluatorTriggerOnSubClass() {
      m_bufferingForwardingAppender.Trigger = new TriggerLoggerException(typeof(Exception), true);
      m_bufferingForwardingAppender.Activate();
      Soyo.Base.Log.BasicConfigurator.Config(controller, m_bufferingForwardingAppender);

      ILogger logger = controller.Get("TestExceptionEvaluatorTriggerOnSubClass");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(3, m_countingAppender.Counter, "Test 3 events flushed on ApplicationException message.");
    }

    [Test]
    public void TestExceptionEvaluatorNoTriggerOnSubClass() {
      m_bufferingForwardingAppender.Trigger = new TriggerLoggerException(typeof(Exception), false);
      m_bufferingForwardingAppender.Activate();
      Soyo.Base.Log.BasicConfigurator.Config(controller, m_bufferingForwardingAppender);

      ILogger logger = controller.Get("TestExceptionEvaluatorNoTriggerOnSubClass");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 3 events buffered");
    }

    [Test]
    public void TestInvalidExceptionEvaluator() {
      // warning: String is not a subclass of Exception
      m_bufferingForwardingAppender.Trigger = new TriggerLoggerException(typeof(String), false);
      m_bufferingForwardingAppender.Activate();
      Soyo.Base.Log.BasicConfigurator.Config(controller, m_bufferingForwardingAppender);

      ILogger logger = controller.Get("TestExceptionEvaluatorNoTriggerOnSubClass");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(0, m_countingAppender.Counter, "Test 3 events buffered");
    }
  }
}

