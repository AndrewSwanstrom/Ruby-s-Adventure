using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreScript : MonoBehaviour
{    
    public static ScoreScript instance { get; set; }

    public TMP_Text scoreText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreText.text = "Robots Fixed: 0";
    }

    public void SetText(TMP_Text value)
    {				      
        scoreText.text = value.text;
    }
}
