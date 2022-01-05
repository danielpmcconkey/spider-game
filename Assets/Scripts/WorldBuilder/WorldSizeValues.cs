using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.WorldBuilder
{
    public enum WorldSizes { SMALL = 0, MEDIUM = 1, LARGE = 2, XL = 3, DEBUG = 4 }
    public static class WorldSizeValues
    {
        public static WorldSize[] sizes { get; set; }

        // all sizes are in "rooms"

        static WorldSizeValues()
        {
            /* 
             * metroid 1 map is 30 "rooms" wide and
             * 29 "rooms" tall, though with a lot of
             * gaps. 
             * 
             * Sizes by rooms
             *   XL = 30 x 30
             *   L = 25 x 25
             *   M = 20 x 20
             *   S = 15 x 15
             * 
             * */
            sizes = new WorldSize[]
            {
                new WorldSize{
                    name = "Small",
                    minWidth = 12,
                    maxWidth = 18,
                    minHeight = 12,
                    maxHeight = 18,
                },
                new WorldSize{
                    name = "Medium",
                    minWidth = 17,
                    maxWidth = 23,
                    minHeight = 17,
                    maxHeight = 23,
                },
                new WorldSize{
                    name = "Large",
                    minWidth = 22,
                    maxWidth = 28,
                    minHeight = 22,
                    maxHeight = 23,
                },
                new WorldSize{
                    name = "Extra Large",
                    minWidth = 27,
                    maxWidth = 32,
                    minHeight = 27,
                    maxHeight = 32,
                },
                new WorldSize{
                    name = "DEBUG",
                    minWidth = 4,
                    maxWidth = 4,
                    minHeight = 4,
                    maxHeight = 4,
                },
            };
        }
    }
}
