using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour {

	public TranslateController tc;
	public string area;
	public int index;
	public bool upperCase;


	Text txt;

	// Use this for initialization
	void Start () {

		if (tc == null) {
			tc = GameObject.Find("Translate").GetComponent<TranslateController>();
		}

		txt = GetComponent<Text> ();
		updateText ();
		tc.registerGO (this);
	}

	public void updateText(){

		txt.text = tc.getText (area, index);

		if (upperCase) {
			txt.text = txt.text.ToUpper();
		}

		txt.text = txt.text.Replace ("BR;", "\n");

	}
	


}
