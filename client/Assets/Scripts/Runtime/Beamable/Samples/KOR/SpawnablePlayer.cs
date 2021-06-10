using System;
using Beamable.Samples.KOR.Behaviours;
using Beamable.Samples.KOR.Data;

namespace Beamable.Samples.KOR
{
   [Serializable]
   public class SpawnablePlayer
   {
      public SpawnablePlayer(long dbid, SpawnPointBehaviour spawnPointBehaviour)
      {
         _dbid = dbid;
         _spawnPointBehaviour = spawnPointBehaviour;
      }

      public long DBID
      {
         get { return _dbid; }
      }

      public SpawnPointBehaviour SpawnPointBehaviour
      {
         get { return _spawnPointBehaviour; }
      }

      public CharacterManager.Character ChosenCharacter { get; set; }

      public Attributes Attributes
      {
         get { return _attributes; }
         set { _attributes = value; }
      }

      public string PlayerAlias
      {
         get { return _playerAlias; }
         set { _playerAlias = value; }
      }

      private long _dbid = -1;
      private SpawnPointBehaviour _spawnPointBehaviour;
      private Attributes _attributes;
      private string _playerAlias;
   }
}