using System;
using System.IO;
using System.Xml;

using Soyo.Base.Log;
using Soyo.Base.Text;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class PatternConverterTest {
    [Test]
    public void PatternLayoutConverterProperties() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""StringAppender"" type=""UnitTest.Base.Log.StringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutPattern"">
                        <converter>
                            <name value=""propertyKeyCount"" />
                            <type value=""UnitTest.Base.Log.PropertyKeyCountPatternLayoutConverter"" />
                            <property>
                                <key value=""one-plus-one"" />
                                <value value=""2"" />
                            </property>
                            <property>
                               <key value=""two-plus-two"" />
                               <value value=""4"" />
                            </property> 
                        </converter>
                        <pattern value=""%propertyKeyCount"" />
                    </layout>
                  </appender>
                  <root>
                    <level value=""ALL"" />                  
                    <appender-ref ref=""StringAppender"" />
                  </root>  
                </Soyo.Base.Log>");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);

      ILog log = LogManager.Get(rep.Name, "PatternLayoutConverterProperties");
      log.Debug("Message");

      PropertyKeyCountPatternLayoutConverter converter =
          PropertyKeyCountPatternLayoutConverter.MostRecentInstance;
      Assert.AreEqual(2, converter.PropertySet.Count);
      Assert.AreEqual("4", converter.PropertySet["two-plus-two"]);

      StringAppender appender =
          (StringAppender)LogManager.GetController(rep.Name).AppenderSet[0];
      Assert.AreEqual("2", appender.GetString());
    }

    [Test]
    public void PatternConverterProperties() {
      XmlDocument log4netConfig = new XmlDocument();
      log4netConfig.LoadXml(@"
                <Soyo.Base.Log>
                  <appender name=""PatternStringAppender"" type=""UnitTest.Base.Log.PatternStringAppender"">
                    <layout type=""Soyo.Base.Text.LayoutLoggerSimple"" />
                    <setting>
                        <converter>
                            <name value=""propertyKeyCount"" />
                            <type value=""UnitTest.Base.Log.PropertyKeyCountPatternConverter"" />
                            <property>
                                <key value=""one-plus-one"" />
                                <value value=""2"" />
                            </property>
                            <property>
                               <key value=""two-plus-two"" />
                               <value value=""4"" />
                            </property> 
                        </converter>
                        <pattern value=""%propertyKeyCount"" />
                    </setting>
                  </appender>
                  <root>
                    <level value=""ALL"" />                  
                    <appender-ref ref=""PatternStringAppender"" />
                  </root>  
                </Soyo.Base.Log>");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      XmlConfig.Config(rep, log4netConfig["Soyo.Base.Log"]);

      ILog log = LogManager.Get(rep.Name, "PatternConverterProperties");
      log.Debug("Message");

      PropertyKeyCountPatternConverter converter =
          PropertyKeyCountPatternConverter.MostRecentInstance;
      Assert.AreEqual(2, converter.PropertySet.Count);
      Assert.AreEqual("4", converter.PropertySet["two-plus-two"]);

      PatternStringAppender appender =
          (PatternStringAppender)LogManager.GetController(rep.Name).AppenderSet[0];
      Assert.AreEqual("2", appender.Setting.Format());
    }
  }

  public class PropertyKeyCountPatternLayoutConverter : PatternConverter {
    private static PropertyKeyCountPatternLayoutConverter mostRecentInstance;

    public PropertyKeyCountPatternLayoutConverter() {
      mostRecentInstance = this;
    }

    protected override void format(TextWriter writer, IRender render,  object state) {
      writer.Write(PropertySet.Keys.Length);
    }

    public static PropertyKeyCountPatternLayoutConverter MostRecentInstance {
      get { return mostRecentInstance; }
    }
  }

  public class PropertyKeyCountPatternConverter : PatternConverter {
    private static PropertyKeyCountPatternConverter mostRecentInstance;

    public PropertyKeyCountPatternConverter() {
      mostRecentInstance = this;
    }

    protected override void format(TextWriter writer, IRender render, object state) {
      writer.Write(PropertySet.Keys.Length);
    }

    public static PropertyKeyCountPatternConverter MostRecentInstance {
      get { return mostRecentInstance; }
    }
  }

  public class PatternStringAppender : StringAppender {
    private static PatternStringAppender mostRecentInstace;

    private PatternString setting;

    public PatternStringAppender() {
      mostRecentInstace = this;
    }

    public PatternString Setting {
      get { return setting; }
      set { setting = value; }
    }

    public static PatternStringAppender MostRecentInstace {
      get { return mostRecentInstace; }
    }
  }
}
