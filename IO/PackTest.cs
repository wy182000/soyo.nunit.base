using Soyo.Base;
using Soyo.Base.IO;

using NUnit.Framework;

namespace UnitTest.Base.IO {
  [TestFixture]
  [Category("Soyo.Base.IO")]
  internal class PackTest {
    private readonly static string test_readonly_pack_name = "test_readonly.pack";
    private readonly static string test_pack_name = "test.pack";
    private readonly static string test_dir_name = "test";
    private readonly static string test_multi_dir_parent_name = "a/bb";
    private readonly static string test_multi_dir_name = "a/bb/cccc";
    private readonly static string test_multi_file_parent_name = "aa/bbb/cccc";
    private readonly static string test_multi_file_name = "aa/bbb/cccc/test.test";
    private readonly static string test_file_name = "test/test.test";
    private readonly static string test_compress_file_name = "test/test_compress.test";
    private readonly static string test_readonly_dir_name = "test_readonly";
    private readonly static string test_readonly_file_name = "test/test_readonly.test";

    [TearDown]
    public void Term() {
      Pack.clear();
    }

    [Test]
    public void TestPack() {
      int fd = Pack.pack_open(test_readonly_pack_name, true);
      Assert.AreEqual(fd, -1);

      fd = Pack.pack_open(test_pack_name, false);
      Assert.AreNotEqual(fd, -1);

      int write_size = 0;
      int data_count = 1000;
      int data_size = sizeof(uint) * data_count;
      int buffer_size = data_size + 400;
      ByteBuffer buffer = new ByteBuffer(buffer_size);

      // rm file first, rmdir can not rm not empty dir
      bool flag = Pack.exist(fd, test_multi_file_name);
      if (flag) {
        flag = Pack.rm(fd, test_multi_file_name);
        Assert.IsTrue(flag);
      }

      flag = Pack.exist(fd, test_multi_dir_name);
      if (flag) {
        flag = Pack.rmdir(fd, test_multi_dir_name);
        Assert.IsTrue(flag);
      }

      flag = Pack.exist(fd, test_multi_dir_parent_name);
      if (flag) {
        flag = Pack.rmdir(fd, test_multi_dir_parent_name);
        Assert.IsTrue(flag);
      }

      flag = Pack.exist(fd, test_multi_file_parent_name);
      if (flag) {
        flag = Pack.rmdir(fd, test_multi_file_parent_name);
        Assert.IsTrue(flag);
      }

      flag = Pack.exist(fd, test_file_name);
      if (flag) {
        flag = Pack.rm(fd, test_file_name);
        Assert.IsTrue(flag);
      }

      flag = Pack.exist(fd, test_compress_file_name);
      if (flag) {
        flag = Pack.rm(fd, test_compress_file_name);
        Assert.IsTrue(flag);
      }

      flag = Pack.exist(fd, test_dir_name);
      if (flag) {
        flag = Pack.rmdir(fd, test_dir_name);
        Assert.IsTrue(flag);
      }

      // test multi dir
      var node = Pack.open(fd, test_multi_dir_name);
      Assert.IsNull(node);

      flag = Pack.mkdir(fd, test_multi_dir_name, false);
      Assert.IsFalse(flag);

      flag = Pack.mkdir(fd, test_multi_dir_name, true);
      Assert.IsTrue(flag);

      flag = Pack.exist(fd, test_multi_dir_name);
      Assert.IsTrue(flag);

      // test multi file
      node = Pack.open(fd, test_multi_file_name);
      Assert.IsNull(node);

      node = Pack.create(fd, test_multi_file_name, false);
      Assert.IsNull(node);

      node = Pack.create(fd, test_multi_file_name, true);
      Assert.IsNotNull(node);

      Pack.close(node);

      flag = Pack.exist(fd, test_multi_file_name);
      Assert.IsTrue(flag);

      // test dir
      node = Pack.open(fd, test_dir_name);
      Assert.IsNull(node);

      flag = Pack.mkdir(fd, test_dir_name);
      Assert.IsTrue(flag);

      flag = Pack.mkdir(fd, test_dir_name);
      Assert.IsFalse(flag);

      node = Pack.open(fd, test_dir_name);
      Assert.IsNotNull(node);

      int size = Pack.size(node);
      Assert.AreEqual(size, 1024);

      flag = Pack.is_diretory(node);
      Assert.IsTrue(flag);

      Pack.close(node);

      // test file
      node = Pack.open(fd, test_file_name);
      Assert.IsNull(node);

      node = Pack.create(fd, test_file_name);
      Assert.IsNotNull(node);

      unsafe {
        for (int i = 0; i < data_count; i++) {
          Pack.write(node, (byte*)&i, sizeof(int), sizeof(int) * i);
          buffer.WriteUint32(sizeof(int) * i, (uint)i);
        }
      }

      Pack.close(node);

      // test compress file
      node = Pack.open(fd, test_compress_file_name);
      Assert.IsNull(node);

      node = Pack.create(fd, test_compress_file_name);
      Assert.IsNotNull(node);

      write_size = Pack.write_compress(node, buffer.Array, data_size);
      Assert.Less(0, write_size);
      Assert.Less(write_size, data_size);

      Pack.close(node);

      // pack close
      Pack.pack_close(fd);

      // readonly
      fd = Pack.pack_open(test_pack_name, true);
      Assert.AreNotEqual(fd, -1);

      // rm file first, rmdir can not rm not empty dir
      flag = Pack.exist(fd, test_file_name);
      Assert.IsTrue(flag);

      flag = Pack.rm(fd, test_file_name);
      Assert.IsFalse(flag);

      flag = Pack.exist(fd, test_dir_name);
      Assert.IsTrue(flag);

      flag = Pack.rmdir(fd, test_dir_name);
      Assert.IsFalse(flag);

      flag = Pack.mkdir(fd, test_readonly_dir_name);
      Assert.IsFalse(flag);

      node = Pack.create(fd, test_readonly_file_name);
      Assert.IsNull(node);

      node = Pack.open(fd, test_dir_name);
      Assert.IsNotNull(node);

      size = Pack.size(node);
      Assert.AreEqual(size, 1024);

      flag = Pack.is_diretory(node);
      Assert.IsTrue(flag);

      Pack.close(node);

      // test file
      node = Pack.open(fd, test_file_name);
      Assert.IsNotNull(node);

      int data = 0;
      unsafe {
        size = Pack.write(node, (byte*)&data, sizeof(int), sizeof(int));
      }
      Assert.AreEqual(size, 0);

      size = Pack.size(node);
      Assert.AreEqual(size, data_size);

      unsafe {
        for (int i = 0; i < data_count; i++) {
          int value;
          size = Pack.read(node, (byte*)&value, sizeof(int), sizeof(int) * i);
          Assert.AreEqual(size, sizeof(uint));
          Assert.AreEqual(value, i);
        }
      }

      Pack.close(node);

      // test compress file
      node = Pack.open(fd, test_compress_file_name);
      Assert.IsNotNull(node);

      size = Pack.size(node);
      Assert.AreEqual(size, write_size);

      var read_buffer = Pack.read_compress(node, ref size);
      Assert.AreEqual(size, data_size);

      flag = ArrayUtil.Equals(read_buffer, buffer.Array, data_size);
      Assert.IsTrue(flag);

      Pack.close(node);
      Pack.pack_close(fd);
    }
  }
}
