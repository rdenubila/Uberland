using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.IO;
using Facebook.Unity;

public class GameController : FacebookCallbacks {

	public CarController mainCar;
	public float health = 100.0f;
	bool volanteTouching = false;
	public GameObject volante;
	public GameObject volanteImg;
	int iFinger = 0;

	public int maxCars;
	public float carSpawnMinDist;
	public float carSpawnMaxDist;
	public LayerMask carPointsLayer;
	public GameObject[] carPrefabs;
	Collider[] carPoints;
	GameObject[] carSpawns;

	public int maxPedestrians;
	public LayerMask pedestrianPointsLayer;
	public string[] pedestrianPrefabsNames;
	Collider[] pedestrianPoints;
	GameObject[] pedestrianSpawns;

	public GameObject[] powerUpPrefabs;

	public GameObject canvas;
	public GameObject taxiIndicatorPrefab;
	GameObject[] taxiIndicators;

	public GameObject passengerIndicatorArrow;
	public GameObject passengerIndicatorPosition;

	GameObject[] passengerPoints;
	public float passengerMinDist = 10f;

	public GameObject damageIndicator;
	float damageIndicatorIniW;

	public AudioControler musicaNormal;
	public AudioControler musicaPerseguicao;
	public GameObject[] honksPrefab;
	public GameObject screamPrefab;

	float a = 0;
	int d;

	public Button btnShield;
	public Button btnFix;
	public Button btnClock;
	public GameObject clockMask;

	public int ControlType;

	// Use this for initialization
	void Start () {

		System.GC.Collect ();
		Resources.UnloadUnusedAssets ();

		FB.Init(this.OnInitComplete, this.OnHideUnity);

		//PlayerPrefs.DeleteAll ();

		//listDirectory ();

		Resources.UnloadUnusedAssets ();

		if (PlayerPrefs.GetInt ("audioEnabled") == 0) {
			AudioListener.pause = true;
		}

		if (!PlayerPrefs.HasKey ("total_money")) {
			PlayerPrefs.SetInt ("total_money", 0);
		} else {
			print ("Total Money: "+PlayerPrefs.GetInt ("total_money"));
		}


		passengerPoints = GameObject.FindGameObjectsWithTag ("passengerPoint");
		carSpawns = new GameObject[maxCars];
		pedestrianSpawns = new GameObject[maxPedestrians];
		addCar ();
		addPedestrian ();

		InvokeRepeating ("removeCar", 1f, 1f);
		InvokeRepeating ("removePedestrian", 1f, 1f);

		damageIndicatorIniW = damageIndicator.GetComponent<RectTransform> ().sizeDelta.x;

		for(int i=0; i<taxiInitialQtd; i++){
			addTaxi();
		}

		callPassenger ();

		addPowerUp ();

		btnShield.GetComponent<Image>().sprite = PowerUpsSprites[3];
		btnFix.GetComponent<Image>().sprite = PowerUpsSprites[4];
		btnClock.GetComponent<Image>().sprite = PowerUpsSprites[5];

		clockMask.SetActive(false);
		pauseMenu.SetActive(false);
		EndGameMenu.SetActive(false);
		TutArea.SetActive(false);

		updateMoney ();

		Invoke ("stopShield", 10f);

		Invoke ("showTutControle", 1.5f);

		ControlType = PlayerPrefs.GetInt ("controle");
		updateControlUI ();

		//PlayerPrefs.DeleteAll ();

	}
	
	// Update is called once per frame
	int iPrint = 1;
	void Update () {

		if (ControlType == 0) {
			if (volanteTouching) {
				Vector3 fp = Input.GetTouch (iFinger).position;
				a = Vector3.Angle (volante.transform.position - fp, -volante.transform.up);
				d = Vector3.Cross (volante.transform.position - fp, -volante.transform.up).z < 0 ? 1 : -1;
			} else {
				a *= .75f;
			}

			mainCar.hDir = -a / 90 * d;
			volanteImg.transform.rotation = Quaternion.Euler (0, 0, a * d);
		} else if (ControlType == 2) {
			mainCar.hDir = Input.acceleration.x;
		}

		updateIndicators ();
		updateIndicatorPassenger ();

		if (!mainCar.hasPassenger) {
			musicaNormal.playSound ();
			musicaPerseguicao.stopSound(2f);
		} else {
			musicaNormal.stopSound(2f);
			musicaPerseguicao.playSound();
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			print ("PRINTSCREEN");
			Application.CaptureScreenshot ("Screenshot"+iPrint+".png", 4);
			iPrint++;
		}
	}

