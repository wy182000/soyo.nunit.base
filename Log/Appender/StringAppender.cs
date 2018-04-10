using System.Text;

using Soyo.Base.Text;
using Soyo.Base.Log.Core;

namespace UnitTest.Base.Log.Appender {
  /// <summary>
  /// Write events to a string
  /// </summary>
  /// <author>Nicko Cadell</author>
  public class StringAppender : AppenderLogger {
    private StringBuilder m_buf = new StringBuilder();

    /// <summary>
    /// Initializes a new instance of the <see cref="StringAppender" /> class.
    /// </summary>
    public StringAppender() {
    }

    /// <summary>
    /// Get the string logged so far
    /// </summary>
    /// <returns></returns>
    public string GetString() {
      return m_buf.ToString();
    }

    /// <summary>
    /// Reset the string
    /// </summary>
    public void Reset() {
      m_buf.Length = 0;
    }

    /// <summary>
    /// </summary>
    /// <param name="loggingEvent">the event to log</param>
    protected override void appendLogger(IRender render, LoggingEvent loggingEvent) {
      m_buf.Append(Render(render, loggingEvent));
    }

    /// <summary>
    /// This appender requires a <see cref="Layout"/> to be set.
    /// </summary>
    /// <value><c>true</c></value>
    protected bool RequiresLayout {
      get { return true; }
    }
  }
}