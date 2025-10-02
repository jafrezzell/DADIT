using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharDeets : MonoBehaviour, IComparable<CharDeets>
{
    public string Name { get { return _name; } set { SetName(value); } } // Name of the character
    public int Initiative { get { return _initative; } set { SetInitiative(value); } } // The current Intiative the character has

    public int Health { get { return _health; } set { SetHealth(value); } }// Current health of the character.

    public bool IsPlayer { get { return _isPlayer; } set { SetPlayerStatus(value); } }


    TMP_Text _nameText;
    TMP_InputField _initInputField;
    TMP_InputField _healthInputField;
    TMP_InputField _healthIncInputField;
    TMP_InputField _healthDecInputField;
    Button _upButton;
    Button _downButton;
    Button _removeCharacterButton;
    Image _backgroundImage;

    int _initative = 0;
    string _name = "";
    int _health = 1;
    bool _isPlayer = false;

    private void Awake()
    {
        _nameText = transform.Find("TXT_Name").GetComponent<TMP_Text>();
        _initInputField = transform.Find("IF_InitField").GetComponent<TMP_InputField>();
        _healthInputField = transform.Find("IF_Health").GetComponent <TMP_InputField>();
        _healthIncInputField = transform.Find("IF_HealthInc").GetComponent<TMP_InputField>();
        _healthDecInputField = transform.Find("IF_HealthDec").GetComponent<TMP_InputField>();
        _upButton = transform.Find("BTN_Up").GetComponent<Button>();
        _downButton = transform.Find("BTN_Down").GetComponent<Button>();
        _removeCharacterButton = transform.Find("BTN_Remove").GetComponent<Button>();
        _backgroundImage = transform.GetComponent<Image>();


        _upButton.onClick.AddListener(delegate { InitiativeBrain.Instance.ShiftCharacter(this, InitiativeBrain.Shift_Direction.UP); });
        _downButton.onClick.AddListener(delegate { InitiativeBrain.Instance.ShiftCharacter(this, InitiativeBrain.Shift_Direction.DOWN); });
        _removeCharacterButton.onClick.AddListener(delegate { InitiativeBrain.Instance.RemoveCharacterFromEncounter(this); });
        _initInputField.onEndEdit.AddListener(delegate { Initiative = int.Parse(_initInputField.text); }); // WORK ON THIS
        _healthInputField.onEndEdit.AddListener(delegate { Health = int.Parse(_healthInputField.text); });
        _healthIncInputField.onEndEdit.AddListener(delegate { IncDecHealth(_healthIncInputField); });
        _healthDecInputField.onEndEdit.AddListener(delegate { IncDecHealth(_healthDecInputField); });
    }

    public CharDeets(string charName, int initiative, int health = 1) 
    {
        Name = charName;
        Initiative = initiative;
        Health = health;
    }

    /// <summary>
    /// Sets the Initiative to the value inputted
    /// </summary>
    /// <param name="val">New initiative value</param>
    public void SetInitiative(int val)
    {
        int oldInit = _initative;
        _initative = val;

        if(gameObject != null)
        {
            _initInputField.text = _initative.ToString();
            if(InitiativeBrain.Instance.characterOrder.Contains(this))
            {
                InitiativeBrain.Instance.MoveCharacterOnInitChanged(this, _initative, oldInit);
            }
        }
    }

    /// <summary>
    /// Sets the Name to the value inputted
    /// </summary>
    /// <param name="val">New name value</param>
    public void SetName(string val)
    {
        _name = val;
        if (gameObject != null)
        {
            _nameText.SetText(val);
        }
    }

    public void SetPlayerStatus(bool val)
    {
        _isPlayer = val;
    }

    public void SetBackgroundColor(Color color)
    {
        _backgroundImage.color = color;
    }

    public void SetHealth(int val)
    {
        _health = val;
        if (gameObject != null)
        {
            _healthInputField.text = _health.ToString();
        }
    }

    public void IncDecHealth(TMP_InputField inputField)
    {
        int val = 0;
        if(int.TryParse(inputField.text, out val))
        {
            if (inputField == _healthDecInputField)
                val = -val;
            Health += val;
        }

        inputField.text = "";
    }

    /// <summary>
    /// Compares this character deet based on Initiative
    /// </summary>
    /// <param name="other">Other character details</param>
    /// <returns>0 if initiative is the same, 1 if this initiative is greater than other, -1 if other is greater</returns>
    public int CompareTo(CharDeets other)
    {
        int val = 0;

        if (_initative > other.Initiative)
            val = -1;
        else if(_initative < other.Initiative)
            val = 1;

        return val;
    }

    private void OnDestroy()
    {
        _upButton.onClick.RemoveAllListeners();
        _downButton.onClick.RemoveAllListeners();
        _healthInputField.onEndEdit.RemoveAllListeners();
        _initInputField.onEndEdit.RemoveAllListeners();
        _removeCharacterButton.onClick.RemoveAllListeners();
        _healthDecInputField.onEndEdit.RemoveAllListeners();
        _healthIncInputField.onEndEdit.RemoveAllListeners();
    }
}
