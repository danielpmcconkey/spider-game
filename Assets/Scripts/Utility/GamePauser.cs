using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class GamePauser
    {
        public static bool isPaused = false;

        public static void PauseGame()
        {
            Time.timeScale = 0;
            isPaused = true;
        }
        public static void ResumeGame()
        {
            Time.timeScale = 1;
            isPaused = false;
        }
    }
}
