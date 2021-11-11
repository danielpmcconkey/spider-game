using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public enum CharacterState
    {
        // states that can be held continuously
        IDLE,
        FALLING,
        RUNNING,
        // transitional states
        BEGINNING_A_JUMP,           // the single frame that starts the jump activities
        EARLY_JUMP_CYCLE,           // the period just after the jump that tells the game to not allow state transitions
        ADDIND_THRUST_TO_JUMP,
        BEGINNING_A_LANDING_H,      // the single frame that starts landing activities
        BEGINNING_A_LANDING_V,      // the single frame that starts landing activities
        EARLY_LANDING_CYCLE_H,      // the period just after the landing that tells the game to not allow state transitions
        EARLY_LANDING_CYCLE_V,      // the period just after the landing that tells the game to not allow state transitions
        BEGINNING_TO_FALL,          // the single frame that starts fall activities
    }
}
