using UnityEngine;
using System.Collections;

public class AudioControler : MonoBehaviour {

	AudioSource a;

	public bool playOnAwake = false;
	bool isPlaying = false;

	// Use this for initialization
	void Start () {
		a = GetComponent<AudioSource> ();
		a.volume = 0;
		if(playOnAwake) playSound();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void stopSound(){
		if(isPlaying) StartCoroutine(stopSoundFade (1f));
	}

	public void stopSound(float time){
		if(isPlaying) StartCoroutine(stopSoundFade (time));
	}


	public IEnumerator stopSoundFade(float time){

		StopCoroutine ("playSoundFade");

		isPlaying = false;
		
		float t = time*20;
		
		for (uint i=0; i<=t; i++) {
			yield return new WaitForSeconds (time/t);
			a.volume = 1-(float)i/t;
			
		}
	}

	public void playSound(){
		if(!isPlaying) StartCoroutine(playSoundFade (1f));
	}

	public void playSound(float time){
		if(!isPlaying) StartCoroutine(playSoundFade (time));
	}
	
	
	public IEnumerator playSoundFade(float time){

		StopCoroutine ("stopSoundFade");

		isPlaying = true;

		float t = time*20;

		for (uint i=0; i<=t; i++) {
			yield return new WaitForSeconds (time/t);
			a.volume = (float)i/t;
		}
	}


	public void setPitch(float value){
		a.pitch = value;
	}

}
