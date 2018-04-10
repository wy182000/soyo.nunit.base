﻿#if UNITY_2017
using System;
using Soyo.Base.Text;
using NUnit.Framework;
using System.IO;

namespace UnitTest.Base.Log.Util {
  [TestFixture]
  public class PatternStringTest : MarshalByRefObject {
    [Test]
    public void TestEnvironmentFolderPathPatternConverter() {
      string[] specialFolderNames = Enum.GetNames(typeof(Environment.SpecialFolder));

      foreach (string specialFolderName in specialFolderNames) {
        string pattern = "%envFolderPath{" + specialFolderName + "}";

        PatternString patternString = new PatternString(pattern);

        string evaluatedPattern = patternString.Format();

        Environment.SpecialFolder specialFolder =
            (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolderName);

        Assert.AreEqual(Environment.GetFolderPath(specialFolder), evaluatedPattern);
      }
    }

    private static string CreateTempConfigFile(string configurationFileContent) {
      string fileName = Soyo.Base.IO.Path.GetTempFile();
      File.WriteAllText(fileName, configurationFileContent);
      return fileName;
    }

    private static AppDomain CreateConfiguredDomain(string domainName, string configurationFileName) {
      AppDomainSetup ads = new AppDomainSetup();
      ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
      ads.ConfigurationFile = configurationFileName;
      AppDomain ad = AppDomain.CreateDomain(domainName, null, ads);
      return ad;
    }
  }
}
#endif
