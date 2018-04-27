using System;
using System.Xml;
using NUnit.Framework;

using Soyo.Base.Log;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class JsonConfigTest {
    [Test]
    public void TestJsonConfig() {
      string config = @"
        {
          ""appender"": [
            {
              ""name"" = ""StringAppender"",
              ""type"" = ""UnitTest.Base.Log.StringAppender"",
              ""layout"" = {
                ""type"" = ""Soyo.Base.Text.LayoutLoggerSimple""
              },
              ""filter"" = {
                ""type"" = ""Soyo.Base.Text.FilterDeny""
              }
            }
          ],
          ""root"": {
            ""level"": ""All"",
            ""appender-ref"" = ""StringAppender""
          },
          ""logger"": {
            ""name"": ""logger"",
            ""appender-ref"" = ""StringAppender"",
            ""Enable"": false,
            ""EnableRoot"": true
          },
          ""threshold"": ""Debug""
        }";

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      JsonConfig.Config(rep, config);
      Assert.AreEqual(rep.AppenderSet.Length, 1);
      Assert.AreEqual(rep.Threshold.Name, "DEBUG");
      Assert.AreEqual(rep.Root.Level.Name, "ALL");
      Assert.AreEqual(rep.Root.AppenderSet.Count, 1);
      Assert.AreEqual(rep.Root.AppenderSet[0], rep.AppenderSet[0]);
      Assert.AreEqual(rep.Root.AppenderSet[0].Name, "StringAppender");
      Assert.AreEqual(rep.Root.AppenderSet[0].GetType(), typeof(StringAppender));
      var appenderBase = rep.Root.AppenderSet[0] as Soyo.Base.Text.AppenderBase;
      Assert.IsNotNull(appenderBase);
      Assert.IsNotNull(appenderBase.Layout);
      Assert.AreEqual(appenderBase.Layout.GetType(), typeof(Soyo.Base.Text.LayoutLoggerSimple));
      Assert.IsNotNull(appenderBase.FilterHead);
      Assert.AreEqual(appenderBase.FilterHead.GetType(), typeof(Soyo.Base.Text.FilterDeny));
      var logger = rep.Get("logger");
      Assert.IsNotNull(logger);
      Assert.AreEqual(logger.Name, "logger");
      Assert.IsNull(logger.Level);
      Assert.AreEqual(logger.ActiveLevel.Name, "ALL");
      Assert.IsFalse(logger.Enable);
      Assert.IsTrue(logger.EnableRoot);

      config = @"
        {
          ""appender"": {
            ""name"": ""StringAppender"",
            ""type"": ""UnitTest.Base.Log.StringAppender"",
            ""layout"": ""Soyo.Base.Text.LayoutLoggerSimple"",
            ""filter"": [
              {
                ""type"": ""Soyo.Base.Text.FilterDeny""
              }
            ]
          },
          ""root"": {
            ""level"": ""All"",
            ""appender-ref"" = ""StringAppender""
          },
          ""logger"": [
            {
              ""name"": ""logger"",
              ""appender-ref"": ""StringAppender"",
              ""Enable"": false,
              ""EnableRoot"": true
            }
          ]
          ""threshold"": ""Debug""
        }";

      JsonConfig.Config(rep, config);
      Assert.AreEqual(rep.AppenderSet.Length, 1);
      Assert.AreEqual(rep.Threshold.Name, "DEBUG");
      Assert.AreEqual(rep.Root.Level.Name, "ALL");
      Assert.AreEqual(rep.Root.AppenderSet.Count, 1);
      Assert.AreEqual(rep.Root.AppenderSet[0], rep.AppenderSet[0]);
      Assert.AreEqual(rep.Root.AppenderSet[0].Name, "StringAppender");
      Assert.AreEqual(rep.Root.AppenderSet[0].GetType(), typeof(StringAppender));
      appenderBase = rep.Root.AppenderSet[0] as Soyo.Base.Text.AppenderBase;
      Assert.IsNotNull(appenderBase);
      Assert.IsNotNull(appenderBase.Layout);
      Assert.AreEqual(appenderBase.Layout.GetType(), typeof(Soyo.Base.Text.LayoutLoggerSimple));
      Assert.IsNotNull(appenderBase.FilterHead);
      Assert.AreEqual(appenderBase.FilterHead.GetType(), typeof(Soyo.Base.Text.FilterDeny));
      logger = rep.Get("logger");
      Assert.IsNotNull(logger);
      Assert.AreEqual(logger.Name, "logger");
      Assert.IsNull(logger.Level);
      Assert.AreEqual(logger.ActiveLevel.Name, "ALL");
      Assert.IsFalse(logger.Enable);
      Assert.IsTrue(logger.EnableRoot);
    }
  }
}