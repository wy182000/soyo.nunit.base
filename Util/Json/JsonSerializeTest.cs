using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Soyo.Base;
using Soyo.Base.Json;

#pragma warning disable 0649

namespace UnitTest.Base.Util.JsonTest {

  [TestFixture]
  [Category("Soyo.Base.Json")]
  internal class JsonSerializeTest {
    // ----------------------------------------------
    // Classes for basic example
    public class Bar {
      public int z = 0;
      public int a = 0;
    }
    public class Foo {
      public int x = 0;
      public int y = 0;
      public Bar[] g = null;
      public int[][] b = null;
    }

    public class Moo {
      public string a;
      public int[] b;
      public Bar c;
    };

    // Classes for SomeUndefinedExample
    public class Something {
      public string a;
      public string b;
    }

    // ----------------------------------------------
    // Classes for Dervied example #1

    [ConverterFrom(typeof(JsonNode), typeof(FruitCreator))]
    public class Fruit {
      public int fruitType; // 0 = Apple, 1 = Raspberry
    };

    public class Apple : Fruit {
      public float height;
      public float radius;
    };

    public class Raspberry : Fruit {
      public int numBulbs;
    }

    public class FruitCreator : IConvertFrom {
      public virtual object ConvertFrom(object node, object state) {
        if (node == null) return null;
        var json = node as JsonObject;
        if (json == null) return null;
        int fruitType = json["fruitType"].Int;
        if (fruitType == 0) {
          return new Apple();
        } else if (fruitType == 1) {
          return new Raspberry();
        }
        return null;
      }

      // Tell the Deserializer the base type we create.
      public virtual bool CanConvertFrom(Type type) {
        return typeof(JsonObject) == type;
      }
    }

    // ----------------------------------------------
    // Classes for Derived example #2

    public class Message {
      public string msgType;
      public MessageData data;
    }

    [ConverterFrom(typeof(JsonNode), typeof(MessageDataCreator))]
    public class MessageData {
    }

    public class MessageDataMouseMove : MessageData {
      public int x;
      public int y;
    }

    public class MessageDataKeyDown : MessageData {
      public int keyCode;
    }

    public class MessageDataCreator : IConvertFrom {
      public virtual object ConvertFrom(object value, object state) {
        var node = state as JsonObject;
        if (node == null) return null;
        string msgType = node["msgType"];
        if (msgType.Equals("mouseMove")) {
          return new MessageDataMouseMove();
        } else if (msgType.Equals("keyDown")) {
          return new MessageDataKeyDown();
        }
        return null;
      }

      public virtual bool CanConvertFrom(Type type) {
        return typeof(JsonObject) == type;
      }
    }

    // ----------------------------------------------
    // Classes to test auto type serialization

    public class Animal {
    }

    public class Dog : Animal {
      public Dog() { }  // must have no param ctor for deseralization
      public Dog(int _barkiness) {
        barkiness = _barkiness;
      }
      public int barkiness;
    }

    public class Cat : Animal {
      public Cat() { }  // must have no param ctor for deseralization
      public Cat(string _stealthiness) {
        stealthiness = _stealthiness;
      }
      public string stealthiness;
    }

    // ----------------------------------------------
    // Classes for more advanced example.
    //
    // This is basically the same as Derived sample #2 except with
    // a few fancier classes to make adding new MessageCmdData derived
    // classes really easy.

    [ConverterFrom(typeof(JsonNode), typeof(MessageCmdDataCreator))]
    public class MessageCmdData {
    };

    public class MessageCmd {
      public string cmd;
      public MessageCmdData data;
    };

    public class MessageToClient {
      public string cmd;  // command 'server', 'update'
      public int id;      // id of client
      public MessageCmd data;
    };

    [CmdName("setColor")]
    public class MessageSetColor : MessageCmdData {
      public string color;
      public string style;
    };

    [CmdName("setName")]
    public class MessageSetName : MessageCmdData {
      public string name;
    };

    [CmdName("launch")]
    public class MessageLaunch : MessageCmdData {
    };

    [CmdName("die")]
    public class MessageDie : MessageCmdData {
      public string killer;
      public bool crash;
    };

