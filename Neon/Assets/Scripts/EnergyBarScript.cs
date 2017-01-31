using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarScript : MonoBehaviour {

    private float barDisplay; //current progress
    public Vector2 pos = new Vector2(200, 500);
    public Vector2 size = new Vector2(128, 20);
    public Texture2D emptyTex;
    public Texture2D fullTex;
    public GUIStyle styleBG;
    public GUIStyle styleFG;

    public float BarDisplay
    {
        get
        {
            return barDisplay;
        }

        set
        {
            barDisplay = value;
        }
    }

    void Start()
    {

    }

    void OnGUI()
    {
            GUI.Box( new Rect(pos.x + size.x * barDisplay, pos.y, size.x, size.y), emptyTex, styleBG);
            GUI.Box(new Rect(pos.x, pos.y, size.x * barDisplay, size.y), fullTex, styleFG);
    }
}
