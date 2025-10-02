using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

public class InitiativeBrain : MonoBehaviour
{
    public static InitiativeBrain Instance { get; private set; }

    public enum Shift_Direction { UP, DOWN };

    //TODO:  GUI SHOULD HAVE DRAG AND DROP LIST OF CHARACTERS GOING
    //TODO:  HAVE TIE BREAKER TOGGLE THAT DETERMINES WHETHER ENEMIES OR PLAYERS GO FIRST IN A TIE
    public List <CharDeets> characterOrder = new List<CharDeets>(); // Represents the order of characters in combat
    public CharDeets currentChar { get; private set; } // Represents current character in combat
    public GameObject charEntryPanelPrefab; // Character Entry Panel Prefab

    public int roundNumber { get; private set; }
    public bool playerPriority { get; private set; } // Priority for if players go first in the instance of a tie, or enemies.

    ScrollRect _initScrollView;
    TMP_InputField _newNameIF;
    TMP_InputField _newInitiativeIF;
    TMP_InputField _newHealthIF;

    Button _nextCharButton;
    Button _prevCharButton;


    TMP_Text _roundNumberTXT;
    TMP_Text _currentCharacterTXT;

    Color _backgroundColor = new Color(255, 255, 255, 255);

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        _initScrollView = GameObject.Find("InitiaveScrollView").GetComponent<ScrollRect>();
        _newNameIF = GameObject.Find("IF_charName").GetComponent<TMP_InputField>();
        _newInitiativeIF = GameObject.Find("IF_charInit").GetComponent<TMP_InputField>();
        _newHealthIF = GameObject.Find("IF_charHealth").GetComponent<TMP_InputField>();
        //_roundNumberTXT = GameObject.Find("TXT_RoundNum").GetComponent<TMP_Text>();
        _currentCharacterTXT = GameObject.Find("TXT_CurrentCharacter").GetComponent<TMP_Text>();
        _nextCharButton = GameObject.Find("BTN_NextCharacter").GetComponent<Button>();
        _prevCharButton = GameObject.Find("BTN_PrevCharacter").GetComponent <Button>();