    [AttributeUsage(AttributeTargets.Class)]
    public class CmdNameAttribute : Attribute {
      public readonly string CmdName;

      public CmdNameAttribute(string cmdName) {
        this.CmdName = cmdName;
      }
    }

    public class MessageCmdDataCreator : Singleton<MessageCmdDataCreator>, IConvertFrom {
      public abstract class Creator {
        public abstract object Create();
      }

      public class TypedCreator<T> : Creator where T : new() {
        public override object Create() {
          return new T();
        }
      }

      private MessageCmdDataCreator() {
        m_creators = new Dictionary<string, Creator>();
      }

      public void RegisterCreator<T>() where T : new() {
        Type type = typeof(T);
        string name = null;

        //Querying Class Attributes
        foreach (Attribute attr in type.GetCustomAttributes(true)) {
          CmdNameAttribute cmdNameAttr = attr as CmdNameAttribute;
          if (cmdNameAttr != null) {
            name = cmdNameAttr.CmdName;
          }
        }

        if (name == null) {
          InvalidOperationException ex = new InvalidOperationException("missing CmdNameAttribute");
          throw ex;
        }

        m_creators[name] = new TypedCreator<T>();
      }

      public virtual object ConvertFrom(object source, object parent) {
        var node = parent as JsonObject;
        if (node == null) return null;
        string typeName = node["cmd"];
        Creator creator;
        if (m_creators.TryGetValue(typeName, out creator)) {
          return creator.Create();
        }
        return null;
      }

      public bool CanConvertFrom(Type type) {
        return type == typeof(JsonObject);
      }

      Dictionary<string, Creator> m_creators;
    };

    public void CheckFoo(Foo f) {
      Assert.AreEqual(f.x, 123);
      Assert.AreEqual(f.y, 456);
      Assert.AreEqual(f.g[0].z, 5);
      Assert.AreEqual(f.g[0].a, 12);
      Assert.AreEqual(f.g[1].z, 4);
      Assert.AreEqual(f.g[1].a, 23);
      Assert.AreEqual(f.g[2].z, 3);
      Assert.AreEqual(f.g[2].a, 34);
      Assert.AreEqual(f.b.Length, 3);
      Assert.AreEqual(f.b[0].Length, 2);
      Assert.AreEqual(f.b[0][0], 1);
      Assert.AreEqual(f.b[0][1], 2);
      Assert.AreEqual(f.b[1].Length, 3);
      Assert.AreEqual(f.b[1][0], 4);
      Assert.AreEqual(f.b[1][1], 5);
      Assert.AreEqual(f.b[1][2], 6);
      Assert.AreEqual(f.b[2].Length, 2);
      Assert.AreEqual(f.b[2][0], 7);
      Assert.AreEqual(f.b[2][1], 6);
    }


    [Test]
    public void BasicTest() {
      string json = "{\"x\":123,\"y\":456,\"g\":[{\"z\":5,\"a\":12},{\"z\":4,\"a\":23},{\"z\":3,\"a\":34}],\"b\":[[1,2],[4,5,6],[7,6]]}";

      Foo f = Json.ToObject<Foo>(json);
      CheckFoo(f);

      string s = Json.ToJson(f);
      Foo f2 = Json.ToObject<Foo>(s);

      CheckFoo(f2);
    }

    public void CheckArray(int[] k) {
      Assert.AreEqual(k.Length, 3);
      Assert.AreEqual(k[0], 4);
      Assert.AreEqual(k[1], 7);
      Assert.AreEqual(k[2], 9);
    }

    [Test]
    public void ArrayTest() {
      string kj = "[4,7,9]";

      int[] k = Json.ToObject<int[]>(kj);

      CheckArray(k);

      string s = Json.ToJson(k);
      int[] k2 = Json.ToObject<int[]>(s);

      CheckArray(k2);
    }

    [Test]
    public void UndefinedTest() {
      string u1 = "{}";

      Moo m = Json.ToObject<Moo>(u1);
      Assert.AreEqual(m.b, null);
      Assert.AreEqual(m.a, null);
      Assert.AreEqual(m.c, null);

      string s = Json.ToJson(m);
      Assert.AreEqual(s, @"{""a"":null,""b"":null,""c"":null}");
    }

