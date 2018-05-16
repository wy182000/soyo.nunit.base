using System;
using System.Diagnostics;

using Soyo.Base.Text;
using Soyo.Base.LogBase;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class TraceAppenderTest {
    [Test]
    public void DefaultCategoryTest() {
      CategoryTraceListener categoryTraceListener = new CategoryTraceListener();
      Trace.Listeners.Clear();
      Trace.Listeners.Add(categoryTraceListener);

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());

      AppenderTrace traceAppender = new AppenderTrace();
      traceAppender.Layout = new LayoutLoggerSimple();
      traceAppender.Activate();

      BasicConfig.Config(rep, traceAppender);

      ILog log = LogManager.Get(rep.Name, GetType());
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

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());

      AppenderTrace traceAppender = new AppenderTrace();
      LayoutPattern methodLayout = new LayoutPattern("%method");
      methodLayout.Activate();
      traceAppender.Category = methodLayout;
      traceAppender.Layout = new LayoutLoggerSimple();
      traceAppender.Activate();

      BasicConfig.Config(rep, traceAppender);

      ILog log = LogManager.Get(rep.Name, GetType());
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
