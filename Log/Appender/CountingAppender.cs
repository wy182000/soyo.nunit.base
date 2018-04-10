using Soyo.Base.Text;
using Soyo.Base.Log.Core;

namespace UnitTest.Base.Log.Appender {
  /// <summary>
  /// Implements an Appender for test purposes that counts the
  /// number of output calls to <see cref="Append" />.
  /// </summary>
  /// <remarks>
  /// This appender is used in the unit tests.
  /// </remarks>
  /// <author>Nicko Cadell</author>
  /// <author>Gert Driesen</author>
  public class CountingAppender : AppenderBase {
    #region Public Instance Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CountingAppender" /> class.
    /// </summary>
    public CountingAppender() {
      m_counter = 0;
    }
    #endregion Public Instance Constructors

    #region Public Instance Properties
    /// <summary>
    /// Returns the number of times <see cref="Append" /> has been called.
    /// </summary>
    /// <value>
    /// The number of times <see cref="Append" /> has been called.
    /// </value>
    public int Counter {
      get { return m_counter; }
    }
    #endregion Public Instance Properties

    /// <summary>
    /// Reset the counter to zero
    /// </summary>
    public void ResetCounter() {
      m_counter = 0;
    }

    #region Override implementation of AppenderBase
    /// <summary>
    /// Registers how many times the method has been called.
    /// </summary>
    /// <param name="logEvent">The logging event.</param>
    protected override void append(IRender render, object logEvent) {
      m_counter++;
    }
    #endregion Override implementation of AppenderBase

    #region Private Instance Fields
    /// <summary>
    /// The number of times <see cref="Append" /> has been called.
    /// </summary>
    private int m_counter;
    #endregion Private Instance Fields
  }
}