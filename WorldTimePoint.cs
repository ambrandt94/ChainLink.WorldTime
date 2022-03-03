using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainWorldTime
{
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
            WorldTimeDay day = profile.GetDayOfWeek(Day);
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