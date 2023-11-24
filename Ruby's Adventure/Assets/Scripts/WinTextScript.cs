using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinTextScript : MonoBehaviour
{
    public static WinTextScript instance { get; set; }

    public TMP_Text winText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        winText.text = "";
    }

    public void SetText(TMP_Text value)
    {				      
        winText.text = value.text;
    }
}
