using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChainWorldTime
{
    public class WorldTimeListener : MonoBehaviour
    {
        [Header("Data"), SerializeField]
        UnityEvent<WorldTimeMonth> OnMonthDataChanged;
        [SerializeField]
        UnityEvent<WorldTimeDay> OnDayDataChanged;
        [SerializeField]
        UnityEvent<WorldTimeOfDay> OnTimeOfDayDataChanged;
        
        [Header("Numerical"), SerializeField]
        UnityEvent<int> OnYearChanged;
        [SerializeField]
        UnityEvent<int> OnMonthChanged;
        [SerializeField]
        UnityEvent<int> OnDayChanged;

        [Header("Ratio"), SerializeField]
        UnityEvent<float> TimeOfDayChanged;
        [SerializeField]
        UnityEvent<float> PreciseTimeOfDayRatioChanged;

        private void Awake()
        {
            WorldTime.Instance.OnYearUpdated += delegate { OnDayChanged?.Invoke(WorldTime.Instance.CurrentYear); };
            OnDayChanged?.Invoke(WorldTime.Instance.CurrentYear);

            WorldTime.Instance.OnMonthUpdated += delegate { OnMonthDataChanged?.Invoke(WorldTime.Instance.CurrentMonth); };
            OnMonthDataChanged?.Invoke(WorldTime.Instance.CurrentMonth);
            WorldTime.Instance.OnMonthUpdated += delegate { OnMonthChanged?.Invoke(WorldTime.Instance.CurrentMonthIndex); };
            OnMonthChanged?.Invoke(WorldTime.Instance.CurrentMonthIndex);

            WorldTime.Instance.OnDayUpdated += delegate { OnDayDataChanged?.Invoke(WorldTime.Instance.CurrentDay); };
            OnDayDataChanged?.Invoke(WorldTime.Instance.CurrentDay); 
            WorldTime.Instance.OnDayUpdated += delegate { OnDayChanged?.Invoke(WorldTime.Instance.CurrentDayIndex); };
            OnDayChanged?.Invoke(WorldTime.Instance.CurrentDayIndex);

            WorldTime.Instance.OnTimeOfDayUpdated += delegate { OnTimeOfDayDataChanged?.Invoke(WorldTime.Instance.TimeOfDay); };
            OnTimeOfDayDataChanged?.Invoke(WorldTime.Instance.TimeOfDay);
            WorldTime.Instance.OnTimeOfDayUpdated += delegate { TimeOfDayChanged?.Invoke(WorldTime.Instance.TimeOfDayRatio); };
            TimeOfDayChanged?.Invoke(WorldTime.Instance.TimeOfDayRatio);
            WorldTime.Instance.OnPreciseTimeOfDayUpdated += delegate { PreciseTimeOfDayRatioChanged?.Invoke(WorldTime.Instance.PreciseTimeOfDayRatio); };
            PreciseTimeOfDayRatioChanged?.Invoke(WorldTime.Instance.PreciseTimeOfDayRatio);
        }
    }
}