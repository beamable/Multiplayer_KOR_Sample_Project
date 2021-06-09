using Beamable.Samples.KOR.CustomContent;

namespace Beamable.Samples.KOR.Data
{
    /// <summary>
    /// Renders one item row of the store.
    /// </summary>
    public class StoreItemData 
    {
        //  Properties -----------------------------------
        public string Title { get { return _title; } }
        public KORItemContent KORItemContent { get { return _korItemContent; } }

        //  Fields ---------------------------------------
        private KORItemContent _korItemContent = null;
        private string _title = null;
        
        //  Constructor ---------------------------------
        public StoreItemData (KORItemContent korItemContent, string title)
        {
            _korItemContent = korItemContent;
            _title = title;
        }
    }
}