using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InitiativeBrain : MonoBehaviour
{
    public static InitiativeBrain Instance { get; private set; }

    //TODO:  GUI SHOULD HAVE DRAG AND DROP LIST OF CHARACTERS GOING
    //TODO:  HAVE TIE BREAKER TOGGLE THAT DETERMINES WHETHER ENEMIES OR PLAYERS GO FIRST IN A TIE
    public SortedList<int, CharDeets> characterOrder = new SortedList<int, CharDeets>(); // Represents the order of characters in combat
    public CharDeets currentChar; // Represents current character in combat
    public GameObject charEntryPanelPrefab; // Character Entry Panel Prefab



    ScrollRect _initScrollView;
    TMP_InputField _newnameIF;
    TMP_InputField _newInitiativeIF;

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
        _newnameIF = GameObject.Find("IF_charName").GetComponent<TMP_InputField>();
        _newInitiativeIF = GameObject.Find("IF_charInit").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Adds a character to the encounter
    /// </summary>
    /// <param name="name">Name of character</param>
    /// <param name="initiave">Initiave of newly added character</param>
    public void AddCharacterToEncounter()
    {

        int charInit;
        if(!int.TryParse(_newInitiativeIF.text, out charInit))
        {
            Debug.Log("Initiative entered was not a valid number.");
            return;
        }
        string charName = _newnameIF.text;

        GameObject newPanelEntry = Instantiate(charEntryPanelPrefab);
        newPanelEntry.transform.SetParent(_initScrollView.content.transform, false);

        CharDeets newCharacter = new CharDeets(charName, charInit, newPanelEntry);
        characterOrder.Add(charInit, newCharacter);
    }

    /// <summary>
    /// Removes a character from the encounter
    /// </summary>
    /// <param name="character">Character to be removed</param>
    public void RemoveCharacterFromEncounter(CharDeets character)
    {
        characterOrder.Remove(character.Initiative);
    }
}
