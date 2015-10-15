using UnityEngine;
using System.Collections;

public class CarPoints : MonoBehaviour {

	public GameObject[] nextPoints;

	// Use this for initialization
	void Start () {
		for (uint i=0; i<nextPoints.Length; i++) {
			if(nextPoints[i]==null){
				print (name);
			}

		}
	}

	public GameObject getNextPoint(){
		return nextPoints[ Random.Range(0, nextPoints.Length-1) ];
	}

}