    [Test]
    public void SomeUndefinedTest() {
      string u1 = "{\"a\":\"b\"}";

      Something s = Json.ToObject<Something>(u1);
      Assert.AreEqual(s.a, "b");
      Assert.AreEqual(s.b, null);

      string ss = Json.ToJson(s);

      Assert.AreEqual(ss, "{\"a\":\"b\",\"b\":null}");

      Something s2 = Json.ToObject<Something>(ss);

      Assert.AreEqual(s2.a, "b");
      Assert.AreEqual(s2.b, null);
    }

    public void CheckApple(Apple a) {
      Assert.AreEqual(a.fruitType, 0);
      Assert.AreEqual(a.height, 1.2f);
      Assert.AreEqual(a.radius, 7);
    }

    public void CheckRaspberry(Raspberry r) {
      Assert.AreEqual(r.fruitType, 1);
      Assert.AreEqual(r.numBulbs, 27);
    }

    [Test]
    public void DerviedTest01() {
      string appleJson = "{\"fruitType\":0,\"height\":1.2,\"radius\":7}";
      string raspberryJson = "{\"fruitType\":1,\"numBulbs\":27}";

      Fruit apple = Json.ToObject<Fruit>(appleJson);
      Fruit raspberry = Json.ToObject<Fruit>(raspberryJson);

      CheckApple((Apple)apple);
      CheckRaspberry((Raspberry)raspberry);

      string sa = Json.ToJson(apple);
      string sr = Json.ToJson(raspberry);

      Fruit apple2 = Json.ToObject<Fruit>(sa);
      Fruit raspberry2 = Json.ToObject<Fruit>(sr);

      CheckApple((Apple)apple2);
      CheckRaspberry((Raspberry)raspberry2);
    }

    public void CheckMessageDataMouseMove(Message m) {
      Assert.AreEqual(m.msgType, "mouseMove");
      Assert.AreEqual(((MessageDataMouseMove)m.data).x, 123);
      Assert.AreEqual(((MessageDataMouseMove)m.data).y, 456);
    }

    public void CheckMessageDataKeyDown(Message m) {
      Assert.AreEqual(m.msgType, "keyDown");
      Assert.AreEqual(((MessageDataKeyDown)m.data).keyCode, 789);
    }

    [Test]
    public void DerviedTest02() {
      string mouseMoveJson = "{\"msgType\":\"mouseMove\",\"data\":{\"x\":123,\"y\":456}}";
      string keyDownJson = "{\"msgType\":\"keyDown\",\"data\":{\"keyCode\":789}}";

      Message msg1 = Json.ToObject<Message>(mouseMoveJson);
      Message msg2 = Json.ToObject<Message>(keyDownJson);

      CheckMessageDataMouseMove(msg1);
      CheckMessageDataKeyDown(msg2);

      string s1 = Json.ToJson(msg1);
      string s2 = Json.ToJson(msg2);

      Message msg12 = Json.ToObject<Message>(s1);
      Message msg22 = Json.ToObject<Message>(s2);

      CheckMessageDataMouseMove(msg12);
      CheckMessageDataKeyDown(msg22);
    }

    [Test]
    public void AutoSerializationTest() {
      Dog da = new Dog(123);
      Cat ca = new Cat("super");
      Animal[] animals = new Animal[2];
      animals[0] = da;
      animals[1] = ca;

      string animalsJson = Json.ToJson(animals, true);
      Animal[] ani = Json.ToObject<Animal[]>(animalsJson);

      Assert.AreEqual(((Dog)ani[0]).barkiness, 123);
      Assert.AreEqual(((Cat)ani[1]).stealthiness, "super");
    }

    public void CheckMessageSetColor(MessageToClient m) {
      Assert.AreEqual(m.cmd, "update");
      Assert.AreEqual(m.id, 123);
      Assert.AreEqual(((MessageSetColor)m.data.data).color, "red");
      Assert.AreEqual(((MessageSetColor)m.data.data).style, "bold");
    }

    public void CheckMessageSetName(MessageToClient m) {
      Assert.AreEqual(m.cmd, "update");
      Assert.AreEqual(m.id, 345);
      Assert.AreEqual(((MessageSetName)m.data.data).name, "gregg");
    }

