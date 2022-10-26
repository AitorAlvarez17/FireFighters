using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject createGame;
    public GameObject joinGame;

    public void SetAsServer()
    {
        SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(createGame.scene.name);
        //Debug.Log(createGame.scene.name);
    }

    public void SetAsClient()
    {
        SceneManager.LoadScene("ClientScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(joinGame.scene.name);
        //Debug.Log(createGame.scene.name);
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
