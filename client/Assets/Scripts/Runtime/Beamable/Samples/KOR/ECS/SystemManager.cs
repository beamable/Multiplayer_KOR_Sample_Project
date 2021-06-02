using Beamable.Api;
using Beamable.Examples.Features.Multiplayer.Core;
using Unity.Entities;

namespace Beamable.Samples.KOR.Multiplayer
{
public static class SystemManager
   {

      public static void DestroyGameSystems()
      {
         var world = World.DefaultGameObjectInjectionWorld;
         var timeSystem = world.GetExistingSystem<SimWorldTimeSystem>();
         var gameController = world.GetExistingSystem<GameController>();
         var gameSystem = world.GetExistingSystem<GameSystem>();
         // var inputSystem = world.GetExistingSystem<InputSystem>();
         var hashingSystem = world.GetExistingSystem<ChecksumSystem>();

         var initGroup = world.GetExistingSystem<InitializationSystemGroup>();
         var fixedGroup = world.GetExistingSystem<FixedStepSimulationSystemGroup>();

         world.DestroySystem(timeSystem);
         world.DestroySystem(gameController);
         world.DestroySystem(gameSystem);
         // world.DestroySystem(inputSystem);
         world.DestroySystem(hashingSystem);

         world.EntityManager.DestroyAndResetAllEntities();

         NetworkController.HighestSeenNetworkFrame = 0;
         NetworkController.NetworkInitialized = false;

         initGroup.RemoveSystemFromUpdateList(timeSystem);

         fixedGroup.RemoveSystemFromUpdateList(gameController);
         fixedGroup.RemoveSystemFromUpdateList(gameSystem);
         // fixedGroup.RemoveSystemFromUpdateList(inputSystem);
         fixedGroup.RemoveSystemFromUpdateList(hashingSystem);

         initGroup.SortSystems();
         fixedGroup.SortSystems();
      }

      public static void StartGameSystems()
      {


         // create systems for game...

         var world = World.DefaultGameObjectInjectionWorld;
         var timeSystem = world.GetOrCreateSystem<SimWorldTimeSystem>();
         var gameController = world.GetOrCreateSystem<GameController>();
         var gameSystem = world.GetOrCreateSystem<GameSystem>();
         // var inputSystem = world.GetOrCreateSystem<InputSystem>();
         var hashingSystem = world.GetOrCreateSystem<ChecksumSystem>();


         var initGroup = world.GetExistingSystem<InitializationSystemGroup>();
         var fixedGroup = world.GetExistingSystem<FixedStepSimulationSystemGroup>();

         var oldTime = world.GetExistingSystem<UpdateWorldTimeSystem>();
         if (oldTime != null)
         {
            world.DestroySystem(oldTime); // don't allow time to ever move forward.
            initGroup.RemoveSystemFromUpdateList(oldTime);
         }

         initGroup.AddSystemToUpdateList(timeSystem);

         fixedGroup.AddSystemToUpdateList(gameController);
         fixedGroup.AddSystemToUpdateList(gameSystem);
         // fixedGroup.AddSystemToUpdateList(inputSystem);
         fixedGroup.AddSystemToUpdateList(hashingSystem);

         initGroup.SortSystems();
         fixedGroup.SortSystems();
      }
   }
}