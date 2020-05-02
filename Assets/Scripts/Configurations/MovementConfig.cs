using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Configurations
{
    [Serializable]
    public class MovementConfig
    {
        public KeyCode UpKey;
        public KeyCode DownKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode BombKey;

        public bool IsValidConfig()
        {
            return UpKey != KeyCode.None
                || DownKey != KeyCode.None
                || LeftKey != KeyCode.None
                || RightKey != KeyCode.None
                || BombKey != KeyCode.None;
        }
    }
}