	public void listDirectory(){
		DirectoryInfo info = new DirectoryInfo ("Assets/Resources/Pedestrians");
		FileInfo[] files = info.GetFiles ();
		string n;
		int i = 0;

		pedestrianPrefabsNames = new string[files.Length/2];

		foreach (FileInfo file in files) { 
			n = file.Name;
			if(n.IndexOf(".meta")<0){
				pedestrianPrefabsNames[i] = n.Replace(".prefab", "");
				i++;
			}
		}
	}


	public GameObject[] volanteUI;
	public GameObject[] setasUI;
	void updateControlUI(){
		if (ControlType == 0 || ControlType == 2) {
			for (int i=0; i<setasUI.Length; i++) {
				setasUI [i].SetActive (false);
			}
		} 

		if (ControlType == 1 || ControlType == 2) {
			for (int i=0; i<volanteUI.Length; i++) {
				volanteUI [i].SetActive (false);
			}
		}

		mainCar.useTouch = true;
	}

	public void startVolanteTouch(){

		iFinger = -1;
		for (int i = 0; i < Input.touchCount; i++) {
			if(Input.GetTouch(i).position.x < Screen.width/2){
				iFinger = i;
			}

		}

		if (iFinger >= 0) {
			mainCar.useTouch = true;
			volanteTouching = true;
		}
	}

	public void stopVolanteTouch(){
		volanteTouching = false;
	}

	public void arrowTouch(int direction){
		mainCar.useTouch = true;
		mainCar.hDir = (float)direction*.8f;
	}

	public void arrowTouchOut(){
		mainCar.hDir = 0;
	}

	void removeCar(){
		for (uint i=0; i<carSpawns.Length; i++) {
			if (carSpawns [i] != null) {
				if( Vector3.Distance(mainCar.transform.position, carSpawns[i].transform.position)>carSpawnMaxDist*1.2){
					Destroy(carSpawns[i]);
					carSpawns [i] = null;
				}
			}
		}

		addCar ();
	}

	public void addCar(){
		carPoints = Physics.OverlapSphere (mainCar.transform.position, carSpawnMaxDist, carPointsLayer);
		GameObject point;
		GameObject o;

		for(uint i=0; i<carSpawns.Length;i++){
			if(carSpawns[i]==null){
				int iCarPoints = Random.Range(0, carPoints.Length-1);
				while(carPoints[ iCarPoints ]==null || Vector3.Distance( (mainCar.transform.position), carPoints[ iCarPoints ].transform.position)<carSpawnMinDist){
					iCarPoints = Random.Range(0, carPoints.Length-1);
				}

				point = carPoints[ iCarPoints ].gameObject;
				o = Instantiate(carPrefabs[Random.Range(0,carPrefabs.Length)], point.transform.position, point.transform.rotation) as GameObject;
				o.GetComponent<CarIA>().followPoints[0] = point;
				carPoints[ iCarPoints ] = null;

				carSpawns[i] = o;
			}
		}
		//GameObject car = Instantiate
	}

	public GameObject taxiPrefab;
	public int taxiInitialQtd = 3;
	public int taxiMaxQtd = 10;
	int taxiCount = 0;

	public void addTaxi(){

		if (taxiCount < taxiMaxQtd) {
			print ("ADD TAXI");

			GameObject[] allCarPoints = GameObject.FindGameObjectsWithTag ("carPoints");
			GameObject point = allCarPoints [Random.Range (0, carPoints.Length)];

			GameObject o = Instantiate (taxiPrefab, point.transform.position, point.transform.rotation) as GameObject;
			o.GetComponent<CarIA> ().followPoints [0] = point;

			taxiCount++;
		}

		addIndicators ();

	}

