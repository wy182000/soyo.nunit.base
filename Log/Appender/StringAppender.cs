using System.Text;

using Soyo.Base.Text;
using Soyo.Base.Log;

namespace UnitTest.Base.Log {
  public class StringAppender : AppenderLogger {
    private StringBuilder buffer_ = new StringBuilder();
    public StringAppender() {
    }

    public string GetString() {
      return buffer_.ToString();
    }

    public void Reset() {
      buffer_.Length = 0;
    }

    protected override void appendLogger(IRender render, LogObject loggingEvent) {
      buffer_.Append(Render(render, loggingEvent));
    }

    protected bool RequiresLayout {
      get { return true; }
    }
  }
}