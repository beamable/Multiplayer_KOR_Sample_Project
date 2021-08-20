namespace Beamable.Samples.Core.Debugging
{
    /// <summary>
    /// Reduce console traffic by wrapping "Debug.Log" calls to use of this Debugger.
    /// </summary>
    public class Debugger
    {
        //  Fields ---------------------------------------
        private readonly DebugLogLevel _debugLogLevel;

        //  Constructor ---------------------------------------

        public Debugger(DebugLogLevel debugLogLevel = DebugLogLevel.Disabled)
        {
            _debugLogLevel = debugLogLevel;
        }
        
        //  Other Methods ---------------------------------------
        public void Log(string message, DebugLogLevel messageDebugLogLevel = DebugLogLevel.Simple)
        {
            // Logging off?
            if (_debugLogLevel == DebugLogLevel.Disabled)
            {
                return;
            } 
         
            // Logging level too low?
            if (_debugLogLevel == DebugLogLevel.Simple && 
                messageDebugLogLevel == DebugLogLevel.Verbose)
            {
                return;
            }
         
            // Keep as "Debug.Log"
            UnityEngine.Debug.Log(message);
        }
    }
}