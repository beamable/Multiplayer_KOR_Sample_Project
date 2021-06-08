using Beamable.Samples.KOR.Data;

namespace Beamable.Samples.KOR.UI
{
   /// <summary>
   /// Handles the view concerns for panels with attribute texts
   /// </summary>
   public class AttributesPanelUI: PanelUI 
   {
      //  Properties -----------------------------------
      public Attributes Attributes { get { return _attributes; } set { _attributes = value; Render();}}
      
      //  Fields ---------------------------------------
      private Attributes _attributes = null;
      
      //  Unity Methods   ------------------------------
      private void Render()
      {
          BodyText.text = string.Format(KORConstants.Player_Attributes,
              _attributes.ChargeSpeed, _attributes.MovementSpeed);
      }
   }
}