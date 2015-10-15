using UnityEngine;
using System.Collections;
using System;

public class PassengerPoint : MonoBehaviour {

	GameController gc;
	public Transform parkPoint;
	public GameObject zonePrefab;

	GameObject particles;
	GameObject p;
	CarController cc;
	PedestrianController pc;

	bool hasPassenger = false;
	bool isDestination = false;

	int moneyToEarn = 0;

	// Use this for initialization
	void Start () {
		gc = GameObject.Find ("Controller").GetComponent<GameController> ();

		//addPassenger ();
	}

	public void addPassenger(){
		particles = Instantiate(zonePrefab, parkPoint.position, Quaternion.Euler(0,0,0)) as GameObject;
		hasPassenger = true;
		gc.passengerIndicatorPosition = this.gameObject;
		StartCoroutine(AddPedestrianAsync());

	}

	IEnumerator AddPedestrianAsync() {
		
		ResourceRequest request = Resources.LoadAsync("Pedestrians/"+gc.returnRandomPassenger(), typeof(GameObject));
		yield return request;

		p = Instantiate(request.asset as GameObject, transform.position, Quaternion.Euler(0,0,0)) as GameObject;
		pc = p.GetComponent<PedestrianController> ();
		pc.isPassenger = true;
		pc.pp = this;		
		
	}

	public void setAsDestiny(){
		particles = Instantiate(zonePrefab, parkPoint.position, Quaternion.Euler(0,0,0)) as GameObject;
		isDestination = true;

		float dist = Vector3.Distance (gc.mainCar.transform.position, transform.position);
		moneyToEarn = (int)(dist/10);
		print (moneyToEarn);

		gc.passengerIndicatorPosition = this.gameObject;
	}

	public void PassengerEntered(){
		Destroy (particles);
		p.transform.SetParent (gc.mainCar.transform);
		pc.inTravel = true;
		pc.agent.Stop ();
		p.SetActive (false);

		cc.passengerObj = p;
		gc.mainCar.hasPassenger = true;

		hasPassenger = false;

		gc.showTut ("taxi");
		gc.getRandomPassengerPoint ().setAsDestiny ();
	}


	public void removePointError(){
		Destroy (particles);
		hasPassenger = false;

	}


	public void endPoint(){
		isDestination = false;
		Destroy (particles);
		cc.passengerObj.SetActive (true);
		cc.passengerObj.transform.SetParent(null);
		cc.passengerObj.GetComponent<PedestrianController>().agent.SetDestination(transform.position);

		gc.mainCar.hasPassenger = false;

		Destroy (cc.passengerObj, 5f);

		gc.passengerIndicatorPosition = null;

		gc.updateMoney (moneyToEarn);
		gc.addTaxi ();

		gc.callPassenger ();
	}
	
	void OnTriggerStay(Collider other) {

		if (other.tag == "Player") {

			cc = other.gameObject.GetComponent<CarController>();
			if(Mathf.Abs(cc.velocity)<1f){
				if(hasPassenger){
					pc.agent.SetDestination(other.transform.position);
				} else if(isDestination){
					endPoint();
				}
			}
			
		}
	}
	

}
