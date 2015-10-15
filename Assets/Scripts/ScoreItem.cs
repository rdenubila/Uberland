using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour {

	public Text name;
	public Text score;
	public Image thumb;

	public RectTransform rc;

	// Use this for initialization
	void Start () {
	
	}

	public void initObj(int i, string nameTxt, string idFB, string scoreTxt){
		name.text = nameTxt;
		score.text = "$ "+scoreTxt;

		StartCoroutine (loadFbPhoto (idFB));

		rc.anchoredPosition = new Vector2 (0, -i * 110);
	}


	IEnumerator loadFbPhoto(string fbId) {
		string _textureURL = "http://graph.facebook.com/"+fbId+"/picture?type=square";
		WWW _www = new WWW(_textureURL);
		yield return _www;
		Sprite s = Sprite.Create (_www.texture, new Rect(0,0,_www.texture.width, _www.texture.height), Vector2.zero );
		thumb.sprite = s;
		thumb.color = Color.white;
	}


}
