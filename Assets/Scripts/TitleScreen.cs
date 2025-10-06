using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    Button _clientStartUpButton;
    Button _serverStartUpButton;

    // Start is called before the first frame update
    void Start()
    {
        _clientStartUpButton = GameObject.Find("BTN_Server").GetComponent<Button>();
        _serverStartUpButton = GameObject.Find("BTN_Client").GetComponent<Button>();

        _serverStartUpButton.onClick.AddListener(delegate {StartDaditServer(); });  
        _clientStartUpButton.onClick.AddListener(delegate { StartDaditClient(); });
    }

    private void OnDestroy()
    {
        _serverStartUpButton?.onClick.RemoveAllListeners();
        _clientStartUpButton?.onClick.RemoveAllListeners();
    }

    void StartDaditServer()
    {
        SceneManager.LoadScene(1);
    }

    void StartDaditClient()
    {
        SceneManager.LoadScene(1);
    }
}
