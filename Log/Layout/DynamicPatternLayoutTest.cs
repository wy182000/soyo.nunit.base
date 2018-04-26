using Soyo.Base.Text;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  public class DynamicPatternLayoutTest : PatternLayoutTest {
    protected override LayoutPattern NewPatternLayout() {
      return new LayoutPatternDynamic();
    }

    protected override LayoutPattern NewPatternLayout(string pattern) {
      return new LayoutPatternDynamic(pattern);
    }
  }
}
