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

/// <summary>
///     Defines a couple of default variables. Should be a singleton
/// </summary>
public class PersistentInfoScript : MonoBehaviour {
    #region Fields
    /// <summary>
    ///     Default width of the field
    /// </summary>
    [Header("Default values")]
    public int DefaultFieldSizeX;

    /// <summary>
    ///     Default height of the field
    /// </summary>
    public int DefaultFieldSizeY;

    /// <summary>
    ///     Default number of mines on the field
    /// </summary>
    public int DefaultMines;

    /// <summary>
    ///     If audio is enabled or not (unused)
    /// </summary>
    [Header("Options")]
    public bool AudioEnabled;

    /// <summary>
    ///     Controls of the game
    /// </summary>
    public ActionMap<EControls> Controls = new ActionMap<EControls>();
    #endregion

    #region Methods
    void Awake() {
        SetDefaultControls();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    ///     Reload default controls scheme
    /// </summary>
    public void SetDefaultControls() {
        Controls = new ActionMap<EControls>();
        SetControl(EControls.TilePressed, KeyCode.Mouse0);
        SetControl(EControls.TileFlagPressed, KeyCode.Mouse1);
        SetControl(EControls.TileSquarePressed, KeyCode.Mouse2);
        SetControl(EControls.GameEscape, KeyCode.Escape);
    }

    /// <summary>
    ///     Set separate controls
    /// </summary>
    /// <param name="control"></param>
    /// <param name="key"></param>
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
}