        _nextCharButton.onClick.AddListener(delegate { GoToNextCharacter(Shift_Direction.DOWN); });
        _prevCharButton.onClick.AddListener(delegate { GoToNextCharacter(Shift_Direction.UP); });
    }

    /// <summary>
    /// Adds a character to the encounter
    /// </summary>
    public void AddCharacterToEncounter()
    {
        int charInit;
        int charHealth;
        if(!int.TryParse(_newInitiativeIF.text, out charInit))
        {
            //Debug.Log("Initiative entered was not a valid number.");
            return;
        }
        if(!int.TryParse(_newHealthIF.text, out charHealth))
        {
            //Debug.Log("Health entered was not a valid number.");
            charHealth = 1;
        }

        string charName = _newNameIF.text;

        GameObject newPanelEntry = Instantiate(charEntryPanelPrefab);

        CharDeets newCharacter = newPanelEntry.GetComponent<CharDeets>();
        newCharacter.Name = charName;
        newCharacter.Initiative = charInit;
        newCharacter.Health = charHealth;

        // If there are no characters in the list, then just add the character.
        if(characterOrder.Count < 1) 
        {
            characterOrder.Add(newCharacter);
            newPanelEntry.transform.SetParent(_initScrollView.content.transform, false);
        }
        else
        {
            int i;

            // Find the right index to add the new character.
            for(i = 0; i < characterOrder.Count; i++)
            {
                if (newCharacter.Initiative < characterOrder[i].Initiative)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            characterOrder.Insert(i, newCharacter);
            newPanelEntry.transform.SetParent(_initScrollView.content.transform, false);
            newPanelEntry.transform.SetSiblingIndex(i);
        }

        SetCurrentCharacter(characterOrder[0]);

        _newInitiativeIF.text = "";
        _newHealthIF.text = "";
        _newNameIF.text = "";
    }

    /// <summary>
    /// Shifts the current character in the character order based on the direction
    /// </summary>
    /// <param name="dir">Direction for next character to be picked</param>
    public void GoToNextCharacter(Shift_Direction dir)
    {
        int nextIndex = characterOrder.IndexOf(currentChar);

        if(nextIndex < 0)
        {
            return;
        }

        nextIndex = dir == Shift_Direction.UP ? nextIndex - 1 : nextIndex + 1;

        if(nextIndex < 0 || nextIndex > characterOrder.Count - 1)
        {
            nextIndex = dir == Shift_Direction.UP ? characterOrder.Count - 1 : 0;
        }

        SetCurrentCharacter(characterOrder[nextIndex]);
    }

    public void SetCurrentCharacter(CharDeets nextCharacter)
    {
        String nextCharacterText = "";

        if(currentChar != null && characterOrder.Count > 1)
        {
            currentChar.SetBackgroundColor(_backgroundColor);
        }
        if(nextCharacter != null && nextCharacter != currentChar)
        {
            nextCharacter.SetBackgroundColor(Color.yellow);
            nextCharacterText = nextCharacter.Name;
        }
        currentChar = nextCharacter;
        _currentCharacterTXT.text = "Current Character: " + nextCharacterText;
    }

    /// <summary>
    /// Removes a character from the encounter
    /// </summary>
    /// <param name="character">Character to be removed</param>
    public void RemoveCharacterFromEncounter(CharDeets character)
    {
        // If current character is the character to be removed
        if (currentChar == character && characterOrder.Count > 1)
        {
            int currentCharIndex = characterOrder.IndexOf(character);

            if(currentCharIndex == characterOrder.Count - 1)
            {
                SetCurrentCharacter(characterOrder[0]);
            }
            else
            {
                SetCurrentCharacter(characterOrder[currentCharIndex + 1]);
            }
        }
        characterOrder.Remove(character);
        Destroy(character.gameObject);
    }

    /// <summary>
    /// Moves character based on the inputted Initiative
    /// </summary>
    /// <param name="character">Character to move</param>
    /// <param name="initNum">New initiative</param>
    /// <param name="oldInit">Old initiative</param>
    public void MoveCharacterOnInitChanged(CharDeets character, int initNum, int oldInit)
    {
        if(characterOrder.Count < 2)
        {
            return;
        }

        int origIndex = characterOrder.IndexOf(character);
        int i = -1;

        // Find appropriate spot to switch Initiative 
        if(initNum > oldInit)
        {
            for(i = origIndex - 1; i < characterOrder.Count; i--)
            {
                if(initNum < characterOrder[i].Initiative)
                {
                    i++;
                    //charToSwitch = characterOrder[i - 1];
                    break;
                }
            }
        }
        else if(initNum < oldInit)
        {
            for(i = origIndex + 1; i >= 0; i++)
            {
                if(initNum > characterOrder[i].Initiative)
                {
                    i--;
                    //charToSwitch = characterOrder[i + 1];
                    break;
                }
            }
        }

        Debug.Log("Moving to character spot: " + i + " " + characterOrder[i].Name);

        if (i < 0) { return; }

        Debug.Log("Moving to character spot: " + i + " " + characterOrder[i].Name);

        characterOrder.Remove(character);
        characterOrder.Insert(i, character);

        character.transform.SetSiblingIndex(i);
    }

    /// <summary>
    /// Clears all characters from the encounter
    /// </summary>
    public void ClearAllCharacters()
    {
        while(characterOrder.Count > 0)
        {
            RemoveCharacterFromEncounter(characterOrder.First());
        }
        currentChar = null;
        _currentCharacterTXT.text = "Current Character: ";
    }

    public void SetPlayerPriority(bool val)
    {
        playerPriority = val;
    }

    /// <summary>
    /// Sorts character order list and gameobjects
    /// </summary>
    public void SortCharacterOrderList()
    {
        characterOrder.Sort();
        for(int i = 0; i < characterOrder.Count; i++)
        {
            characterOrder[i].transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// Shifts a characters order in the list by one based one the direction inputted.
    /// </summary>
    /// <param name="character">Character to be initially switched.</param>
    /// <param name="dir">Direction to move them.</param>
    public void ShiftCharacter(CharDeets character, Shift_Direction dir)
    {
        if(characterOrder.Count > 1)
        {
            CharDeets otherChar = null;
            int ndxOfChar = characterOrder.IndexOf(character);
            int otherCharNdx = -1;

            if(dir == Shift_Direction.UP && ndxOfChar > 0)
            {
                otherCharNdx = ndxOfChar - 1;
            }
            else if(dir == Shift_Direction.DOWN && ndxOfChar < characterOrder.Count - 1)
            {
                otherCharNdx = ndxOfChar + 1;
            }

            if(otherCharNdx < 0)
            {
                return;
            }

            otherChar = characterOrder[otherCharNdx];

            characterOrder[ndxOfChar] = otherChar;
            characterOrder[otherCharNdx] = character;

            character.transform.SetSiblingIndex(otherCharNdx);
            otherChar.transform.SetSiblingIndex(ndxOfChar);
        }
    }

    private void OnDestroy()
    {
        _nextCharButton.onClick.RemoveAllListeners();
        _prevCharButton.onClick.RemoveAllListeners();
    }
}
