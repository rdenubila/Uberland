using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity.Example;

public class BuyImprovements : MonoBehaviour {

	public MenuController c;
	public string tipo;
	public Text labelValor;
	public Button btnBuy;

	public Image indicator;
	public Sprite[] indicatorSprites;

	public AudioSource asCompra;
	public AudioSource asCompraErro;

	int maxLevel = 6;
	int level;
	public int initialCost = 150;
	public int currentCost;

	// Use this for initialization
	void Start () {
		UpdateIndicators ();
	}


	void UpdateIndicators () {
		level = PlayerPrefs.GetInt ("Improv_" + tipo);

		//PlayerPrefs.DeleteKey ("Improv_" + tipo);

		if (level >= maxLevel) {
			labelValor.gameObject.SetActive(false);
			btnBuy.gameObject.SetActive(false);
		}

		currentCost = initialCost;
		for (int i=0; i<level; i++) {
			currentCost *= 2;
		}

		labelValor.text = "$ " + currentCost;

		indicator.sprite = indicatorSprites [level];

		c.updateMoney ();

	}

	public void Buy(){
		if (currentCost <= PlayerPrefs.GetInt ("total_money")) {

			PlayerPrefs.SetInt ("total_money", PlayerPrefs.GetInt ("total_money") - currentCost);

			PlayerPrefs.SetInt ("Improv_" + tipo, level + 1);
			UpdateIndicators ();
			asCompra.Play ();
		} else {
			asCompraErro.Play();
		}
	}
}
