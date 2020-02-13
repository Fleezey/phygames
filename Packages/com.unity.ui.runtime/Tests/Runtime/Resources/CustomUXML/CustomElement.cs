using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

[assembly: Preserve]
namespace MyNamespace
{
    public class CustomElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<CustomElement> {}

        public CustomElement()
        {
            Add(new Label() { text = "Foo" });
        }
    }
}
