using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

namespace ChainLink.WorldTime
{
    public class WorldTime : MonoBehaviour
    {
        #region Variables
        private static WorldTime instance = null;
        public static WorldTime Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (WorldTime)FindObjectOfType(typeof(WorldTime));
                    //if (instance == null)
                    //instance = (new GameObject("WorldTime")).AddComponent<WorldTime>();

                    if (instance != null)
                        GameObject.DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        public WorldTimeAsset ProfileAsset;

        public WorldTimeEra CurrentEra
        {
            get
            {
                if (ProfileAsset != null)
                    if (ProfileAsset.Profile != null)
                        return ProfileAsset.Profile.GetEra(GetCurrentPoint());
                return null;
            }
        }
        [HideInInspector]
        public int CurrentYear;

        [HideInInspector]
        public WorldTimeMonth CurrentMonth
        {
            get
            {
                if (ProfileAsset == null)
                    return null;
                return ProfileAsset.Profile.GetMonth(CurrentMonthIndex);
            }
        }        
        [HideInInspector]
        public int CurrentMonthIndex;

        [HideInInspector]
        public int CurrentWeek;

        [HideInInspector]
        public WorldTimeDay CurrentDay => GetDayOfMonth(CurrentDayIndex);
        public WorldTimeDay GetDayOfMonth(int index)
        {
            if (ProfileAsset == null)
                return null;
            return ProfileAsset.Profile.GetDayPrecise(CurrentMonthIndex, index, CurrentYear);
        }
        [HideInInspector]
        public int CurrentDayIndex;
        public int DaysInTheWeek
        {
            get
            {
                return ProfileAsset.Profile.DaysInWeek.Count;
            }
        }
        public int DayOfTheWeekIndex
        {
            get => GetDayOfTheWeek(CurrentDayIndex);
        }
        public int GetDayOfTheWeekPrecise(int day)
        {
            return ProfileAsset.Profile.GetDayOfTheWeekPrecise(CurrentMonthIndex, day , CurrentYear);
        }
        public int GetDayOfTheWeek(int day)
        {
            if (ProfileAsset == null)
                return -1;

            return CurrentDayIndex % ProfileAsset.Profile.DaysInWeek.Count;
        }

        [HideInInspector]
        public WorldTimeOfDay TimeOfDay
        {
            get
            {
                if (ProfileAsset == null)
                    return null;

                if (ProfileAsset.Profile.TimesOfDay != null && ProfileAsset.Profile.TimesOfDay.Count > 0)
                {
                    int timeOfDay = (TimeOfDayIndex) % ProfileAsset.Profile.TimesOfDay.Count;
                    if (timeOfDay >= 0 && timeOfDay <= ProfileAsset.Profile.TimesOfDay.Count)
                        return ProfileAsset.Profile.TimesOfDay[TimeOfDayIndex];
                }
                return null;
            }
        }
        [HideInInspector]
        public int TimeOfDayIndex;
        public float TimeOfDayRatio => (float)TimeOfDayIndex/(float)ProfileAsset.Profile.TimesOfDay.Count;
        public float PreciseTimeOfDayRatio
        {
            get
            {
                if (ProfileAsset == null)
                    return 0;
                float totalTime = TotalSecondsInSegments * ProfileAsset.Profile.TimesOfDay.Count;
                float currentTime = TimeOfDayIndex * TotalSecondsInSegments;
                currentTime += SecondsInSegment;
                return currentTime / totalTime;
            }
        }

        [HideInInspector]
        public float TotalSecondsInSegments = 60;
        [HideInInspector]
        public float SecondsInSegment
        {
            get => _SecondsInSegment;
            set
            {
                if (_SecondsInSegment != value)
                {
                    _SecondsInSegment = value;
                    OnPreciseTimeOfDayUpdated?.Invoke();
                }
            }
        }
        float _SecondsInSegment;
        [HideInInspector]
        public float TimeSpeedModifier = 1;
        [HideInInspector]
        public bool TimeIsActive;
        [HideInInspector]
        public float TimeSegmentRatio => SecondsInSegment / TotalSecondsInSegments;

        public Action OnChange;
        public Action OnTimeOfDayUpdated;
        public Action OnPreciseTimeOfDayUpdated;
        public Action OnDayUpdated;
        public Action OnMonthUpdated;
        public Action OnYearUpdated;
        #endregion

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
            OnTimeOfDayUpdated += OnChange;
            OnPreciseTimeOfDayUpdated += OnChange;
            OnDayUpdated += OnChange;
            OnMonthUpdated += OnChange;
            OnYearUpdated += OnChange;
        }

