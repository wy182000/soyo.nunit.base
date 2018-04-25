using Soyo.Base.Text;

namespace UnitTest.Base.Log {
  public class CountingAppender : AppenderBase {
    #region constructors
    public CountingAppender() {
      Counter = 0;
    }
    #endregion constructors

    #region public property
    public int Counter { get; private set; }
    #endregion public property

    #region public function
    public void ResetCounter() {
      Counter = 0;
    }
    #endregion public function

    #region AppenderBase
    protected override void append(IRender render, object logEvent) {
      Counter++;
    }
    #endregion AppenderBase
  }
}