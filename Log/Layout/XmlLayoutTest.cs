using System;
using System.IO;
using System.Xml;

using Soyo.Base;
using Soyo.Base.Text;
using Soyo.Base.Log;
using Soyo.Base.Log.Config;
using Soyo.Base.Log.Core;
using Soyo.Base.Log.Repository;
using UnitTest.Base.Log.Appender;
using Soyo.Base.Log.Util;

using NUnit.Framework;
using System.Globalization;

namespace UnitTest.Base.Log.Layout {
  [TestFixture]
  public class XmlLayoutTest {
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
      // restore previous culture
      System.Threading.Thread.CurrentThread.CurrentCulture = _currentCulture;
      System.Threading.Thread.CurrentThread.CurrentUICulture = _currentUICulture;
    }

    /// <summary>
    /// Build a basic <see cref="LoggingEventData"/> object with some default values.
    /// </summary>
    /// <returns>A useful LoggingEventData object</returns>
    private LoggingEventData CreateBaseEvent() {
      LoggingEventData ed = new LoggingEventData();
      ed.Domain = "Tests";
      ed.ExceptionString = "";
      ed.Identity = "TestRunner";
      ed.Level = Level.Info;
      ed.LocationInfo = new LocationInfo(GetType());
      ed.LoggerName = "TestLogger";
      ed.Message = "Test message";
      ed.ThreadName = "TestThread";
      ed.TimeStampUtc = DateTime.Today.ToUniversalTime();
      ed.UserName = "TestRunner";
      ed.Properties = new PropertySet();

      return ed;
    }

    private static string CreateEventNode(string message) {
      return String.Format("<event logger=\"TestLogger\" timestamp=\"{0}\" level=\"INFO\" thread=\"TestThread\" domain=\"Tests\" identity=\"TestRunner\" username=\"TestRunner\"><message>{1}</message></event>" + Environment.NewLine,
			                     XmlConvert.ToString(DateTime.Today, XmlDateTimeSerializationMode.Local),
                           message);
    }

    private static string CreateEventNode(string key, string value) {
      return String.Format("<event logger=\"TestLogger\" timestamp=\"{0}\" level=\"INFO\" thread=\"TestThread\" domain=\"Tests\" identity=\"TestRunner\" username=\"TestRunner\"><message>Test message</message><properties><data name=\"{1}\" value=\"{2}\" /></properties></event>" + Environment.NewLine,
			                     XmlConvert.ToString(DateTime.Today, XmlDateTimeSerializationMode.Local),
                           key,
                           value);
    }

