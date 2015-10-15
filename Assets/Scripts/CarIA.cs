using UnityEngine;
using System.Collections;

public class CarIA : FollowPoints {

	public bool isTaxi = false;
	public float taxiPlayerDistance = 15f;

	public GameObject frontTransform;
	public LayerMask carsLayer;
	public float minDistance = 3f;

	bool inTrafficLight = false;

	Collider[] carsNear;

	float timeIni = -1;
	MeshRenderer r;

	// Use this for initialization
	void Start () {
		base.Start ();
		r = GetComponentInChildren<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		base.FixedUpdate ();

		if (isTaxi && carPlayer!=null) {

			if( carPlayerController.hasPassenger && Vector3.Distance(transform.position, carPlayer.transform.position) < taxiPlayerDistance ){
				followPlayer = true;
			} else {
				if(followPlayer){
					stopFollowing();
				}
				followPlayer = false;
			}
		}


		if (!isTaxi) {
			carsNear = Physics.OverlapSphere (frontTransform.transform.position, minDistance, carsLayer);
			//print (carsNear.Length);
			if (inTrafficLight || carsNear.Length > 0) {
				if(carsNear.Length>0 && carsNear[0].tag=="Player") gc.playHonk();
				stopCar ();
				if(timeIni<=-1) timeIni = Time.time;
			} else {
				restartCar ();
				timeIni = -1;
			}
		}

		if (timeIni > -1 && Time.time - timeIni > 10f && !r.isVisible) {
			Destroy(gameObject);
		}
	}


	
	void OnTriggerEnter(Collider other) {
		if (!isTaxi) {
			if (other.tag == "trafficCollider") {
				inTrafficLight = true;
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (!isTaxi) {
			if (other.tag == "trafficCollider") {
				inTrafficLight = false;
			}
		}
	}


}
