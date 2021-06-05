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
    public class CharacterManager : SingletonMonobehavior<CharacterManager>
    {
        private IBeamableAPI _beamableAPI = null;
        private List<CharacterContentObject> _allCharacterContentObjects = null;
        private Dictionary<string, CharacterContentObject> _mapCharacterObjectNameToContent = new Dictionary<string, CharacterContentObject>();
        private const string ChosenCharacterStatKey = "ChosenCharacterContentID";
        private CharacterContentObject _currentlyChosenCharacter = null;

        public List<CharacterContentObject> AllCharacterContentObjects { get { return _allCharacterContentObjects; } }

        public CharacterContentObject CurrentlyChosenCharacter { get { return _currentlyChosenCharacter; } }

        public event Action OnChoiceHasBeenMade;

        private void Start()
        {
            SetupBeamable();
        }

        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            _allCharacterContentObjects = await GetAllCharacterContentObjects();

            if (_allCharacterContentObjects.Count == 0)
                throw new InvalidOperationException("No characters found in manifest. Did you forget to add some?");

            foreach (var cco in _allCharacterContentObjects)
            {
                Debug.Log($"CharacterContentObject name={cco.ContentName} type={cco.ContentType} id={cco.Id} something={cco.ReadableName}");
                _mapCharacterObjectNameToContent.Add(cco.ContentName, cco);
            }

            Dictionary<string, string> allCurrentUserStats = await _beamableAPI.StatsService.GetStats("client", "public", "player", _beamableAPI.User.id);

            foreach (KeyValuePair<string, string> entry in allCurrentUserStats)
                Debug.Log($"CharacterContentObject key={entry.Key} value={entry.Value}");

            string chosenCharacterName;
            if (allCurrentUserStats.TryGetValue(ChosenCharacterStatKey, out chosenCharacterName))
            {
                if (!_mapCharacterObjectNameToContent.TryGetValue(chosenCharacterName, out _currentlyChosenCharacter))
                    throw new InvalidOperationException(
                        $"Chosen character name={chosenCharacterName} from stats does not refer to an existing character content object. " +
                        "You've probably removed this recently.");

                OnChoiceHasBeenMade?.Invoke();
            }
            else
                ChooseCharacter(_allCharacterContentObjects[0]);
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