    [Test]
    public void TestBasicEventLogging() {
      TextWriter writer = new StringWriter();
      LayoutLoggerXml layout = new LayoutLoggerXml();
      LoggingEventData evt = CreateBaseEvent();

      layout.Format(writer, null, new LoggingEvent(evt));

      string expected = CreateEventNode("Test message");

      Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void TestIllegalCharacterMasking() {
      TextWriter writer = new StringWriter();
      LayoutLoggerXml layout = new LayoutLoggerXml();
      LoggingEventData evt = CreateBaseEvent();

      evt.Message = "This is a masked char->\uFFFF";

      layout.Format(writer, null, new LoggingEvent(evt));

      string expected = CreateEventNode("This is a masked char-&gt;?");

      Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void TestCDATAEscaping1() {
      TextWriter writer = new StringWriter();
      LayoutLoggerXml layout = new LayoutLoggerXml();
      LoggingEventData evt = CreateBaseEvent();

      //The &'s trigger the use of a cdata block
      evt.Message = "&&&&&&&Escape this ]]>. End here.";

      layout.Format(writer, null, new LoggingEvent(evt));

      string expected = CreateEventNode("<![CDATA[&&&&&&&Escape this ]]>]]<![CDATA[>. End here.]]>");

      Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void TestCDATAEscaping2() {
      TextWriter writer = new StringWriter();
      LayoutLoggerXml layout = new LayoutLoggerXml();
      LoggingEventData evt = CreateBaseEvent();

      //The &'s trigger the use of a cdata block
      evt.Message = "&&&&&&&Escape the end ]]>";

      layout.Format(writer, null, new LoggingEvent(evt));

      string expected = CreateEventNode("<![CDATA[&&&&&&&Escape the end ]]>]]&gt;");

      Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void TestCDATAEscaping3() {
      TextWriter writer = new StringWriter();
      LayoutLoggerXml layout = new LayoutLoggerXml();
      LoggingEventData evt = CreateBaseEvent();

      //The &'s trigger the use of a cdata block
      evt.Message = "]]>&&&&&&&Escape the begining";

      layout.Format(writer, null, new LoggingEvent(evt));

      string expected = CreateEventNode("<![CDATA[]]>]]<![CDATA[>&&&&&&&Escape the begining]]>");

      Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void TestBase64EventLogging() {
      TextWriter writer = new StringWriter();
      LayoutLoggerXml layout = new LayoutLoggerXml();
      LoggingEventData evt = CreateBaseEvent();

      layout.base64EncodeMessage = true;
      layout.Format(writer, null, new LoggingEvent(evt));

      string expected = CreateEventNode("VGVzdCBtZXNzYWdl");

      Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void TestPropertyEventLogging() {
      LoggingEventData evt = CreateBaseEvent();
      evt.Properties["Property1"] = "prop1";

      LayoutLoggerXml layout = new LayoutLoggerXml();
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

      log1.Logger.Log(new LoggingEvent(evt));

      string expected = CreateEventNode("Property1", "prop1");

      Assert.AreEqual(expected, stringAppender.GetString());
    }

    [Test]
    public void TestBase64PropertyEventLogging() {
      LoggingEventData evt = CreateBaseEvent();
      evt.Properties["Property1"] = "prop1";

      LayoutLoggerXml layout = new LayoutLoggerXml();
      layout.base64EncodeProperties = true;
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

      log1.Logger.Log(new LoggingEvent(evt));

      string expected = CreateEventNode("Property1", "cHJvcDE=");

      Assert.AreEqual(expected, stringAppender.GetString());
    }

    [Test]
    public void TestPropertyCharacterEscaping() {
      LoggingEventData evt = CreateBaseEvent();
      evt.Properties["Property1"] = "prop1 \"quoted\"";

      LayoutLoggerXml layout = new LayoutLoggerXml();
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

      log1.Logger.Log(new LoggingEvent(evt));

      string expected = CreateEventNode("Property1", "prop1 &quot;quoted&quot;");

      Assert.AreEqual(expected, stringAppender.GetString());
    }

    [Test]
    public void TestPropertyIllegalCharacterMasking() {
      LoggingEventData evt = CreateBaseEvent();
      evt.Properties["Property1"] = "mask this ->\uFFFF";

      LayoutLoggerXml layout = new LayoutLoggerXml();
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

      log1.Logger.Log(new LoggingEvent(evt));

      string expected = CreateEventNode("Property1", "mask this -&gt;?");

      Assert.AreEqual(expected, stringAppender.GetString());
    }

    [Test]
    public void TestPropertyIllegalCharacterMaskingInName() {
      LoggingEventData evt = CreateBaseEvent();
      evt.Properties["Property\uFFFF"] = "mask this ->\uFFFF";

      LayoutLoggerXml layout = new LayoutLoggerXml();
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

      log1.Logger.Log(new LoggingEvent(evt));

      string expected = CreateEventNode("Property?", "mask this -&gt;?");

      Assert.AreEqual(expected, stringAppender.GetString());
    }

    [Test]
    public void BracketsInStackTracesKeepLogWellFormed() {
      LayoutLoggerXml layout = new LayoutLoggerXml();
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestLogger");
      Action<int> bar = foo => {
        try {
          throw new NullReferenceException();
        } catch (Exception ex) {
          log1.Error(string.Format("Error {0}", foo), ex);
        }
      };
      bar(42);

      // really only asserts there is no exception
      var loggedDoc = new XmlDocument();
      loggedDoc.LoadXml(stringAppender.GetString());
    }

    [Test]
    public void BracketsInStackTracesAreEscapedProperly() {
      LayoutLoggerXml layout = new LayoutLoggerXml();
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = layout;

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);
      ILog log1 = LogManager.GetLogger(rep.Name, "TestLogger");
      Action<int> bar = foo => {
        try {
          throw new NullReferenceException();
        } catch (Exception ex) {
          log1.Error(string.Format("Error {0}", foo), ex);
        }
      };
      bar(42);

      var log = stringAppender.GetString();
      var startOfExceptionText = log.IndexOf("<exception>", StringComparison.InvariantCulture) + 11;
      var endOfExceptionText = log.IndexOf("</exception>", StringComparison.InvariantCulture);
      var sub = log.Substring(startOfExceptionText, endOfExceptionText - startOfExceptionText);
      if (sub.StartsWith("<![CDATA[")) {
        StringAssert.EndsWith("]]>", sub);
      } else {
        StringAssert.DoesNotContain("<", sub);
        StringAssert.DoesNotContain(">", sub);
      }
    }
  }
}