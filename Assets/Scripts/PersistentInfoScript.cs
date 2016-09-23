using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;


public enum EControls {
    TilePressed,
    TileReleased,
    TileFlagPressed,
    TileFlagReleased,
    TileSquarePressed,
    TileSquareReleased,
    GameEscape
}

public class PersistentInfoScript : MonoBehaviour {

    #region Structs/Enums

    #endregion

    #region Fields
    // Public fields
    [Header("Default values")]
    public int DefaultFieldSizeX;
    public int DefaultFieldSizeY;
    public int DefaultMines;

    [Header("Options")]
    public bool AudioEnabled;
    public ActionMap<EControls> Controls = new ActionMap<EControls>();
    // Private fields
    
    #endregion

    #region Properties

    #endregion

    public PersistentInfoScript() {
        SetDefaultControls();
    }
    
    #region Methods
    void Awake() {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	    SetDefaultControls();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    // Reload default controls
    public void SetDefaultControls() {
        Controls = new ActionMap<EControls>();
        SetControl(EControls.TilePressed, KeyCode.Mouse0);
        SetControl(EControls.TileFlagPressed, KeyCode.Mouse1);
        SetControl(EControls.TileSquarePressed, KeyCode.Mouse2);
        SetControl(EControls.GameEscape, KeyCode.Escape);
    }



    // Set separate controls
    public void SetControl(EControls control, KeyCode key) {
        switch (control) {
            case EControls.TilePressed:
            case EControls.TileReleased:
                Controls[EControls.TilePressed] = new Action(key, Action.EActionType.PressOnce);
                Controls[EControls.TileReleased] = new Action(key, Action.EActionType.ReleaseOnce);
                break;
            case EControls.TileFlagPressed:
            case EControls.TileFlagReleased:
                Controls[EControls.TileFlagPressed] = new Action(key, Action.EActionType.PressOnce);
                Controls[EControls.TileFlagReleased] = new Action(key, Action.EActionType.ReleaseOnce);
                break;
            case EControls.TileSquarePressed:
            case EControls.TileSquareReleased:
                Controls[EControls.TileSquarePressed] = new Action(key, Action.EActionType.PressOnce);
                Controls[EControls.TileSquareReleased] = new Action(key, Action.EActionType.ReleaseOnce);
                break;
            case EControls.GameEscape:
                Controls[EControls.GameEscape] = new Action(key, Action.EActionType.ReleaseOnce);
                break;
            default:
                throw new ArgumentOutOfRangeException("control");
        }
    }

    #endregion

    #region Events

    #endregion

    #region Helper methods

    #endregion
}
