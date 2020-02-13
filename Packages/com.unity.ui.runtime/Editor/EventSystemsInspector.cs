using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Runtime.Editor
{
    [CustomEditor(typeof(UIElementsEventSystem))]
    public class EventSystemInspector : UnityEditor.Editor
    {
        private List<string> m_EventGenerationChoices;
        private PropertyField SendNavigationField;
        private PopupField<string> inputEventsField;
        private void Awake()
        {
            m_EventGenerationChoices = new List<string>() {"Read Input", "IMGUI Events"};
        }


        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var system = target as UIElementsEventSystem;

            var defaultSelection = system.sendIMGUIEvents ? m_EventGenerationChoices[1] : m_EventGenerationChoices[0];
            inputEventsField = new PopupField<string>("Event Generation", m_EventGenerationChoices, defaultSelection);
            inputEventsField.RegisterValueChangedCallback((evt) => OnPopupValueChanged(evt));
            root.Add(inputEventsField);

            var serializedProperty = serializedObject.FindProperty("m_SendNavigationEvents");

            SendNavigationField = new PropertyField(serializedProperty);
            root.Add(SendNavigationField);

            //TODO: Group Navigation events parameters together in a foldout and toggle visibility when navigation is enabled
            //TODO: Group Input delay and repeat together and only show when sendInputEvents is selected.
            while (serializedProperty.Next(false))
            {
                root.Add(new PropertyField(serializedProperty));
            }

            inputEventsField.schedule.Execute(() => UpdatePopupFieldValues()).Every(100);
            UpdateNavigationFieldVisibility();
            return root;
        }

        void UpdateNavigationFieldVisibility()
        {
            if (SendNavigationField != null)
            {
                var system = target as UIElementsEventSystem;
                if (system != null)
                {
                    if (system.sendInputEvents)
                    {
                        SendNavigationField.SetEnabled(true);
                    }
                    else
                    {
                        SendNavigationField.SetEnabled(false);
                    }
                }
            }
        }

        void UpdatePopupFieldValues()
        {
            int neededIndex = serializedObject.FindProperty("m_SendIMGUIEvents").boolValue ? 1 : 0;
            inputEventsField.index = neededIndex;
        }
        
        void SaveInputEventsValues(bool inputEvents, bool imguiEvents)
        {
            serializedObject.FindProperty("m_SendInputEvents").boolValue = inputEvents;
            serializedObject.FindProperty("m_SendIMGUIEvents").boolValue = imguiEvents;

            serializedObject.ApplyModifiedProperties();
        }

        void OnPopupValueChanged(ChangeEvent<string> evt)
        {
            var system = target as UIElementsEventSystem;
            if (system != null && evt.target is PopupField<string> popup)
            {
                switch (popup.index)
                {
                    case 0:
                        SaveInputEventsValues(true, false);
                       
                        break;
                    case 1:
                        SaveInputEventsValues(false, true);
                        break;
                }

                UpdateNavigationFieldVisibility();
            }
        }
    }
}
