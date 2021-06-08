using UnityEditor;
using UnityEngine;

namespace Editor.Beamable
{
   [CustomPropertyDrawer(typeof(sfloat))]
   public class SFloatPropertyDrawer : PropertyDrawer
   {
      private const int LABEL_HEIGHT = 14;
      private const int LABEL_WIDTH = 100;

      public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
      {
         return EditorGUIUtility.singleLineHeight + LABEL_HEIGHT;
      }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {

         var labelRect = new Rect(position.x, position.y, position.width, LABEL_HEIGHT);

         var fieldRect = new Rect(position.x, position.y + LABEL_HEIGHT, position.width, position.height - LABEL_HEIGHT);

         var labelSkin = new GUIStyle("label");
         labelSkin.fontSize = 10;
         var title = label.text;
         EditorGUI.LabelField(labelRect, " ", "(soft float)", labelSkin);

         var rawProperty = property.FindPropertyRelative("rawValue");
         var raw = (uint) rawProperty.longValue;
         var value = (float)sfloat.FromRaw(raw);

         EditorGUI.BeginChangeCheck();
         value = EditorGUI.FloatField(fieldRect, title, value);
         var softValue = (sfloat) value;
         raw = softValue.RawValue;
         if (EditorGUI.EndChangeCheck())
         {
            rawProperty.longValue = raw;
            EditorUtility.SetDirty(property.serializedObject.targetObject);
         }
      }
   }
}