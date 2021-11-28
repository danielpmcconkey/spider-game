using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    internal static class Raycaster
    {
        // how to raycast https://www.youtube.com/watch?v=wkKsl1Mfp5M
        // raycasting starts at 13:38 

        internal static RaycastHit2D FireAtTargetPoint(Vector2 firePoint, Vector2 targetPoint, 
            float maxDistance, LayerMask layerMask)
        {
            Vector2 normalizedDirection = GetNormalizedDirectionBetweenPoints(firePoint, targetPoint);

            return Physics2D.Raycast(firePoint, normalizedDirection,
                maxDistance, layerMask);
        }
        internal static Vector2 GetNormalizedDirectionBetweenPoints(Vector2 point1, Vector2 point2)
        {
            Vector2 targetDirection = point2 - point1;
            float distanceBetween = targetDirection.magnitude;
            return targetDirection / distanceBetween;
        }
    }
}
