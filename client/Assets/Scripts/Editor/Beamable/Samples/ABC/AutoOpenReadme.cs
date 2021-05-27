using Beamable.Samples.SampleProjectBase;
using UnityEditor;

namespace Beamable.Samples.TBF
{
	/// <summary>
	/// Automatically focus on the sample project readme file when Unity first opens.
	/// </summary>
	[CustomEditor(typeof(Readme))]
	[InitializeOnLoad]
	public class AutoOpenReadme : ReadmeEditor
	{
		private const string Title = "TBF Readme";
		private const string FindAssetsFilter = "Readme t:Readme";
		private const string SessionStateKeyWasAlreadyShown = "Beamable.Samples.TBF.AutoOpenReadme.wasAlreadyShown";
		private static string[] FindAssetsFolders = new string[] { "Assets" };

		static AutoOpenReadme()
		{
			EditorApplication.delayCall += SelectReadmeAutomatically;
		}

		private static void SelectReadmeAutomatically()
		{
			if (!SessionState.GetBool(SessionStateKeyWasAlreadyShown, false))
			{
				SelectSpecificReadmeMenuItem();
				SessionState.SetBool(SessionStateKeyWasAlreadyShown, true);
			}
		}

		[MenuItem(
			BeamableConstants.MENU_ITEM_PATH_WINDOW_BEAMABLE_SAMPLES + "/Multiplayer/" +
			BeamableConstants.OPEN + " " + Title,
			priority = BeamableConstants.MENU_ITEM_PATH_WINDOW_PRIORITY_4)]
		private static Readme SelectSpecificReadmeMenuItem()
		{
			// Reset SessionState if/when MenuItem is used
			SessionState.SetBool(SessionStateKeyWasAlreadyShown, false);
			return ReadmeEditor.SelectReadme(FindAssetsFilter, FindAssetsFolders);
		}
	}
}
