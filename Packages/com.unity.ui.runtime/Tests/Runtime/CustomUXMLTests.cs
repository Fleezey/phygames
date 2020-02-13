using System.IO;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class CustomUXMLTests
{
    [Test]
    public void CustomUxmlLoadWithoutError()
    {
        var root = new VisualElement();
        var vta = Resources.Load<VisualTreeAsset>(Path.Combine("CustomUXML", "CustomUXML"));
        vta.CloneTree(root);
        LogAssert.NoUnexpectedReceived();
    }
}