        private void Update()
        {
            if (TimeIsActive && TotalSecondsInSegments > 0)
                HandleActiveTime();
        }

        void HandleActiveTime()
        {
            SecondsInSegment += Time.deltaTime * TimeSpeedModifier;
            if (SecondsInSegment > TotalSecondsInSegments)
            { 
                AdvanceTimeOfDay();
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void SetWorldTime(WorldTimePoint timePoint)
        {
            TimeOfDayIndex = timePoint.Time;
            CurrentDayIndex = timePoint.Day;
            CurrentMonthIndex = timePoint.Month;
            CurrentYear = timePoint.Year;
            OnTimeOfDayUpdated?.Invoke();
            OnDayUpdated?.Invoke();
            OnMonthUpdated?.Invoke();
            OnYearUpdated?.Invoke();
        }

        #region Day Time Adjustment
        public void AdvanceTimeOfDay()
        {
            if (ProfileAsset != null)
            {
                if (ProfileAsset.Profile.TimesOfDay != null && ProfileAsset.Profile.TimesOfDay.Count > 0)
                {
                    TimeOfDayIndex++;

                    if (TimeOfDayIndex >= ProfileAsset.Profile.TimesOfDay.Count)
                    {
                        TimeOfDayIndex = 0;
                        AdvanceDay();
                    }
                }
            }
            OnTimeOfDayUpdated?.Invoke();
            SecondsInSegment = 0;
        }
        public void DecreaseTimeOfDay()
        {
            if (ProfileAsset != null)
            {
                TimeOfDayIndex--;
                if (TimeOfDayIndex < 0)
                {
                    TimeOfDayIndex = ProfileAsset.Profile.TimesOfDay.Count - 1;
                    DecreaseDay();
                }
                OnTimeOfDayUpdated?.Invoke();
            }
        }
        public void AdjustTimeOfDay(int adjustmentAmount)
        {
            for (int i = 0; i < Mathf.Abs(adjustmentAmount); i++)
            {
                if (adjustmentAmount > 0)
                    AdvanceTimeOfDay();
                else
                    DecreaseTimeOfDay();
            }
        }

        public void AdvanceDay()
        {
            if (ProfileAsset != null)
            {
                CurrentDayIndex ++;
                if (CurrentDayIndex >= CurrentMonth.DaysInMonth)
                {
                    CurrentDayIndex = 0;
                    AdvanceMonth();
                }
                OnDayUpdated?.Invoke();
            }
        }
        public void DecreaseDay()
        {
            if (ProfileAsset != null)
            {
                CurrentDayIndex--;
                if (CurrentDayIndex < 0)
                {
                    DecreaseMonth();
                    CurrentDayIndex = CurrentMonth.DaysInMonth - 1;
                }
                OnDayUpdated?.Invoke();
            }
        }
        public void AdjustDay(int adjustmentAmount)
        {
            for (int i = 0; i < Mathf.Abs(adjustmentAmount); i++)
            {
                if (adjustmentAmount > 0)
                    AdvanceDay();
                else
                    DecreaseDay();
            }
        }

        public void AdvanceMonth()
        {
            if (ProfileAsset != null)
            {
                CurrentMonthIndex++;
                if (CurrentMonthIndex >= ProfileAsset.Profile.MonthsInYear.Count)
                {
                    CurrentMonthIndex = 0;
                    AdvanceYear();
                }

                if (CurrentDayIndex >= CurrentMonth.DaysInMonth)
                    CurrentDayIndex = CurrentMonth.DaysInMonth - 1;
                
                OnMonthUpdated?.Invoke();
            }
            else
                Debug.LogError("World Time Asset was Null");
        }
        public void DecreaseMonth()
        {
            if (ProfileAsset != null)
            {
                CurrentMonthIndex--;
                if (CurrentMonthIndex < 0)
                {
                    CurrentMonthIndex = ProfileAsset.Profile.MonthsInYear.Count - 1;
                    DecreaseYear();
                }

                if (CurrentDayIndex >= CurrentMonth.DaysInMonth)
                    CurrentDayIndex = CurrentMonth.DaysInMonth - 1;

                OnMonthUpdated?.Invoke();                
            }
        }
        public void AdjustMonth(int adjustmentAmount)
        {
            for (int i = 0; i < Mathf.Abs(adjustmentAmount); i++)
            {
                if (adjustmentAmount > 0)
                    AdvanceMonth();
                else
                    DecreaseMonth();
            }
        }

        public void AdvanceYear()
        {
            CurrentYear++;
            OnYearUpdated?.Invoke();
        }
        public void DecreaseYear()
        {
            CurrentYear--;
            if (CurrentYear < 0)
                CurrentYear = 0;
            OnYearUpdated?.Invoke();
        }
        public void AdjustYear(int adjustment)
        {
            CurrentYear += adjustment;
        }

        public void AdvanceTime(WorldTimePoint point)
        {
            for (int i = 0; i < point.Time; i++)
            {
                AdvanceTimeOfDay();
            }
            for (int i = 0; i < point.Day; i++)
            {
                AdvanceDay();
            }
            for (int i = 0; i < point.Month; i++)
            {
                AdvanceMonth();
            }
            for (int i = 0; i < point.Year; i++)
            {
                AdvanceYear();
            }
        }
        public void DecreaseTime(WorldTimePoint point)
        {
            for (int i = 0; i < point.Time; i++)
            {
                DecreaseTimeOfDay();
            }
            for (int i = 0; i < point.Day; i++)
            {
                DecreaseDay();
            }
            for (int i = 0; i < point.Month; i++)
            {
                DecreaseMonth();
            }
            for (int i = 0; i < point.Year; i++)
            {
                DecreaseYear();
            }
        }
        #endregion

        public bool DateHasPassed(WorldTimePoint point)
        {
            return point.IsAfter(GetCurrentPoint());
        }

        public WorldTimePoint GetCurrentPoint()
        {
            WorldTimePoint timePoint = new WorldTimePoint(CurrentYear, CurrentMonthIndex, CurrentDayIndex, TimeOfDayIndex);
            return timePoint;
        }
        public WorldTimePoint GetPointInFuture(WorldTimePoint period)
        {
            return GetPointInFuture(period.Year, period.Month, period.Day, period.Time);
        }
        public WorldTimePoint GetPointInFuture(int years, int months, int days, int daySegments)
        {
            WorldTime returnTime = new WorldTime();
            returnTime.ProfileAsset = ProfileAsset;
            returnTime.CurrentYear = CurrentYear;
            returnTime.CurrentMonthIndex = CurrentMonthIndex;
            returnTime.CurrentDayIndex = CurrentDayIndex;
            returnTime.TimeOfDayIndex = TimeOfDayIndex;

            returnTime.AdvanceTime(new WorldTimePoint(years,months,days,daySegments));

            return returnTime.GetCurrentPoint();
        }

        public static WorldTimePoint GetRandomTimePoints(bool multiYear, bool multiMonth)
        {
            int years = 0;
            int months = 0;
            int days = 0;
            int daySegments;

            if (multiYear)
                years = Random.Range(1,10);
            if (multiMonth)
                months = Random.Range(1,10);

            days = Random.Range(0,10);
            daySegments = Random.Range(0,10);

            return new WorldTimePoint(years, months, days, daySegments);
        }

        public WorldTimePoint TestReferencePoint;

        public string GetDateString()
        {
            string returnString = "";

            if (TimeOfDay != null)
            {
                returnString += TimeOfDay.TimeOfDay + ", ";
            }
            if (CurrentDay != null)
            {
                returnString += CurrentDay.DayName + ", ";// + CurrentDayIndex + ", ";
            }
            if (CurrentMonth != null)
            {
                returnString += CurrentMonth.MonthName + ", ";
            }
            returnString += CurrentYear;

            return returnString;
        }
        public string GetYearEraString(int year)
        {
            string returnValue = "";

            if (ProfileAsset == null)
                return returnValue;
            if (ProfileAsset.Profile == null)
                return returnValue;
            returnValue += (ProfileAsset.Profile.GetYearOfEra(year) + 1).ToString() + " - ";
            WorldTimeEra era = ProfileAsset.Profile.GetEra(new WorldTimePoint(year,0,0,0));
            if (era != null)
                returnValue += era.Suffix;
            else
                Debug.LogError("Era not found");

            return returnValue;
        }
    }    
}