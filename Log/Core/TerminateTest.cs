using System;

using Soyo.Base.Text;
using Soyo.Base.LogBase;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class TerminateTest {
    [Test]
    public void TestShutdownAndReconfigure() {
      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());

      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%m");
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "logger1");

      log1.Info("TestMessage1");
      Assert.AreEqual("TestMessage1", stringAppender.GetString(), "Test logging configured");
      stringAppender.Reset();

      rep.Terminate();

      log1.Info("TestMessage2");
      Assert.AreEqual("", stringAppender.GetString(), "Test not logging while shutdown");
      stringAppender.Reset();

      stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%m");
      BasicConfig.Config(rep, stringAppender);

      log1.Info("TestMessage3");
      Assert.AreEqual("TestMessage3", stringAppender.GetString(), "Test logging re-configured");
      stringAppender.Reset();
    }
  }
}