    public void CheckMessageLaunch(MessageToClient m) {
      Assert.AreEqual(m.cmd, "update");
      Assert.AreEqual(m.id, 789);
    }

    public void CheckMessageDie(MessageToClient m) {
      Assert.AreEqual(m.cmd, "update");
      Assert.AreEqual(m.id, 101112);
      Assert.AreEqual(((MessageDie)m.data.data).killer, "jill");
      Assert.AreEqual(((MessageDie)m.data.data).crash, true);
    }

    [Test]
    public void MoreAdvancedTest() {
      MessageCmdDataCreator.Instance.RegisterCreator<MessageSetColor>();
      MessageCmdDataCreator.Instance.RegisterCreator<MessageSetName>();
      MessageCmdDataCreator.Instance.RegisterCreator<MessageLaunch>();
      MessageCmdDataCreator.Instance.RegisterCreator<MessageDie>();

      string ja = "{\"cmd\":\"update\",\"id\":123,\"data\":{\"cmd\":\"setColor\",\"data\":{\"color\":\"red\",\"style\":\"bold\"}}}";
      string jb = "{\"cmd\":\"update\",\"id\":345,\"data\":{\"cmd\":\"setName\",\"data\":{\"name\":\"gregg\"}}}";
      string jc = "{\"cmd\":\"update\",\"id\":789,\"data\":{\"cmd\":\"launch\",\"data\":{}}}";
      string jd = "{\"cmd\":\"update\",\"id\":101112,\"data\":{\"cmd\":\"die\",\"data\":{\"killer\":\"jill\",\"crash\":true}}}";

      MessageToClient ma = Json.ToObject<MessageToClient>(ja);
      MessageToClient mb = Json.ToObject<MessageToClient>(jb);
      MessageToClient mc = Json.ToObject<MessageToClient>(jc);
      MessageToClient md = Json.ToObject<MessageToClient>(jd);

      CheckMessageSetColor(ma);
      CheckMessageSetName(mb);
      CheckMessageLaunch(mc);
      CheckMessageDie(md);

      string sa = Json.ToJson(ma);
      string sb = Json.ToJson(mb);
      string sc = Json.ToJson(mc);
      string sd = Json.ToJson(md);

      MessageToClient ma2 = Json.ToObject<MessageToClient>(sa);
      MessageToClient mb2 = Json.ToObject<MessageToClient>(sb);
      MessageToClient mc2 = Json.ToObject<MessageToClient>(sc);
      MessageToClient md2 = Json.ToObject<MessageToClient>(sd);

      CheckMessageSetColor(ma2);
      CheckMessageSetName(mb2);
      CheckMessageLaunch(mc2);
      CheckMessageDie(md2);
    }

    bool DictionariesAreSame(Dictionary<string, object> a, Dictionary<string, object> b) {
      foreach (string key in a.Keys) {
        object valueb;
        if (!b.TryGetValue(key, out valueb)) {
          Console.Error.WriteLine(String.Format("b missing key: {0}", key));
          return false;
        }
        object valuea = a[key];
        Type aType = valuea.GetType();
        if (aType != valueb.GetType()) {
          Console.Error.WriteLine(String.Format("not same type for key: {0}", key));
          return false;
        }

        if (aType.IsValueType) {
          if (!valuea.Equals(valueb)) {
            Console.Error.WriteLine(String.Format("{0} != {1} for key: {2}", valuea.ToString(), valueb.ToString(), key));
            return false;
          }
        } else if (aType == typeof(string)) {
          if (valuea.ToString() != valueb.ToString()) {
            Console.Error.WriteLine(String.Format("{0} != {1} for key: {2}", valuea.ToString(), valueb.ToString(), key));
            return false;
          }
        } else {
          // Need to handle none dictionaries
          bool same = DictionariesAreSame((Dictionary<string, object>)valuea, (Dictionary<string, object>)valueb);
          if (!same) {
            return false;
          }
        }
      }

      return true;
    }

