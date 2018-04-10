#if UNITY_2017
using System;
using System.Xml;
using NUnit.Framework;

using Soyo.Base.Log.Repository.Hierarchy;

namespace UnitTest.Base.Log.Hierarchy {
  [TestFixture]
  public class XmlHierarchyConfiguratorTest {

    private string testProp;

    public string TestProp {
      set {
        testProp = value;
      }
      get {
        return testProp;
      }
    }

    [Test]
    [Platform(Include = "Win")]
    public void EnvironmentOnWindowsIsCaseInsensitive() {
      SetTestPropWithPath();
      Assert.AreNotEqual("Path=", TestProp);
    }

    [Test]
    [Platform(Include = "Unix")]
    public void EnvironmentOnUnixIsCaseSensitive() {
      SetTestPropWithPath();
      Assert.AreEqual("Path=", TestProp);
    }

    private void SetTestPropWithPath() {
      XmlDocument doc = new XmlDocument();
      XmlElement el = doc.CreateElement("param");
      el.SetAttribute("name", "TestProp");
      el.SetAttribute("value", "Path=${path}");
      new TestConfigurator().PublicSetParameter(el, this);
    }

    // workaround for SetParameter being protected
    private class TestConfigurator : XmlHierarchyConfigurator {
      public TestConfigurator() : base(null) {
      }
      public void PublicSetParameter(XmlElement element, object target) {
        SetParameter(element, target);
      }
    }
  }
}
#endif