using Soyo.Base;
using Soyo.Base.Crypto;
using NUnit.Framework;

namespace UnitTest.Base.Crypto {
  [TestFixture]
  [Category("Soyo.Base.Crypto")]
  internal class TestEncrypt {
    const string data_null = "";
    const string data_space = " ";
    const string data_number = "723894719341372894012343432890347190247";
    const string data_a = "a";
    const string data_abc = "abc";
    const string data_a_z = "abcdefghijklmnopqrstuvwxyz";
    const string data_random = "aslkdnvfoafiaklsdfjai897l;dfja;asld;fkja;fihy7u";
    const uint crc_seed = 0x68969a4d;

    [Test]
    public void TestCrc() {
      uint crc;
      crc = Crc.Value(data_null);
      Assert.AreEqual(crc, 0);

      crc = Crc.Value(data_space);
      Assert.AreEqual(crc, 997073096);

      crc = Crc.Value(data_number);
      Assert.AreEqual(crc, 267473786);

      crc = Crc.Value(data_a);
      Assert.AreEqual(crc, 984961486);

      crc = Crc.Value(data_abc);
      Assert.AreEqual(crc, 3395655888);

      crc = Crc.Value(data_a_z);
      Assert.AreEqual(crc, 3821792911);

      crc = Crc.Value(data_random);
      Assert.AreEqual(crc, 2013122120);

      crc = Crc.Value(data_null, crc_seed);
      Assert.AreEqual(crc, crc_seed);

      crc = Crc.Value(data_space, crc_seed);
      Assert.AreEqual(crc, 862686079);

      crc = Crc.Value(data_number);
      Assert.AreEqual(crc, 267473786);

      crc = Crc.Value(data_a, crc_seed);
      Assert.AreEqual(crc, 850459257);

      crc = Crc.Value(data_abc, crc_seed);
      Assert.AreEqual(crc, 551932618);

      crc = Crc.Value(data_a_z, crc_seed);
      Assert.AreEqual(crc, 1939388903);

      crc = Crc.Value(data_random, crc_seed);
      Assert.AreEqual(crc, 2022238629);
    }

    [Test]
    public void TestMd5() {
      var m = Md5.ToString(data_null);
      Assert.AreEqual(m, "d41d8cd98f00b204e9800998ecf8427e");

      m = Md5.ToString(data_space);
      Assert.AreEqual(m, "7215ee9c7d9dc229d2921a40e899ec5f");

      m = Md5.ToString(data_number);
      Assert.AreEqual(m, "74617c242bdcea8acbf5ab39ed92b13c");

      m = Md5.ToString(data_a);
      Assert.AreEqual(m, "0cc175b9c0f1b6a831c399e269772661");

      m = Md5.ToString(data_abc);
      Assert.AreEqual(m, "900150983cd24fb0d6963f7d28e17f72");

      m = Md5.ToString(data_a_z);
      Assert.AreEqual(m, "c3fcd3d76192e4007dfb496cca67e13b");

      m = Md5.ToString(data_random);
      Assert.AreEqual(m, "abc4088a7ec47f844f419674484fd251");
    }

    [Test]
    public void TestSha1() {
      var m = Sha1.ToString(data_null);
      Assert.AreEqual(m, "da39a3ee5e6b4b0d3255bfef95601890afd80709");

      m = Sha1.ToString(data_space);
      Assert.AreEqual(m, "b858cb282617fb0956d960215c8e84d1ccf909c6");

      m = Sha1.ToString(data_number);
      Assert.AreEqual(m, "0a42946f27a84f32de8de91a8174129d0bfd02ad");

      m = Sha1.ToString(data_a);
      Assert.AreEqual(m, "86f7e437faa5a7fce15d1ddcb9eaeaea377667b8");

      m = Sha1.ToString(data_abc);
      Assert.AreEqual(m, "a9993e364706816aba3e25717850c26c9cd0d89d");

      m = Sha1.ToString(data_a_z);
      Assert.AreEqual(m, "32d10c7b8cf96570ca04ce37f2a19d84240d3a89");

      m = Sha1.ToString(data_random);
      Assert.AreEqual(m, "c6435c905dabb8c149792bf8afb44e3a7501466e");
    }
  }
}

