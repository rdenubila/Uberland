using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour {

	public GameObject[] colliders_0;
	public GameObject[] colliders_1;

	public float lightTimer = 5f;

	bool actualLight = false;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("toggleLight", 0, lightTimer);
	}

	void toggleLight(){
		actualLight = !actualLight;

		changeColliders (colliders_0, actualLight);
		changeColliders (colliders_1, !actualLight);
	}

	void changeColliders(GameObject[] colliders, bool active){
		Vector3 p;
		for(uint i=0; i<colliders.Length; i++){
			p = colliders[i].transform.position;
			p.y = active ? 0 : 10;
			colliders[i].transform.position = p;
		}
	}
}
