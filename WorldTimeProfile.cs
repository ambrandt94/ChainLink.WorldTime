using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainLink.WorldTime
{
    [Serializable]
    public class WorldTimeProfile
    {
        public List<WorldTimeEra> TimelineEras;
        public List<WorldTimeMonth> MonthsInYear;
        public List<WorldTimeDay> DaysInWeek;
        public List<WorldTimeOfDay> TimesOfDay;

        public int DaysInYear {
            get {
                int total = 0;
                foreach (WorldTimeMonth month in MonthsInYear) {
                    total += month.DaysInMonth;
                }
                return total;
            }
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
            foreach (WorldTimeEra era in TimelineEras) {
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

            for (int i = 0; i < TimelineEras.Count; i++) {
                WorldTimeEra currentEra = GetEra(i);
                WorldTimeEra nextEra = GetEra(i + 1);
                if (nextEra == null) {
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
        public WorldTimeMonth GetMonth(int index)
        {
            if (MonthsInYear != null) {
                if (MonthsInYear.Count > 0 && index >= 0 && index < MonthsInYear.Count)
                    return MonthsInYear[index];
            }

            return null;
        }
        public WorldTimeDay GetDayOfWeek(int index)
        {
            if (DaysInWeek != null) {
                if (DaysInWeek.Count > 0 && index >= 0 && index < DaysInWeek.Count)
                    return DaysInWeek[index];
            }

            return null;
        }
        public WorldTimeDay GetDayPrecise(int month, int day, int year)
        {
            int totalDays = 0;
            if (year > 0)
                totalDays += year * DaysInYear;
            for (int i = 0; i < month; i++) {
                WorldTimeMonth timeMonth = GetMonth(i);
                if (timeMonth != null) {
                    totalDays += timeMonth.DaysInMonth;
                }
            }
            totalDays += (day);
            return GetDayOfWeek(totalDays % DaysInWeek.Count);
        }
        public int GetDayOfTheWeekPrecise(int month, int day, int year)
        {
            int totalDays = 0;
            if (year > 0)
                totalDays += year * DaysInYear;
            for (int i = 0; i < month; i++) {
                WorldTimeMonth timeMonth = GetMonth(i);
                if (timeMonth != null) {
                    totalDays += timeMonth.DaysInMonth;
                }
            }
            totalDays += (day);
            return totalDays % DaysInWeek.Count;
        }
        public WorldTimeOfDay GetTimeOfDay(int index)
        {
            if (TimesOfDay != null) {
                if (TimesOfDay.Count > 0 && index >= 0 && index < TimesOfDay.Count)
                    return TimesOfDay[index];
            }

            return null;
        }
    }

    [Serializable]
    public class WorldTimeEra
    {
        public string Name;
        public string Suffix;
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
        public Color StartingLightColor = Color.white;
        public Color EndingLightColor = Color.white;
        public float PrimaryLightIntensity = .5f;
    }
}