    [Test]
    public void RoundTrip01Test() {
      string j = "{\"cmd\":\"update\",\"id\":123,\"data\":{\"cmd\":\"setColor\",\"data\":{\"color\":\"red\",\"style\":\"bold\"}}}";

      Dictionary<string, object> data1 = Json.ToObject<Dictionary<string, object>>(j);

      string newJ = Json.ToJson(data1);

      Dictionary<string, object> data2 = Json.ToObject<Dictionary<string, object>>(newJ);

      Assert.IsTrue(DictionariesAreSame(data1, data2));

      string anotherJ = Json.ToJson(data2);
      Assert.AreEqual(newJ, anotherJ);    //  this assumes given the same data we'll get the same string with fields in the same order
    }

    struct Vector2 {
      public float x;
      public float y;
    };

    [Test]
    public void StructTest01() {
      string j = "{\"x\":1.2,\"y\":3.4}";

      Vector2 v = Json.ToObject<Vector2>(j);
      Assert.AreEqual(v.x, 1.2f);
      Assert.AreEqual(v.y, 3.4f);

      string newJ = Json.ToJson(v);

      Vector2 v2 = Json.ToObject<Vector2>(newJ);

      Assert.AreEqual(v2.x, v.x);
      Assert.AreEqual(v2.y, v.y);
    }

    [Test]
    public void StructTest02() {
      string j = "[{\"x\":1.2,\"y\":3.4},{\"x\":5.6,\"y\":7.8}]";

      Vector2[] v = Json.ToObject<Vector2[]>(j);
      Assert.AreEqual(v.Length, 2);
      Assert.AreEqual(v[0].x, 1.2f);
      Assert.AreEqual(v[0].y, 3.4f);
      Assert.AreEqual(v[1].x, 5.6f);
      Assert.AreEqual(v[1].y, 7.8f);

      string newJ = Json.ToJson(v);

      Vector2[] v2 = Json.ToObject<Vector2[]>(newJ);

      Assert.AreEqual(v2.Length, 2);
      Assert.AreEqual(v2[0].x, v[0].x);
      Assert.AreEqual(v2[0].y, v[0].y);
      Assert.AreEqual(v2[1].x, v[1].x);
      Assert.AreEqual(v2[1].y, v[1].y);
    }

    struct HasStatic {
      public int someProp;
      public static int someStaticProp;
    }

    [Test]
    public void StructWithStaticTest() {
      HasStatic s = new HasStatic();
      s.someProp = 123;
      HasStatic.someStaticProp = 456;

      string json = Json.ToJson(s);
      Assert.That(json, Is.Not.Contains("someStaticProp"));

      string j = "{\"someProp\":123,\"someStaticProp\":789}";
      HasStatic s2 = Json.ToObject<HasStatic>(j);
      Assert.AreEqual(s2.someProp, 123);
      Assert.AreEqual(HasStatic.someStaticProp, 456);
    }

    class Primitives {
      public Boolean someBoolean = true;
      public Byte someByte = 1;
      public SByte someSByte = -1;
      public Int16 someInt16 = -2;
      public UInt16 someUInt16 = 2;
      public Int32 someInt32 = -3;
      public UInt32 someUInt32 = 3;
      public Int64 someInt64 = -4;
      public UInt64 someUInt64 = 4;
      public Char someChar = 'a';
      public Double someDouble = 1.23;
      public Single someSingle = 2.34f;
      public float somefloat = 3.45f;
      public int someint = 5;
      public bool somebool = true;
    };

    [Test]
    public void PrimitivesTest() {
      Primitives p = new Primitives();
      p.someBoolean = false;
      p.someByte = 6;
      p.someSByte = -6;
      p.someInt16 = -7;
      p.someUInt16 = 7;
      p.someInt32 = -8;
      p.someUInt32 = 8;
      p.someInt64 = -9;
      p.someUInt64 = 9;
      p.someChar = 'b';
      p.someDouble = 5.6;
      p.someSingle = 6.7f;
      p.somefloat = 7.8f;
      p.someint = 12;
      p.somebool = false;

      string json = Json.ToJson(p);
      Primitives p2 = Json.ToObject<Primitives>(json);

      Assert.AreEqual(p.someBoolean, p2.someBoolean);
      Assert.AreEqual(p.someByte, p2.someByte);
      Assert.AreEqual(p.someSByte, p2.someSByte);
      Assert.AreEqual(p.someInt16, p2.someInt16);
      Assert.AreEqual(p.someUInt16, p2.someUInt16);
      Assert.AreEqual(p.someInt32, p2.someInt32);
      Assert.AreEqual(p.someUInt32, p2.someUInt32);
      Assert.AreEqual(p.someInt64, p2.someInt64);
      Assert.AreEqual(p.someUInt64, p2.someUInt64);
      Assert.AreEqual(p.someChar, p2.someChar);
      Assert.AreEqual(p.someDouble, p2.someDouble);
      Assert.AreEqual(p.someSingle, p2.someSingle);
      Assert.AreEqual(p.somefloat, p2.somefloat);
      Assert.AreEqual(p.someint, p2.someint);
      Assert.AreEqual(p.somebool, p2.somebool);
    }

