using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using TMPro;

public class StartScreen : BaseScreen
{
	private enum MenuType
	{
		Start,
		Load
	}
	
	[Space(10)]
	[SerializeField] private TextMeshProUGUI gameVersionLabel;
	
	[Space(10)]
	[SerializeField] private GameObject startMenu;
	[SerializeField] private ButtonDefault btnNewGame;
	[SerializeField] private ButtonDefault btnLoadGame;
	
	[Space(10)]
	[SerializeField] private GameObject loadMenu;
	[SerializeField] private LoadingFileButton savedGameBtnTemplate;
	[SerializeField] private Transform savedGameBtnContainer;

	private List<LoadingFileButton> savedGameBtnList;
	
	private UserData[] saves;
	private Sequence sequence;
	private float fadeInTime = 0.5f;
	private float fadeOutTime = 0.3f;
	private MenuType currentMenu = MenuType.Start;
	
	private void Reset()
	{
		startMenu.SetActive(false);
		btnNewGame.CanvasGroup.alpha = 0;
		btnLoadGame.CanvasGroup.alpha = 0;
		loadMenu.SetActive(false);

		gameVersionLabel.text = Application.version;
	}
	
	public override void Show()
	{
		base.Show();
		
		Reset();

		saves = DataController.Instance.GetSaveFiles();
		
		ShowStartMenu();
	}
	
	public void SwitchMenus()
	{
		if(currentMenu == MenuType.Start)
		{
			HideStartMenu();
			Invoke("ShowLoadMenu", 0.3f);
		}
		else
		{
			HideLoadMenu();
			Invoke("ShowStartMenu", 0.3f);
		}
	}
	
	private void ShowStartMenu()
	{
		currentMenu = MenuType.Start;
		btnNewGame.transform.DOMoveY(btnNewGame.transform.position.y - 40, fadeInTime).From();
		btnNewGame.CanvasGroup.DOFade(1, fadeInTime);
		btnLoadGame.transform.DOMoveY(btnNewGame.transform.position.y - 40, fadeInTime).From().SetDelay(0.1f);
		btnLoadGame.CanvasGroup.DOFade(1, fadeInTime).SetDelay(0.1f);
		
		btnLoadGame.Enabled = saves.Length > 0;
		
		startMenu.SetActive(true);
	}
	
	private void HideStartMenu()
	{
		btnNewGame.CanvasGroup.DOFade(0, fadeInTime);
		btnLoadGame.CanvasGroup.DOFade(0, fadeInTime);
	}
	
	private void ShowLoadMenu()
	{
		currentMenu = MenuType.Load;
		PopulateLoadFiles(saves);
		
		loadMenu.SetActive(true);
	}
	
	private void HideLoadMenu()
	{
		loadMenu.SetActive(false);
	}
	
	public void StartNewGame()
	{
		ScreenController.Instance.ShowScreen(ScreenType.CreateTeam);
	}
	
	private void PopulateLoadFiles(UserData[] _users)
	{
		if (savedGameBtnList != null) foreach (LoadingFileButton btn in savedGameBtnList) Destroy(btn.gameObject);
		savedGameBtnList = new List<LoadingFileButton>();


		foreach (UserData user in _users)
		{
			LoadingFileButton btn = Instantiate(savedGameBtnTemplate, savedGameBtnContainer);
			btn.Label = user.TeamName;
			btn.TimeStamp = user.LastTimeSaved;
			btn.gameObject.SetActive(true);
			btn.UserId = user.Id;
			savedGameBtnList.Add(btn);
		}
	}
	
	public void LoadFile(string _file)
	{
		ScreenController.Instance.ShowScreen(ScreenType.Loading);
		DataController.Instance.LoadGame(_file);
		Debug.Log("LOAD GAME - " + _file);
	}
	
	public void DeleteSaveFile(LoadingFileButton _fileButton)
	{
		DataController.Instance.DeleteSavedData(_fileButton.UserId);
		savedGameBtnList.Remove(_fileButton);
		Destroy(_fileButton.gameObject);
		
		saves = DataController.Instance.GetSaveFiles();
		
		if(savedGameBtnList.Count == 0) SwitchMenus();
		
	}
	
	public void BackButtonHandler()
	{
		SwitchMenus();
	}
}
