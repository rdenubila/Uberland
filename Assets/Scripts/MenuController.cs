using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using System;
using UnityEngine.Advertisements;

public class MenuController : FacebookCallbacks {

	public float rotMax = 10f;
	Quaternion iniRot;

	public Toggle audioToggle;

	public AsyncOperation loadOp;
	public RectTransform preloadBar;
	public GameObject preloaderScreen;

	public Text moneyLabel;

	public TranslateController Language;
	public Dropdown LanguageSelector;

	DateTime earnMoneyDate;

	void Start(){

		System.GC.Collect ();
		Resources.UnloadUnusedAssets ();

		FB.Init(this.OnInitComplete, this.OnHideUnity);


		preloaderScreen.SetActive (false);

		if (!PlayerPrefs.HasKey ("audioEnabled")) {
			PlayerPrefs.SetInt ("audioEnabled", 1);
		}

		if (PlayerPrefs.GetInt ("audioEnabled") == 0) {
			AudioListener.pause = true;
			audioToggle.isOn = true;
		}

		iniRot = transform.rotation;

		updateMoney ();

		if (!PlayerPrefs.HasKey ("idioma")) {
			if(Application.systemLanguage == SystemLanguage.Portuguese){
				PlayerPrefs.SetInt ("idioma", 1);
			} else {
				PlayerPrefs.SetInt ("idioma", 0);
			}
		}
		LanguageSelector.value = PlayerPrefs.GetInt ("idioma");


		if (!PlayerPrefs.HasKey ("controle")) {
			PlayerPrefs.SetInt ("controle", 0);
		}
		iniControleRadio ();

		if (!PlayerPrefs.HasKey ("earnMoneyDate")) {
			PlayerPrefs.SetString ("earnMoneyDate", DateTime.Now.ToString() );
		}

		earnMoneyDate = DateTime.Parse (PlayerPrefs.GetString ("earnMoneyDate"));
		checkEarnMoneyBtn ();

	}

	public GameObject EarnMoneyBtn;
	void checkEarnMoneyBtn(){
		if (DateTime.Compare (earnMoneyDate, DateTime.Now) > 0) {
			EarnMoneyBtn.SetActive(false);
		}
	}

	public void EarnMoneyAction(){

		if (Advertisement.IsReady ("rewardedVideo")) { 
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			print ("addNotReady");
		}

	}

	private void HandleShowResult(ShowResult result){
		switch (result)	{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			PlayerPrefs.SetInt("total_money", PlayerPrefs.GetInt("total_money")+100);
			EarnMoneyBtn.SetActive(false);

			PlayerPrefs.SetString ("earnMoneyDate", DateTime.Now.AddHours( UnityEngine.Random.Range(5.0f, 24.0f)).ToString() );

			updateMoney();
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	public void updateMoney(){
		moneyLabel.text = "$ " + PlayerPrefs.GetInt ("total_money");
	}

	public void changeScene(string nextScene){
		//Application.LoadLevel(nextScene);
		preloaderScreen.SetActive (true);
		loadOp = Application.LoadLevelAsync(nextScene); 
		StartCoroutine (ScenePreload ());
	}
	
	IEnumerator ScenePreload () {
		while (true) {
			print (loadOp.progress);
			preloadBar.localScale = new Vector2(loadOp.progress, 1f);
			yield return null;
		}
	}

	Quaternion dest;
	int iPrint = 0;
	void Update(){

		dest = Quaternion.Euler( iniRot.eulerAngles.x+Input.acceleration.y*rotMax, iniRot.eulerAngles.y+Input.acceleration.x*rotMax, iniRot.eulerAngles.z);
		transform.rotation = Quaternion.Lerp (transform.rotation, dest, .2f);

		if (Input.GetKeyDown (KeyCode.A)) {
			print ("PRINTSCREEN");
			Application.CaptureScreenshot ("Screenshot"+iPrint+".png", 4);
			iPrint++;
		}
	}

	public void ToggleAudio(){
		PlayerPrefs.SetInt ("audioEnabled", audioToggle.isOn ? 0 : 1 );

		if (PlayerPrefs.GetInt ("audioEnabled") == 0) {
			AudioListener.pause = true;
		}

		if (PlayerPrefs.GetInt ("audioEnabled") == 1) {
			AudioListener.pause = false;
		}
	}

	public void changeLanguage(Dropdown drop){
		PlayerPrefs.SetInt ("idioma", drop.value);
		Language.updateLanguage ();
	}

	public void openLink(string url){
		Application.OpenURL(url);
	}


	public void exitGame(){
		Application.Quit();
	}

	public Toggle[] controleRadio;
	void iniControleRadio(){
		controleRadio [PlayerPrefs.GetInt ("controle")].isOn = true;
	}

	public void changeControleRadio(int i){
		PlayerPrefs.SetInt ("controle", i);
	}


}

