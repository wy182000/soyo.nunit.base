using System.Collections.Generic;

using Soyo.Base.IO;

using NUnit.Framework;

namespace UnitTest.Base.IO {
  [TestFixture]
  [Category("Soyo.Base.IO")]
  internal class PathTest {
    [SetUp]
    public void Init() {
      Path.RootPath = "";
    }

    [TearDown]
    public void Term() {
      Path.RootPath = "";
    }

    [Test]
    public void TestPath() {
      var file = "test.txt";
      var ret = Path.IsRealPath(file);
      Assert.IsFalse(ret, "file should not be real path, file: " + file);
      Assert.IsFalse(Path.Exists(file), "file should not be exists, file: " + file);
      Assert.IsFalse(Path.DirectoryExists(file), "file should not be exists directory, file: " + file);
      Assert.IsFalse(Path.FileExists(file), "file should not be exists file, file: " + file);

      var root = Path.DefaultRootPath;
      ret = Path.IsRealPath(root);
      Assert.IsTrue(ret, "root path should be real path, root path: " + root);
      Assert.AreEqual(root, Path.RootPath, "root path should be default, root path: " + root);
      Assert.IsTrue(Path.Exists(root), "root path should be exists, root path: " + root);
      Assert.IsTrue(Path.DirectoryExists(root), "root path should be exists directory, root path: " + root);
      Assert.IsFalse(Path.FileExists(root), "root path should not be exists file, root path: " + root);

      var directory = Path.GetDirectory(root);
      Assert.AreEqual(directory, root, "directory should be root, directory: " + directory);
      ret = Path.IsRealPath(directory);
      Assert.IsTrue(ret, "root path directory should be real path, directory: " + directory);
      Assert.IsTrue(Path.Exists(directory), "directory should be exists, directory: " + directory);
      Assert.IsTrue(Path.DirectoryExists(directory), "directory should be exists directory, directory: " + directory);
      Assert.IsFalse(Path.FileExists(directory), "directory should not be exists file, directory: " + directory);

      Path.RootPath = "data/";
      var path = Path.RootPath;
      ret = Path.IsRealPath(path);
      Assert.IsTrue(ret, "root path should be real path, root path: " + root);
      Assert.IsTrue(path.StartsWith(root), "set root path to not real path, root path should be start with default root, path: " + path);
      Assert.AreEqual(path.Substring(path.Length - 5, 5), "data/", "path should be end with data, path: " + path);
      Assert.IsFalse(Path.Exists(path), "path should not be exists, path: " + path);
      Assert.IsFalse(Path.DirectoryExists(path), "path should not be exists directory, path: " + path);
      Assert.IsFalse(Path.FileExists(path), "path should not be exists file, path: " + path);

      directory = Path.GetDirectory(path);
      Assert.AreEqual(directory, path.Substring(0, path.Length - 1), "directory should be path, directory: " + directory);

      directory = Path.GetDirectory(file);
      ret = Path.IsRealPath(directory);
      Assert.AreEqual(directory, string.Empty, "directory should be empty, directory: " + directory);
      Assert.IsFalse(ret, "file directory should not be real path, directory: " + directory);
      Assert.IsFalse(Path.Exists(directory), "directory should not be exists, directory: " + directory);
      Assert.IsFalse(Path.DirectoryExists(directory), "directory should not be exists directory, directory: " + directory);
      Assert.IsFalse(Path.FileExists(directory), "directory should not be exists file, directory: " + directory);

      path = Path.GetPath(file);
      ret = Path.IsRealPath(path);
      Assert.IsTrue(ret, "path should be real path, path: " + path);
      Assert.IsFalse(Path.Exists(path), "path should not be exists, path: " + path);
      Assert.IsFalse(Path.DirectoryExists(path), "path should not be exists directory, path: " + path);
      Assert.IsFalse(Path.FileExists(path), "path should not be exists file, path: " + path);

      file = Path.GetFileNameWithoutExtension(path);
      Assert.AreEqual(file, "test", "file without extension should be test, path: " + path);

      file = Path.GetFileName(path);
      Assert.AreEqual(file, "test.txt", "file without extension should be test, path: " + path);

      var extension = Path.GetExtension(file);
      Assert.AreEqual(extension, ".txt", "extension should be txt");

      file = Path.ChangeExtension(file, ".xtx");

      extension = Path.GetExtension(file);
      Assert.AreEqual(extension, ".xtx", "extension should be xtx");

      path = Path.TempPath;
      ret = Path.IsRealPath(path);
      Assert.IsTrue(ret, "path should be real path, path: " + path);
      Assert.IsTrue(Path.Exists(path), "path should be exists, path: " + path);
      Assert.IsTrue(Path.DirectoryExists(path), "path should be exists directory, path: " + path);
      Assert.IsFalse(Path.FileExists(path), "path should not be exists file, path: " + path);

      path = Path.GetTempFile();
      ret = Path.IsRealPath(path);
      Assert.IsTrue(ret, "path should be real path, path: " + path);
      Assert.IsTrue(Path.Exists(path), "path should be exists, path: " + path);
      Assert.IsFalse(Path.DirectoryExists(path), "path should not be exists directory, path: " + path);
      Assert.IsTrue(Path.FileExists(path), "path should be exists file, path: " + path);

      // check null
      ret = Path.IsRealPath(null);
      Assert.IsFalse(ret, "null should not be real path");

      ret = Path.Exists(null);
      Assert.IsFalse(ret, "null should not be exist");

      path = Path.GetPath(null);
      Assert.AreEqual(path, Path.RootPath, "null get path should be root path, path: " + path);

      directory = Path.GetDirectory(null);
      Assert.AreEqual(directory, string.Empty, "directory should be empty, directory: " + directory);

      file = Path.GetFileName(null);
      Assert.IsNull(file, "file name should be null, file: " + file);

      file = Path.GetFileNameWithoutExtension(null);
      Assert.IsNull(file, "file name should be null, file: " + file);

      extension = Path.GetExtension(null);
      Assert.IsNull(extension, "extension should be null, extension: " + extension);

      path = Path.Combine(root, null);
      Assert.AreEqual(path, root, "path should be root, path: " + path);

      path = Path.Combine(null, root);
      Assert.AreEqual(path, root, "path should be root, path: " + path);

      path = Path.Combine(null, null);
      Assert.IsNull(path, "path should be null, path: " + path);
    }
  }
}
