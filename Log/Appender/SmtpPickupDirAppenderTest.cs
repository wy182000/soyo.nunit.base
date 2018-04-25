using System;
using System.IO;
using System.Text;

using Soyo.Base;
using Soyo.Base.Text;
using Soyo.Base.Log;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  /// <summary>
  /// Used for internal unit testing the <see cref="AppenderSmtpPickup"/> class.
  /// </summary>
  [TestFixture]
  public class SmtpPickupDirAppenderTest {
    private readonly string _testPickupDir;

    private class SilentErrorHandler : IErrorHandler {
      private StringBuilder m_buffer = new StringBuilder();

      public string Message {
        get { return m_buffer.ToString(); }
      }

      public void Error(string message) {
        m_buffer.Append(message + "\n");
      }

      public void Error(string message, Exception e) {
        m_buffer.Append(message + "\n" + e.Message + "\n");
      }

      public void Error(string message, Exception e, ErrorCode errorCode) {
        m_buffer.Append(message + "\n" + e.Message + "\n");
      }
    }

    public SmtpPickupDirAppenderTest() {
      _testPickupDir = Soyo.Base.IO.Path.Combine(Soyo.Base.IO.Path.RootPath, "SmtpPickupDirAppenderTest_PickupDir");
    }
    /// <summary>
    /// Sets up variables used for the tests
    /// </summary>
    private void InitializePickupDir() {
      Directory.CreateDirectory(_testPickupDir);
    }

    /// <summary>
    /// Shuts down any loggers in the hierarchy, along
    /// with all appenders, and deletes any test files used
    /// for logging.
    /// </summary>
    private void ResetLogger() {
      Utils.GetController().Terminate();
    }

    /// <summary>
    /// Any initialization that happens before each test can
    /// go here
    /// </summary>
    [SetUp]
    public void SetUp() {
      ResetLogger();
      DeleteTestFiles();
      InitializePickupDir();
    }

    /// <summary>
    /// Any steps that happen after each test go here
    /// </summary>
    [TearDown]
    public void TearDown() {
      ResetLogger();
      DeleteTestFiles();
    }

    /// <summary>
    /// Removes all test files that exist
    /// </summary>
    private void DeleteTestFiles() {
      if (Directory.Exists(_testPickupDir)) {
        Directory.Delete(_testPickupDir, true);
      }
    }

    /// <summary>
    /// Creates a logger hierarchy, configures a SMTP pickup dir appender and returns an ILogger
    /// </summary>
    /// <param name="appender">The appender to use</param>
    /// <returns>A configured ILogger</returns>
    private ILogger CreateLogger(AppenderSmtpPickup appender) {
      Soyo.Base.Log.LoggerController h = (Soyo.Base.Log.LoggerController)LogManager.CreateController("TestRepository");

      LayoutPattern layout = new LayoutPattern();
      layout.Pattern = "%m%n";
      layout.Activate();

      appender.Layout = layout;
      appender.Activate();

      h.Root.AddAppender(appender);
      h.Initialize();

      ILogger log = h.Get("Logger");
      return log;
    }

    /// <summary>
    /// Create an appender to use for the logger
    /// </summary>
    /// <param name="handler">The error handler to use.</param>
    /// <returns></returns>
    private AppenderSmtpPickup CreateSmtpPickupDirAppender(IErrorHandler handler) {
      AppenderSmtpPickup appender = new AppenderSmtpPickup();
      appender.PickupDir = _testPickupDir;
      appender.ErrorHandler = handler;
      return appender;
    }

    /// <summary>
    /// Destroys the logger hierarchy created by <see cref="SmtpPickupDirAppenderTest.CreateLogger"/>
    /// </summary>
    private static void DestroyLogger() {
      Soyo.Base.Log.LoggerController h = (Soyo.Base.Log.LoggerController)LogManager.GetController("TestRepository");
      h.Reset();
      //Replace the repository selector so that we can recreate the hierarchy with the same name if necessary
      LoggerController.Selector = new LoggerControllerSelector(typeof(Soyo.Base.Log.LoggerController));
    }

    /// <summary>
    /// Tests if the sent message contained the date header.
    /// </summary>
    [Test]
    public void TestOutputContainsSentDate() {
      SilentErrorHandler sh = new SilentErrorHandler();
      AppenderSmtpPickup appender = CreateSmtpPickupDirAppender(sh);
      ILogger log = CreateLogger(appender);
      log.Log(GetType(), Level.Info, "This is a message", null);
      log.Log(GetType(), Level.Info, "This is a message 2", null);
      DestroyLogger();

      Assert.AreEqual(1, Directory.GetFiles(_testPickupDir).Length);
      string[] fileContent = File.ReadAllLines((Directory.GetFiles(_testPickupDir)[0]));
      bool hasDateHeader = false;
      const string dateHeaderStart = "Date: ";
      foreach (string line in fileContent) {
        if (line.StartsWith(dateHeaderStart)) {
          string datePart = line.Substring(dateHeaderStart.Length);
          DateTime date = DateTime.ParseExact(datePart, "r", System.Globalization.CultureInfo.InvariantCulture);
          double diff = Math.Abs((DateTime.UtcNow - date).TotalMilliseconds);
          Assert.LessOrEqual(diff, 1000, "Times should be equal, allowing a diff of one second to make test robust");
          hasDateHeader = true;
        }
      }
      Assert.IsTrue(hasDateHeader, "Output must contains a date header");

      Assert.AreEqual("", sh.Message, "Unexpected error message");
    }

    /// <summary>
    /// Verifies that file extension is applied to output file name.
    /// </summary>
    [Test]
    public void TestConfigurableFileExtension() {
      const string fileExtension = "eml";
      SilentErrorHandler sh = new SilentErrorHandler();
      AppenderSmtpPickup appender = CreateSmtpPickupDirAppender(sh);
      appender.FileExtension = fileExtension;
      ILogger log = CreateLogger(appender);
      log.Log(GetType(), Level.Info, "This is a message", null);
      log.Log(GetType(), Level.Info, "This is a message 2", null);
      DestroyLogger();

      Assert.AreEqual(1, Directory.GetFiles(_testPickupDir).Length);
      FileInfo fileInfo = new FileInfo(Directory.GetFiles(_testPickupDir)[0]);
      Assert.AreEqual("." + fileExtension, fileInfo.Extension);
      Assert.DoesNotThrow(delegate { new Guid(fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length)); }); // Assert that filename before extension is a guid

      Assert.AreEqual("", sh.Message, "Unexpected error message");
    }

    /// <summary>
    /// Verifies that logging a message actually produces output
    /// </summary>
    [Test]
    public void TestDefaultFileNameIsAGuid() {
      SilentErrorHandler sh = new SilentErrorHandler();
      AppenderSmtpPickup appender = CreateSmtpPickupDirAppender(sh);
      ILogger log = CreateLogger(appender);
      log.Log(GetType(), Level.Info, "This is a message", null);
      log.Log(GetType(), Level.Info, "This is a message 2", null);
      DestroyLogger();

      Assert.AreEqual(1, Directory.GetFiles(_testPickupDir).Length);
      FileInfo fileInfo = new FileInfo(Directory.GetFiles(_testPickupDir)[0]);
      Assert.IsEmpty(fileInfo.Extension);
      Assert.DoesNotThrow(delegate { new Guid(fileInfo.Name); }); // Assert that filename is a guid

      Assert.AreEqual("", sh.Message, "Unexpected error message");
    }
  }
}