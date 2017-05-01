using System;
using UnityEngine;

public static class Mouse {
    #region Properties
    /// <summary>
    ///     Returns current mouse coordinates in screen coordinates
    /// </summary>
    public static Vector2 MouseScreenCoordinates {
        get { return Input.mousePosition; }
    }

    /// <summary>
    ///     Difference of the mouse scroll this frame
    /// </summary>
    public static Vector2 ScrollDelta {
        get { return Input.mouseScrollDelta; }
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Returns the position of the mouse in pixels
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetMouseScreenCoordinates() {
        return MouseScreenCoordinates;
    }

    /// <summary>
    ///     Returns the position of the mouse in world coordinates
    /// </summary>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldCoordinates(Camera cam) {
        return cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
    }

    /// <summary>
    ///     Returns true if button is currently held down
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool IsButtonDown(KeyCode button) {
        ThrowIfNoMouseKey(button);
        return Input.GetMouseButton(GetMouseButton(button));
    }

    /// <summary>
    ///     Return true if button is currently not down
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool IsButtonUp(KeyCode button) {
        ThrowIfNoMouseKey(button);
        return !IsButtonDown(button);
    }

    /// <summary>
    ///     Returns true if the button was pressed this frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool IsButtonPressed(KeyCode button) {
        ThrowIfNoMouseKey(button);
        return Input.GetMouseButtonDown(GetMouseButton(button));
    }

    /// <summary>
    ///     Returns true if the button was released this frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool IsButtonReleased(KeyCode button) {
        ThrowIfNoMouseKey(button);
        return Input.GetMouseButtonUp(GetMouseButton(button));
    }

    private static void ThrowIfNoMouseKey(KeyCode button) {
        if (GetMouseButton(button) == -1)
            throw new ArgumentException("Only KeyCode.Mouse# is accepted here", "button");
    }
    #endregion

    #region Helper methods
    /// <summary>
    ///     Returns the number of the mouse button or -1 if not a mouse button
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    private static int GetMouseButton(KeyCode button) {
        switch (button) {
        case KeyCode.Mouse0:
            return 0;
        case KeyCode.Mouse1:
            return 1;
        case KeyCode.Mouse2:
            return 2;
        case KeyCode.Mouse3:
            return 3;
        case KeyCode.Mouse4:
            return 4;
        case KeyCode.Mouse5:
            return 5;
        case KeyCode.Mouse6:
            return 6;
        default:
            return -1;
        }
    }
    #endregion
}
