using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Objects")]
    private bool loadScene = false;

    [Header("Parameters")]
    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private float FakeWait = 0.0f;


    private AsyncOperation _asynch;

    private void Awake()
    {
        LoadSceneNow();
    }

    // Updates once per frame
    void Update()
    {
        
    }

    public void LoadSceneNow()
    {
        // If the player has pressed the space bar and a new scene is not loading yet...
        if (loadScene == false)
        {

            // ...set the loadScene boolean to true to prevent loading a new scene more than once...
            loadScene = true;

            // ...and start a coroutine that will load the desired scene.
            StartCoroutine(LoadNewScene());
        }
    }


    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene()
    {

        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        _asynch = SceneManager.LoadSceneAsync(scene);
        _asynch.allowSceneActivation = false;

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (_asynch.progress < 0.89)
        {
            Debug.Log(_asynch.progress);
            yield return null;
        }

        //yield return new WaitForSeconds(FakeWait);

        Debug.Log("Reached end of while");

        loadingText.gameObject.GetComponent<Animator>().Play("fade");

        yield return new WaitForSeconds(1);

        ActivateScene();

        //LoadButton.GetComponent<Button>().interactable = true;
    }

    public void ActivateScene()
    {
        _asynch.allowSceneActivation = true;
    }

}