    enum CarType {
      Ford,
      Chevy,
      Toyota,
    };

    class Car {
      public Car() {
      }
      public Car(CarType _type) {
        type = _type;
      }
      public CarType type = CarType.Ford;
    };

    [Test]
    public void EnumTest() {
      Car a = new Car(CarType.Chevy);
      string json = Json.ToJson(a);
      Car b = Json.ToObject<Car>(json);

      string expected = "{\"type\":\"Chevy\"}";
      Assert.AreEqual(json, expected);
      Assert.AreEqual(a.type, b.type);
    }

    class ThingWithArray {
      public float[] stuff = new float[3];
    }
    class ThingWithClass {
      public ThingWithArray inner;
    }

    [Test]
    public void ArrayOFClassOfClassWithArray() {
      var twa = new ThingWithArray();
      twa.stuff[0] = 123.0f;
      twa.stuff[1] = 456.0f;
      twa.stuff[2] = 789.0f;
      ThingWithClass twc = new ThingWithClass();
      twc.inner = twa;
      ThingWithClass[] arr = new ThingWithClass[2];
      arr[0] = twc;
      arr[1] = twc;
      string json = Json.ToJson(arr);
      string expected = "[{\"inner\":{\"stuff\":[123,456,789]}},{\"inner\":{\"stuff\":[123,456,789]}}]";
      Assert.AreEqual(expected, json);

      object temp = Json.ToObject<object>(json);
      Assert.AreEqual(typeof(List<object>), temp.GetType());
      string json2 = Json.ToJson(temp);
      Assert.AreEqual(json, json2);

      ThingWithClass[] n = Json.ToObject<ThingWithClass[]>(json2);
      Assert.AreEqual(n.Length, arr.Length);
      Assert.AreEqual(n[0].inner.stuff[0], arr[0].inner.stuff[0]);
      Assert.AreEqual(n[0].inner.stuff[1], arr[0].inner.stuff[1]);
      Assert.AreEqual(n[0].inner.stuff[2], arr[0].inner.stuff[2]);
      Assert.AreEqual(n[1].inner.stuff[0], arr[1].inner.stuff[0]);
      Assert.AreEqual(n[1].inner.stuff[1], arr[1].inner.stuff[1]);
      Assert.AreEqual(n[1].inner.stuff[2], arr[1].inner.stuff[2]);
    }

    [Test]
    public void ObjectTest() {
      Car a = new Car(CarType.Chevy);
      string json = Json.ToJson(a);
      object o = Json.ToObject<object>(json);
      string json2 = Json.ToJson(o);
      Car b = Json.ToObject<Car>(json2);
      string expected = "{\"type\":\"Chevy\"}";
      Assert.AreEqual(json, expected);
      Assert.AreEqual(a.type, b.type);
    }

    public class MCD {
    }

    public class MSD : MCD {
      public int abc;
    }

    [Test]
    public void DerivedTestThroughDictionary() {

      var m = new MSD();
      m.abc = 123;
      string json = Json.ToJson(m);
      Dictionary<string, object> dict = Json.ToObject<Dictionary<string, object>>(json);
      Assert.IsTrue(dict.ContainsKey("abc"));
      Assert.AreEqual(Convert.ToInt32(dict["abc"]), 123);
      json = Json.ToJson(dict);
      var m2 = Json.ToObject<MSD>(json);
      Assert.AreEqual(m2.abc, m.abc);
    }

