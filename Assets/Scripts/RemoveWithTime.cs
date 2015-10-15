using UnityEngine;
using System.Collections;

public class RemoveWithTime : MonoBehaviour {

	public float delayTime;

	// Use this for initialization
	void Start () {
		Invoke ("RemoveObj", delayTime);
	}
	
	void RemoveObj(){
		Destroy (gameObject);
	}
}
