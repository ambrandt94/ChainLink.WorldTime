using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChainLink.WorldTime
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