using System;
using System.Collections.Generic;
using UnityEngine;

public class CalendarController : MonoBehaviour
{   
    [SerializeField]
    public List<DateTime> Days;

    public int CurrentDay;
    public int CurrentYear;

    void Start()
    {
        int y = int.Parse(DateTime.Now.ToString("yy"));
        InitializeCalendar(y);
    }

    public void InitializeCalendar(int _year)
    {
        CurrentDay = 128;
        CurrentYear = _year;
        Days = new List<DateTime>();
        DateTime date;

        int month = 1;
        int day = 1;
        for (int i = 0; i < 336; ++i)
        {
            date = new DateTime(_year, month, day);
            Days.Add(date);
            if(day == 28)
            {
                day = 0;
                month++;
            }
            day++;
        }
    }

    public string GetCurrentDate(string _format)
    {
        return Days[CurrentDay].ToString(_format);
    }

    public string GetCurrentMonth()
    {
        return Days[CurrentDay].ToString("MMMM");
    }

    public string GetCurrentDay()
    {
        return Days[CurrentYear].ToString("dd");
    }
}
