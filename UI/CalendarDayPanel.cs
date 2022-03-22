using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChainWorldTime.Calendar
{
    public class CalendarDayPanel : MonoBehaviour
    {
        int DayIndex;
        WorldTimeDay Day;

        [SerializeField]
        TextMeshProUGUI DayLabelText;
        [SerializeField]
        GameObject CurrentDayIndication;
        [SerializeField]
        Slider TimeOfDaySlider;

        private void OnDestroy()
        {
            //RemoveCallbacks();
        }

        public void SetToDay(int dayOfMonth, WorldTimeDay day)
        {
            DayIndex = dayOfMonth;
            Day = day;
            string label = (dayOfMonth + 1).ToString() + ". " + day.DayName;
            DayLabelText?.SetText(label);
            gameObject.name = label;

            AssignCallbacks();
            //SetTimeRatio();
            SetTimeRatioPrecise();
            SetCurrentDay();
        }
        void SetCurrentDay()
        { 
            SetToCurrentDay(DayIndex == WorldTime.Instance.CurrentDayIndex);
        }
        public void SetToCurrentDay(bool currentDay)
        {
            if(CurrentDayIndication)
                CurrentDayIndication.SetActive(currentDay);
        }
        void SetTimeRatio()
        {
            if (TimeOfDaySlider)
            {
                TimeOfDaySlider.minValue = 0;
                TimeOfDaySlider.maxValue = WorldTime.Instance.ProfileAsset.Profile.TimesOfDay.Count-1;
                TimeOfDaySlider.value = WorldTime.Instance.TimeOfDayIndex;
            }
        }
        void SetTimeRatioPrecise()
        {
            if (TimeOfDaySlider)
            {
                TimeOfDaySlider.minValue = 0;
                TimeOfDaySlider.maxValue = 1;
                TimeOfDaySlider.value = WorldTime.Instance.PreciseTimeOfDayRatio;
            }
        }

        void AssignCallbacks()
        {
            WorldTime.Instance.OnPreciseTimeOfDayUpdated += delegate
            {
                SetTimeRatioPrecise();
            };
            WorldTime.Instance.OnTimeOfDayUpdated += delegate
            {
                //SetTimeRatio();
            };
            WorldTime.Instance.OnDayUpdated += delegate
            {
                SetCurrentDay();
                //SetTimeRatio();
            };
        }
        void RemoveCallbacks()
        {
            Debug.Log("Remove Callbacks");
            WorldTime.Instance.OnTimeOfDayUpdated -= delegate
            {
                SetTimeRatio();
            };
            WorldTime.Instance.OnDayUpdated -= delegate
            {
                SetCurrentDay();
                SetTimeRatio();
            };
        }
    }
}