using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public enum MovementTrigger
    {
        TRIGGER_JUMP,
        TRIGGER_LANDING,
        TRIGGER_FALL,
        TRIGGER_CORNER,
        TRIGGER_GRAPPLE_ATTEMPT,
        TRIGGER_GRAPPLE_SUCCESS,
        TRIGGER_GRAPPLE_RELEASE,
        TRIGGER_GRAPPLE_REACHED_ANCHOR,
    }
}
