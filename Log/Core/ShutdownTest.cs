using System;

using Soyo.Base.Text;
using Soyo.Base.Log;
using UnitTest.Base.Log.Appender;

using NUnit.Framework;

namespace UnitTest.Base.Log.Core {
  /// <summary>
  /// </remarks>
  [TestFixture]
  public class ShutdownTest {
    /// <summary>
    /// Test that a repository can be shutdown and reconfigured
    /// </summary>
    [Test]
    public void TestShutdownAndReconfigure() {
      // Create unique repository
      IRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

      // Create appender and configure repos
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%m");
      BasicConfigurator.Config(rep, stringAppender);

      // Get logger from repos
      ILog log1 = LogManager.GetLogger(rep.Name, "logger1");

      log1.Info("TestMessage1");
      Assert.AreEqual("TestMessage1", stringAppender.GetString(), "Test logging configured");
      stringAppender.Reset();

      rep.Terminate();

      log1.Info("TestMessage2");
      Assert.AreEqual("", stringAppender.GetString(), "Test not logging while shutdown");
      stringAppender.Reset();

      // Create new appender and configure
      stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%m");
      BasicConfigurator.Config(rep, stringAppender);

      log1.Info("TestMessage3");
      Assert.AreEqual("TestMessage3", stringAppender.GetString(), "Test logging re-configured");
      stringAppender.Reset();
    }
  }
}