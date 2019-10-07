using System;
using System.Globalization;

using Soyo.Base.Text;
using Soyo.Base.LogBase;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class StringFormatTest {
    private CultureInfo currentCulture_;
    private CultureInfo currentUICulture_;

    [SetUp]
    public void SetUp() {
      // set correct thread culture
      currentCulture_ = System.Threading.Thread.CurrentThread.CurrentCulture;
      currentUICulture_ = System.Threading.Thread.CurrentThread.CurrentUICulture;
      System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
    }

    [TearDown]
    public void TearDown() {
      // restore previous culture
      System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture_;
      System.Threading.Thread.CurrentThread.CurrentUICulture = currentUICulture_;
    }

    [Test]
    public void TestFormatString() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestFormatString");

      // ***
      log1.Info("TestMessage");
      Assert.AreEqual("TestMessage", stringAppender.GetString(), "Test simple INFO event");
      stringAppender.Reset();


      // ***
      log1.DebugFormat("Before {0} After", "Middle");
      Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted DEBUG event");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("Before {0} After", "Middle");
      Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted INFO event");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("Before {0} After", "Middle");
      Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted WARN event");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("Before {0} After", "Middle");
      Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted ERROR event");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("Before {0} After", "Middle");
      Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted FATAL event");
      stringAppender.Reset();


      // ***
      log1.InfoFormat("Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("Before Middle After End", stringAppender.GetString(), "Test simple formatted INFO event 2");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("IGNORE THIS WARNING - EXCEPTION EXPECTED Before {0} After {1} {2}", "Middle", "End");
      Assert.AreEqual(STRING_FORMAT_ERROR, stringAppender.GetString(), "Test formatting error");
      stringAppender.Reset();
    }

    private const string STRING_FORMAT_ERROR = "<Error>Exception during StringFormat: Index (zero based) must be greater than or equal to zero and less than the size of the argument list. <format>IGNORE THIS WARNING - EXCEPTION EXPECTED Before {0} After {1} {2}</format><args>{Middle, End}</args></Error>";


    [Test]
    public void TestLogFormatApi_Debug() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Debug");

      // ***
      log1.Debug("TestMessage");
      Assert.AreEqual("DEBUG:TestMessage", stringAppender.GetString(), "Test simple DEBUG event 1");
      stringAppender.Reset();

      // ***
      log1.Debug("TestMessage", null);
      Assert.AreEqual("DEBUG:TestMessage", stringAppender.GetString(), "Test simple DEBUG event 2");
      stringAppender.Reset();

      // ***
      log1.Debug("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("DEBUG:TestMessage\r\nException: Message: Exception message" + Environment.NewLine + "System.Exception: Exception message" + Environment.NewLine, stringAppender.GetString(), "Test simple DEBUG event 3");
      stringAppender.Reset();

      // ***
      log1.DebugFormat("a{0}", "1");
      Assert.AreEqual("DEBUG:a1", stringAppender.GetString(), "Test formatted DEBUG event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.DebugFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("DEBUG:a1b2", stringAppender.GetString(), "Test formatted DEBUG event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.DebugFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("DEBUG:a1b2c3", stringAppender.GetString(), "Test formatted DEBUG event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.DebugFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("DEBUG:aQbWcEdReTf", stringAppender.GetString(), "Test formatted DEBUG event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.DebugFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("DEBUG:Before Middle After End", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.DebugFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("DEBUG:Before Middle After End", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_NoDebug() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Threshold = Level.Info;
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Debug");

      // ***
      log1.Debug("TestMessage");
      Assert.AreEqual("", stringAppender.GetString(), "Test simple DEBUG event 1");
      stringAppender.Reset();

      // ***
      log1.Debug("TestMessage", null);
      Assert.AreEqual("", stringAppender.GetString(), "Test simple DEBUG event 2");
      stringAppender.Reset();

      // ***
      log1.Debug("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("", stringAppender.GetString(), "Test simple DEBUG event 3");
      stringAppender.Reset();

      // ***
      log1.DebugFormat("a{0}", "1");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted DEBUG event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.DebugFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted DEBUG event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.DebugFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted DEBUG event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.DebugFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted DEBUG event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.DebugFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.DebugFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }


    [Test]
    public void TestLogFormatApi_Info() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Info");

      // ***
      log1.Info("TestMessage");
      Assert.AreEqual("INFO:TestMessage", stringAppender.GetString(), "Test simple INFO event 1");
      stringAppender.Reset();

      // ***
      log1.Info("TestMessage", null);
      Assert.AreEqual("INFO:TestMessage", stringAppender.GetString(), "Test simple INFO event 2");
      stringAppender.Reset();

      // ***
      log1.Info("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("INFO:TestMessage\r\nException: Message: Exception message" + Environment.NewLine + "System.Exception: Exception message" + Environment.NewLine, stringAppender.GetString(), "Test simple INFO event 3");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("a{0}", "1");
      Assert.AreEqual("INFO:a1", stringAppender.GetString(), "Test formatted INFO event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("INFO:a1b2", stringAppender.GetString(), "Test formatted INFO event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("INFO:a1b2c3", stringAppender.GetString(), "Test formatted INFO event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.InfoFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("INFO:aQbWcEdReTf", stringAppender.GetString(), "Test formatted INFO event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.InfoFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("INFO:Before Middle After End", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.InfoFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("INFO:Before Middle After End", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_NoInfo() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Threshold = Level.Warn;
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Info");

      // ***
      log1.Info("TestMessage");
      Assert.AreEqual("", stringAppender.GetString(), "Test simple INFO event 1");
      stringAppender.Reset();

      // ***
      log1.Info("TestMessage", null);
      Assert.AreEqual("", stringAppender.GetString(), "Test simple INFO event 2");
      stringAppender.Reset();

      // ***
      log1.Info("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("", stringAppender.GetString(), "Test simple INFO event 3");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("a{0}", "1");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted INFO event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted INFO event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.InfoFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted INFO event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.InfoFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted INFO event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.InfoFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.InfoFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }


    [Test]
    public void TestLogFormatApi_Warn() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Warn");

      // ***
      log1.Warn("TestMessage");
      Assert.AreEqual("WARN:TestMessage", stringAppender.GetString(), "Test simple WARN event 1");
      stringAppender.Reset();

      // ***
      log1.Warn("TestMessage", null);
      Assert.AreEqual("WARN:TestMessage", stringAppender.GetString(), "Test simple WARN event 2");
      stringAppender.Reset();

      // ***
      log1.Warn("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("WARN:TestMessage\r\nException: Message: Exception message" + Environment.NewLine + "System.Exception: Exception message" + Environment.NewLine, stringAppender.GetString(), "Test simple WARN event 3");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("a{0}", "1");
      Assert.AreEqual("WARN:a1", stringAppender.GetString(), "Test formatted WARN event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("WARN:a1b2", stringAppender.GetString(), "Test formatted WARN event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("WARN:a1b2c3", stringAppender.GetString(), "Test formatted WARN event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.WarnFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("WARN:aQbWcEdReTf", stringAppender.GetString(), "Test formatted WARN event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.WarnFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("WARN:Before Middle After End", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.WarnFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("WARN:Before Middle After End", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_NoWarn() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Threshold = Level.Error;
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Warn");

      // ***
      log1.Warn("TestMessage");
      Assert.AreEqual("", stringAppender.GetString(), "Test simple WARN event 1");
      stringAppender.Reset();

      // ***
      log1.Warn("TestMessage", null);
      Assert.AreEqual("", stringAppender.GetString(), "Test simple WARN event 2");
      stringAppender.Reset();

      // ***
      log1.Warn("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("", stringAppender.GetString(), "Test simple WARN event 3");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("a{0}", "1");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted WARN event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted WARN event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.WarnFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted WARN event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.WarnFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted WARN event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.WarnFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.WarnFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }


    [Test]
    public void TestLogFormatApi_Error() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Error");

      // ***
      log1.Error("TestMessage");
      Assert.AreEqual("ERROR:TestMessage", stringAppender.GetString(), "Test simple ERROR event 1");
      stringAppender.Reset();

      // ***
      log1.Error("TestMessage", null);
      Assert.AreEqual("ERROR:TestMessage", stringAppender.GetString(), "Test simple ERROR event 2");
      stringAppender.Reset();

      // ***
      log1.Error("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("ERROR:TestMessage\r\nException: Message: Exception message" + Environment.NewLine + "System.Exception: Exception message" + Environment.NewLine, stringAppender.GetString(), "Test simple ERROR event 3");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("a{0}", "1");
      Assert.AreEqual("ERROR:a1", stringAppender.GetString(), "Test formatted ERROR event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("ERROR:a1b2", stringAppender.GetString(), "Test formatted ERROR event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("ERROR:a1b2c3", stringAppender.GetString(), "Test formatted ERROR event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.ErrorFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("ERROR:aQbWcEdReTf", stringAppender.GetString(), "Test formatted ERROR event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("ERROR:Before Middle After End", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("ERROR:Before Middle After End", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_NoError() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Threshold = Level.Exception;
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Error");

      // ***
      log1.Error("TestMessage");
      Assert.AreEqual("", stringAppender.GetString(), "Test simple ERROR event 1");
      stringAppender.Reset();

      // ***
      log1.Error("TestMessage", null);
      Assert.AreEqual("", stringAppender.GetString(), "Test simple ERROR event 2");
      stringAppender.Reset();

      // ***
      log1.Error("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("", stringAppender.GetString(), "Test simple ERROR event 3");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("a{0}", "1");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ERROR event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ERROR event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ERROR event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.ErrorFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ERROR event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.ErrorFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_Assert() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Assert");

      // ***
      log1.Assert("TestMessage");
      Assert.AreEqual("ASSERT:TestMessage", stringAppender.GetString(), "Test simple ASSERT event 1");
      stringAppender.Reset();

      // ***
      log1.Assert("TestMessage", null);
      Assert.AreEqual("ASSERT:TestMessage", stringAppender.GetString(), "Test simple ASSERT event 2");
      stringAppender.Reset();

      // ***
      log1.Assert("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("ASSERT:TestMessage\r\nException: Message: Exception message" + Environment.NewLine + "System.Exception: Exception message" + Environment.NewLine, stringAppender.GetString(), "Test simple ASSERT event 3");
      stringAppender.Reset();

      // ***
      log1.AssertFormat("a{0}", "1");
      Assert.AreEqual("ASSERT:a1", stringAppender.GetString(), "Test formatted ASSERT event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.AssertFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("ASSERT:a1b2", stringAppender.GetString(), "Test formatted ASSERT event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.AssertFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("ASSERT:a1b2c3", stringAppender.GetString(), "Test formatted ASSERT event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.AssertFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("ASSERT:aQbWcEdReTf", stringAppender.GetString(), "Test formatted ASSERT event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.AssertFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("ASSERT:Before Middle After End", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.AssertFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("ASSERT:Before Middle After End", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_NoAssert() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Threshold = Level.Off;
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Assert");

      // ***
      log1.Assert("TestMessage");
      Assert.AreEqual("", stringAppender.GetString(), "Test simple ASSERT event 1");
      stringAppender.Reset();

      // ***
      log1.Assert("TestMessage", null);
      Assert.AreEqual("", stringAppender.GetString(), "Test simple ASSERT event 2");
      stringAppender.Reset();

      // ***
      log1.Assert("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("", stringAppender.GetString(), "Test simple ASSERT event 3");
      stringAppender.Reset();

      // ***
      log1.AssertFormat("a{0}", "1");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ASSERT event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.AssertFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ASSERT event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.AssertFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ASSERT event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.AssertFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted ASSERT event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.AssertFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.AssertFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_Exception() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Exception");

      // ***
      log1.Exception("TestMessage");
      Assert.AreEqual("EXCEPTION:TestMessage", stringAppender.GetString(), "Test simple EXCEPTION event 1");
      stringAppender.Reset();

      // ***
      log1.Exception("TestMessage", null);
      Assert.AreEqual("EXCEPTION:TestMessage", stringAppender.GetString(), "Test simple EXCEPTION event 2");
      stringAppender.Reset();

      // ***
      log1.Exception("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("EXCEPTION:TestMessage\r\nException: Message: Exception message" + Environment.NewLine + "System.Exception: Exception message" + Environment.NewLine, stringAppender.GetString(), "Test simple EXCEPTION event 3");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("a{0}", "1");
      Assert.AreEqual("EXCEPTION:a1", stringAppender.GetString(), "Test formatted EXCEPTION event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("EXCEPTION:a1b2", stringAppender.GetString(), "Test formatted EXCEPTION event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("EXCEPTION:a1b2c3", stringAppender.GetString(), "Test formatted EXCEPTION event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.ExceptionFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("EXCEPTION:aQbWcEdReTf", stringAppender.GetString(), "Test formatted EXCEPTION event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("EXCEPTION:Before Middle After End", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("EXCEPTION:Before Middle After End", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }

    [Test]
    public void TestLogFormatApi_NoException() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Threshold = Level.Off;
      stringAppender.Layout = new LayoutPattern("%level:%message");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestLogFormatApi_Exception");

      // ***
      log1.Exception("TestMessage");
      Assert.AreEqual("", stringAppender.GetString(), "Test simple EXCEPTION event 1");
      stringAppender.Reset();

      // ***
      log1.Exception("TestMessage", null);
      Assert.AreEqual("", stringAppender.GetString(), "Test simple EXCEPTION event 2");
      stringAppender.Reset();

      // ***
      log1.Exception("TestMessage", new Exception("Exception message"));
      Assert.AreEqual("", stringAppender.GetString(), "Test simple EXCEPTION event 3");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("a{0}", "1");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted EXCEPTION event with 1 parm");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("a{0}b{1}", "1", "2");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted EXCEPTION event with 2 parm");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat("a{0}b{1}c{2}", "1", "2", "3");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted EXCEPTION event with 3 parm");
      stringAppender.Reset();


      // ***
      log1.ExceptionFormat("a{0}b{1}c{2}d{3}e{4}f", "Q", "W", "E", "R", "T", "Y");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatted EXCEPTION event with 5 parms (only 4 used)");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat(null, "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with null provider");
      stringAppender.Reset();

      // ***
      log1.ExceptionFormat(new CultureInfo("en"), "Before {0} After {1}", "Middle", "End");
      Assert.AreEqual("", stringAppender.GetString(), "Test formatting with 'en' provider");
      stringAppender.Reset();
    }
  }
}