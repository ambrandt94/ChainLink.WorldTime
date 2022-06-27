using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainLink.WorldTime
{
    [CreateAssetMenu(fileName = "NewWorldTimeEvent", menuName = "ChainTime/New Event", order = 1)]
    public class WorldTimeEvent : ScriptableObject
    {
        public string Name;
        [Multiline]
        public string Description;

        public WorldTimePoint StartPoint;
        public WorldTimePoint LengthOfTime;
        public WorldTimePoint EndPoint
        {
            get
            {
                if (LengthOfTime == WorldTimePoint.Zero)
                    return StartPoint;

                return StartPoint;
            }
        }
    }
}