    public class FooList {
      public int x = 0;
      public int y = 0;
      public List<Bar> g = new List<Bar>();
    }

    [Test]
    public void ClassWithGenericListOfClasses() {
      FooList fl = new FooList();
      fl.x = 12;
      fl.y = 34;
      Bar b1 = new Bar();
      b1.z = 56;
      Bar b2 = new Bar();
      b2.z = 78;
      fl.g.Add(b1);
      fl.g.Add(b2);
      string json = Json.ToJson(fl);
      FooList fl2 = Json.ToObject<FooList>(json);
      Assert.AreEqual(fl2.x, fl.x);
      Assert.AreEqual(fl2.y, fl.y);
      Assert.AreEqual(fl2.g.Count, fl.g.Count);
      Assert.AreEqual(fl2.g[0].z, fl.g[0].z);
      Assert.AreEqual(fl2.g[1].z, fl.g[1].z);
    }

    public class FooDict {
      public int x = 0;
      public int y = 0;
      public Dictionary<string, Bar> g = new Dictionary<string, Bar>();
      public Dictionary<int, Bar> h = new Dictionary<int, Bar>();
    }

    [Test]
    public void ClassWithGenericDictOfClasses() {
      FooDict fl = new FooDict();
      fl.x = 12;
      fl.y = 34;
      Bar b1 = new Bar(); b1.z = 56;
      Bar b2 = new Bar(); b2.z = 78;
      Bar b3 = new Bar(); b1.z = 56;
      Bar b4 = new Bar(); b2.z = 78;
      fl.g["abc"] = b1;
      fl.g["def"] = b2;
      fl.h[246] = b3;
      fl.h[357] = b4;
      string json = Json.ToJson(fl);
      FooDict fl2 = Json.ToObject<FooDict>(json);
      Assert.AreEqual(fl2.x, fl.x);
      Assert.AreEqual(fl2.y, fl.y);
      Assert.AreEqual(fl2.g.Count, fl.g.Count);
      Assert.AreEqual(fl2.h.Count, fl.h.Count);
      Assert.AreEqual(fl2.g["abc"].z, fl.g["abc"].z);
      Assert.AreEqual(fl2.g["def"].z, fl.g["def"].z);
      Assert.AreEqual(fl2.h[246].z, fl.h[246].z);
      Assert.AreEqual(fl2.h[357].z, fl.h[357].z);
    }

    public class PrivateFields {
      public PrivateFields() { } // needed for deserialization
      public PrivateFields(int n, string s) {
        num = n;
        str = s;
      }

      public int Num {
        get {
          return num;
        }
        set {
          num = value;
        }
      }

      public string Str {
        get {
          return str;
        }
        set {
          str = value;
        }
      }

      private string str;
      private int num;
    }
    [Test]
    public void ClassWithPrivateFields() {
      var p = new PrivateFields(123, "abc");
      string json = Json.ToJson(p, false, true);
      PrivateFields p2 = Json.ToObject<PrivateFields>(json);
      Assert.AreEqual(p2.Num, p.Num);
      Assert.AreEqual(p2.Str, p.Str);
    }

    [Test]
    public void CheckTypeNamesWork() {
      Dog d = new Dog();
      Type t = d.GetType();
      string typeStr = t.AssemblyQualifiedName;
      Type t2 = Type.GetType(typeStr);
      Type t3 = SystemInfo.GetType(typeStr);
      Assert.AreEqual(t, t2);
      Assert.AreEqual(t, t3);
    }

    private class PropertyTestClass {
      public string name;
      public int value;
      public PropertySet porpertySet;
    }

    [Test]
    public void CheckProperty() {
      var p = new PropertySet();
      var subP = new PropertySet();
      var key = "Sub Property";
      subP.Add(key, "Success");
      var valueP = new PropertyTestClass() { name = "TestClass", value = 5, porpertySet = subP };
      p.Add("Property", valueP);

      var json = Json.ToJson(new ReadOnlyPropertySet(p));
      var checkP = Json.ToObject<PropertySet>(json);

      var checkValueP = checkP["Property"] as PropertyTestClass;
      Assert.IsNotNull(checkValueP);
      Assert.AreEqual(valueP.name, checkValueP.name);
      Assert.AreEqual(valueP.value, checkValueP.value);
      Assert.IsNotNull(checkValueP.porpertySet);

      var checkValue = checkValueP.porpertySet[key];
      Assert.IsNotNull(checkValue);
      Assert.AreEqual(checkValue, "Success");
    }

