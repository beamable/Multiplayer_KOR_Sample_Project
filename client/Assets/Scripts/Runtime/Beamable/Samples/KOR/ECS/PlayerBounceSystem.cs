using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Audio;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Physics;
using UnityS.Physics.Systems;

namespace Beamable.Samples.KOR.Multiplayer
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class PlayerBounceSystem : JobComponentSystem
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        protected override void OnCreate()
        {
            base.OnCreate();
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!NetworkController.NetworkInitialized)
            {
                return default;
            }

            var job = new PlayerBounceSystemJob();
            job.bouncyGroup = GetComponentDataFromEntity<BouncyTag>(true);
            job.impulseGroup = GetComponentDataFromEntity<PhysicsImpulse>();
            job.velocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(true);

            var jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
            jobHandle.Complete();

            return jobHandle;
        }

        private struct PlayerBounceSystemJob : ICollisionEventsJob
        {
            [ReadOnly]
            public ComponentDataFromEntity<BouncyTag> bouncyGroup;

            [ReadOnly]
            public ComponentDataFromEntity<PhysicsVelocity> velocityGroup;

            public ComponentDataFromEntity<PhysicsImpulse> impulseGroup;

            private sfloat GetImpulse(Entity e)
            {
                if (!impulseGroup.HasComponent(e))
                    return (sfloat)0.0f;

                sfloat sqrMagImpulse = impulseGroup[e].Impulse.x * impulseGroup[e].Impulse.x
                       + impulseGroup[e].Impulse.y * impulseGroup[e].Impulse.y
                       + impulseGroup[e].Impulse.z * impulseGroup[e].Impulse.z;

                return sqrMagImpulse;
            }

            public void Execute(CollisionEvent collisionEvent)
            {
                var a = collisionEvent.EntityA;
                var b = collisionEvent.EntityB;

                var aIsBouncy = bouncyGroup.HasComponent(a);
                var bIsBouncy = bouncyGroup.HasComponent(b);
                var aHasImpulse = impulseGroup.HasComponent(a);
                var bHasImpulse = impulseGroup.HasComponent(b);

                sfloat minImpulse = (sfloat)0.01f;
                sfloat totalImpulse = GetImpulse(a) + GetImpulse(b);
                if (totalImpulse > minImpulse)
                {
                    GameSceneManager.Instance.EnqueueConcurrent(() =>
                    {
                        List<string> collisionClips = new List<string>() { SoundConstants.Collision01, SoundConstants.Collision02, SoundConstants.Collision03 };
                        SoundManager.Instance.PlayAudioClip(collisionClips[Random.Range(0, collisionClips.Count)], SoundManager.GetRandomPitch(1.0f, 0.3f));
                    });
                }

                if (aIsBouncy && bIsBouncy) return; // don't do anything, because I can't figure out how correctly handle it yet...

                if (aIsBouncy && bHasImpulse)
                {
                    // apply an explosion impulse to b.
                    var bImpulse = impulseGroup[b];
                    bImpulse.Impulse = collisionEvent.Normal * bouncyGroup[a].Bounce;
                    impulseGroup[b] = bImpulse;
                }

                if (bIsBouncy && aHasImpulse)
                {
                    // apply an explosion impulse to a
                    var aImpulse = impulseGroup[a];
                    aImpulse.Impulse = collisionEvent.Normal * bouncyGroup[b].Bounce;
                    impulseGroup[a] = aImpulse;
                }
            }
        }
    }
}