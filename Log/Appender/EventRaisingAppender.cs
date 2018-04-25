using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Soyo.Base.Text;

namespace UnitTest.Base.Log {
  /// <summary>
  /// Provides data for the <see cref="EventRaisingAppender.LoggingEventAppended"/> event.
  /// </summary>
  /// <seealso cref="System.EventArgs" />
  public class LoggingEventEventArgs : EventArgs {
    public Soyo.Base.Log.LogObject LoggingEvent { get; private set; }

    public LoggingEventEventArgs(Soyo.Base.Log.LogObject loggingEvent) {
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
      OnLoggingEventAppended(new LoggingEventEventArgs(loggingEvent as Soyo.Base.Log.LogObject));
    }

    public string Name {
      get; set;
    }
  }
}