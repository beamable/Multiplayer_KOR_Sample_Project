using Beamable.Samples.Core;
using Beamable.Samples.TBF.Audio;
using Beamable.Samples.TBF.Data;
using Beamable.Samples.TBF.Exceptions;
using Beamable.Samples.TBF.Multiplayer;
using Beamable.Samples.TBF.Multiplayer.Events;
using Beamable.Samples.TBF.Views;
using System;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;
using static Beamable.Samples.TBF.Data.GameProgressData;
using static Beamable.Samples.TBF.UI.TMP_BufferedText;

namespace Beamable.Samples.TBF
{
   /// <summary>
   /// List of all phases of the gameplay.
   /// There are arguably more states here than are needed, 
   /// however all are indeed used, in the order shown, for deliberate separation.
   /// </summary>
   public enum GameState
   {
      //Game loads within here
      Null,
      Loading,
      Loaded,
      Initializing,
      Initialized,
      Connecting,
      Connected,
      GameStarting,
      GameStarted,

      //Game repeats within here
      RoundStarting,
      RoundStarted,
      RoundPlayerMoving,
      RoundPlayerMoved,
      RoundEvaluating,
      RoundEvaluated,

      //Game ends here
      GameEvaluating,
      GameEnding,
      GameEnded
   }

   /// <summary>
   /// Handles the <see cref="GameState"/> for the <see cref="GameSceneManager"/>.
   /// </summary>
   public class GameStateHandler
   {
      //  Properties -----------------------------------
      public GameState GameState { get { return _gameState; } }

      //  Fields ---------------------------------------
      private GameState _gameState = GameState.Null;
      private GameSceneManager _gameSceneManager;

      //  Other Methods  -----------------------------
      public GameStateHandler(GameSceneManager gameSceneManager)
      {
         _gameSceneManager = gameSceneManager;
      }


      /// <summary>
      /// Store and handle changes to the <see cref="GameState"/>.
      /// </summary>
      /// <param name="gameState"></param>
      /// <returns></returns>
      public async Task SetGameState(GameState gameState)
      {
         DebugLog($"SetGameState() from {_gameState} to {gameState}");

         //NOTE: Do not set "_gameState" directly anywhere, except here.
         _gameState = gameState;

         // SetGameState() is async...
         //    Pros: We can use operations like "Task.Delay" to slow down execution
         //    Cons: Error handling is tricky. 
         //    Workaround: AsyncUtility helps with its try/catch.
         await AsyncUtility.AsyncSafe(async () =>
         {
            switch (_gameState)
            {
               case GameState.Null:
                  break;

               case GameState.Loading:
                  // **************************************
                  // Render the scene before any latency 
                  // of multiplayer begins
                  // **************************************

                  _gameSceneManager.SetStatusText("", BufferedTextMode.Immediate);
                  _gameSceneManager.SetRoundText(1);

                  _gameSceneManager.GameUIView.AvatarViews[TBFConstants.PlayerIndexLocal].PlayAnimationIdle();
                  _gameSceneManager.GameUIView.AvatarViews[TBFConstants.PlayerIndexRemote].PlayAnimationIdle();

                  _gameSceneManager.GameProgressData = new GameProgressData(_gameSceneManager.Configuration);
                  _gameSceneManager.GameUIView.MoveButtonsCanvasGroup.interactable = false;
                  _gameSceneManager.SetStatusText(TBFConstants.StatusText_GameState_Loading, BufferedTextMode.Queue);

                  break;

               case GameState.Loaded:
                  // **************************************
                  //  Update UI
                  //  
                  // **************************************

                  _gameSceneManager.SetStatusText(TBFConstants.StatusText_GameState_Loaded, BufferedTextMode.Queue);
                  break;

               case GameState.Initializing:
                  // **************************************
                  //  Update UI
                  //  
                  // **************************************

                  _gameSceneManager.SetStatusText(TBFConstants.StatusText_GameState_Initializing, BufferedTextMode.Queue);
                  break;

               case GameState.Initialized:
                  // **************************************
                  //  Update UI
                  //  
                  // **************************************

                  _gameSceneManager.SetStatusText(TBFConstants.StatusText_GameState_Initialized, BufferedTextMode.Queue);
                  break;

               case GameState.Connecting:
                  // **************************************
                  //  Update UI
                  //  
                  // **************************************

                  _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_Connecting,
                     _gameSceneManager.MultiplayerSession.PlayerDbidsCount.ToString(),
                     _gameSceneManager.MultiplayerSession.TargetPlayerCount), BufferedTextMode.Queue);
                  break;

               case GameState.Connected:
                  // **************************************
                  //  Advanced the state 
                  //  
                  // **************************************

                  await SetGameState(GameState.GameStarting);
                  break;

               case GameState.GameStarting:
                  // **************************************
                  //  Reset the game-specific data
                  //  
                  // **************************************

                  _gameSceneManager.GameProgressData.StartGame();
                  break;

               case GameState.GameStarted:
                  // **************************************
                  //  Now that all players have connected, setup AI
                  //  
                  // **************************************

                  // RemotePlayerAI is always created, but enabled only sometimes
                  bool isEnabledRemotePlayerAI = _gameSceneManager.MultiplayerSession.IsHumanVsBotMode;
                  System.Random random = _gameSceneManager.MultiplayerSession.Random;

                  DebugLog($"[Debug] isEnabledRemotePlayerAI={isEnabledRemotePlayerAI}");

                  _gameSceneManager.RemotePlayerAI = new RemotePlayerAI(random, isEnabledRemotePlayerAI);

                  await SetGameState(GameState.RoundStarting);
                  break;

               case GameState.RoundStarting:
                  // **************************************
                  //  Reste the round-specific data.
                  //  Advance the state. 
                  //  This happens before EACH round during a game
                  // **************************************

                  _gameSceneManager.GameProgressData.StartNextRound();

                  _gameSceneManager.SetRoundText(_gameSceneManager.GameProgressData.CurrentRoundNumber);

                  await SetGameState(GameState.RoundStarted);
                  break;

               case GameState.RoundStarted:
                  // **************************************
                  //  Advance the state
                  //  
                  // **************************************

                  while (_gameSceneManager.GameUIView.BufferedText.HasRemainingQueueText)
                  {
                     // Wait for old messages to pass before allowing button clicks
                     await Await.NextUpdate();
                  }
                  _gameSceneManager.GameUIView.MoveButtonsCanvasGroup.interactable = true;

                  await SetGameState(GameState.RoundPlayerMoving);
                  break;

               case GameState.RoundPlayerMoving:
                  // **************************************
                  //  Update UI
                  //  
                  // **************************************

                  _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_PlayerMoving), 
                     BufferedTextMode.Queue);

