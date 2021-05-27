using System;
using UnityEngine;

namespace Beamable.Samples.TBF.Multiplayer.Events
{
   [Serializable]
   public class GameMoveEvent : TBFEvent
   {
      //  Properties -----------------------------------

      /// <summary>
      /// The move SENT by the player.
      /// </summary>
      public GameMoveType GameMoveType {  get { return _gameMoveType; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private GameMoveType _gameMoveType;

      //  Constructor   --------------------------------
      public GameMoveEvent(GameMoveType gameMoveType)
      {
         _gameMoveType = gameMoveType;
      }
   }
}