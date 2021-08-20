using System;
using Beamable.Samples.Core;
using Beamable.Samples.Core.Attributes;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
   public class MovePreviewBehaviour : MonoBehaviour
   {
      [Header("Configuration")]
      public float MaxLength = 8;

      [Range(2, 25)]
      public int LineResolution = 12;

      public float Speed = 1f;

      public float SmoothTime = .1f;

      [Header("References")]
      [Tooltip("A reference to the actual line renderer that represents the preview")]
      public LineRenderer LineRenderer;

      [Tooltip("This transform is the position that the arrow preview is going to start from. This should be the player's transform")]
      public Transform PreviewAnchor;

      [Header("Internal Data")]
      [ReadOnly]
      public Vector3 Direction;

      [ReadOnly]
      public float CurrLength;

      [ReadOnly]
      public float Ratio;

      [ReadOnly]
      public bool IsPowering;

      [ReadOnly]
      public float LengthPerSegment;

      [ReadOnly]
      public Vector3[] SegmentVelocities;

      private void Start()
      {
         LengthPerSegment = MaxLength / LineResolution;
         LineRenderer.positionCount = LineResolution;
         SegmentVelocities = new Vector3[LineResolution];
         for (var i = 0; i < LineResolution; i++)
         {
            // LineRenderer.SetPosition(i, i * LengthPerSegment * Vector3.right);
            SegmentVelocities[i] = Vector3.zero;
         }
      }

      private void Update()
      {
         LineRenderer.gameObject.SetActive(IsPowering);
         CurrLength = Ratio * MaxLength;
         CurrLength += 1; // min.

         for (var i = 0; i < LineResolution; i++)
         {
            var segmentRatio = i / (LineResolution - 1f);
            var currPosition = LineRenderer.GetPosition(i); // in world position...
            
            var velocity = SegmentVelocities[i];
            var targetPosition = segmentRatio * CurrLength * Direction;
            targetPosition.y = .1f;
            // convert target position to world position...
            targetPosition = transform.TransformPoint(targetPosition);

            var actualPosition = Vector3.SmoothDamp(currPosition, targetPosition, ref velocity, SmoothTime, Speed);

            LineRenderer.SetPosition(i, actualPosition);
         }
      }

      public void Set(bool isPowering, Vector3 direction, float powerRatio)
      {
         IsPowering = isPowering;
         Direction = transform.InverseTransformDirection(direction.normalized);
         Ratio = powerRatio;
      }
   }
}