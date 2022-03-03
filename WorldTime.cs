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
                return ProfileAsset.Profile.GetDay(CurrentMonthIndex, CurrentDayIndex, CurrentYear);
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
            if (CurrentYear > point.Year)
                return true;

            if (CurrentYear == point.Year)
            {
                if (CurrentMonthIndex > point.Month)
                    return true;

                if (CurrentMonthIndex == point.Month)
                {
                    if (CurrentDayIndex > point.Day)
                        return true;

                    if (CurrentDayIndex == point.Day)
                    {
                        if (TimeOfDayIndex >= point.Time)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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

    [Serializable]
    public class WorldTimeProfile
    {
        public List<WorldTimeEra> TimelineEras;

        public List<WorldTimeMonth> MonthsInYear;
        public WorldTimeMonth GetMonth(int index)
        {
            if (MonthsInYear != null)
            {
                if (MonthsInYear.Count > 0 && index >= 0 && index < MonthsInYear.Count)
                    return MonthsInYear[index];
            }

            return null;
        }

        public List<WorldTimeDay> DaysInWeek;
        public WorldTimeDay GetDay(int index)
        {
            if (DaysInWeek != null)
            {
                if (DaysInWeek.Count > 0 && index >= 0 && index < DaysInWeek.Count)
                    return DaysInWeek[index];
            }

            return null;
        }

        public List<WorldTimeOfDay> TimesOfDay;
        public WorldTimeOfDay GetTimeOfDay(int index)
        {
            if (TimesOfDay != null)
            {
                if (TimesOfDay.Count > 0 && index >= 0 && index < TimesOfDay.Count)
                    return TimesOfDay[index];
            }

            return null;
        }

        public int DaysInYear
        {
            get
            {
                int total = 0;
                foreach (WorldTimeMonth month in MonthsInYear)
                {
                    total += month.DaysInMonth;
                }
                return total;
            }
        }
        public WorldTimeDay GetDay(int month, int day, int year)
        {
            int totalDays = 0;
            if (year > 0)
                totalDays += year * DaysInYear;
            for (int i = 0; i < month; i++)
            {
                WorldTimeMonth timeMonth = GetMonth(i);
                if (timeMonth != null)
                { 
                    totalDays += timeMonth.DaysInMonth;
                }
            }
            totalDays += (day);
            return GetDay(totalDays % DaysInWeek.Count);
        }
        public WorldTimeEra GetEra(int index)
        {
            if (TimelineEras == null)
                return null;
            if (index < 0 || index >= TimelineEras.Count)
                return null;
            return TimelineEras[index];
        }
        public WorldTimeEra GetEra(WorldTimePoint point)
        {
            if (TimelineEras == null)
                return null;
            if (TimelineEras.Count <= 0)
                return null;

            WorldTimeEra returnEra = TimelineEras[0];
            foreach (WorldTimeEra era in TimelineEras)
            {
                if (point.IsAfter(era.StartPoint))
                    returnEra = era;
            }
            return returnEra;
        }
        public int GetYearOfEra(int totalYear)
        {
            if (TimelineEras == null)
                return -1;

            int yearsPassed = 0;

            for (int i = 0; i < TimelineEras.Count; i++)
            {
                WorldTimeEra currentEra = GetEra(i);
                WorldTimeEra nextEra = GetEra(i+1);
                if (nextEra == null)
                { 
                    //yearsPassed = currentEra
                    return totalYear - yearsPassed;
                }
                if (nextEra.StartPoint.Year <= totalYear)
                    yearsPassed = nextEra.StartPoint.Year;
                else
                    return totalYear - yearsPassed;
            }

            return -1;
        }
    }

    [Serializable]
    public class WorldTimeEra
    {
        public string Name;
        public WorldTimePoint StartPoint;
    }

    [Serializable]
    public class WorldTimeMonth
    {
        public string MonthName;
        public int DaysInMonth;
    }

    [Serializable]
    public class WorldTimeDay
    {
        public string DayName;
    }

    [Serializable]
    public class WorldTimeOfDay
    {
        public string TimeOfDay;
    }

    [Serializable]
    public class WorldTimePoint
    {
        public int Year;
        public int Month;
        public int Day;
        public int Time;

        public WorldTimePoint()
        { }

        public WorldTimePoint(int year, int month, int day, int dayTime)
        {
            Year = year;
            Month = month;
            Day = day;
            Time = dayTime;
        }

        public string GetDateString(WorldTimeProfile profile)
        {
            string returnString = "";

            WorldTimeOfDay timeOfDay = profile.GetTimeOfDay(Time);
            if (timeOfDay != null)
            {
                returnString += timeOfDay.TimeOfDay + ", ";
            }
            WorldTimeDay day = profile.GetDay(Day);
            if (day != null)
            {
                returnString += day.DayName + " " + Day + ", ";
            }
            WorldTimeMonth month = profile.GetMonth(Month);
            if (month != null)
            {
                returnString += month.MonthName + ", ";
            }
            returnString += Year;

            return returnString;
        }

        public bool IsAfter(WorldTimePoint point)
        {
            //Debug.Log("Years: " + Year + " vs " + point.Year);
            //Debug.Log("Month: " + Month + " vs " + point.Month);
            //Debug.Log("Day: " + Day + " vs " + point.Day);
            //Debug.Log("Time: " + Time + " vs " + point.Time);
            if (Year < point.Year)
                return false;
            if (Year == point.Year)
            {
                if (Month < point.Month)
                    return false;

                if (Month == point.Month)
                {
                    if (Day < point.Day)
                        return false;

                    if (Day == point.Day)
                    {
                        if (Time < point.Time)
                            return false;
                    }
                }
            }
            return true;
        }

        public void Print()
        {
            Debug.Log(
                "Year: " + Year + 
                "\nMonth: " + Month +
                "\nDay: " + Day +
                "\nTime of Day: " + Time
                );
        }

    }
}