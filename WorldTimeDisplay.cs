using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChainWorldTime
{
    public class WorldTimeDisplay : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI TimeText;

        private void Awake()
        {
            if (WorldTime.Instance != null)
            {
                if (TimeText != null)
                    TimeText.SetText(WorldTime.Instance.GetDateString());
            }
        }
    }
}