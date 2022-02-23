using Beamable.Api;
using Beamable.Common;
using Beamable.Player;
using Beamable.Service;

namespace Beamable.Samples.Core.Utilities
{
    /// <summary>
    /// These are hacks and other shortcuts used to
    /// assist in the examples.
    ///
    /// HACK: These hacks are not recommended for production usage
    /// 
    /// </summary>
    public static class ExampleProjectHacks
    {

        //  Methods  --------------------------------------
        
        /// <summary>
        /// Clears all data related to the active runtime user(s).
        /// Reloads the game to first scene.
        /// </summary>
        public static void ClearDeviceUsersAndReloadGame()
        {
            BeamContext.Default.Api.ClearDeviceUsers();
            Beam.ClearAndStopAllContexts();
            Beam.ResetToScene("1.Intro");
        }
    }
}