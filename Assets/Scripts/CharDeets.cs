using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharDeets

{
    public string Name { get { return _name; } set { SetName(value); } } // Name of the character
    public int Initiative { get { return _initative; } set { SetInitiative(value); } } // The current Intiative the character has

    
    public GameObject panelGameObject; // Panel gameobject that represents this entry panel

    int _initative = 0;
    string _name = "";

    public CharDeets(string charName, int initiative, GameObject panelGameObject = null) 
    {
        this.panelGameObject = panelGameObject;
        Name = charName;
        Initiative = initiative;
    }

    /// <summary>
    /// Sets the Initiative to the value inputted
    /// </summary>
    /// <param name="val">New initiative value</param>
    public void SetInitiative(int val)
    {
        _initative = val;

        if(panelGameObject != null)
        {
            TMP_Text panelText = panelGameObject.transform.Find("TXT_description").GetComponent<TMP_Text>();
            panelText.text = _name + ": " + _initative;
        }
    }

    /// <summary>
    /// Sets the Name to the value inputted
    /// </summary>
    /// <param name="val">New name value</param>
    public void SetName(string val)
    {
        _name = val;

        if(panelGameObject != null)
        {
            TMP_Text panelText = panelGameObject.transform.Find("TXT_description").GetComponent<TMP_Text>();
            panelText.text = _name + ": " + _initative;
        }
    }
}
