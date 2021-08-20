using Beamable.Samples.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace Beamable.Samples.Core.PropertyDrawers
{
   [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
   public class ReadOnlyPropertyDrawer : PropertyDrawer
   {
      public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
      {
         return EditorGUI.GetPropertyHeight( property, label, true );
      }
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
         GUI.enabled = false;
         EditorGUI.PropertyField(position, property, label);
         GUI.enabled = true;
      }
   }
}