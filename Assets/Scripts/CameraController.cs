using UnityEngine;
using System.Collections;

public enum cameraTypes{
	isometrico,terceiraPessoa
}

public class CameraController : MonoBehaviour {

	public cameraTypes cameraType;

	public LayerMask buildingLayer;
	public GameObject mainCar;
	public CarController mainCarController;
	public float cameraZDif;
	public float cameraRotEase = .5f;
	Vector3 cameraIniPos;

	RaycastHit[] hits;
	RaycastHit[] lastHits;
	uint i;

	public float speed = 0.1F;

	// Use this for initialization
	void Start () {
		/*if (cameraType == cameraTypes.terceiraPessoa) {
			transform.SetParent(mainCar.transform);
		}*/
		mainCarController = mainCar.GetComponent<CarController> ();
		cameraIniPos = Camera.main.transform.localPosition;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (cameraType == cameraTypes.isometrico) {

			hits = Physics.RaycastAll(transform.position, transform.forward, Vector3.Distance(transform.position, mainCar.transform.position), buildingLayer);

			if (lastHits != null) {
				for (i=0; i<lastHits.Length; i++) {
					lastHits [i].transform.GetComponent<Renderer> ().enabled = true;
				}
			}

			for (i=0; i<hits.Length; i++) {
				hits[i].transform.GetComponent<Renderer>().enabled = false;
			}

			lastHits = hits;


			transform.position = new Vector3 (mainCar.transform.position.x, transform.position.y, mainCar.transform.position.z + cameraZDif);
		} else if (cameraType == cameraTypes.terceiraPessoa) {
			transform.position = mainCar.transform.position;
			//transform.rotation = transform.rotation + (mainCar.transform.rotation-transform.rotation)/10;
			//transform.position = Vector3.Lerp(transform.position, mainCar.transform.position, Time.time * speed * 25 );
			//Quaternion destRotation = mainCarController.velocity<0 ? Quaternion.Euler( mainCar.transform.eulerAngles + Vector3.up*180 ) : mainCar.transform.rotation;

			Vector3 destPos;
			Quaternion destRotation = mainCar.transform.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, destRotation, cameraRotEase * (1 / Time.timeScale));

			if(mainCarController.velocity<0){
				destRotation = Quaternion.Euler(90,0,0);
				destPos = new Vector3(0, 40,-7);
			} else {
				destRotation = Quaternion.Euler(45,0,0);
				destPos = cameraIniPos;
			}

			Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, destPos, cameraRotEase * (1 / Time.timeScale));
			Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, destRotation, cameraRotEase * (1 / Time.timeScale));


		}
	
	}
}
