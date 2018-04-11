using System;
using System.Diagnostics;

using Soyo.Base.Text;
using Soyo.Base.Log;
using Soyo.Base.Log.Config;
using Soyo.Base.Log.Repository;
using NUnit.Framework;

namespace UnitTest.Base.Log.Appender {
  [TestFixture]
  public class TraceAppenderTest {
    [Test]
    public void DefaultCategoryTest() {
      CategoryTraceListener categoryTraceListener = new CategoryTraceListener();
      Trace.Listeners.Clear();
      Trace.Listeners.Add(categoryTraceListener);

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

      AppenderTrace traceAppender = new AppenderTrace();
      traceAppender.Layout = new LayoutLoggerSimple();
      traceAppender.Activate();

      BasicConfigurator.Configure(rep, traceAppender);

      ILog log = LogManager.GetLogger(rep.Name, GetType());
      log.Debug("Message");

      Assert.AreEqual(
          GetType().ToString(),
          categoryTraceListener.Category);
    }

    [Test, Ignore("failed build release on .net core on linux")]
    public void MethodNameCategoryTest() {
      CategoryTraceListener categoryTraceListener = new CategoryTraceListener();
      Trace.Listeners.Clear();
      Trace.Listeners.Add(categoryTraceListener);

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

      AppenderTrace traceAppender = new AppenderTrace();
      LayoutPattern methodLayout = new LayoutPattern("%method");
      methodLayout.Activate();
      traceAppender.Category = methodLayout;
      traceAppender.Layout = new LayoutLoggerSimple();
      traceAppender.Activate();

      BasicConfigurator.Configure(rep, traceAppender);

      ILog log = LogManager.GetLogger(rep.Name, GetType());
      log.Debug("Message");

      Assert.AreEqual(
          System.Reflection.MethodInfo.GetCurrentMethod().Name,
          categoryTraceListener.Category);
    }
  }

  public class CategoryTraceListener : TraceListener {
    private string lastCategory;

    public override void Write(string message) {
      // empty
    }

    public override void WriteLine(string message) {
      Write(message);
    }

    public override void Write(string message, string category) {
      lastCategory = category;
      base.Write(message, category);
    }

    public string Category {
      get { return lastCategory; }
    }
  }
}