    [Test]
    public void CheckList() {
      var size = Rand.Default.Range(1, 8);
      var intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      var bitField = new BitField(intData);
      var json = Json.ToJson(bitField);
      var checkBitField = Json.ToObject<BitField>(json);
      Assert.IsTrue(bitField.Equals(checkBitField));
    }

    private class MergeClass {
      public int Field;
      public int Property { get; set; }
      public void AddValue(int value) {
        Field += value;
      }
    }

    [Test]
    public void TestMerge() {
      var list = new List<object>();
      list.Add(1);
      list.Add(2);
      var node = Json.ToJson(list);
      Json.MergeObject(node, list);
      Assert.AreEqual(list.Count, 4);
      Assert.AreEqual(list[0], 1);
      Assert.AreEqual(list[1], 2);
      Assert.AreEqual(list[2], 1);
      Assert.AreEqual(list[3], 2);

      var intList = new List<int>();
      intList.Add(1);
      intList.Add(2);
      node = Json.ToJson(intList);
      Json.MergeObject(node, intList);
      Assert.AreEqual(intList.Count, 4);
      Assert.AreEqual(intList[0], 1);
      Assert.AreEqual(intList[1], 2);
      Assert.AreEqual(intList[2], 1);
      Assert.AreEqual(intList[3], 2);


      var array = new int[] { 1, 2 };
      node = Json.ToJson(array);
      array = (int[])Json.MergeObject(node, array);
      Assert.AreEqual(array.Length, 4);
      Assert.AreEqual(array[0], 1);
      Assert.AreEqual(array[1], 2);
      Assert.AreEqual(array[2], 1);
      Assert.AreEqual(array[3], 2);

      var dict1 = new Dictionary<string, object>();
      dict1.Add("1", 1);
      dict1.Add("2", 2);
      var dict2 = new Dictionary<string, object>();
      dict2.Add("3", 3);
      dict2.Add("4", 4);
      node = Json.ToJson(dict1);
      Json.MergeObject(node, dict2);
      Assert.AreEqual(dict2.Count, 4);
      Assert.AreEqual(dict2["1"], 1);
      Assert.AreEqual(dict2["2"], 2);
      Assert.AreEqual(dict2["3"], 3);
      Assert.AreEqual(dict2["4"], 4);

      var intDict1 = new Dictionary<string, int>();
      intDict1.Add("1", 1);
      intDict1.Add("2", 2);
      var intDict2 = new Dictionary<string, int>();
      intDict2.Add("3", 3);
      intDict2.Add("4", 4);
      node = Json.ToJson(intDict1);
      Json.MergeObject(node, intDict2);
      Assert.AreEqual(intDict2.Count, 4);
      Assert.AreEqual(intDict2["1"], 1);
      Assert.AreEqual(intDict2["2"], 2);
      Assert.AreEqual(intDict2["3"], 3);
      Assert.AreEqual(intDict2["4"], 4);

      var mergeValue = new MergeClass();
      var temp = mergeValue;
      mergeValue.Field = 1;

      string jsonString = @"{ ""Property"": 2 }";
      mergeValue = (MergeClass)Json.MergeObject(jsonString, mergeValue);
      Assert.AreEqual(mergeValue, temp);
      Assert.AreEqual(mergeValue.Field, 1);
      Assert.AreEqual(mergeValue.Property, 2);

      jsonString = @"{ ""Field"": 3 }";
      Json.MergeObject(jsonString, mergeValue);
      Assert.AreEqual(mergeValue, temp);
      Assert.AreEqual(mergeValue.Field, 3);
      Assert.AreEqual(mergeValue.Property, 2);

      jsonString = @"{ ""AddValue"": 3 }";
      Json.MergeObject(jsonString, mergeValue);
      Assert.AreEqual(mergeValue, temp);
      Assert.AreEqual(mergeValue.Field, 6);
      Assert.AreEqual(mergeValue.Property, 2);
    }
  }
}