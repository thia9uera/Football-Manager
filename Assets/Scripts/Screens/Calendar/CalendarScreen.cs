using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CalendarScreen : BaseScreen
{
	[SerializeField] private TMP_Text monthLabel; 
	[SerializeField] private Button navNext; 
	[SerializeField] private Button navPrev; 
	[SerializeField] private Transform calendarDaysHolder; 
	[SerializeField] private CalendarDay calendarDaysTemplate; 
	
	private List<CalendarDay> calendarDayList;
	private CalendarController calendarController;
	
	private int monthShowing;
	
	private void Start()
	{
						
	}
	
	override public void Show()
	{
		base.Show();	
		calendarController = CalendarController.Instance;
		monthShowing = calendarController.CurrentDate.Month;
		UpdateCalendar();		
		UpdateNavigation();
	}
	
	private void UpdateCalendar()
	{
		if(calendarDayList == null) InitializeList();		
		
		int startDay = 28 * (monthShowing-1);
		int endDay = startDay + 28;
		int monthDay = 0;
		MatchDay matchDay;
		
		for(int i = startDay; i < endDay; i ++)
		{
			matchDay = calendarController.Days[i];
			calendarDayList[monthDay].Populate(matchDay);
			monthDay++;
		}
		
		monthLabel.text = CalendarController.Instance.GetMonthString(monthShowing) + " " + CalendarController.Instance.CurrentYear;
	}
	
	private void InitializeList()
	{
		calendarDayList = new List<CalendarDay>();
		CalendarDay day;
		for (int i = 0; i < 28; i++)
		{
			day = Instantiate(calendarDaysTemplate, calendarDaysHolder);
			calendarDayList.Add(day);
		}
	}
	
	public void NavigateCalendar(int _value)
	{
		monthShowing += _value;
		UpdateNavigation();
		UpdateCalendar();
	}
	
	private void UpdateNavigation()
	{
		navPrev.interactable = monthShowing > 1;
		navNext.interactable = monthShowing < 12;
	}
	
}