                  break;

               case GameState.RoundPlayerMoved:
                  // **************************************
                  //  
                  //  
                  // **************************************

                  long localPlayerDbid = _gameSceneManager.MultiplayerSession.GetPlayerDbidForIndex(TBFConstants.PlayerIndexLocal);
                  GameMoveEvent localGameMoveEvent = _gameSceneManager.GameProgressData.GameMoveEventsThisRoundBucket.GetByPlayerDbid(localPlayerDbid);

                  GameMoveType localGameMoveType = localGameMoveEvent.GameMoveType;
                  
                  long remotePlayerDbid;

                  if (_gameSceneManager.RemotePlayerAI.IsEnabled)
                  {
                     // HumanVSBot: Create an AI movement here...
                     remotePlayerDbid = _gameSceneManager.RemotePlayerAI.RemotePlayerDbid;
                     GameMoveEvent gameMoveEvent = _gameSceneManager.RemotePlayerAI.GetNextRemoteGameMoveEvent(localGameMoveType);
                     _gameSceneManager.GameProgressData.GameMoveEventsThisRoundBucket.Add(gameMoveEvent);
                  }
                  else
                  {
                     remotePlayerDbid = _gameSceneManager.MultiplayerSession.GetPlayerDbidForIndex(TBFConstants.PlayerIndexRemote);
                  }

                  GameMoveEvent remoteGameEvent = _gameSceneManager.GameProgressData.GameMoveEventsThisRoundBucket.GetByPlayerDbid(remotePlayerDbid);
                  GameMoveType remoteGameMoveType = remoteGameEvent.GameMoveType;

                  // 1 LOCAL
                  await RenderPlayerMove(TBFConstants.PlayerIndexLocal, localGameMoveType);

                  // 2 REMOTE - Always show this second, it builds drama
                  await RenderPlayerMove(TBFConstants.PlayerIndexRemote, remoteGameMoveType);

