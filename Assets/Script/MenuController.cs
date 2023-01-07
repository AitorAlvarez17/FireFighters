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
        // SceneManager.UnloadSceneAsync(createGame.scene.name);
    }

    public void SetAsClient()
    {
        SceneManager.LoadScene("ClientScene", LoadSceneMode.Single);
        // SceneManager.UnloadSceneAsync(joinGame.scene.name);

    }
    // Start is called before the first frame update

    public void CloseGame()
    {
        Application.Quit();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
