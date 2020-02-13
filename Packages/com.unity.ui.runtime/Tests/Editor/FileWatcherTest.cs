using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Threading;
using Unity.UIElements.Runtime;
using Unity.UIElements.Runtime.Editor;
using UnityEngine.UIElements;

class FileWatcherTest {

	class DummyFileWatcher : IFileChangedNotify
	{
		public bool changed { get; set; }
		
		public void OnFileChanged(string path)
		{
			changed = true;
		}
	}

	string path = "Assets/Resources/test.txt";
	
	[SetUp]
	public void Setup()
	{
		Unity.UIElements.Runtime.Editor.FileWatcher.Reset();
	}

	[Test]
	public void WatcherAcceptsNonExistingPath()
	{
		DummyFileWatcher dfw = new DummyFileWatcher();
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw, "random/path/that/does/not/exists.blah");
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().EnableWatcher(dfw);
		
		LogAssert.NoUnexpectedReceived();
	}
	
	[Test]
	public void WatcherDoesNotAcceptsEmptyPath()
	{
		DummyFileWatcher dfw = new DummyFileWatcher();
		
		Exception e = Assert.Catch(() => Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw, ""));
		Assert.AreSame(typeof(ArgumentOutOfRangeException), e.GetType());
	}

	[Test] public void WatcherCanNotBeNull()
	{
		Exception e = Assert.Catch(() => Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(null, path));
		Assert.AreSame(typeof(ArgumentNullException), e.GetType());
	}
	
	[UnityTest]
	[Ignore("Unsable")]
	public IEnumerator WatcherIsNotifiedOnceWhenFileIsModified()
	{
		DummyFileWatcher dfw = new DummyFileWatcher();
		
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw, path);
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().EnableWatcher(dfw);
		
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().SimulateChange(path);
		
		yield return null;
		yield return null;
		
		Assert.IsTrue(dfw.changed);

		dfw.changed = false;
		
		yield return null;
		yield return null;
		
		Assert.IsFalse(dfw.changed);
	}
	
	[UnityTest]
	[Ignore("Unsable")]
	public IEnumerator WatcherIsNotNotifiedWhenFileIsModifiedButWatcherNotEnabled()
	{
		DummyFileWatcher dfw = new DummyFileWatcher();
		
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw, path);

		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().SimulateChange(path);
		yield return null;
		yield return null;
		
		Assert.IsFalse(dfw.changed);
	}
	
	[UnityTest]
	[Ignore("Unsable")]
	public IEnumerator WatcherIsNotNotifiedWhenOtherFileIsModified()
	{
		DummyFileWatcher dfw = new DummyFileWatcher();
		
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw, path);
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().EnableWatcher(dfw);
		
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().SimulateChange("blah");
		yield return null;
		yield return null;
		
		Assert.IsFalse(dfw.changed);
	}
	
	[UnityTest]
	[Ignore("Unsable")]
	public IEnumerator BothWatchersAreNotifiedWhenFileIsModified()
	{
		DummyFileWatcher dfw = new DummyFileWatcher();
		DummyFileWatcher dfw2 = new DummyFileWatcher();
		
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw, path);
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().EnableWatcher(dfw);

		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().AddFile(dfw2, path);
		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().EnableWatcher(dfw2);

		Unity.UIElements.Runtime.Editor.FileWatcher.Instance().SimulateChange(path);
		yield return null;
		yield return null;
		
		Assert.IsTrue(dfw.changed);
		Assert.IsTrue(dfw2.changed);
	}
}
