using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ControlMainMenuScript : MonoBehaviour {



    void Awake() {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #region Events

    public void OnStartButtonClick() {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnOptionsButtonClick() {
        //TODO: add options
    }

    public void OnQuitButtonClick() {
        Application.Quit();
    }
    #endregion
}
