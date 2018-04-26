using System;
using Soyo.Base.Text;
using Soyo.Base.Log;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class EvaluatorTest {
    private AppenderLoggerBufferForward bufferingForwardingAppender_;
    private CountingAppender countingAppender_;
    private ILoggerController controller_;

    [SetUp]
    public void SetupRepository() {
      controller_ = LogManager.CreateController("");

      countingAppender_ = new CountingAppender();
      countingAppender_.Activate();

      bufferingForwardingAppender_ = new AppenderLoggerBufferForward();
      bufferingForwardingAppender_.AddAppender(countingAppender_);

      bufferingForwardingAppender_.Size = 5;
      bufferingForwardingAppender_.ClearFilter();
      //m_bufferingForwardingAppender.Fix = FixFlags.Partial;
      bufferingForwardingAppender_.Lossy = false;
      bufferingForwardingAppender_.TriggerLossy = null;
      bufferingForwardingAppender_.Threshold = Level.All;
    }

    [Test]
    public void TestLevelEvaluator() {
      bufferingForwardingAppender_.Trigger = new TriggerLoggerLevel(Level.Info);
      bufferingForwardingAppender_.Activate();
      BasicConfig.Config(controller_, bufferingForwardingAppender_);

      ILogger logger = controller_.Get("TestLevelEvaluator");

      logger.Log(typeof(EvaluatorTest), Level.Debug, "Debug message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Debug, "Debug message logged", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Info, "Info message logged", null);
      Assert.AreEqual(3, countingAppender_.Counter, "Test 3 events flushed on Info message.");
    }

    [Test]
    public void TestExceptionEvaluator() {
      bufferingForwardingAppender_.Trigger = new TriggerLoggerException(typeof(ApplicationException), true);
      bufferingForwardingAppender_.Activate();
      BasicConfig.Config(controller_, bufferingForwardingAppender_);

      ILogger logger = controller_.Get("TestExceptionEvaluator");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(3, countingAppender_.Counter, "Test 3 events flushed on ApplicationException message.");
    }

    [Test]
    public void TestExceptionEvaluatorTriggerOnSubClass() {
      bufferingForwardingAppender_.Trigger = new TriggerLoggerException(typeof(Exception), true);
      bufferingForwardingAppender_.Activate();
      BasicConfig.Config(controller_, bufferingForwardingAppender_);

      ILogger logger = controller_.Get("TestExceptionEvaluatorTriggerOnSubClass");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(3, countingAppender_.Counter, "Test 3 events flushed on ApplicationException message.");
    }

    [Test]
    public void TestExceptionEvaluatorNoTriggerOnSubClass() {
      bufferingForwardingAppender_.Trigger = new TriggerLoggerException(typeof(Exception), false);
      bufferingForwardingAppender_.Activate();
      BasicConfig.Config(controller_, bufferingForwardingAppender_);

      ILogger logger = controller_.Get("TestExceptionEvaluatorNoTriggerOnSubClass");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(0, countingAppender_.Counter, "Test 3 events buffered");
    }

    [Test]
    public void TestInvalidExceptionEvaluator() {
      // warning: String is not a subclass of Exception
      bufferingForwardingAppender_.Trigger = new TriggerLoggerException(typeof(String), false);
      bufferingForwardingAppender_.Activate();
      BasicConfig.Config(controller_, bufferingForwardingAppender_);

      ILogger logger = controller_.Get("TestExceptionEvaluatorNoTriggerOnSubClass");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", null);
      Assert.AreEqual(0, countingAppender_.Counter, "Test 2 events buffered");

      logger.Log(typeof(EvaluatorTest), Level.Warn, "Warn message logged", new ApplicationException());
      Assert.AreEqual(0, countingAppender_.Counter, "Test 3 events buffered");
    }
  }
}