	public void addPowerUp(){
		carPoints = Physics.OverlapSphere (mainCar.transform.position, carSpawnMaxDist, carPointsLayer);
		GameObject point;
		GameObject o;

		int iCarPoints = Random.Range(0, carPoints.Length-1);

		point = carPoints[ iCarPoints ].gameObject;
		o = Instantiate(powerUpPrefabs[Random.Range(0,powerUpPrefabs.Length)], point.transform.position+Vector3.up, point.transform.rotation) as GameObject;

		Invoke("addPowerUp", Random.Range(10.0f, 15.0f) );

	}


	void removePedestrian(){
		for (uint i=0; i<pedestrianSpawns.Length; i++) {
			if (pedestrianSpawns [i] != null) {
				if( Vector3.Distance(mainCar.transform.position, pedestrianSpawns[i].transform.position)>carSpawnMaxDist*1.1){
					Destroy(pedestrianSpawns[i]);
					pedestrianSpawns [i] = null;
				}
			}
		}
		
		addPedestrian ();
	}

	public string returnRandomPassenger(){
		int i = Random.Range (0, pedestrianPrefabsNames.Length);
		string p = pedestrianPrefabsNames [i];
		return p;
	}

	public GameObject empty;
	public void addPedestrian(){
		pedestrianPoints = Physics.OverlapSphere (mainCar.transform.position, carSpawnMaxDist, pedestrianPointsLayer);
		GameObject point;
		
		for(uint i=0; i<pedestrianSpawns.Length;i++){
			if(pedestrianSpawns[i]==null){
				int iPoint = Random.Range(0, pedestrianPoints.Length-1);
				while(pedestrianPoints[ iPoint ]==null || Vector3.Distance( mainCar.transform.position, pedestrianPoints[ iPoint ].transform.position)<carSpawnMinDist*.5f){
					iPoint = Random.Range(0, pedestrianPoints.Length-1);
				}

				pedestrianSpawns[i] = empty;
				point = pedestrianPoints[ iPoint ].gameObject;

				StartCoroutine(AddPedestrianAsync(point.transform, i));

				//o.GetComponent<CarIA>().followPoints[0] = point;
				pedestrianPoints[ iPoint ] = null;
			}
		}

		Resources.UnloadUnusedAssets ();
		//GameObject car = Instantiate
	}

	IEnumerator AddPedestrianAsync(Transform t, uint i) {

		ResourceRequest request = Resources.LoadAsync("Pedestrians/"+returnRandomPassenger(), typeof(GameObject));
		yield return request;

		GameObject o = Instantiate(request.asset as GameObject, t.position, t.rotation) as GameObject;
		pedestrianSpawns[i] = o;

	}

	CarIA[] taxisCarIA;

