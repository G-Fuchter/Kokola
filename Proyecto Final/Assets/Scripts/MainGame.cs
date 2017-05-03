using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour {

    private GameObject[] objBubbles; //botones o burbujas 
    private int difficulty; //dificultad del nivel
    private float time; //tiempo a llegar al numero objetivo
    private float timeStart;
    private int number;// numero objetivo
    private int[] numButtons; //numeros del botones
    private int score; //score
    private Ray raycast; // raycast para detectar presión de botones
    private RaycastHit2D rayhit;
    private int playernumber; // se le suma los numeros de los botones apretados y se lo compara con number
    private TextMesh numObj; //el numero principal
    private TextMesh playernumObj; // el numero del jugador (el que es sumado)
    private int gameState; //estado del juego 1 = jugando 2 = perdio 0 = empezando 3 = score 4 = menu
    private TextMesh scoreObj;
    private TextMesh txtBest;
    private bool playIsClicked = false;
    private bool replayIsClicked = false;
    public AudioClip lose;
    public AudioClip pressed;
    private AudioSource aSource;
    private int bestScore;


    private void Awake()
    {
        InitBubble(); //iniciar botones
        numObj = GameObject.Find("Number").GetComponent<TextMesh>(); //agarra el textmesh del numero 
        playernumObj = GameObject.Find("PlayerNumber").GetComponent<TextMesh>(); //agarra el textmesh del numero del jugador
        scoreObj = GameObject.Find("TxtScore").GetComponent<TextMesh>();
        aSource = this.gameObject.GetComponent<AudioSource>();
        bestScore = PlayerPrefs.GetInt("highscore");
        txtBest = GameObject.Find("TxtBest").GetComponent<TextMesh>();
    }

    void Start () {
        timeStart = 10;
        difficulty = 0; //empieza en dificultad 0
        numButtons = new int[objBubbles.Length]; // asegurase que la cantidad de numeros sean igual a la cantidad de botones
        gameState = 4;
        txtBest.text = "Best \n" + bestScore.ToString();
    }

    void Update() {
        switch (gameState)
        {
            case 0: //empieza la ronda
                difficulty++;
                RoundStart();
                break;
            case 1: //juego
                InputTouch();
                Timer();
                if(number == playernumber)
                {
                    Win();
                }
                else if (number < playernumber)
                {
                    Loose();
                }
                break;
            case 2: // perdio
                if (CameraMoveLeft(10,-10))
                {
                    gameState = 3;
                }
                break;
            case 3: //replay menu
                InputTouch();
                if (replayIsClicked)
                {
                    if (CameraMoveRight(-10, 0))
                    {
                        replayIsClicked = false;
                        Restart();
                    }
                }
                break;
            case 4: // menu
                InputTouch();
                if(playIsClicked == true)
                {
                    if (CameraMoveLeft(10,0))
                    {
                        playIsClicked = false;
                        gameState = 0;
                    }
                }
                break;
        }
    }

    void RoundStart(){ //inicia la ronda
        playernumber = 0;
        playernumObj.text = playernumber.ToString();
        gameState = 1;
        time = timeStart;
        number = Random.Range(10 * difficulty,20 * difficulty); //crea un numero objetivo
        int tempnumber = number; //lo guarda temporalmente
        int randomSize = Random.Range(2, 5); //cantidad de botones que forman el numero
        for (int a = 0; a < randomSize; a++) //llena los botones con numeros random
        {
            numButtons[a] = Random.Range(2, (int )(tempnumber * 0.75f)); //que sean menores al numero objetivo
            if (tempnumber > 2) //para que no me den numeros negativos
            {
                tempnumber = tempnumber - numButtons[a]; // limite de los proximos numeros
            }
        }
        numButtons[randomSize - 1] = tempnumber; //llena el ultimo boton con el resto
        
        for(int a = randomSize; a < numButtons.Length; a++) //llena el resto de botones con numero aleatorios
        {
            numButtons[a] = Random.Range(2, number - 1);
        }

        Shuffle(numButtons); //mezcla el orden de los botones

        for(int b = 0; b < numButtons.Length; b++)
        {
            objBubbles[b].GetComponent<Bubble>().SetNumber(numButtons[b]); //asigna numeros a los botones
            objBubbles[b].GetComponent<Bubble>().SetisClicked(false);
        }

        numObj.text = number.ToString(); //display on screen
    }

    private void MakeObject(int a) // crea los numeros
    {
        objBubbles[a] = new GameObject("Number" + a.ToString()); //asigna nombre
        new GameObject(a.ToString()).transform.parent = objBubbles[a].transform; //asigna numero (objeto)
        objBubbles[a].AddComponent<Bubble>(); //Le agrega script Bubble
        objBubbles[a].transform.position = new Vector2(1.8f * (a / 3 - 1), 3.0f * (a % 3 - 1)); // Modifica posición
    }

    private void Shuffle(int[] num)
    {
        // Knuth shuffle algorithm
        for (int a = 0; a < num.Length; a++)
        {
            int tmp = num[a];
            int r = Random.Range(a, num.Length);
            num[a] = num[r];
            num[r] = tmp;
        }
    }

    private void InitBubble()//inicializa los botones
    {
        objBubbles = new GameObject[9];
        for (int a = 0; a < objBubbles.Length; a++)
        {
            MakeObject(a);
        }
    }

    private void InputTouch()
    {
        if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))//(Input.touchCount > 0)
        {
            raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);//(Input.GetTouch(0).position);
            rayhit = Physics2D.Raycast(raycast.origin, raycast.direction, 100.0f);
            if (rayhit.collider != null)
            {
                if (rayhit.collider.gameObject.name == "btnPlay")
                {
                    playIsClicked = true;
                }
                else if(rayhit.collider.gameObject.name == "btnReplay")
                {
                    replayIsClicked = true;
                }
                else
                {
                    playernumber += rayhit.collider.gameObject.GetComponent<Bubble>().ClickBubble();
                    playernumObj.text = playernumber.ToString();
                    aSource.PlayOneShot(pressed);
                }
            }
        }
    }

    private void Timer()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Loose();

        }
    }

    public float GetTime()
    {
        return time;
    }

    public float GetTimeStart()
    {
        return timeStart;
    }

    private bool CameraMoveLeft(int vel,int pos)
    {
        if(this.transform.localPosition.x <= pos)
        {
            return true;
        }
        else
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x - vel * Time.deltaTime, 0, -10);
            return false;
        }
    }

    private bool CameraMoveRight(int vel, int pos)
    {
        if (this.transform.localPosition.x >= pos)
        {
            return true;
        }
        else
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x - vel * Time.deltaTime, 0, -10);
            return false;
        }
    }

    private void Loose()
    {
        gameState = 2;
        scoreObj.text = score.ToString();
        aSource.PlayOneShot(lose);
        if (bestScore < score)
        {
            bestScore = score;
            txtBest.text ="Best \n" + bestScore.ToString();
            PlayerPrefs.SetInt("highscore", bestScore);
            PlayerPrefs.Save();
        }
        Debug.Log(bestScore);
    }

    private void Win()
    {
        score += 5;
        score += Mathf.RoundToInt(time);
        gameState = 0;
    }

    private void Restart()
    {
        score = 0;
        difficulty = 0;
        gameState = 0;
    }
}

/* _____       _ _ _                             ______          _     _            
  / ____|     (_) | |                           |  ____|        | |   | |           
 | |  __ _   _ _| | | ___ _ __ _ __ ___   ___   | |__ _   _  ___| |__ | |_ ___ _ __ 
 | | |_ | | | | | | |/ _ \ '__| '_ ` _ \ / _ \  |  __| | | |/ __| '_ \| __/ _ \ '__|
 | |__| | |_| | | | |  __/ |  | | | | | | (_) | | |  | |_| | (__| | | | ||  __/ |   
  \_____|\__,_|_|_|_|\___|_|  |_| |_| |_|\___/  |_|   \__,_|\___|_| |_|\__\___|_|   
                       
                                                                                    
                                                                                    */
