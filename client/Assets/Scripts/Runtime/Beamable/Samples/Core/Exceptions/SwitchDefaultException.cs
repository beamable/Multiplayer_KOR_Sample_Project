using System;

namespace Beamable.Samples.Core.Exceptions
{
   /// <summary>
   /// Handles errors generated when the default clause of a
   /// switch statement is reached but not expected.
   /// </summary>
   public class SwitchDefaultException : Exception
   {
      private object obj;

      public SwitchDefaultException(object obj)
      {
         this.obj = obj;
      }

      public override string Message
      {
         get
         {
            return string.Format("Switch must contain case of '{0}' for " +
                                 "type '{1}'.", obj.GetType().Name, obj);
         }
      }

      /// <summary>
      /// Call this instead of directly calling 'throw'. 
      /// This is to avoid warning of "warning CS0162: Unreachable code detected"
      /// </summary>
      public static void Throw(object obj)
      {
         throw new SwitchDefaultException(obj);
      }
   }
}