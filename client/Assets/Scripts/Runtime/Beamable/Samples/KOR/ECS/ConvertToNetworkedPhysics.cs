
using Unity.Entities;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Physics;
using Collider = UnityS.Physics.Collider;

namespace Beamable.Samples.KOR.Multiplayer
{
   public class ConvertToNetworkedPhysics : MonoBehaviour
   {
      public UnityEngine.BoxCollider BoxCollider;

      public bool IsDynamic;

      public float Mass = .5f;

      public bool XRotationLocked;
      public bool YRotationLocked;
      public bool ZRotationLocked;

      public Entity Entity { get; private set; }
      // public bool AsCircle;

      // private void OnDrawGizmos()
      // {
      //    if (AsCircle)
      //    {
      //       var count = 14;
      //       Gizmos.color = Color.green;
      //       for (var i = 0f; i < count; i++)
      //       {
      //          var next = (i + 1) % count;
      //
      //          var ratio = i / count;
      //          var nextRatio = next / count;
      //
      //          var angle = ratio * 6.28f;
      //          var nextAngle = nextRatio * 6.28f;
      //
      //          var radius = BoxCollider.size.x * .5f;
      //          var halfHeight = BoxCollider.size.y * .5f;
      //
      //          var spot = radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
      //          var nextSpot = radius * new Vector3(Mathf.Cos(nextAngle), 0, Mathf.Sin(nextAngle));
      //
      //          var top = BoxCollider.transform.TransformPoint(spot + Vector3.up * halfHeight);
      //          var nextTop = BoxCollider.transform.TransformPoint(nextSpot + Vector3.up * halfHeight);
      //          var low = BoxCollider.transform.TransformPoint(spot - Vector3.up * halfHeight);
      //          var nextLow = BoxCollider.transform.TransformPoint(nextSpot - Vector3.up * halfHeight);
      //          Gizmos.DrawLine(top, nextTop);
      //          Gizmos.DrawLine(low, nextLow);
      //          Gizmos.DrawLine(low, top);
      //
      //       }
      //    }
      // }

      private async void Awake()
      {
         // var verts = new float3[Collider.sharedMesh.vertices.Length];
         // for (var i = 0; i < verts.Length; i++)
         // {
         //    var v = Collider.sharedMesh.vertices[i];
         //    verts[i] = new float3(
         //       x: (sfloat) v.x,
         //       y: (sfloat) v.y,
         //       z: (sfloat) v.z
         //    );
         // }
         //
         // var triangles = new int3[Collider.sharedMesh.triangles.Length / 3];
         // for (var i = 0; i < triangles.Length; i++)
         // {
         //    triangles[i] = new int3(
         //       Collider.sharedMesh.triangles[(i * 3) + 0],
         //       Collider.sharedMesh.triangles[(i * 3) + 1],
         //       Collider.sharedMesh.triangles[(i * 3) + 2]
         //    );
         // }

         UnityS.Physics.Material material = UnityS.Physics.Material.Default;

         // var collider = UnityS.Physics.MeshCollider.Create(
         //    new NativeArray<float3>(verts, Allocator.Temp),
         //    new NativeArray<int3>(triangles, Allocator.Temp),
         //    CollisionFilter.Default
         //    );



         var position = new float3(
            (sfloat) transform.position.x,
            (sfloat) transform.position.y,
            (sfloat) transform.position.z
         );

         var controller = await GameController.OnInstance;

         var physicsParams = PhysicsParams.Default;
         physicsParams.isDynamic = IsDynamic;
         physicsParams.lockAxis.x = XRotationLocked;
         physicsParams.lockAxis.y = YRotationLocked;
         physicsParams.lockAxis.z = ZRotationLocked;
         physicsParams.mass = (sfloat) Mass;

         var size = new float3(
            (sfloat) BoxCollider.size.x,
            (sfloat) BoxCollider.size.y,
            (sfloat) BoxCollider.size.z
         );
         var center = new float3(
            (sfloat) BoxCollider.center.x,
            (sfloat) BoxCollider.center.y,
            (sfloat) BoxCollider.center.z
         );

         BlobAssetReference<Collider> collider;

         // if (AsCircle)
         // {
         //    // collider = UnityS.Physics.SphereCollider.Create(
         //    //    new SphereGeometry
         //    //    {
         //    //       Center = center,
         //    //       Radius = size.x * (sfloat).5f
         //    //    }, CollisionFilter.Default, material);
         //    collider = UnityS.Physics.CylinderCollider.Create(
         //       new CylinderGeometry()
         //       {
         //          Center = center,
         //          Radius = size.x * (sfloat).5f,
         //          Height = size.y,
         //
         //          BevelRadius = sfloat.Zero,
         //          Orientation = quaternion.Euler((sfloat)0, (sfloat)0,(sfloat)180),
         //          SideCount = 14
         //
         //       }, CollisionFilter.Default, material
         //    );
         // }
         // else
         {
            collider = UnityS.Physics.BoxCollider.Create(
               new BoxGeometry
               {
                  Center = center,
                  Size = size,
                  Orientation = quaternion.identity
               }, CollisionFilter.Default, material
            );
         }

         Entity = controller.CreatePhysicsBody(position, quaternion.identity, collider, physicsParams);
         GameController.Instance.Register(BoxCollider.gameObject, Entity);
         // Destroy(Collider);
      }
   }
}