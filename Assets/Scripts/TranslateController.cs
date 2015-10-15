using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class TranslateController : MonoBehaviour {

	public TextAsset[] idiomasXml;
	XmlDocument xmldoc;

	public List<TranslateText> TranslateTextList = new List<TranslateText>();

	// Use this for initialization
	void Start () {
		loadXml ();
	}

	void loadXml(){

		if (!PlayerPrefs.HasKey ("idioma")) {
			if(Application.systemLanguage == SystemLanguage.Portuguese){
				PlayerPrefs.SetInt ("idioma", 1);
			} else {
				PlayerPrefs.SetInt ("idioma", 0);
			}
		}

		xmldoc = new XmlDocument ();
		xmldoc.LoadXml ( idiomasXml[ PlayerPrefs.GetInt ("idioma") ].text );
	}

	public void updateLanguage(){
		loadXml ();

		for (int i=0; i<TranslateTextList.Count; i++) {
			TranslateTextList[i].updateText();
		}
	}

	public void registerGO(TranslateText tt){
		TranslateTextList.Add (tt);
	}

	public string getText(string level_1, int index){

		if (xmldoc == null) {
			loadXml();
		}

		if (index == 107) {
			index = Random.Range( 0, xmldoc.GetElementsByTagName(level_1)[0].ChildNodes.Count);
		}

		print (level_1);
		return xmldoc.GetElementsByTagName(level_1)[0].ChildNodes[index].Attributes["value"].Value;
	}

}
