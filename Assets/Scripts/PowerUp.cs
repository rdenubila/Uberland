using UnityEngine;
using System.Collections;

public enum powerUpType{
	shield,fix,bulletTime
}

public class PowerUp : MonoBehaviour {

	public powerUpType type;
	public float rotVel = 5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate (Vector3.up * rotVel);
	}
}
