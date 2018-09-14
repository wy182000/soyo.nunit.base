using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;

using Soyo.Base.Json;

namespace UnitTest.Base.Util.JsonTest {
  [TestFixture]
  [Category("Soyo.Base.Json")]
  internal class JsonBaseTest {
    public static string JsonStringEqualsInsteadOfColon = @"
        {
            ""integer"": {
                ""negative"" = 1
        }}";
    public static string JsonStringMissingComma = @"
        {
            ""integer"": {
                ""negative"": 1
                ""positive"": 1
        }}";
    public static string JsonStringMissingClosingBracket = @"
        {
            ""integer"": {
                ""negative"": 1,
                ""positive"": 1
            },
            ""floating"": 1.0";
    public static string JsonStringMissingOpeningBracket = @"
        
            ""integer"": {
                ""negative"": 1,
                ""positive"": 1
        }}";
    public static string JsonObjectStringWithAllTypes = @"
        {
            'string': {
                ""normal"": ""this is a string"",
                ""special"": "":,[]{}\""\\\t\n\r\b\u0041\f\m\/""
            },
            ""integer"": {
                ""positive"": 1,
                ""explicitPositive"": +1,
                ""negative"" : -1
            },
            ""long"": {
                ""positive"": 1234567890000,
                ""explicitPositive"": +1234567890000,
                ""negative"" : -1234567890000
            },
            ""floating"": {
                ""positive"": 3.14,
                ""explicitPositive"": +3.14,
                ""negative"": -3.14
            },
            ""double"": {
                ""positive"": 3.14159265359,
                ""explicitPositive"": +3.14159265359,
                ""negative"": -3.14159265359
            },
            ""exponential"": {
                ""positive"": 3E4,
                ""explicitPositive"": 3E+4,
                ""negative"": 3E-4
            },
            ""boolean"": {
                ""positive"": true,
                ""negative"": false
            },
            ""array"": {
                ""empty"": [],
                ""populated"": [1, 1.0, null, ""string"", false, {}]
            },
            ""null"": null
        }";
    public static string JsonObjectStringWithAllNull = @"
        {
            ""string"": null,
            ""integer"": null,
            ""floating"": null,
            ""exponential"": null,
            ""boolean"": null,
            ""emptyArray"": null,
            ""null"": null
        }";

    [Test]
    public void Parse_EqualsInsteadOfColon() {
      // arrange
      // nothing

      // act nodify as correct
      var node = JsonUtil.Parse(JsonStringEqualsInsteadOfColon);
      Assert.AreEqual(1, node["integer"]["negative"].Int);
    }

