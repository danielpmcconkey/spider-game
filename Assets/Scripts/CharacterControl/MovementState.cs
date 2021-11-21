using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    
    public enum MovementState
    {
        // character can only be in one of these at a time

        FLOATING,
        GROUNDED,
        TETHERED,
        JUMP_ACCELERATING,
        //CORNERING,  // the state of transitioning between walking on the floor and walking on a wall (or vice versa)
    }
    
    
}
