using UnityEngine;
using System.Collections;

public class FollowPoints : MonoBehaviour {

	public GameObject[] followPoints  = new GameObject[30];
	public float distPoint = 1.5f;

	public GameObject carPlayer;
	public CarController carPlayerController;
	public bool followPlayer = false;


	public float maxVelocity = 15f;
	public float rotateVal = 30;
	float velocity;
	float carSpeed = 0;

	float initialSpeed;
	float followSpeed = 30f;

	public GameController gc;
	NavMeshAgent agent;
	Rigidbody rb;

	int iPoint = 0;
	bool calledNextPoint = false;
	bool carStoped = false;

	public float angle;

	public Vector3 destPosition;

	public void Start () {

		iPoint = 1;
		fillPoints();

		transform.LookAt (followPoints [1].transform.position);

		carPlayer = GameObject.FindGameObjectWithTag ("Player");
		carPlayerController = carPlayer.GetComponent<CarController> ();
		gc = GameObject.Find("Controller").GetComponent<GameController> ();
		agent = GetComponent<NavMeshAgent> ();
		rb = GetComponent<Rigidbody> ();
		initialSpeed = agent.speed;
		followPlayer = false;
		nextPoint (0);
	}

	public void FixedUpdate () {

		if (followPlayer) {

			agent.speed = followSpeed;

			if (carPlayer != null) {
				destPosition = carPlayer.transform.position;
			} else {
				Vector3 v = Input.mousePosition;
				v.z = 51;
				destPosition = Camera.main.ScreenToWorldPoint (v);
			}

			angle = Vector3.Angle (destPosition - transform.position, transform.forward);

			if (agent.path.corners.Length > 2) {
				agent.enabled = true;
				agent.SetDestination (destPosition);
			} else {
				if (angle > 90) {

					if (agent.isActiveAndEnabled) {
						carSpeed = transform.InverseTransformDirection (agent.velocity).z;
						agent.enabled = false;
					}
					rbMoveCar (-0.2f);
					if (carSpeed < 0)
						rbRotateCar ();

				} else {

					if (agent.isActiveAndEnabled) {
						agent.SetDestination (destPosition);
					} else {
						rbMoveCar (0.2f);
						rbRotateCar ();
						if (carSpeed > 0)
							agent.enabled = true;
					}
					
				}
			}

		} else {

			/*
			if (agent.path.corners.Length > 0) {
				angle = Vector3.Angle (followPoints [iPoint].transform.position - transform.position, transform.forward);	
				if (angle > 100) {
					transform.Rotate (0, 100, 0);
				}
			}*/

		}

		if (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < distPoint) {
			if(!followPlayer){
				nextPoint ();
			}
		} else {
			calledNextPoint = false;
		}

	}

	void fillPoints(){
		for (uint i=1; i<followPoints.Length; i++) {
			followPoints[i] = followPoints[i-1].GetComponent<CarPoints>().getNextPoint();
		}
	}

	void resetPoints(){
		iPoint = 1;
		followPoints [0] = followPoints[followPoints.Length-1];
		fillPoints ();
	}

	void rbMoveCar(float vel){
		if (!agent.isActiveAndEnabled) {
			carSpeed += vel;
			carSpeed = Mathf.Max (-maxVelocity, carSpeed);
			carSpeed = Mathf.Min (maxVelocity, carSpeed);

			rb.velocity = transform.forward * carSpeed; //transform.forward * vel;
		}
	}

	void rbRotateCar(){
		if (!agent.isActiveAndEnabled) {
			int Direction = Vector3.Cross (destPosition - transform.position, transform.forward).y > 0 ? 1 : -1;
			transform.Rotate (Vector3.up * rotateVal / 10 * Direction * (carSpeed / maxVelocity));
		}
	}

	public void stopFollowing(){
		print ("para de serguir");
		agent.speed = initialSpeed;
		agent.enabled = true;
		nextPoint (0);
	}

	public void stopCar(){
		stopCar (true);
	}

	public void stopCar(bool quick){
		if (!carStoped) {
			agent.Stop ();
			if(quick){
				agent.velocity = Vector3.zero;
			}
			carStoped = true;
		}

	}

	public void restartCar(){
		if (carStoped) {
			//agent.acceleration = acceleration;
			agent.Resume ();
			carStoped = false;
		}
	}

	void nextPoint(){
		nextPoint (1);
	}

	void nextPoint(int qtdSoma){
		if (!calledNextPoint) {
			iPoint += qtdSoma;
			calledNextPoint = true;
		}

		if (iPoint >= followPoints.Length) {
			resetPoints();
		}

		agent.SetDestination (followPoints [iPoint].transform.position);
	}

}
