using System;
using System.Collections.Generic;
using System.Xml;
using Soyo.Base.Text;
using Soyo.Base.Log;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class FilterTest {
    [Test]
    public void FilterConfigurationTest() {
      XmlDocument log4netConfig = new XmlDocument();
      #region Load log4netConfig
      log4netConfig.LoadXml(@"
            <Soyo.Base.Log>
            <appender name=""AppenderMemory"" type=""Soyo.Base.Text.AppenderMemory"">
                <filter type=""UnitTest.Base.Log.MultiplePropertyFilter"">
                    <condition>
                        <key value=""ABC"" />
                        <stringToMatch value=""123"" />
                    </condition>
                    <condition>
                        <key value=""DEF"" />
                        <stringToMatch value=""456"" />
                    </condition>
                </filter>
            </appender>
            <root>
                <level value=""ALL"" />
                <appender-ref ref=""AppenderMemory"" />
            </root>
            </Soyo.Base.Log>");
      #endregion

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);

      IAppender[] appenders = LogManager.GetController(rep.Name).Appenders;
      Assert.IsTrue(appenders.Length == 1);

      IAppender appender = Array.Find(appenders, delegate (IAppender a) {
        return a.Name == "AppenderMemory";
      });
      Assert.IsNotNull(appender);

      MultiplePropertyFilter multiplePropertyFilter =
          ((AppenderBase)appender).FilterHead as MultiplePropertyFilter;

      MultiplePropertyFilter.Condition[] conditions = multiplePropertyFilter.GetConditions();
      Assert.AreEqual(2, conditions.Length);
      Assert.AreEqual("ABC", conditions[0].Key);
      Assert.AreEqual("123", conditions[0].StringToMatch);
      Assert.AreEqual("DEF", conditions[1].Key);
      Assert.AreEqual("456", conditions[1].StringToMatch);
    }
  }

  public class MultiplePropertyFilter : FilterBase {
    private readonly List<Condition> _conditions = new List<Condition>();

    public override FilterResult Filter(object filterable) {
      return FilterResult.Accept;
    }

    public Condition[] GetConditions() {
      return _conditions.ToArray();
    }

    public void AddCondition(Condition condition) {
      _conditions.Add(condition);
    }

    public class Condition {
      private string key, stringToMatch;
      public string Key {
        get { return key; }
        set { key = value; }
      }
      public string StringToMatch {
        get { return stringToMatch; }
        set { stringToMatch = value; }
      }
    }
  }
}
