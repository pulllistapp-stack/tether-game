using System;
using System.Collections.Generic;

namespace Tether.Save
{
    [Serializable]
    public class SaveData
    {
        public int Version = 1;
        public int CurrentFloor = 1;
        public string CurrentChamberId = "";
        public float MaxHeightReached = 0f;
        public List<string> DiscoveredChambers = new List<string>();
        public List<string> CollectedEars = new List<string>();
        public string CurrentCharacter = "male";
        public int RunCount = 0;
        public bool FirstRunCompleted = false;
        public bool SecondRunUnlocked = false;
    }
}
