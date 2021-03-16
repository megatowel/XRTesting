using System;
using UnityEngine;
using UnityEditor;

namespace Megatowel.Multiplex
{
    [CreateAssetMenu(fileName = "MultiplexSettings", menuName = "Megatowel/MultiplexSettings")]
    public class MultiplexSettings : ScriptableObject
    {
        public string Address;
        public ushort Port = 3000;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MultiplexSettings))]
    public class MultiplexSettingsEditor : Editor
    {
        private SerializedProperty _address;
        private SerializedProperty _port;

        private bool _showServerSettings = true;

        void OnEnable()
        {
            _address = serializedObject.FindProperty("Address");
            _port = serializedObject.FindProperty("Port");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _showServerSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showServerSettings, "Server Settings");
            if (_showServerSettings) 
            {
                EditorGUILayout.PropertyField(_address);
                EditorGUILayout.PropertyField(_port);
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
        }

    }
#endif
}


