using System.Collections.Generic;
using Beamable.Samples.KOR.Data;
using Unity.Entities;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Physics;
using UnityS.Physics.Extensions;
using UnityS.Physics.Systems;
using UnityS.Transforms;

namespace Beamable.Samples.KOR.Multiplayer
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class GameController : SystemBase
    {
        public static GameController Instance;

        // store entity-gameobject pairs for rendering (not really efficient to use gameobjects here)
        private readonly Dictionary<Entity, GameObject> objects = new Dictionary<Entity, GameObject>(32);

        private MaterialPropertyBlock matPropBlock;

        private Dictionary<(float3 size, UnityS.Physics.Material material), BlobAssetReference<UnityS.Physics.Collider>> boxColliders = new Dictionary<(float3 size, UnityS.Physics.Material material), BlobAssetReference<UnityS.Physics.Collider>>();
        private readonly List<ComponentType> componentTypes = new List<ComponentType>(20);

        private PCG _rand = default;
        private bool randInitialized = false;

        public PCG Random
        {
            get
            {
                if (!randInitialized)
                {
                    _rand = new PCG(0, (ulong)1); // TODO: Get this seed from the sim client.
                    randInitialized = true;
                }

                return _rand;
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("Creating Game Controller");
            Instance = this;
            UnityEngine.Physics.autoSimulation = false;

            matPropBlock = new MaterialPropertyBlock();

            // setup physics parameters
            var systemGroup = World.GetOrCreateSystem<FixedStepSimulationSystemGroup>();
            systemGroup.Timestep = (float) (sfloat.One / (sfloat) 60.0f);
            systemGroup.World.MaximumDeltaTime = 100000;

            var fixedRateManager = new UnboundedFixedRateCatchUpManager(systemGroup.Timestep);
            systemGroup.FixedRateManager = fixedRateManager;
            Entity physicsStep = EntityManager.CreateEntity(typeof(PhysicsStep));
            PhysicsStep physicsStepParams = PhysicsStep.Default;
            physicsStepParams.SolverStabilizationHeuristicSettings = new Solver.StabilizationHeuristicSettings
            {
                EnableSolverStabilization = true,
                EnableFrictionVelocities = true,
                InertiaScalingFactor = (sfloat) 0.0f,
                VelocityClippingFactor = (sfloat) 1.0f
            };

            physicsStepParams.SolverIterationCount = 3;
            physicsStepParams.MultiThreaded = 1;
            physicsStepParams.Gravity = new float3(sfloat.Zero, (sfloat) (-60.0f), sfloat.Zero);
            EntityManager.SetComponentData(physicsStep, physicsStepParams);

            UnityS.Physics.Material material = UnityS.Physics.Material.Default;
            material.Friction = sfloat.One;

            PhysicsParams physicsParamsStatic = PhysicsParams.Default;
            physicsParamsStatic.isDynamic = false;

            PhysicsParams physicsParamsDynamic = PhysicsParams.Default;
            physicsParamsDynamic.isDynamic = true;

            CreateBoxColliderObject(GameResourceManager.Instance.CubePrefab, new float3(sfloat.Zero, sfloat.Zero, sfloat.Zero),
                new float3((sfloat) 500.0f, (sfloat) 2.0f, (sfloat) 500.0f), quaternion.identity, material,
                physicsParamsStatic);

            var c = 50;
            for (var i = 0f; i < c; i++)
            {
                var x = ((sfloat)8) * math.cos((sfloat) (14 * (i/100f) * 3.14f));
                var (renderer, entity) = CreateBoxColliderObject(GameResourceManager.Instance.CubePrefab,
                    new float3(x, (sfloat) (4 + (i*8)), (sfloat)2),
                    new float3(sfloat.One, sfloat.One, sfloat.One), quaternion.identity, material,
                    physicsParamsDynamic);
                renderer.material = GameResourceManager.Instance.colorMaterials[((int)i)%GameResourceManager.Instance.colorMaterials.Length];

            }
        }


        public (MeshRenderer, Entity) SpawnCube(long colorIndex, float3 position)
        {
            Debug.Log("Spawning cube!");
            UnityS.Physics.Material material = UnityS.Physics.Material.Default;
            material.Friction = (sfloat)0.05f;

            PhysicsParams physicsParams = PhysicsParams.Default;
            physicsParams.isDynamic = true;

            var direction = position / math.length(position);
            physicsParams.startingLinearVelocity = -direction * Random.SFloatExclusive((sfloat)5, (sfloat)6); // launch towards center for now...
            physicsParams.mass = (sfloat).25f;

            var cube = GameController.Instance.CreateBoxColliderObject(GameResourceManager.Instance.CubePrefab,
                //new float3((sfloat)0, (sfloat)5, -(sfloat)2),
                position,
                new float3((sfloat)1f, (sfloat)1f, (sfloat)1f), quaternion.identity, material, physicsParams);

            cube.Item1.material = GameResourceManager.Instance.colorMaterials[(colorIndex)%GameResourceManager.Instance.colorMaterials.Length];
            return cube;
        }


        protected override void OnUpdate()
        {
            Entities.ForEach((ref Entity e, ref Translation t, ref Rotation r, ref PhysicsMass mass, ref PhysicsVelocity _vel) =>
            {
                if (objects.TryGetValue(e, out GameObject obj))
                {
                    obj.transform.localPosition = (Vector3)t.Value;
                    obj.transform.localRotation = (Quaternion)r.Value;

                }
            }).WithoutBurst().Run();
        }


        public (T, Entity) CreateBoxColliderObject<T>(T prefab, float3 position, float3 size, quaternion rotation, UnityS.Physics.Material material,
            PhysicsParams physicsParams)
        where T : Component
        {
            if (!boxColliders.TryGetValue((size, material), out BlobAssetReference<UnityS.Physics.Collider> collider))
            {
                collider = UnityS.Physics.BoxCollider.Create(
                    new BoxGeometry
                    {
                        Center = float3.zero,
                        Size = size,
                        Orientation = quaternion.identity
                    }, CollisionFilter.Default, material
                );

                boxColliders.Add((size, material), collider);
            }

            Entity entity = CreateEntity(position, rotation, collider, physicsParams);


            //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube); // TODO: replace this with a prefab reference?

            var obj = GameObject.Instantiate(prefab);
            obj.transform.localScale = (Vector3) size;
            obj.transform.localPosition = (Vector3) position;
            obj.transform.localRotation = (Quaternion) rotation;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            // renderer.material = (ResourceManager.Instance.defaultMaterial);
            renderer.GetPropertyBlock(matPropBlock);
            // matPropBlock.SetColor("_Color", RandomColor());
            renderer.SetPropertyBlock(matPropBlock);

            objects.Add(entity, obj.gameObject);
            return (obj, entity);
        }

        public Entity CreateEntity(float3 position, quaternion rotation, BlobAssetReference<UnityS.Physics.Collider> collider,
            PhysicsParams physicsParams)
        {
            return CreatePhysicsBody(position, rotation, collider, physicsParams);
        }

        public void DestroyEntity(Entity entity)
        {
            EntityManager.DestroyEntity(entity);
        }

        public unsafe Entity CreatePhysicsBody(float3 position, quaternion orientation,
            BlobAssetReference<UnityS.Physics.Collider> collider,
            PhysicsParams physicsParams)
        {
            componentTypes.Clear();

            componentTypes.Add(typeof(Translation));
            componentTypes.Add(typeof(Rotation));
            componentTypes.Add(typeof(LocalToWorld));
            componentTypes.Add(typeof(PhysicsCollider));
            componentTypes.Add(typeof(PhysicsCustomTags));

            if (physicsParams.isDynamic)
            {
                componentTypes.Add(typeof(PhysicsVelocity));
                componentTypes.Add(typeof(PhysicsMass));
                componentTypes.Add(typeof(PhysicsDamping));

                // componentTypes.Add(typeof(MoveForceData));
            }

            Entity entity = EntityManager.CreateEntity(componentTypes.ToArray());

            EntityManager.SetComponentData(entity, new Translation {Value = position});
            EntityManager.SetComponentData(entity, new Rotation {Value = orientation});

            EntityManager.SetComponentData(entity, new PhysicsCollider {Value = collider});

            if (physicsParams.isDynamic)
            {
                UnityS.Physics.Collider* colliderPtr = (UnityS.Physics.Collider*) collider.GetUnsafePtr();
                EntityManager.SetComponentData(entity,
                    PhysicsMass.CreateDynamic(colliderPtr->MassProperties, physicsParams.mass));
                // Calculate the angular velocity in local space from rotation and world angular velocity
                float3 angularVelocityLocal =
                    math.mul(math.inverse(colliderPtr->MassProperties.MassDistribution.Transform.rot),
                        physicsParams.startingAngularVelocity);
                EntityManager.SetComponentData(entity, new PhysicsVelocity()
                {
                    Linear = physicsParams.startingLinearVelocity,
                    Angular = angularVelocityLocal
                });
                EntityManager.SetComponentData(entity, new PhysicsDamping()
                {
                    Linear = physicsParams.linearDamping,
                    Angular = physicsParams.angularDamping
                });
                // EntityManager.SetComponentData(entity, new MoveForceData {Direction = new float2((sfloat)0, (sfloat)0), Magnitude = (sfloat)0});

            }

            return entity;
        }
    }
}