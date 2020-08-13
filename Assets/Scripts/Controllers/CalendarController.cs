using System;
using System.Collections.Generic;
using UnityEngine;

public class CalendarController : MonoBehaviour
{   
	public static CalendarController Instance;
	
	public List<MatchDay> Days;

    public int CurrentDay;
    public int CurrentYear;

	private void Awake()
	{
		if(Instance == null) Instance = this;
	}

	private void Start()
    {
	    //int y = int.Parse(DateTime.Now.ToString("yy"));
	    //InitializeCalendar(y);
    }

	public void InitializeCalendar(int _year)
	{
	    CurrentDay = 0;
        CurrentYear = _year;
	    Days = new List<MatchDay>();
	    
	    DateTime date;
	    MatchDay matchDay;
	    
        int month = 1;
	    int day = 1;
	    
        for (int i = 0; i < 336; ++i)
        {
	        date = new DateTime(_year, month, day);
	        matchDay = new MatchDay(date);
            Days.Add(matchDay);
            if(day == 28)
            {
                day = 0;
                month++;
            }
	        day++;
        }
	}
    
	public string GetDay(int _day)
	{
		return Days[_day].Date.ToString("dd MMM yyyy");
	}

    public string GetCurrentDate(string _format)
    {
	    return Days[CurrentDay].Date.ToString(_format);
    }

    public string GetCurrentMonth()
    {
	    return Days[CurrentDay].Date.ToString("MMMM");
    }

    public string GetCurrentDay()
    {
	    return Days[CurrentYear].Date.ToString("dd");
    }
    
	public string GetWeekDay(int _day)
	{
		int value = _day % 7;
		
		switch(value)
		{
			case 0 : return "Monday";	break;
			case 1 : return "Tuesday";	break;
			case 2 : return "Wednesday";break;
			case 3 : return "Thrusday";	break;
			case 4 : return "Friday";	break;
			case 5 : return "Saturday"; break;
			case 6 : return "Sunday";	break;
		}
		
		return "DOOMSDAY";
	}
}

[System.Serializable]
public class MatchDay
{
	public DateTime Date;
	public MatchData Match;
	
	public MatchDay(DateTime _date, MatchData _match = null)
	{
		Date = _date;
		Match = _match;
	}
}

public enum WeekDay
{
	Monday,
	Tuesday,
	Wednesday,
	Thursday,
	Friday,
	Saturday,
	Sunday
}

public enum Month
{
	January,
	February,
	March,
	April,
	May,
	June, 
	July,
	August,
	Septembet,
	October,
	November,
	Decemember
}


