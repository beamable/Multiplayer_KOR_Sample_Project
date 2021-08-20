using System;
using UnityEngine;

namespace Beamable.Samples.Core.Attributes
{
   [AttributeUsage(AttributeTargets.Field)]
   public class ReadOnlyAttribute : PropertyAttribute
   {

   }
}