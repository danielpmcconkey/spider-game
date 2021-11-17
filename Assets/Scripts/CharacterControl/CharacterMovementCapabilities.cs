using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CharacterControl
{
    public class CharacterMovementCapabilities
    {
        public bool canWallCrawl;
        public bool canCeilingCrawl;
        public bool isHorizontalMovementInAirAllowed;
        public bool canGrapple;
    }
}
