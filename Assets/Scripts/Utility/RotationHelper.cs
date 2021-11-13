using Assets.Scripts.CharacterControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public struct RotationTarget
    {
        public FacingDirection currentHeadingDirection;
        public FacingDirection currentThrustingDirection;
        public FacingDirection targetHeadingDirection;
        public FacingDirection targetThrustingDirection;
        public Vector3 startingRotation;
        public Vector3 targetRotation;

    }
    public static class RotationHelper
    {
        static List<RotationTarget> dataTable;
        
        static RotationHelper()
        {
            PopulateDataTable();
        }
        public static RotationTarget getRotationVectors(RotationTarget inRotator)
        {
            RotationTarget outRotator = new RotationTarget()
            {
                currentHeadingDirection = inRotator.currentHeadingDirection,
                currentThrustingDirection = inRotator.currentThrustingDirection,
                targetHeadingDirection = inRotator.targetHeadingDirection,
                targetThrustingDirection = inRotator.targetThrustingDirection,
                startingRotation = new Vector3(),
                targetRotation = new Vector3(),
            };
            RotationTarget match = dataTable.Where(x => x.currentHeadingDirection == inRotator.currentHeadingDirection
                && x.currentThrustingDirection == inRotator.currentThrustingDirection
                && x.targetHeadingDirection == inRotator.targetHeadingDirection
                && x.targetThrustingDirection == inRotator.targetThrustingDirection
            ).FirstOrDefault();

            outRotator.startingRotation = match.startingRotation;
            outRotator.targetRotation = match.targetRotation;
            return outRotator;
        }

        private static void PopulateDataTable()
        {
            dataTable = new List<RotationTarget>();
            #region generated code
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(180, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(0, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(0, 180, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(0, 0, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(0, 180, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(0, 180, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 180, 0), targetRotation = new Vector3(0, 0, -90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(0, 180, 0) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(180, 180, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(180, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(180, 180, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(0, 180, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(0, 180, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.LEFT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(180, 180, 0), targetRotation = new Vector3(0, 0, -90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(0, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(0, 0, 180) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(180, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(0, 0, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(0, 180, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(0, 180, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.UP, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 0, 0), targetRotation = new Vector3(0, 0, -90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(0, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(180, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(0, 0, 0) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(180, 0, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(180, 0, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(0, 180, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.RIGHT, currentThrustingDirection = FacingDirection.DOWN, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(180, 0, 0), targetRotation = new Vector3(0, 0, -90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(0, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(0, 0, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(0, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(180, 0, 0) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(0, 180, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(180, 0, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 0, 90), targetRotation = new Vector3(0, 0, -90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(0, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(180, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(0, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(0, 180, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(0, 0, 90) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(0, 180, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.UP, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 180, 90), targetRotation = new Vector3(180, 180, 90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(0, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(180, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(0, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(0, 180, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(180, 0, 180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(0, 180, 90) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(null) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.LEFT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 180, -90), targetRotation = new Vector3(0, 0, -90) });

            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(0, 180, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.LEFT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(0, 0, -180) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.UP, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(0, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.RIGHT, targetThrustingDirection = FacingDirection.DOWN, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(180, 0, 0) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(0, 0, 90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.UP, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(180, 0, -90) });
            dataTable.Add(new RotationTarget() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.LEFT, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(0, 180, -90) });
            //dataTable.Add(new Rotator() { currentHeadingDirection = FacingDirection.DOWN, currentThrustingDirection = FacingDirection.RIGHT, targetHeadingDirection = FacingDirection.DOWN, targetThrustingDirection = FacingDirection.RIGHT, startingRotation = new Vector3(0, 0, -90), targetRotation = new Vector3(null) });
            #endregion
        }
    }
}
