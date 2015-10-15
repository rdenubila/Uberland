using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BtnTut : MonoBehaviour {

	GameController gc;

	// Use this for initialization
	void Start () {
		gc = GameObject.Find ("Controller").GetComponent<GameController> ();
	}
	
	public void closeTut(){
		gc.closeTut ();
	}

	public void showAd(){
		if (transform.parent.GetComponentInChildren<Toggle> ().isOn) {
			PlayerPrefs.SetInt ("ad_power_up", 1);
		}
		gc.showAd ();
		gc.closeTut ();
	}
}
