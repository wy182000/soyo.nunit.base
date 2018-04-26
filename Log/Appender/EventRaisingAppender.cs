using System;
using Soyo.Base.Log;
using Soyo.Base.Text;

namespace UnitTest.Base.Log {
  public class LoggingEventEventArgs : EventArgs {
    public LogObject LoggingEvent { get; private set; }

    public LoggingEventEventArgs(LogObject loggingEvent) {
      if (loggingEvent == null) throw new ArgumentNullException("loggingEvent");
      LoggingEvent = loggingEvent;
    }
  }

  public class EventRaisingAppender : IAppender {
    public event EventHandler<LoggingEventEventArgs> LoggingEventAppended;

    protected void OnLoggingEventAppended(LoggingEventEventArgs e) {
      LoggingEventAppended?.Invoke(this, e);
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