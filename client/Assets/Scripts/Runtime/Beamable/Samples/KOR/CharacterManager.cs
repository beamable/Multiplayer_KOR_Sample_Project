using Beamable.Api.Stats;
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Content;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.CustomContent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Beamable.Samples.KOR
{
    public class CharacterManager
    {
        private IBeamableAPI _beamableAPI = null;
        private List<CharacterContentObject> _allCharacterContentObjects = null;
        private Dictionary<string, CharacterContentObject> _mapCharacterObjectNameToContent = new Dictionary<string, CharacterContentObject>();
        private const string ChosenCharacterStatKey = "ChosenCharacterContentID";
        private CharacterContentObject _currentlyChosenCharacter = null;

        public List<CharacterContentObject> AllCharacterContentObjects { get { return _allCharacterContentObjects; } }

        public CharacterContentObject CurrentlyChosenCharacter { get { return _currentlyChosenCharacter; } }

        public event Action OnChoiceHasBeenMade;

        public CharacterManager()
        {
            SetupBeamable();
        }

        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            _allCharacterContentObjects = await GetAllCharacterContentObjects();

            if (_allCharacterContentObjects.Count == 0)
            {
                Debug.LogError("No characters found in manifest. Did you forget to add some? This will break.");
                return;
            }

            foreach (var cco in _allCharacterContentObjects)
            {
                Debug.Log($"CharacterContentObject name={cco.ContentName} type={cco.ContentType} id={cco.Id} something={cco.ReadableName}");
                _mapCharacterObjectNameToContent.Add(cco.ContentName, cco);
            }

            CharacterContentObject chosenCCO = await GetChosenCharacterByDBID(_beamableAPI.User.id);

            if (chosenCCO == null)
                ChooseCharacter(_allCharacterContentObjects[0]);
            else
            {
                _currentlyChosenCharacter = chosenCCO;
                OnChoiceHasBeenMade?.Invoke();
            }
        }

        public async Task<CharacterContentObject> GetChosenCharacterByDBID(long dbid)
        {
            Dictionary<string, string> allCurrentUserStats = await _beamableAPI.StatsService.GetStats("client", "public", "player", dbid);

            foreach (KeyValuePair<string, string> entry in allCurrentUserStats)
                Debug.Log($"CharacterContentObject key={entry.Key} value={entry.Value}");

            string chosenCharacterName;
            if (!allCurrentUserStats.TryGetValue(ChosenCharacterStatKey, out chosenCharacterName))
                return null;

            CharacterContentObject cco;
            if (!_mapCharacterObjectNameToContent.TryGetValue(chosenCharacterName, out cco))
            {
                Debug.LogError($"Chosen character name={ chosenCharacterName} by dbid={dbid} from stats does not refer to an existing character content object. " +
                    "You've probably removed/renamed this recently.");
                return null;
            }

            return cco;
        }

        public int GetChosenCharacterIndex()
        {
            if (_allCharacterContentObjects == null || _currentlyChosenCharacter == null)
                return -1;

            return _allCharacterContentObjects.IndexOf(_currentlyChosenCharacter);
        }

        public void ChooseCharacter(CharacterContentObject newlyChosenCharacter)
        {
            _currentlyChosenCharacter = newlyChosenCharacter;

            _beamableAPI.StatsService.SetStats("public", new Dictionary<string, string>()
            {
               { ChosenCharacterStatKey, newlyChosenCharacter.ContentName }
            });

            OnChoiceHasBeenMade?.Invoke();
        }

        private async Task<List<CharacterContentObject>> GetAllCharacterContentObjects()
        {
            var filteredManifest = await _beamableAPI.ContentService.GetManifest(new ContentQuery
            {
                TypeConstraints = new HashSet<Type> { typeof(CharacterContentObject) }
            });
            var results = await filteredManifest.ResolveAll();
            return results.Cast<CharacterContentObject>().ToList();
        }
    }
}