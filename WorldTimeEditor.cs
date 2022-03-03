﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.PackageManager.UI;
using UnityEditor;
#endif

namespace ChainWorldTime
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WorldTime))]
    public class WorldTimeEditor : Editor
    {
        WorldTime Target { get { return target as WorldTime; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawTime();
        }

        void DrawTime()
        {
            if (Target != null)
            {
                //Target.ProfileAsset = (WorldTimeAsset)(EditorGUILayout.ObjectField((Object)Target.ProfileAsset, typeof(Object), true));
                WorldTimeEra era = Target.CurrentEra;
                if(era != null)
                    EditorGUILayout.LabelField("Current Era: " + era.Name + " | Year " + (Target.ProfileAsset.Profile.GetYearOfEra(Target.CurrentYear) + 1).ToString());
                else
                    EditorGUILayout.LabelField("Current Era: None");

                EditorGUILayout.LabelField("Current Year: " + (Target.CurrentYear + 1).ToString());

                WorldTimeMonth month = Target.CurrentMonth;
                string monthString = "Current Month: " + (Target.CurrentMonthIndex + 1).ToString();
                if (month != null)
                    monthString += "[" + month.MonthName + "]";
                EditorGUILayout.LabelField(monthString);

                EditorGUILayout.LabelField("Current Week: " + (Target.CurrentWeek + 1).ToString());

                WorldTimeDay day = Target.CurrentDay;
                string dayString = "Current Day: " + (Target.CurrentDayIndex + 1).ToString();
                if (day != null)
                    dayString += "[" + day.DayName + "]";
                EditorGUILayout.LabelField(dayString);

                WorldTimeOfDay timeOfDay = Target.TimeOfDay;
                string timeString = "Time: " + (Target.TimeOfDayIndex+1).ToString();
                if (timeOfDay != null)
                    timeString += "[" + timeOfDay.TimeOfDay + "]";

                EditorGUILayout.LabelField(timeString);

                DrawModificationButtons();
            }
        }

        void DrawModificationButtons()
        {
            if (GUILayout.Button("Set to Test Point"))
            {
                Target.CurrentYear = Target.TestReferencePoint.Year;
                Target.CurrentMonthIndex = Target.TestReferencePoint.Month;
                Target.CurrentDayIndex = Target.TestReferencePoint.Day;
                Target.TimeOfDayIndex = Target.TestReferencePoint.Time;
            }
            //Beginnig to test for passage of time
            //if (GUILayout.Button("Test Random"))
            //{
            //    WorldTimePoint timePassage = WorldTime.GetRandomTimePoints(false, false);
            //    timePassage.Print();

            //    WorldTimePoint newPoint = Target.GetPointInFuture(timePassage.Year, timePassage.Month, timePassage.Day, timePassage.Time);
            //    newPoint.Print();
            //}
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Time-"))
            {
                Target.DecreaseTimeOfDay();
            }
            if (GUILayout.Button("Time+"))
            {
                Target.AdvanceTimeOfDay();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Day-"))
            {
                Target.DecreaseDay();
            }
            if (GUILayout.Button("Day+"))
            {
                Target.AdvanceDay();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Month-"))
            {
                Target.DecreaseMonth();
            }
            if (GUILayout.Button("Month+"))
            {
                Target.AdvanceMonth();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Year-"))
            {
                Target.DecreaseYear();
            }
            if (GUILayout.Button("Year+"))
            {
                Target.AdvanceYear();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}