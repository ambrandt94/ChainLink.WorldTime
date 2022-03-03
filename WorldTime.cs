using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

namespace ChainWorldTime
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

                if (ProfileAsset.Profile.MonthsInYear != null && ProfileAsset.Profile.MonthsInYear.Count > 0)
                {
                    int monthOfYear = (CurrentMonthIndex) % ProfileAsset.Profile.MonthsInYear.Count;
                    if (monthOfYear >= 0 && monthOfYear <= ProfileAsset.Profile.MonthsInYear.Count)
                        return ProfileAsset.Profile.MonthsInYear[monthOfYear];
                }
                return null;
            }
        }
        [HideInInspector]
        public int CurrentMonthIndex;

        [HideInInspector]
        public int CurrentWeek;

        [HideInInspector]
        public WorldTimeDay CurrentDay
        {
            get
            {
                if (ProfileAsset == null)
                    return null;
                return ProfileAsset.Profile.GetDayPrecise(CurrentMonthIndex, CurrentDayIndex, CurrentYear);
                if (ProfileAsset.Profile.DaysInWeek != null && ProfileAsset.Profile.DaysInWeek.Count > 0)
                {
                    int dayOfWeek = (CurrentDayIndex) % ProfileAsset.Profile.DaysInWeek.Count;
                    if (dayOfWeek >= 0 && dayOfWeek <= ProfileAsset.Profile.DaysInWeek.Count)
                        return ProfileAsset.Profile.DaysInWeek[dayOfWeek];
                }
                return null;
            }
        }
        [HideInInspector]
        public int CurrentDayIndex;

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
        #endregion

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void SetWorldTime(WorldTimePoint timePoint)
        {
            TimeOfDayIndex = timePoint.Time;
            CurrentDayIndex = timePoint.Day;
            CurrentMonthIndex = timePoint.Month;
            CurrentYear = timePoint.Year;
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
                        AdvanceDay();
                        TimeOfDayIndex = 0;
                    }
                }
            }
        }
        public void DecreaseTimeOfDay()
        {
            if (ProfileAsset != null)
            {
                TimeOfDayIndex--;
                if (TimeOfDayIndex < 0)
                {
                    DecreaseDay();
                    TimeOfDayIndex = ProfileAsset.Profile.TimesOfDay.Count - 1;
                }
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
                    AdvanceMonth();
                    CurrentDayIndex = 0;
                }
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
                    AdvanceYear();
                    CurrentMonthIndex = 0;
                }

                if (CurrentMonth.DaysInMonth >= CurrentDayIndex)
                    CurrentDayIndex = CurrentMonth.DaysInMonth - 1;
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
                    DecreaseYear();
                    CurrentMonthIndex = ProfileAsset.Profile.MonthsInYear.Count - 1;
                }
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
        }
        public void DecreaseYear()
        {
            CurrentYear--;
            if (CurrentYear < 0)
                CurrentYear = 0;
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
                returnString += CurrentDay.DayName + " " + CurrentDayIndex + ", ";
            }
            if (CurrentMonth != null)
            {
                returnString += CurrentMonth.MonthName + ", ";
            }
            returnString += CurrentYear;

            return returnString;
        }
    }    
}