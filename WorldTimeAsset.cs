using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainWorldTime
{
    [CreateAssetMenu(fileName = "NewWorldTimeAsset", menuName = "ChainLink/World Time Asset", order = 1)]
    public class WorldTimeAsset : ScriptableObject
    {
        public WorldTimeProfile Profile;
    }
}