using System;
using UnityEngine;

namespace Beamable.Samples.TBF.Data
{
   /// <summary>
   /// Store data related to: Avatar
   /// </summary>
   [Serializable]
   public class AvatarData
   {
      //  Fields  -----------------------------------
      public string Location {  get { return _location; } }
      public Color Color { get { return _color; } }

      //  Properties -----------------------------------
      [SerializeField]
      private string _location = "";

      [SerializeField]
      private Color _color = Color.blue;
   }
}