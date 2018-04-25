using System;
using System.IO;

using Soyo.Base.Log;
using Soyo.Base;
using Soyo.Base.Text;

using NUnit.Framework;
using System.Globalization;

namespace UnitTest.Base.Log {
  /// <summary>
  /// Used for internal unit testing the <see cref="LayoutPattern"/> class.
  /// </summary>
  /// <remarks>
  /// Used for internal unit testing the <see cref="LayoutPattern"/> class.
  /// </remarks>
  [TestFixture]
  public class PatternLayoutTest {
    private CultureInfo _currentCulture;
    private CultureInfo _currentUICulture;

    [SetUp]
    public void SetUp() {
      // set correct thread culture
      _currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
      _currentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
      System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
    }


    [TearDown]
    public void TearDown() {
      Utils.RemovePropertyFromAllContexts();
      // restore previous culture
      System.Threading.Thread.CurrentThread.CurrentCulture = _currentCulture;
      System.Threading.Thread.CurrentThread.CurrentUICulture = _currentUICulture;
    }

    protected virtual LayoutPattern NewPatternLayout() {
      return new LayoutPattern();
    }

    protected virtual LayoutPattern NewPatternLayout(string pattern) {
      return new LayoutPattern(pattern);
    }

    [Test]
    public void TestThreadPropertiesPattern() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = NewPatternLayout("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestThreadProperiesPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no thread properties value set");
      stringAppender.Reset();

      ThreadContext.PropertySet[Utils.PROPERTY_KEY] = "val1";

      log1.Info("TestMessage");
      Assert.AreEqual("val1", stringAppender.GetString(), "Test thread properties value set");
      stringAppender.Reset();

      ThreadContext.Remove(Utils.PROPERTY_KEY);

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread properties value removed");
      stringAppender.Reset();
    }

    [Test, Ignore("failed build release on .net core on linux")]
    public void TestStackTracePattern() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = NewPatternLayout("%stacktrace{2}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestStackTracePattern");

      log1.Info("TestMessage");
      StringAssert.EndsWith("PatternLayoutTest.TestStackTracePattern", stringAppender.GetString(), "stack trace value set");
      stringAppender.Reset();
    }

    [Test]
    public void TestGlobalPropertiesPattern() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = NewPatternLayout("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestGlobalProperiesPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no global properties value set");
      stringAppender.Reset();

      GlobalContext.Set(Utils.PROPERTY_KEY, "val1");

      log1.Info("TestMessage");
      Assert.AreEqual("val1", stringAppender.GetString(), "Test global properties value set");
      stringAppender.Reset();

      GlobalContext.Remove(Utils.PROPERTY_KEY);

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test global properties value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestAddingCustomPattern() {
      StringAppender stringAppender = new StringAppender();
      LayoutPattern layout = NewPatternLayout();

      layout.AddConverter("TestAddingCustomPattern", typeof(TestMessagePatternConverter));
      layout.Pattern = "%TestAddingCustomPattern";
      layout.Activate();

      stringAppender.Layout = layout;

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestAddingCustomPattern");

      log1.Info("TestMessage");
      Assert.AreEqual("TestMessage", stringAppender.GetString(), "%TestAddingCustomPattern not registered");
      stringAppender.Reset();
    }

    [Test]
    public void NamedPatternConverterWithoutPrecisionShouldReturnFullName() {
      StringAppender stringAppender = new StringAppender();
      LayoutPattern layout = NewPatternLayout();
      layout.AddConverter("message-as-name", typeof(MessageAsNamePatternConverter));
      layout.Pattern = "%message-as-name";
      layout.Activate();
      stringAppender.Layout = layout;
      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);
      ILog log1 = LogManager.Get(rep.Name, "TestAddingCustomPattern");

      log1.Info("NoDots");
      Assert.AreEqual("NoDots", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("One.Dot");
      Assert.AreEqual("One.Dot", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("Tw.o.Dots");
      Assert.AreEqual("Tw.o.Dots", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("TrailingDot.");
      Assert.AreEqual("TrailingDot.", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info(".LeadingDot");
      Assert.AreEqual(".LeadingDot", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      // empty string and other evil combinations as tests for of-by-one mistakes in index calculations
      log1.Info(string.Empty);
      Assert.AreEqual(string.Empty, stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info(".");
      Assert.AreEqual(".", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("x");
      Assert.AreEqual("x", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();
    }

    [Test]
    public void NamedPatternConverterWithPrecision1ShouldStripLeadingStuffIfPresent() {
      StringAppender stringAppender = new StringAppender();
      LayoutPattern layout = NewPatternLayout();
      layout.AddConverter("message-as-name", typeof(MessageAsNamePatternConverter));
      layout.Pattern = "%message-as-name{1}";
      layout.Activate();
      stringAppender.Layout = layout;
      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);
      ILog log1 = LogManager.Get(rep.Name, "TestAddingCustomPattern");

      log1.Info("NoDots");
      Assert.AreEqual("NoDots", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("One.Dot");
      Assert.AreEqual("Dot", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("Tw.o.Dots");
      Assert.AreEqual("Dots", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("TrailingDot.");
      Assert.AreEqual("TrailingDot.", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info(".LeadingDot");
      Assert.AreEqual("LeadingDot", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      // empty string and other evil combinations as tests for of-by-one mistakes in index calculations
      log1.Info(string.Empty);
      Assert.AreEqual(string.Empty, stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("x");
      Assert.AreEqual("x", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info(".");
      Assert.AreEqual(".", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();
    }

    [Test]
    public void NamedPatternConverterWithPrecision2ShouldStripLessLeadingStuffIfPresent() {
      StringAppender stringAppender = new StringAppender();
      LayoutPattern layout = NewPatternLayout();
      layout.AddConverter("message-as-name", typeof(MessageAsNamePatternConverter));
      layout.Pattern = "%message-as-name{2}";
      layout.Activate();
      stringAppender.Layout = layout;
      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);
      ILog log1 = LogManager.Get(rep.Name, "TestAddingCustomPattern");

      log1.Info("NoDots");
      Assert.AreEqual("NoDots", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("One.Dot");
      Assert.AreEqual("One.Dot", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("Tw.o.Dots");
      Assert.AreEqual("o.Dots", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("TrailingDot.");
      Assert.AreEqual("TrailingDot.", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info(".LeadingDot");
      Assert.AreEqual("LeadingDot", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      // empty string and other evil combinations as tests for of-by-one mistakes in index calculations
      log1.Info(string.Empty);
      Assert.AreEqual(string.Empty, stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info("x");
      Assert.AreEqual("x", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();

      log1.Info(".");
      Assert.AreEqual(".", stringAppender.GetString(), "%message-as-name not registered");
      stringAppender.Reset();
    }

    /// <summary>
    /// Converter to include event message
    /// </summary>
    private class TestMessagePatternConverter : PatternConverter {
      protected override void format(TextWriter writer, IRender render, object state) {
        if (render != null) {
          render.Render(writer, state);
        }
      }
    }

    [Test]
    public void TestExceptionPattern() {
      StringAppender stringAppender = new StringAppender();
      LayoutPattern layout = NewPatternLayout("%exception{stacktrace}");
      stringAppender.Layout = layout;

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestExceptionPattern");

      Exception exception = new Exception("Oh no!");
      log1.Info("TestMessage", exception);

      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString());

      stringAppender.Reset();
    }

    private class MessageAsNamePatternConverter : NamedPatternConverter {
      protected override string GetName(object state) {
        var loggingEvent = state as LogObject;
        if (loggingEvent == null) return string.Empty;
        return loggingEvent.Message.ToString();
      }
    }
  }
}