using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject createGame;
    public GameObject joinGame;

    private string menuScene = "MenuScene";
    public void SetAsServer()
    {
        SceneManager.LoadScene("ServerScene", LoadSceneMode.Single);
    }

    public void SetAsClient()
    {
        SceneManager.LoadScene("ClientScene", LoadSceneMode.Single);

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
