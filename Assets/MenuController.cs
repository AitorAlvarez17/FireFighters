using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public void SetAsServer()
    {
        SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
        if (SceneManager.GetSceneByName("MenuScene").isLoaded)
        {
            SceneManager.UnloadScene("MenuScene");
        }
        else if (SceneManager.GetSceneByName("MenuScene2").isLoaded)
        {
            SceneManager.UnloadScene("MenuScene2");
        }
    }

    public void SetAsClient()
    {
        SceneManager.LoadScene("ClientScene", LoadSceneMode.Additive);
        if (SceneManager.GetSceneByName("MenuScene").isLoaded)
        {
            SceneManager.UnloadScene("MenuScene");
        }
        else if(SceneManager.GetSceneByName("MenuScene2").isLoaded)
        {
            SceneManager.UnloadScene("MenuScene2");
        }
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