	void addIndicators(){
		GameObject[] taxis = GameObject.FindGameObjectsWithTag ("Taxi");
		uint i;

		if (taxiIndicators!=null) {
			for (i=0; i<taxiIndicators.Length; i++) {
				Destroy(taxiIndicators[i]);
			}
		}

		taxiIndicators = new GameObject[taxis.Length];
		taxisCarIA = new CarIA[taxis.Length];

		for (i=0; i<taxis.Length; i++) {
			taxisCarIA[i] = taxis[i].GetComponent<CarIA>();
			taxiIndicators[i] = Instantiate(taxiIndicatorPrefab);
			taxiIndicators[i].transform.SetParent(canvas.transform);
			taxiIndicators[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(0,-77,0);
			taxiIndicators[i].SetActive(false);
		}
	}

	void updateIndicators(){
		float a;
		float ac;
		int d;

		int countTx = 0;

		ac = mainCar.transform.rotation.eulerAngles.y-Camera.main.transform.rotation.eulerAngles.y;
		for (uint i=0; i<taxiIndicators.Length; i++) {
			if(taxisCarIA[i].followPlayer){
				countTx++;
				taxiIndicators[i].SetActive(true);
				a = Vector3.Angle(taxisCarIA[i].transform.position-mainCar.transform.position, mainCar.transform.forward);
				d = Vector3.Cross(taxisCarIA[i].transform.position-mainCar.transform.position, mainCar.transform.forward).y<0 ? -1 : 1;
				taxiIndicators[i].transform.rotation = Quaternion.Euler(0,0,a*d-ac);
			} else {
				taxiIndicators[i].SetActive(false);
			}
		}
	}

	void updateIndicatorPassenger(){
		float a;
		float ac;
		int d;
		if(passengerIndicatorPosition!=null){
			passengerIndicatorArrow.SetActive(true);
			a = Vector3.Angle(passengerIndicatorPosition.transform.position-mainCar.transform.position, mainCar.transform.forward);
			ac = mainCar.transform.rotation.eulerAngles.y-Camera.main.transform.rotation.eulerAngles.y;
			d = Vector3.Cross(passengerIndicatorPosition.transform.position-mainCar.transform.position, mainCar.transform.forward).y<0 ? -1 : 1;
			passengerIndicatorArrow.transform.rotation = Quaternion.Euler(0,0,a*d-ac);
		} else {
			passengerIndicatorArrow.SetActive(false);
		}
	}

	public void playAudio(GameObject prefab){
		GameObject o = Instantiate (prefab, Camera.main.transform.position, Quaternion.Euler (0, 0, 0)) as GameObject;
		o.transform.SetParent(Camera.main.transform);
	}


	bool canHonk = true;
	float honkInterv = 3f;
	public void playHonk(){
		if (canHonk) {
			playAudio (honksPrefab [Random.Range (0, honksPrefab.Length)]);
			canHonk = false;
			Invoke("autorizeHonk", honkInterv);
		}
	}

	void autorizeHonk(){
		canHonk = true;
	}

	public void Scream(){
		playAudio (screamPrefab);
	}

	public PassengerPoint getRandomPassengerPoint(){
		int i = Random.Range (0, passengerPoints.Length-1);
		while(Vector3.Distance(mainCar.transform.position, passengerPoints[i].transform.position)<passengerMinDist){
			i = Random.Range (0, passengerPoints.Length-1);
		}
		//print (passengerPoints [i]);
		return passengerPoints [i].GetComponent<PassengerPoint> ();
		
	}

	public void callPassenger(){
		PassengerPoint pp = getRandomPassengerPoint ();
		print (pp);
		try{
			pp.addPassenger ();
		} catch{
			print("erro");
			pp.removePointError();
			callPassenger();
		}
	}

	public float takeDamage(float damage){
		if (!withShield) {
			health -= (damage * (1-(float)PlayerPrefs.GetInt("Improv_resistencia")/7) );


			health = Mathf.Max (health, 0);
			health = Mathf.Min (health, 100);

			mainCar.smokeParticle.Stop ();
			mainCar.fireParticle.Stop ();

			Vector3 s = damageIndicator.GetComponent<RectTransform> ().sizeDelta;
			s.x = damageIndicatorIniW * health / 100;
			damageIndicator.GetComponent<RectTransform> ().sizeDelta = s;
		}

		return health;
	}

	public Sprite[] PowerUpsSprites;
	bool hasShield = false;
	bool hasFix = false;
	bool hasClock = false;

	public void pickedPowerUp(powerUpType whatPU){
		switch(whatPU){
		case powerUpType.shield:
			btnShield.GetComponent<Image>().sprite = PowerUpsSprites[0];
			hasShield = true;
			break;
		case powerUpType.fix:
			btnFix.GetComponent<Image>().sprite = PowerUpsSprites[1];
			hasFix = true;
			break;
		case powerUpType.bulletTime:
			btnClock.GetComponent<Image>().sprite = PowerUpsSprites[2];
			hasClock = true;
			break;
		}

		showTut ("power_up");
	}

	bool withShield = true;
	public void PU_Shield(){
		if (hasShield) {
			hasShield = false;
			btnShield.GetComponent<Image> ().sprite = PowerUpsSprites [3];
			mainCar.shieldParticle.Play ();
			withShield = true;

			Invoke ("stopShield", 5f + PlayerPrefs.GetInt("Improv_escudo")*5f);
		} else {
			showAd(powerUpType.shield);
		}
	}

	void stopShield(){
		mainCar.shieldParticle.Stop ();
		withShield = false;
	}

	public void PU_Fix(){
		if (hasFix) {
			hasFix = false;
			takeDamage ( -(25.0f + (float)PlayerPrefs.GetInt("Improv_conserto")/6*75.0f) );
			btnFix.GetComponent<Image> ().sprite = PowerUpsSprites [4];
		} else {
			showAd(powerUpType.fix);
		}
	}

	public void PU_Clock(){
		if (hasClock) {
			hasClock = false;
			btnClock.GetComponent<Image> ().sprite = PowerUpsSprites [5];
			Time.timeScale = .5f;
			clockMask.SetActive (true);

			musicaNormal.setPitch (Time.timeScale);
			musicaPerseguicao.setPitch (Time.timeScale);

			Invoke ("stopClock", 5f + PlayerPrefs.GetInt("Improv_cameraLenta")*5f);
		} else {
			showAd(powerUpType.bulletTime);
		}
	}

	void stopClock(){
		Time.timeScale = 1f;

		musicaNormal.setPitch (Time.timeScale);
		musicaPerseguicao.setPitch (Time.timeScale);

		clockMask.SetActive(false);
	}

	int currentMoney = 0;
	public Text[] moneyTFs;
	public void updateMoney(int value){
		currentMoney += value;

		currentMoney = Mathf.Max (0, currentMoney);

		foreach (Text moneyTF in moneyTFs) {
			moneyTF.text = "$ " + currentMoney;
		}
	}

	public void updateMoney(){
		updateMoney(0);
	}

	public GameObject pauseMenu;
	public GameObject EndGameMenu;

	public void pauseGame(){
		Time.timeScale = 0;
		pauseMenu.SetActive (true);
	}

	public void continueGame(){
		Time.timeScale = 1;
		pauseMenu.SetActive (false);
	}

	public Score scoreController;
	public void endGame(){
		SaveMoney ();
		Time.timeScale = 0;
		EndGameMenu.SetActive (true);
		scoreController.loadScore (true);
		scoreController.SaveScore (currentMoney);
	}

	public void SaveMoney(){
		PlayerPrefs.SetInt("total_money", PlayerPrefs.GetInt("total_money")+currentMoney);
	}

	string lastAdReward;
	powerUpType lastAdRewardPU;

	public void showAd(powerUpType wich){
		lastAdReward = "powerUp";
		lastAdRewardPU = wich;
		if (PlayerPrefs.HasKey ("ad_power_up")) {
			showAd ();
		} else {
			showTut ("ad_power_up");
		}

	}
	
	public void showAd(){
		if (Advertisement.IsReady ("rewardedVideo")) { 
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			print ("addNotReady");
		}
	}

	private void HandleShowResult(ShowResult result){
		switch (result)	{
		case ShowResult.Finished:
		Debug.Log("The ad was successfully shown.");
			if(lastAdReward=="powerUp"){
				pickedPowerUp(lastAdRewardPU);
			}
			pickedPowerUp(powerUpType.shield);
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	public void changeScene(string nextScene){
		Time.timeScale = 1;
		Application.LoadLevel(nextScene);
	}

	public GameObject TutArea;
	public GameObject[] TutPrefabs;
	GameObject lastTut;

	void showTutControle(){
		showTut ("controles");
	}

	public void showTut(string qual){

		if (!PlayerPrefs.HasKey (qual) && PlayerPrefs.GetInt (qual) < 1) {

			GameObject o = null;

			switch (qual) {
			case "controles":
				o = TutPrefabs [0];
				break;

			case "marcador":
				o = TutPrefabs [1];
				break;

			case "taxi":
				o = TutPrefabs [2];
				break;

			case "power_up":
				o = TutPrefabs [3];
				break;

			case "ad_power_up":
				o = TutPrefabs [4];
				break;
			}

			if (o != null) {
				lastTut = Instantiate (o);
				lastTut.transform.SetParent (TutArea.transform, false);
				Time.timeScale = 0.01f;
				TutArea.SetActive (true);

				if(qual!="ad_power_up"){
					PlayerPrefs.SetInt (qual, 1);
				}
			}

		}
	}

	public void closeTut(){
		Destroy (lastTut);
		TutArea.SetActive (false);
		Time.timeScale = 1;
	}

	public void exitGame(){
		Application.Quit();
	}

}

