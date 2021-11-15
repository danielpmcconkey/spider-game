using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public enum CharacterState
    {
        // states that can be held more than a frame
        IDLE,
        FALLING,
        RUNNING,
        EARLY_JUMP_CYCLE,               // the period just after the jump that tells the game to not allow state transitions
        ADDIND_THRUST_TO_JUMP,
        EARLY_LANDING_CYCLE_H,          // the period just after the landing that tells the game to not allow state transitions
        EARLY_LANDING_CYCLE_V,          // the period just after the landing that tells the game to not allow state transitions
        // trigger states
        TRIGGER_JUMP,               // the single frame that starts the jump activities
        TRIGGER_LANDING_H,          // the single frame that starts landing activities
        TRIGGER_LANDING_V,          // the single frame that starts landing activities
        TRIGGER_LANDING_CEILING,    // the single frame that starts landing activities
        TRIGGER_FALL,               // the single frame that starts fall activities
        TRIGGER_CORNER,             // We're about to start wall crawling
    }
}
