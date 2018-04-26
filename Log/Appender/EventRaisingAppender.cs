using System;
using Soyo.Base.Log;
using Soyo.Base.Text;

namespace UnitTest.Base.Log {
  /// <summary>
  /// Provides data for the <see cref="EventRaisingAppender.LoggingEventAppended"/> event.
  /// </summary>
  /// <seealso cref="System.EventArgs" />
  public class LoggingEventEventArgs : EventArgs {
    public LogObject LoggingEvent { get; private set; }

    public LoggingEventEventArgs(LogObject loggingEvent) {
      if (loggingEvent == null) throw new ArgumentNullException("loggingEvent");
      LoggingEvent = loggingEvent;
    }
  }

  /// <summary>
  /// A Soyo.Base.Log appender that raises an event each time a logging event is appended
  /// </summary>
  /// <remarks>
  /// This class is intended to provide a way for test code to inspect logging
  /// events as they are generated.
  /// </remarks>
  public class EventRaisingAppender : IAppender {
    public event EventHandler<LoggingEventEventArgs> LoggingEventAppended;

    protected void OnLoggingEventAppended(LoggingEventEventArgs e) {
      var loggingEventAppended = LoggingEventAppended;
      if (loggingEventAppended != null) {
        loggingEventAppended(this, e);
      }
    }

    public void Close() {
    }

    public void Append(IRender render, object loggingEvent) {
      OnLoggingEventAppended(new LoggingEventEventArgs(loggingEvent as LogObject));
    }

    public string Name {
      get; set;
    }
  }
}