using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChainWorldTime.Calendar
{
    public class CalendarMonthPage : MonoBehaviour
    {
        [SerializeField]
        WorldTime WorldTime;

        WorldTimeMonth CurrentMonth;
        [SerializeField]
        CalendarDayPanel DayPanelPrefab;

        [SerializeField]
        GameObject CalendarWeekPrefab;
        List<GameObject> SpawnedRows;

        [SerializeField]
        Transform WeekRowParent;

        [Header("UI Elements"), SerializeField]
        TextMeshProUGUI MonthNameText;
        [SerializeField]
        TextMeshProUGUI YearEraNameText;

        private void Awake()
        {
            WorldTime = WorldTime.Instance;
            WorldTime.OnMonthUpdated += delegate { SetToCurrentTime(); };
            WorldTime.OnYearUpdated += delegate { SetToCurrentTime(); };
            SetToCurrentTime();
        }

        [ContextMenu("Set to Current")]
        void SetToCurrentTime()
        {
            if (WorldTime != null)
            { 
                SetMonth(WorldTime.CurrentYear, WorldTime.Instance.CurrentMonth);
                SpawnCalendarInteractables();
            }
        }

        public void SetMonth(int year, WorldTimeMonth month)
        {
            CurrentMonth = month;
            MonthNameText?.SetText(month.MonthName);
            SetYear(year);
        }

        void ClearSpawnedRows()
        {
            if (SpawnedRows == null)
                SpawnedRows = new List<GameObject>();
            foreach (GameObject obj in SpawnedRows)
            {
                Destroy(obj);
            }
            SpawnedRows = new List<GameObject>();
        }

        void SpawnCalendarInteractables()
        {
            ClearSpawnedRows();
            if (CurrentMonth != null)
            {
                Transform weekTransform = null;
                int preBufferDays = WorldTime.Instance.GetDayOfTheWeekPrecise(0);
                int postBufferDays = (WorldTime.Instance.DaysInTheWeek-1) - WorldTime.Instance.GetDayOfTheWeekPrecise(CurrentMonth.DaysInMonth-1);

                if (preBufferDays != 0)
                { 
                    weekTransform = Instantiate(CalendarWeekPrefab, WeekRowParent).transform;
                    SpawnedRows.Add(weekTransform.gameObject);
                }
                
                for (int i = 0; i < preBufferDays; i++)
                {
                    SpawnBufferDay(weekTransform);
                }
                for (int i = 0; i < CurrentMonth.DaysInMonth; i++)
                {
                    int remainder = (i + preBufferDays) % WorldTime.Instance.DaysInTheWeek;
                    if (remainder == 0)
                    {
                        weekTransform = Instantiate(CalendarWeekPrefab, WeekRowParent).transform;
                        SpawnedRows.Add(weekTransform.gameObject);
                    }
                    SpawnCalendarDay(weekTransform, i);
                }
                for (int i = 0; i < postBufferDays; i++)
                {
                    SpawnBufferDay(weekTransform);
                }
            }
        }
        void SpawnBufferDay(Transform parent)
        {
            if (parent == null)
            {
                Debug.LogError("Row was Null");
                return;
            }
            GameObject bufferDay = new GameObject("BUFFER");
            bufferDay.transform.SetParent(parent);
            bufferDay.AddComponent<RectTransform>();
        }
        void SpawnCalendarDay(Transform rowParent, int day)
        {
            if (rowParent == null)
            { 
                Debug.LogError("Row was Null");
                return;
            }

            CalendarDayPanel dayPanel = Instantiate(DayPanelPrefab.gameObject, rowParent).GetComponent<CalendarDayPanel>();
            if (dayPanel != null)
                dayPanel.SetToDay(day, WorldTime.Instance.GetDayOfMonth(day));
        }

        public void SetYear(int year)
        {
            YearEraNameText?.SetText(WorldTime.GetYearEraString(year));
        }
    }
}