using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour {

    private GameObject mainGame;
    private MainGame gameScript;

    private void Awake()
    {
        mainGame = GameObject.Find("Main Camera"); 
        gameScript = mainGame.GetComponent<MainGame>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetClock();
	}

    private void SetClock() //modifica su tamaño dependiendo del tiempo que le queda
    {
        this.gameObject.transform.localScale = new Vector3(6.65f / (gameScript.GetTimeStart() / gameScript.GetTime()), 0.5f, 1);
    }
}
