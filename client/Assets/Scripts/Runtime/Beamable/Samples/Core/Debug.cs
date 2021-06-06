namespace Beamable.Core.Debugging
{
    /// <summary>
    /// Reduce console traffic by wrapping "Debug.Log" calls to use of this Debugger.
    /// </summary>
    public class Debugger
    {
        //  Properties ---------------------------------------
        public DebugLogLevel DebugLogLevel { get { return _debugLogLevel; } }

        //  Fields ---------------------------------------
        private DebugLogLevel _debugLogLevel;

        //  Constructor ---------------------------------------

        public Debugger(DebugLogLevel debugLogLevel = DebugLogLevel.Disabled)
        {
            _debugLogLevel = debugLogLevel;
        }
        
        //  Other Methods ---------------------------------------
        public void Log(string message, DebugLogLevel messageDebugLogLevel = DebugLogLevel.Simple)
        {
            // Logging off?
            if (DebugLogLevel == DebugLogLevel.Disabled)
            {
                return;
            } 
         
            // Logging level too low?
            if (DebugLogLevel == DebugLogLevel.Verbose && 
                messageDebugLogLevel == DebugLogLevel.Simple)
            {
                return;
            }
         
            // Keep as "Debug.Log"
            UnityEngine.Debug.Log(message);
        }
    }
}