                  // All players have moved
                  _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_PlayersAllMoved), 
                     BufferedTextMode.Queue);

                  if (_gameSceneManager.RemotePlayerAI.IsEnabled)
                  {
                     // HumanVSBot: The human move is done. Don't wait for other moves.
                     await SetGameState(GameState.RoundEvaluating);
                  }
                  else if  (_gameSceneManager.GameProgressData.GameMoveEventsThisRoundBucket.Count ==
                     _gameSceneManager.MultiplayerSession.TargetPlayerCount)
                  {
                     // HumanVSHuman: All moves are complete, so evaluate
                     await SetGameState(GameState.RoundEvaluating);
                  }
                  else
                  {
                     // HumanVSHuman: NOT all moves are complete, so wait...
                     await SetGameState(GameState.RoundPlayerMoving);
                  }
                  break;

               case GameState.RoundEvaluating:

                  // **************************************
                  //  Evalute all the player moves and store result.
                  //  Advance the state
                  //  
                  // **************************************

                  _gameSceneManager.GameProgressData.EvaluateGameMoveEventsThisRound();
                  await SetGameState(GameState.RoundEvaluated);

                  break;

               case GameState.RoundEvaluated:
                  // **************************************
                  // Render results onscreen (animation, sounds).
                  // Decide: Advance round or end game
                  //  
                  // **************************************

                  RoundResult currentRoundResult = _gameSceneManager.GameProgressData.CurrentRoundResult;

                  switch (currentRoundResult)
                  {
                     case RoundResult.Tie:
                        _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_EvaluatedTie,
                           _gameSceneManager.GameProgressData.CurrentRoundNumber), BufferedTextMode.Queue);

                        while (_gameSceneManager.GameUIView.BufferedText.HasRemainingQueueText)
                        {
                           // Wait for old messages to pass before allowing button clicks
                           await Await.NextUpdate();
                        }

                        await SetGameState(GameState.RoundStarting);
                        return;
                     case RoundResult.Winner:
                        //pass through to code below
                        break;
                     default:
                        SwitchDefaultException.Throw(currentRoundResult);
                        break;
                  }

                  bool currentRoundHasWinnerPlayerDbid = _gameSceneManager.GameProgressData.CurrentRoundHasWinnerPlayerDbid;

                  if (!currentRoundHasWinnerPlayerDbid)
                  {
                     throw new InvalidOperationException("This is never expected. #2");
                  }

                  long currentRoundWinnerPlayerDbid = _gameSceneManager.GameProgressData.CurrentRoundWinnerPlayerDbid;
                  string roundWinnerName = GetPlayerNameByPlayerDbid(currentRoundWinnerPlayerDbid);

                  _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_EvaluatedWinner,
                     _gameSceneManager.GameProgressData.CurrentRoundNumber, roundWinnerName), BufferedTextMode.Queue);

                  while (_gameSceneManager.GameUIView.BufferedText.HasRemainingQueueText)
                  {
                     // Wait for old messages to pass before allowing button clicks
                     await Await.NextUpdate();
                  }

                  // Ex. Do 34 damage for each round of 3 rounds so that 3 hits = total death
                  int deltaHealth = -(1 + HealthBarView.MaxValue / _gameSceneManager.Configuration.GameRoundsTotal);
                  UpdateHealth(currentRoundWinnerPlayerDbid, deltaHealth);

                  //Wait for animations to finish
                  await AsyncUtility.TaskDelaySeconds(_gameSceneManager.Configuration.DelayGameBeforeGameOver);
                  await SetGameState(GameState.GameEvaluating);
                  break;

               case GameState.GameEvaluating:
                  // **************************************
                  //  Advance the state 
                  //  
                  // **************************************

                  if (_gameSceneManager.GameProgressData.GameHasWinnerPlayerDbid)
                  {
                     await SetGameState(GameState.GameEnding);
                  }
                  else
                  {
                     await SetGameState(GameState.RoundStarting);
                  }

                  break;
               case GameState.GameEnding:
                  // **************************************
                  //  Render loss (animation and sound)
                  //  Game stays here. 
                  // **************************************

                  // if the game loser does not have 0 health, move to 0 health
                  long gameWinnerDbid = _gameSceneManager.GameProgressData.GameWinnerPlayerDbid;
                  UpdateHealth(gameWinnerDbid, -HealthBarView.MaxValue);

                  string gameWinnerName;
                  if (_gameSceneManager.MultiplayerSession.IsLocalPlayerDbid(gameWinnerDbid))
                  {
                     gameWinnerName = GetPlayerNameByIndex(TBFConstants.PlayerIndexLocal);

                     //Local winner
                     SoundManager.Instance.PlayAudioClip(SoundConstants.GameOverWin);
                     _gameSceneManager.GameUIView.AvatarViews[TBFConstants.PlayerIndexLocal].PlayAnimationWin();
                     _gameSceneManager.GameUIView.AvatarViews[TBFConstants.PlayerIndexRemote].PlayAnimationLoss();
                  }
                  else
                  {
                     gameWinnerName = GetPlayerNameByIndex(TBFConstants.PlayerIndexRemote);   

                     //Remote winner
                     SoundManager.Instance.PlayAudioClip(SoundConstants.GameOverLoss);
                     _gameSceneManager.GameUIView.AvatarViews[TBFConstants.PlayerIndexLocal].PlayAnimationLoss();
                     _gameSceneManager.GameUIView.AvatarViews[TBFConstants.PlayerIndexRemote].PlayAnimationWin();
                  }

                  _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_Ending,
                    _gameSceneManager.GameProgressData.CurrentRoundNumber, gameWinnerName), BufferedTextMode.Queue);

                  await SetGameState(GameState.GameEnded);

                  break;
               case GameState.GameEnded:
                  // **************************************
                  //  Game stays here. 
                  //  User must click "Back" buton
                  //
                  // NOTE: We come here from GameState.GameEnding and/or when a player disconnects
                  //
                  // **************************************

                  //Turn off buttons. We may come here from any state, if a player disconnects
                  _gameSceneManager.GameUIView.MoveButtonsCanvasGroup.interactable = false;
                  break;
               default:
                  SwitchDefaultException.Throw(_gameState);
                  break;
            }
         }, new System.Diagnostics.StackTrace(true));
      }


      /// <summary>
      /// Decrements the LOSERS health
      /// </summary>
      /// <param name="currentRoundWinnerPlayerDbid"></param>
      /// <param name="deltaHealth"></param>
      private void UpdateHealth(long currentRoundWinnerPlayerDbid, int deltaHealth)
      {
         if (_gameSceneManager.MultiplayerSession.IsLocalPlayerDbid(currentRoundWinnerPlayerDbid))
         {
            _gameSceneManager.GameUIView.AvatarUIViews[TBFConstants.PlayerIndexRemote].HealthBarView.Value += deltaHealth;
         }
         else
         {
            _gameSceneManager.GameUIView.AvatarUIViews[TBFConstants.PlayerIndexLocal].HealthBarView.Value += deltaHealth;
         }
      }


      private async Task RenderPlayerMove(int playerIndex, GameMoveType gameMoveType)
      {
         string playerName = GetPlayerNameByIndex(playerIndex);
         _gameSceneManager.SetStatusText(string.Format(TBFConstants.StatusText_GameState_PlayerMoved,
            playerName, gameMoveType), BufferedTextMode.Queue);

         AvatarView avatarView = _gameSceneManager.GameUIView.AvatarViews[playerIndex];
         avatarView.PlayAnimationByGameMoveType(gameMoveType);

         // 1 Unity needs time to START non-IDLE animation ...
         await Await.While(() =>
         {
            return avatarView.IsIdleAnimation;
         });

         // 2 Unity needs time to RETURN to the IDLE animation ...
         await Await.While(() =>
         {
            return !avatarView.IsIdleAnimation;
         });
      }


      private string GetPlayerNameByPlayerDbid(long currentRoundWinnerPlayerDbid)
      {
         if (_gameSceneManager.MultiplayerSession.IsLocalPlayerDbid(currentRoundWinnerPlayerDbid))
         {
            return GetPlayerNameByIndex(TBFConstants.PlayerIndexLocal);
         }
         else
         {
            return GetPlayerNameByIndex(TBFConstants.PlayerIndexRemote);
         }
      }


      private string GetPlayerNameByIndex(int playerIndex)
      {
         return _gameSceneManager.Configuration.AvatarDatas[playerIndex].Location;
      }


      private void DebugLog(string message)
      {
         if (TBFConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }
   }
}
