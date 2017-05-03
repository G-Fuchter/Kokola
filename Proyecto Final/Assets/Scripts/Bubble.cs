using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {
    private int number;
    private bool isClicked = false;
    private GameObject txt;
    private TextMesh txtMesh;
    private MeshRenderer renderMesh;
    private SpriteRenderer spriteRender;
    private CircleCollider2D col;

    private void Awake()
    {
        txt = this.transform.GetChild(0).gameObject; //consigue el child
        txtMesh = txt.AddComponent<TextMesh>();// le agrega text mesh
        renderMesh = txt.GetComponent<MeshRenderer>();// consigue mesh renderer para material de font
        spriteRender = this.gameObject.AddComponent<SpriteRenderer>(); //agrega sprite renderer
        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/Number"); //carga el sprite
        spriteRender.color = new Color(0.81f, 0.84f, 0.86f, 1f);
        col = this.gameObject.AddComponent<CircleCollider2D>();

        IniText(); //Inicializa texto
    }

    // Use this for initialization
    void Start () {
        txtMesh.text = number.ToString();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetNumber()
    {
        return number;
    }

    public void SetNumber(int a)
    {
        number = a;
        txtMesh.text = a.ToString();
    }

    private void IniText() //inicializa el texto
    {
        txtMesh.characterSize = 0.015f;
        txtMesh.color = Color.black;
        txtMesh.anchor = TextAnchor.MiddleCenter;
        txtMesh.font = Resources.Load<Font>("GlacialIndifference-Regular"); // cambia el font
        renderMesh.material = txtMesh.font.material;
        txt.GetComponent<Renderer>().sortingLayerName = "Text";
    }

    public int ClickBubble()
    {
        if (!isClicked)
        {
            isClicked = true;
            this.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            return number;
        }
        return 0;
    }

    public void SetisClicked(bool set)
    {
        isClicked = set;

        if(set == false)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    
}
