using UnityEngine;
using System.Collections;

public class ObjectHit : MonoBehaviour {

	public GameObject[] Prefabs;
	public string type;
	MeshRenderer r;
	BoxCollider c;



	// Use this for initialization
	void Start () {
		r = GetComponent<MeshRenderer> ();
		c = GetComponent<BoxCollider> ();
	}

	/*
	// Update is called once per frame
	void Update () {
	
	}
	*/

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" || other.tag == "Cars") {
			foreach(GameObject Prefab in Prefabs){
				Instantiate(Prefab, transform.position, Quaternion.Euler(90,0,0));
			}
			r.enabled = false;
			c.enabled = false;
			Invoke("respawn", 15f);
			other.gameObject.SendMessage("setPatrimonialDamage", type);
		}
	}

	void respawn(){
		r.enabled = true;
		c.enabled = true;
	}
}
