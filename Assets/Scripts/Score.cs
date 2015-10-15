using UnityEngine;
using System.Collections;

using com.shephertz.app42.paas.sdk.csharp;    
using com.shephertz.app42.paas.sdk.csharp.game;
using System; 

public class Score : MonoBehaviour { 

	public bool loadScoreOnLoad = false;

	public GameObject prefabItem;
	public GameObject areaScore;
	public GameObject preloader;
	String gameName = "Uberland";

	GameService gameService;
	ScoreBoardService scoreBoardService;

	// Use this for initialization
	void Start () {

		//App42Log.SetDebug(true);
	
		App42API.Initialize("ca4db85228af0c40bbb7ae3ff63c9e2060d4394c1b9ea7ca6202c9691347d3b5","aa6f7deb1366c230429a849f91371d15d03a75b307653a147b2475c76d9bf955");  
		
		gameService = App42API.BuildGameService();   
		scoreBoardService = App42API.BuildScoreBoardService();  

		if (loadScoreOnLoad)
			loadScore (true);

	}

	public void SaveScore(double Score){
		if (PlayerPrefs.GetInt ("fbLogged") == 1) {
			string userName = PlayerPrefs.GetString ("fbId") + "|" + PlayerPrefs.GetString ("fbName");
			scoreBoardService.SaveUserScore (gameName, userName, Score, new SaveScoreCallback ());   
		}
	}

	bool isScoreLoaded = false;
	public void loadScore(bool forceReload){

		if (!isScoreLoaded || forceReload) {

			preloader.SetActive(true);

			foreach (Transform child in areaScore.transform) {
				Destroy (child.gameObject);
			}
		
			scoreBoardService.GetTopNRankers (gameName, 10, new GetScoreCallback ());   

			isScoreLoaded = true;
		}
	}

	GameObject o;
	public void addItem(int i, string name, double score){

		string[] d = name.Split ('|');

		o = Instantiate (prefabItem) as GameObject;
		o.transform.SetParent (areaScore.transform, false);
		o.GetComponent<ScoreItem> ().initObj (i, d[1], d[0], score.ToString ());

		preloader.SetActive(false);

		areaScore.GetComponent<RectTransform> ().sizeDelta = new Vector2 (areaScore.GetComponent<RectTransform> ().sizeDelta.x, (i+1) * 110);

	}

}


public class SaveScoreCallback : App42CallBack  
{  
	public void OnSuccess(object response)  
	{  
		Game game = (Game) response;       
		App42Log.Console("gameName is " + game.GetName());   
		for(int i = 0;i<game.GetScoreList().Count;i++)  
		{  
			App42Log.Console("userName is : " + game.GetScoreList()[i].GetUserName());  
			App42Log.Console("score is : " + game.GetScoreList()[i].GetValue());  
			App42Log.Console("scoreId is : " + game.GetScoreList()[i].GetScoreId());  
		}  
	}  
	public void OnException(Exception e)  
	{  
		App42Log.Console("Exception : " + e);  
	}  
}  


public class GetScoreCallback : MonoBehaviour, App42CallBack  
{  

	public void OnSuccess(object response)  
	{  

		Score s = GameObject.Find("Score").GetComponent<Score>();
		Game game = (Game) response;       
		//Debug.Log("gameName is " + game.GetName());   
		for(int i = 0;i<game.GetScoreList().Count;i++)  
		{  

			s.addItem(i, game.GetScoreList()[i].GetUserName(), game.GetScoreList()[i].GetValue());

			/*Debug.Log("userName is : " + game.GetScoreList()[i].GetUserName());  
			Debug.Log("score is : " + game.GetScoreList()[i].GetValue());  
			Debug.Log("scoreId is : " + game.GetScoreList()[i].GetScoreId());*/  
		}  
	}  

	public void OnException(Exception e)  
	{  
		Debug.Log("Exception : " + e);  
	}  
}  

