using UnityEngine;
using System.Collections;
using Facebook.Unity;
using Facebook.MiniJSON;
using System.Collections.Generic;
using UnityEngine.UI;


public class FacebookCallbacks : MonoBehaviour {

	public void loginFB(){
		if (!FB.IsLoggedIn) {
			FB.LogInWithReadPermissions (new List<string> () { "public_profile", "email", "user_friends" }, HandleResult);
		} else {
			getUserData();
		}
	}


	public void OnHideUnity(bool isGameShown)
	{
		print( "Success - Check logk for details");
		print( string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown));
		print("Is game shown: " + isGameShown);
	}


	public void OnInitComplete()
	{
		print( "Success - Check logk for details");
		print ("Success Response: OnInitComplete Called\n");
		print("OnInitComplete Called");

		if (PlayerPrefs.GetInt ("fbLogged") == 1) {
			updatePhoto();
		}
	}

	public void HandleResult(IResult result)
	{
		if (result == null)
		{
			print ( "Null Response\n");
			return;
		}
		
		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			print("Error Response:\n" + result.Error);
		}
		else if (result.Cancelled)
		{
			print("Cancelled Response:\n" + result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			print("Success Response:\n" + result.RawResult);
			getUserData();
		}
		else
		{
			print("Empty Response\n");
		}
	}

	public void getUserData(){
		FB.API("/me?fields=id,name", HttpMethod.GET, ReturnUserCallback);
	}

	public void ReturnUserCallback(IGraphResult result){

		var dict = Json.Deserialize(result.RawResult) as Dictionary<string,object>;
	
		PlayerPrefs.SetString ("fbId", dict ["id"].ToString() );
		PlayerPrefs.SetString ("fbName", dict ["name"].ToString() );
		PlayerPrefs.SetInt ("fbLogged", 1 );


		updatePhoto ();

	}

	public Image fbImageReplace;
	public GameObject fbRemove;
	public void updatePhoto(){

		if (fbImageReplace!=null && PlayerPrefs.GetInt ("fbLogged")==1 && PlayerPrefs.GetString ("fbId") != "") {
			StartCoroutine(loadFbPhoto());
		}

	}

	IEnumerator loadFbPhoto() {
		string _textureURL = "http://graph.facebook.com/"+PlayerPrefs.GetString ("fbId")+"/picture?type=square";
		WWW _www = new WWW(_textureURL);
		yield return _www;
		Sprite s = Sprite.Create (_www.texture, new Rect(0,0,_www.texture.width, _www.texture.height), Vector2.zero );
		fbImageReplace.sprite = s;
		if(fbRemove!=null) fbRemove.SetActive (false);
	}




}
