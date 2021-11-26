using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public enum FacingDirection { RIGHT = 0, UP = 1, LEFT = 2, DOWN = 3 }

    public class CharacterOrienter
    {
        private FacingDirection _headingDirection; // the direction you would walk
        private FacingDirection _thrustingDirection; // the direction you would jump
        private FacingDirection _gravityDirection; // the direction gravity is pushing in

        public FacingDirection headingDirection { get { return _headingDirection; } }
        public FacingDirection thrustingDirection { get { return _thrustingDirection; } }
        public FacingDirection gravityDirection { get { return _gravityDirection; } }

        

        public CharacterOrienter()
        {
            _headingDirection = FacingDirection.RIGHT;
            _thrustingDirection = FacingDirection.UP;
            _gravityDirection = FacingDirection.DOWN;
        }
        public FacingDirection GetOppositeDirection(FacingDirection direction)
        {
            int directionInt = (int)direction;
            int oppositeDirectionInt = directionInt + 2;
            if (oppositeDirectionInt > 3) oppositeDirectionInt -= 4;
            FacingDirection oppositeDirection = (FacingDirection)oppositeDirectionInt;
            return oppositeDirection;
        }
        public void SetGravityDirection(FacingDirection newDir)
        {
            if (_gravityDirection != newDir)
            {
                LogGravityChange(_gravityDirection, newDir);
                _gravityDirection = newDir;
            }
        }
        public void SetHeadingDirection(FacingDirection newDir)
        {
            if (_headingDirection != newDir)
            {
                LogHeadingChange(_headingDirection, newDir);
                _headingDirection = newDir;
            }
        }
        public void SetThrustingDirection(FacingDirection newDir)
        {
            if (_thrustingDirection != newDir)
            {
                LogThrustingChange(_thrustingDirection, newDir);
                _thrustingDirection = newDir;
            }
        }
        private void LogGravityChange(FacingDirection oldDir, FacingDirection newDir)
        {
            LoggerCustom.DEBUG(string.Format("Gravity direction has changed from {0} to {1}",
                    oldDir, newDir));
        }
        private void LogThrustingChange(FacingDirection oldDir, FacingDirection newDir)
        {
            LoggerCustom.DEBUG(string.Format("Thrusting direction has changed from {0} to {1}",
                    oldDir, newDir));
        }
        private void LogHeadingChange(FacingDirection oldDir, FacingDirection newDir)
        {
            LoggerCustom.DEBUG(string.Format("Heading direction has changed from {0} to {1}",
                    oldDir, newDir));
        }
    }
}