    [Test]
    public void Parse_MissingClosingBracket() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonStringMissingClosingBracket);
      Assert.IsNull(node);
    }

    [Test]
    public void Parse_MissingOpeningBracket() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonStringMissingOpeningBracket);
      Assert.IsNull(node);
    }

    [Test]
    public void Parse_MissingComma() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonStringMissingComma);
      Assert.IsNull(node);
    }

    [Test]
    public void Parse_EmptyJsonObject_NotNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse("{}");

      // assert
      Assert.IsNotNull(node);
    }

    [Test]
    public void Parse_EmptyString_ReturnsNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse("");

      // assert
      Assert.IsNull(node);
    }

    [Test]
    public void Parse_SimpleObject_NormalStringSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual("this is a string", node["string"]["normal"].Value);
    }

    [Test]
    public void Parse_SimpleObject_SpecialStringSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(@":,[]{}""\\t\n\r\bA\f\m/", node["string"]["special"].Value);
    }

    [Test]
    public void Parse_SimpleObject_PlainIntegerSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(1, node["integer"]["positive"].Int);
    }

    [Test]
    public void Parse_SimpleObject_ExplicitPositiveIntegerSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(1, node["integer"]["explicitPositive"].Int);
    }

    [Test]
    public void Parse_SimpleObject_NegativeIntegerSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(-1, node["integer"]["negative"].Int);
    }

    [Test]
    public void Parse_SimpleObject_FloatingSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(3.14, node["floating"]["positive"].Float, 0.000001);
    }

    [Test]
    public void Parse_SimpleObject_FloatingExplicitPositiveSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(3.14, node["floating"]["explicitPositive"].Float, 0.000001);
    }

    [Test]
    public void Parse_SimpleObject_FloatingNegativeSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(-3.14, node["floating"]["negative"].Float, 0.000001);
    }

    [Test]
    public void Parse_SimpleObject_DoubleSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(3.14159265359, node["double"]["positive"].Double, 0.000000000001);
    }

    [Test]
    public void Parse_SimpleObject_DoubleExplicitPositiveSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(3.14159265359, node["double"]["explicitPositive"].Double, 0.000000000001);
    }

    [Test]
    public void Parse_SimpleObject_DoubleNegativeSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(-3.14159265359, node["double"]["negative"].Double, 0.000000000001);
    }

    [Test]
    public void Parse_SimpleObject_PlainExponentialSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(30000, node["exponential"]["positive"].Int);
    }

    [Test]
    public void Parse_SimpleObject_ExplicitPositiveExponentialSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(30000, node["exponential"]["explicitPositive"].Int);
    }

    [Test]
    public void Parse_SimpleObject_NegativeExponentialSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(0.0003, node["exponential"]["negative"].Float, 0.000001);
    }

    [Test]
    public void Parse_Null_ThrowsException() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(null);
      Assert.IsNull(node);

      // assert
      // Expectes ArgumentNullException
    }

    [Test]
    public void Parse_SimpleObject_TrueBoolSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(true, node["boolean"]["positive"].Bool);
    }

    [Test]
    public void Parse_SimpleObject_FalseBoolSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(false, node["boolean"]["negative"].Bool);
    }

    [Test]
    public void Parse_SimpleObject_NullSuccess() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.IsTrue(node["null"].IsNull);
    }

    [Test]
    public void Parse_SimpleObject_EmptyArrayCountZero() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(0, node["array"]["empty"].Count);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayCountSix() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(6, node["array"]["populated"].Count);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayIntAtZero() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(1, node["array"]["populated"].JsonArray[0].Int);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayFloatAtOne() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual(1.0, node["array"]["populated"].JsonArray[1].Float, 0.00001);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayNullAtTwo() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.IsTrue(node["array"]["populated"].JsonArray[2].IsNull);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayStringAtThree() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.AreEqual("string", node["array"]["populated"].JsonArray[3].Value);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayBoolAtFour() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.IsFalse(node["array"]["populated"].JsonArray[4].Bool);
    }

    [Test]
    public void Parse_SimpleObject_PopulatedArrayObjectAtFive() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);

      // assert
      Assert.IsNotNull(node["array"]["populated"].JsonArray[5].JsonObject);
    }

    [Test]
    public void Parse_NullObject_IntegerNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);

      // assert
      Assert.IsTrue(node["integer"].IsNull);
    }

    [Test]
    public void Parse_NullObject_StringNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);

      // assert
      Assert.AreEqual(null, node["string"].Value);
    }

    [Test]
    public void Parse_NullObject_BoolNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);

      // assert
      Assert.IsTrue(node["boolean"].IsNull);
    }

    [Test]
    public void Parse_NullObject_FloatNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);

      // assert
      Assert.IsTrue(node["floating"].IsNull);
    }

    [Test]
    public void Parse_NullObject_ArrayNull() {
      // arrange
      // nothing

      // act
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);

      // assert
      Assert.IsTrue(node["floating"].IsNull);
    }

    [Test]
    public void Parse_ObjectWithIdenticalItems_Overwrite() {
      // arrange
      const string jsonString = @"
            {
                ""value"": ""first"",
                ""value"": ""second""
            }";

      // act
      var node = JsonUtil.Parse(jsonString);

      // assert
      Assert.AreEqual("second", node["value"].Value);
    }

    [Test]
    public void Serialize_NullObject_IntegerNull() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsTrue(node["integer"].IsNull);
    }

    [Test]
    public void Serialize_NullObject_StringNull() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);


      // assert
      Assert.AreEqual(null, node["string"].Value);
    }

    [Test]
    public void Serialize_NullObject_BoolNull() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsTrue(node["boolean"].IsNull);
    }

    [Test]
    public void Serialize_NullObject_FloatNull() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsTrue(node["floating"].IsNull);
    }

    [Test]
    public void Serialize_NullObject_ArrayNull() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsTrue(node["floating"].IsNull);
    }

    [Test]
    public void Serialize_SimpleObject_TrueBoolSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(true, node["boolean"]["positive"].Bool);
    }

    [Test]
    public void Serialize_SimpleObject_FalseBoolSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(false, node["boolean"]["negative"].Bool);
    }

    [Test]
    public void Serialize_SimpleObject_NullSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsTrue(node["null"].IsNull);
    }

    [Test]
    public void Serialize_SimpleObject_EmptyArrayCountZero() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(0, node["array"]["empty"].Count);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayCountSix() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(6, node["array"]["populated"].Count);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayIntAtZero() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(1, node["array"]["populated"].JsonArray[0].Int);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayFloatAtOne() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(1.0, node["array"]["populated"].JsonArray[1].Float, 0.00001);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayNullAtTwo() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsTrue(node["array"]["populated"].JsonArray[2].IsNull);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayStringAtThree() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual("string", node["array"]["populated"].JsonArray[3].Value);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayBoolAtFour() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsFalse(node["array"]["populated"].JsonArray[4].Bool);
    }

    [Test]
    public void Serialize_SimpleObject_PopulatedArrayObjectAtFive() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.IsNotNull(node["array"]["populated"].JsonArray[5].JsonObject);
    }

    [Test]
    public void Serialize_SimpleObject_NormalStringSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual("this is a string", node["string"]["normal"].Value);
    }

    [Test]
    public void Serialize_SimpleObject_SpecialStringSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(@":,[]{}""\\t\n\r\bA\f\m/", node["string"]["special"].Value);
    }

    [Test]
    public void Serialize_SimpleObject_PlainIntegerSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(1, node["integer"]["positive"].Int);
    }

    [Test]
    public void Serialize_SimpleObject_ExplicitPositiveIntegerSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(1, node["integer"]["explicitPositive"].Int);
    }

    [Test]
    public void Serialize_SimpleObject_NegativeIntegerSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(-1, node["integer"]["negative"].Int);
    }

    [Test]
    public void Serialize_SimpleObject_PlainLongSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(1234567890000, node["long"]["positive"].Long);
    }

    [Test]
    public void Serialize_SimpleObject_ExplicitPositiveLongSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(1234567890000, node["long"]["explicitPositive"].Long);
    }

    [Test]
    public void Serialize_SimpleObject_NegativeLongSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(-1234567890000, node["long"]["negative"].Long);
    }

    [Test]
    public void Serialize_SimpleObject_FloatingSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(3.14, node["floating"]["positive"].Float, 0.000001);
    }

    [Test]
    public void Serialize_SimpleObject_FloatingExplicitPositiveSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(3.14, node["floating"]["explicitPositive"].Float, 0.000001);
    }

    [Test]
    public void Serialize_SimpleObject_FloatingNegativeSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(-3.14, node["floating"]["negative"].Float, 0.000001);
    }

    [Test]
    public void Serialize_SimpleObject_DoubleSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(3.14159265359, node["double"]["positive"].Double, 0.000000000001);
    }

    [Test]
    public void Serialize_SimpleObject_DoubleExplicitPositiveSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(3.14159265359, node["double"]["explicitPositive"].Double, 0.000000000001);
    }

    [Test]
    public void Serialize_SimpleObject_DoubleNegativeSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(-3.14159265359, node["double"]["negative"].Double, 0.000000000001);
    }

    [Test]
    public void Serialize_SimpleObject_PlainExponentialSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(30000, node["exponential"]["positive"].Int);
    }

    [Test]
    public void Serialize_SimpleObject_ExplicitPositiveExponentialSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(30000, node["exponential"]["explicitPositive"].Int);
    }

    [Test]
    public void Serialize_SimpleObject_NegativeExponentialSuccess() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllTypes);
      var memoryStream = new MemoryStream();
      var binaryWriter = new BinaryWriter(memoryStream);
      var binaryReader = new BinaryReader(memoryStream);

      // act
      node.Save(binaryWriter);
      memoryStream.Seek(0, SeekOrigin.Begin);
      node = JsonUtil.Load(binaryReader);

      // assert
      Assert.AreEqual(0.0003, node["exponential"]["negative"].Float, 0.000001);
    }

    [Test]
    public void Serialize_SimpleObject_Base64() {
      // arrange
      var node = JsonUtil.Parse(JsonObjectStringWithAllNull);

      // act
      var base64 = JsonUtil.ToBase64String(node);
      var loaded = JsonUtil.FromBase64String(base64).ToString();

      // assert
      Assert.IsTrue(
          String.Compare(JsonObjectStringWithAllNull, loaded, CultureInfo.CurrentCulture,
              CompareOptions.IgnoreSymbols) ==
          0);
    }

    [Test]
    public void JsonClass_Set_EscapeNewLine() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\n");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\n"));
    }

    [Test]
    public void JsonClass_Set_EsapeCarriageReturn() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\r");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\r"));
    }

    [Test]
    public void JsonClass_Set_EscapeForwardSlash() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("/");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\/"));
    }

    [Test]
    public void JsonClass_Set_EscapeBackSlash() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\\");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\\\"));
    }

    [Test]
    public void JsonClass_Set_EscapeQuote() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\"\"");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\\"\\\""));
    }

    [Test]
    public void JsonClass_Set_EscapeTab() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\t");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\t"));
    }

    [Test]
    public void JsonClass_Set_EscapeB() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\b");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\b"));
    }

    [Test]
    public void JsonClass_Set_EscapeF() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\f");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\f"));
    }

    [Test]
    public void JsonClass_Set_DontEscapeG() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("\\g");


      // act
      var result = node.ToString();

      // assert
      Assert.IsTrue(result.Contains("\\g"));
    }

    [Test]
    public void JsonClass_Set_WriteOverItemUpdatesValue() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("string");

      // act
      node["value"] = new JsonString("string2");

      // assert
      Assert.AreEqual("string2", node["value"].Value);
    }

    [Test]
    public void JsonClass_Set_WriteOverDoesNotAddItem() {
      // arrange
      var node = new JsonObject();
      node["value"] = new JsonString("string");

      // act
      node["value"] = new JsonString("string2");

      // assert
      Assert.AreEqual(1, node.Count);
    }

    [Test]
    public void JsonClass_Remove_NonExistantNode() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");

      // act
      var removed = node.Remove(toRemove);

      // assert
      if (removed != null) {
        Assert.Fail();
      }
    }

    [Test]
    public void JsonClass_Remove_NonNode() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");
      node.Add(toRemove);

      // act
      var removed = node.Remove(toRemove);

      // assert
      Assert.AreEqual(toRemove, removed);
    }

    [Test]
    public void JsonClass_Remove_NegativeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");
      node.Add(toRemove);

      // act
      var removed = node.Remove(-1);

      // assert
      Assert.IsNull(removed);
    }

    [Test]
    public void JsonClass_Remove_IndexTooHigh() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");
      node.Add(toRemove);

      // act
      var removed = node.Remove(2);

      // assert
      Assert.IsNull(removed);
    }

    [Test]
    public void JsonClass_Remove_ExistingIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");
      node.Add(toRemove);

      // act
      var removed = node.Remove(0);

      // assert
      Assert.AreEqual(toRemove, removed);
    }

    [Test]
    public void JsonClass_Remove_NonExistantKey() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");
      node.Add(toRemove);

      // act
      var removed = node.Remove("value");

      // assert
      Assert.IsNull(removed);
    }

    [Test]
    public void JsonClass_Remove_ExistingKey() {
      // arrange
      var node = new JsonObject();
      JsonNode toRemove = new JsonString("string");
      node["value"] = toRemove;

      // act
      var removed = node.Remove("value");

      // assert
      Assert.AreEqual(toRemove, removed);
    }

    [Test]
    public void JsonClass_Get_AtNegativeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toAccess = new JsonString("string");
      node.Add(toAccess);

      // act
      var accessed = node[-1];

      // assert
      if (accessed != null) {
        Assert.Fail();
      }
    }

    [Test]
    public void JsonClass_Get_AtOutOfRangeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toAccess = new JsonString("string");
      node.Add(toAccess);

      // act
      var accessed = node[2];

      // assert
      if (accessed != null) {
        Assert.Fail();
      }
    }

    [Test]
    public void JsonClass_Get_AtExistingIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toAccess = new JsonString("string");
      node.Add(toAccess);

      // act
      var accessed = node[0];

      // assert
      Assert.AreEqual(toAccess, accessed);
    }

    [Test]
    public void JsonClass_Set_AtNegativeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toAccess = new JsonString("string");

      // act
      node[-1] = toAccess;

      // assert
      Assert.AreEqual(1, node.Count);
    }

    [Test]
    public void JsonClass_Set_AtOutOfRangeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toAccess = new JsonString("string");

      // act
      node[2] = toAccess;

      // assert
      Assert.AreEqual(1, node.Count);
    }

    [Test]
    public void JsonClass_Set_AtExistingIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode toAccess = new JsonString("string");
      node.Add(new JsonString(null));

      // act
      node[0] = toAccess;

      // assert
      Assert.AreEqual(toAccess, node[0]);
    }

    [Test]
    public void JsonClass_GetEnumerator_TwoItems() {
      // arrange
      var node = new JsonObject();
      node.Add(new JsonString("string"));
      node.Add(new JsonString(null));

      // act
      var i = 0;
      foreach (var item in node) {
        ++i;
      }

      // assert
      Assert.AreEqual(2, i);
    }

    [Test]
    public void JsonClass_Children_TwoItems() {
      // arrange
      var node = new JsonObject();
      node.Add(new JsonString("string"));
      node.Add(new JsonString(null));

      // act
      var i = 0;
      foreach (var item in node.Children) {
        ++i;
      }

      // assert
      Assert.AreEqual(2, i);
    }

    [Test]
    public void Array_Add_Null() {
      // arrange
      var node = new JsonObject();

      // act
      node["value"].Add(JsonNull.Null);

      // assert
      Assert.IsTrue(node["value"][0].IsNull);
    }

    [Test]
    public void Array_Add_String() {
      // arrange
      var node = new JsonObject();

      // act
      node["value"].Add(new JsonString("string"));

      // assert
      Assert.AreEqual("string", node["value"][0].Value);
    }

    [Test]
    public void Array_Add_Int() {
      // arrange
      var node = new JsonObject();

      // act
      node["value"].Add(new JsonNumber(1));

      // assert
      Assert.AreEqual(1, node["value"][0].Int);
    }

    [Test]
    public void Array_Add_Long() {
      // arrange
      var node = new JsonObject();
      const long expected = 12345678900000;

      // act
      node["value"].Add(new JsonNumber(expected));

      // assert
      Assert.AreEqual(expected, node["value"][0].Long);
    }

    [Test]
    public void Array_Add_Bool() {
      // arrange
      var node = new JsonObject();
      const bool expected = true;

      // act
      node["value"].Add(new JsonBool(expected));

      // assert
      Assert.AreEqual(expected, node["value"][0].Bool);
    }

    [Test]
    public void Array_Add_Double() {
      // arrange
      var node = new JsonObject();
      const double expected = 1.5;

      // act
      node["value"].Add(new JsonNumber(expected));

      // assert
      Assert.AreEqual(expected, node["value"][0].Double);
    }

    [Test]
    public void Array_Add_Float() {
      // arrange
      var node = new JsonObject();
      const float expected = 1.5f;

      // act
      node["value"].Add(new JsonNumber(expected));

      // assert
      Assert.AreEqual(expected, node["value"][0].Float);
    }

    [Test]
    public void Array_Remove_WithOneItemAtIndex() {
      // arrange
      var node = new JsonObject();
      const float value = 1.5f;
      node["value"].Add(new JsonNumber(value));

      // act
      node["value"].Remove(0);

      // assert
      Assert.AreEqual(0, node["value"].Count);
    }

    [Test]
    public void Array_Remove_WithOneItemByObject() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      node["value"].Remove(value);

      // assert
      Assert.AreEqual(0, node["value"].Count);
    }

    [Test]
    public void Array_Remove_WithTwoItemsAtIndex() {
      // arrange
      var node = new JsonObject();
      const float value = 1.5f;
      node["value"].Add(new JsonNumber(value));
      node["value"].Add(new JsonNumber(value));

      // act
      node["value"].Remove(0);

      // assert
      Assert.AreEqual(1, node["value"].Count);
    }

    [Test]
    public void Array_Remove_WithTwoItemsByObject() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);
      node["value"].Add(new JsonNumber(2.0));

      // act
      node["value"].Remove(value);

      // assert
      Assert.AreEqual(1, node["value"].Count);
    }

    [Test]
    public void Array_Remove_NegativeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      var removed = node["value"].Remove(-1);

      // assert
      Assert.IsNull(removed);
    }

    [Test]
    public void Array_Remove_IndexTooHigh() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      var removed = node["value"].Remove(2);

      // assert
      Assert.IsNull(removed);
    }

    [Test]
    public void Array_Get_NegativeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      var accessed = node["value"][-1];

      // assert
      Assert.IsNotNull(accessed);
    }

    [Test]
    public void Array_Get_IndexTooHigh() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      var accessed = node["value"][2];

      // assert
      Assert.IsNotNull(accessed);
    }

    [Test]
    public void Array_GetByString_Object() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      var accessed = node["value"]["key"];

      // assert
      Assert.IsNotNull(accessed);
    }

    [Test]
    public void Array_Set_NegativeIndex() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      node["value"][-1] = new JsonString(null);

      // assert
      Assert.AreEqual(1, node["value"].Count);
    }

    [Test]
    public void Array_Set_IndexEqualCount() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      node["value"][1] = new JsonString(null);

      // assert
      Assert.AreEqual(2, node["value"].Count);
    }

    [Test]
    public void Array_Set_IndexTooHigh() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      node["value"][2] = new JsonString(null);

      // assert
      Assert.AreEqual(1, node["value"].Count);
    }

    [Test]
    public void Array_SetByString_IndexTooHigh() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      node["value"]["key"] = new JsonString(null);

      // assert
      Assert.AreEqual(1, node["value"].Count);
    }

    [Test]
    public void Array_Set_OverWrite() {
      // arrange
      var node = new JsonObject();
      JsonNode value = new JsonNumber(1.5);
      node["value"].Add(value);

      // act
      node["value"][0] = new JsonString(null);

      // assert
      Assert.AreEqual(1, node["value"].Count);
    }

    [Test]
    public void Array_GetEnumerator_EmptyArray() {
      // arrange
      var array = new JsonArray();
      // act
      foreach (var item in array) {
        Assert.Fail();
      }

      // assert
    }

    [Test]
    public void Array_GetEnumerator_OneItem() {
      // arrange
      var array = new JsonArray();
      array.Add(new JsonString(null));
      // act
      var i = 0;
      foreach (var item in array) {
        ++i;
      }

      // assert
      Assert.AreEqual(1, i);
    }

    [Test]
    public void Array_Children_EmptyArray() {
      // arrange
      var array = new JsonArray();
      // act
      foreach (var item in array.Children) {
        Assert.Fail();
      }

      // assert
    }

    [Test]
    public void Array_Children_OneItem() {
      // arrange
      var array = new JsonArray();
      array.Add(new JsonString(null));
      // act
      var i = 0;
      foreach (var item in array.Children) {
        ++i;
      }

      // assert
      Assert.AreEqual(1, i);
    }

    [Test]
    public void ToString_SimpleObject_NullSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": null
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_IntegerSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": 1
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_DoubleSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": 1.5
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_StringSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": ""String""
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_LongSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": 12345678900000,
                ""value2"": 12345678900001
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_BoolSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": true,
                ""value2"": false
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_ObjectSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": {},
                ""value2"": {
                    ""array"": [],
                    ""integer"": 1,
                    ""float"": 1.5,
                    ""string "": ""string"",
                    ""boolean"": false
                }
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToString_SimpleObject_ArraySuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": [],
                ""value2"": [1, 1.5, ""string"", false, null, {}]
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_NullSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": null
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_IntegerSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": 1
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_DoubleSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": 1.5
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_StringSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": ""String""
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_LongSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": 12345678900000,
                ""value2"": 12345678900001
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_BoolSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": true,
                ""value2"": false
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringToStringFormatted_SimpleObject_ObjectSuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": {},
                ""value2"": {
                    ""array"": [],
                    ""integer"": 1,
                    ""float"": 1.5,
                    ""string "": ""string"",
                    ""boolean"": false
                }
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }

    [Test]
    public void ToStringFormatted_SimpleObject_ArraySuccess() {
      // arrange
      const string expected = @"
            {
                ""value"": [],
                ""value2"": [1, 1.5, ""string"", false, null, {}]
            }";
      var node = JsonUtil.Parse(expected);

      // act
      var actual = node.ToString();

      // assert
      Assert.IsTrue(String.Compare(expected, actual, CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols) ==
                    0);
    }
  }
}