using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

	public float maxVelocity;
	public float acceleration;
	public float desacceleration;
	public float breakVal;
	public float rotateVal;

	public float velocity;

	public float inputCanvas;
	public float volanteCanvas;

	public float hDir = 0;
	public bool useTouch = false;

	public bool hasPassenger = false;
	public GameObject passengerObj;

	Rigidbody rb;
	AudioSource a;
	public GameController c;

	public AudioClip somAcelerando;
	public AudioClip somParado;
	public AudioClip somFreando;
	public GameObject somFreandoPrefab;
	public GameObject somBatidaPrefab;
	public GameObject somPickItemPrefab;
	public GameObject somExplosao;
	string somAtual;

	public ParticleSystem smokeParticle;
	public ParticleSystem fireParticle;
	public ParticleSystem shieldParticle;
	public GameObject explosionPrefab;

	Vector3 iniPos;


	// Use this for initialization
	void Start () {
		iniPos = transform.position;
		rb = GetComponent<Rigidbody> ();
		a = GetComponent<AudioSource> ();

		smokeParticle.Stop ();
		fireParticle.Stop ();
		shieldParticle.Stop ();
	}

	void changeAudio(string what){

		if (somAtual != what) {
			switch (what) {
			case "acelerando":
				a.clip = somAcelerando;
				Invoke("backChangeAudio", somAcelerando.length);
				a.Play ();
				break;
			case "freando":
				c.playAudio (somFreandoPrefab);
				//a.clip = somFreando;
				//Invoke("backChangeAudio", somFreando.length);
				break;
			default:
				a.clip = somParado;
				a.Play ();
				break;

			}

			somAtual = what;

		}

	}

	void backChangeAudio(){
		changeAudio ("parado");
	}
	
	// Update is called once per frame
	float tsMult;
	void FixedUpdate () {

		tsMult = (1 / Time.timeScale);

		if (Input.GetAxis ("Vertical")>0 || inputCanvas>0) {
			if(velocity<-0.1f){
				velocity *= breakVal*Time.timeScale;
			} else {
				velocity += acceleration*tsMult;
				velocity = Mathf.Min(velocity, maxVelocity);
			}
			if(velocity<maxVelocity*.5) changeAudio("acelerando");
		} else if (Input.GetAxis ("Vertical")<0 || inputCanvas<0) {
			if(velocity>0.1f){
				velocity *= breakVal*Time.timeScale;
				changeAudio("freando");
			} else {
				velocity -= acceleration*tsMult;
				velocity = Mathf.Max(velocity, -maxVelocity*.7f);
			}
		} else if (Input.GetAxis ("Vertical")==0 || inputCanvas==0) {
			//velocity *= desacceleration;
			if(velocity>0){
				velocity = Mathf.Max(0, velocity-acceleration*tsMult);
			} else if(velocity<0){
				velocity = Mathf.Min(0, velocity+acceleration*tsMult);
			}
		}

		if (!useTouch) {
			hDir = Input.GetAxis ("Horizontal");
		}

		if (exploded || Time.timeScale<.1f) {
			velocity = 0;
		}

		if (!PlayerPrefs.HasKey ("marcador") && velocity==maxVelocity && Mathf.Abs(hDir)>.75f){
			velocity = 0;
			hDir = 0;
			c.showTut("marcador");
		}

		transform.Rotate (Vector3.up * (rotateVal/10 * tsMult) * hDir * (velocity/maxVelocity));

		//transform.Translate (new Vector3(0,0, velocity/10));
		
		rb.velocity = transform.forward*velocity * tsMult ; 
		a.volume = (0.3f + Mathf.Abs(velocity/maxVelocity*0.7f))*.5f;
				
	}

	public void setInputCanvas(float valor){
		inputCanvas = valor;
	}


	public void setPatrimonialDamage(string what){
		print (what);
		int cost = 0;
		switch(what){
		case "hydrant":
			cost = 2;
			break;
		case "traffic_light":
			cost = 4;
			break;
		}

		velocity = velocity*0.3f;
		c.playAudio (somBatidaPrefab);

		c.updateMoney (-cost);

		checkDamage (c.takeDamage (1.5f));

	}

	bool exploded = false;
	void checkDamage(float currentDamage){
		if (currentDamage < 30) {
			smokeParticle.Play ();
		}

		if (currentDamage < 10) {
			fireParticle.Play();
		}

		if (!exploded && currentDamage == 0) {
			exploded = true;
			GameObject o = Instantiate(explosionPrefab, transform.position+Vector3.up, transform.rotation) as GameObject;
			o.transform.SetParent(transform);
			c.playAudio (somExplosao);
			c.Invoke("endGame", 1.5f);
		}
	}

	void resetCar(){
		c.takeDamage (-100f);
		smokeParticle.Stop();
		fireParticle.Stop();
		transform.position = iniPos;
		exploded = false;
	}


	void OnCollisionEnter(Collision collision) {
		c.playAudio (somBatidaPrefab);
		checkDamage (c.takeDamage (1f));
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "PowerUp") {
			c.playAudio (somPickItemPrefab);

			PowerUp pu = other.GetComponent<PowerUp>();
			c.pickedPowerUp(pu.type);


			Destroy(other.gameObject);
		}
	}
}
