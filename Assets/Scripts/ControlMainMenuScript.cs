using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Controll class for the main menu view. It's pretty barebones
/// </summary>
public class ControlMainMenuScript : MonoBehaviour {

    ///// <summary>
    ///// Executes before all Start methods are called. This makes sure it doesn't get destroyed.
    ///// </summary>
    //void Awake() {
    //    DontDestroyOnLoad(this);
    //}

    #region Events
    /// <summary>
    /// Action is executed when the user presses the Start button
    /// </summary>
    public void OnStartButtonClick() {
        SceneManager.LoadScene("PlayScene");
    }

    /// <summary>
    /// Action is executed when the user presses the Options button
    /// </summary>
    public void OnOptionsButtonClick() {
        //TODO: add options
    }

    /// <summary>
    /// Action is executed when te user presses the Quit button
    /// </summary>
    public void OnQuitButtonClick() {
        Application.Quit();
    }
    #endregion
}
