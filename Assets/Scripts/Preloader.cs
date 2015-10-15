using UnityEngine;
using System.Collections;

public class Preloader : MonoBehaviour {

	public AsyncOperation loadOp;

	// Use this for initialization
	void Start () {
		loadOp = Application.LoadLevelAsync("city");  
	}
	
	// Update is called once per frame
	void Update () {
		print (loadOp.progress);
	}
}
