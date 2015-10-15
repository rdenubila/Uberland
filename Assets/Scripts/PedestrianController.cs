using UnityEngine;
using System.Collections;

public class PedestrianController : MonoBehaviour {

	Collider[] points;
	public LayerMask pointsLayer;
	public float pointsMaxDist = 30f;

	public GameObject[] staticPoints;
	int iPoints = 0;

	public NavMeshAgent agent;
	Animator anim;

	public bool isPassenger = false;
	public bool inTravel = false;
	public PassengerPoint pp;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();

		if (isPassenger) {

		} else {
			if (staticPoints.Length > 0) {
				getNextStaticPoint (0);
			} else {
				getNextPoint ();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetFloat("velocity", agent.velocity.magnitude);

		anim.SetBool("isWalking", anim.GetCurrentAnimatorStateInfo (0).shortNameHash==Animator.StringToHash("Walking"));
		anim.SetBool("isJumping", anim.GetCurrentAnimatorStateInfo (0).shortNameHash==Animator.StringToHash("Standing_Jump"));


		if (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < agent.stoppingDistance) {
			if (isPassenger) {
				if(!inTravel){
					pp.PassengerEntered();
				}
			} else {
				if (staticPoints.Length > 0) {
					getNextStaticPoint(1);
				} else {
					if(!inInvoke){
						callNextPoint ();
					}
				}
			}
		}

	}

	bool inInvoke = false;
	void callNextPoint(){
		inInvoke = true;
		Invoke("getNextPoint", Random.Range(10.0f, 20.0f));
	}

	public void setRandomIdle(){
		if (anim.GetInteger ("randomIdle") == 0) {
			anim.SetInteger ("randomIdle", Random.Range (0, 11));

			Invoke ("resetIdle", Random.Range (8.0f, 16.0f));
		}
	}

	void resetIdle(){
		anim.SetInteger("randomIdle", 0);
	}

	void getNextPoint(){
		points = Physics.OverlapSphere (transform.position, pointsMaxDist, pointsLayer);
		agent.SetDestination ( points[Random.Range(0, points.Length-1)].transform.position );
		inInvoke = false;
	}

	void getNextStaticPoint(int soma){
		iPoints += soma;
		if (iPoints >= staticPoints.Length) {
			iPoints = 0;
		}

		if(staticPoints[iPoints]!=null) agent.SetDestination ( staticPoints[iPoints].transform.position );
	}


	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			anim.SetTrigger("jump");
			if(!isPassenger) GameObject.Find("Controller").GetComponent<GameController> ().Scream();
		}
	}

}
