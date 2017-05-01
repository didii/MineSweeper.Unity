using System;
using UnityEngine;

/// <summary>
///     Behaviour to show the amount of bombs left
/// </summary>
public class BombNumberScript : MonoBehaviour {
    #region Types
    /// <summary>
    ///     Digit number X from a number where the rightmost is considered as the first
    /// </summary>
    public enum EDigit {
        First,
        Second,
        Third
    }
    #endregion

    #region Fields
    /// <summary>
    ///     Which digit this script is attached to
    /// </summary>
    public EDigit Digit;

    /// <summary>
    ///     The collection of all the number sprites (must be ordered)
    /// </summary>
    public Sprite[] NumberSprites;

    private int _number;
    private ControlScript _parent;
    private bool _leftDown, _rightDown;
    #endregion

    #region Properties
    /// <summary>
    ///     The control script of the game
    /// </summary>
    public ControlScript Parent {
        set { _parent = value; }
    }
    #endregion

    #region Unity events
    /// <summary>
    ///     LateUpdate is called at the end of every frame.
    /// </summary>
    void LateUpdate() {
        if (Input.GetMouseButtonUp(0))
            _leftDown = false;
        if (Input.GetMouseButtonUp(1))
            _rightDown = false;
    }

    /// <summary>
    ///     OnMouseOver is called every frame as long as the mouse is hovering over this objects bounding box
    /// </summary>
    void OnMouseOver() {
        if (_parent.GameState != ControlScript.EGameState.Uninitialized)
            return;
        if (Input.GetMouseButtonDown(0))
            _leftDown = true;
        if (Input.GetMouseButtonUp(0) && _leftDown)
            _parent.OnBombNumberClick(Digit);
        if (Input.GetMouseButtonDown(1))
            _rightDown = true;
        if (Input.GetMouseButtonUp(1) && _rightDown)
            _parent.OnBombNumberRightClick(Digit);
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Sets the number for this digit based on the total amount of bombs
    /// </summary>
    /// <param name="number"></param>
    public void SetNumber(int number) {
        _number = number;
        ChangeSprite();
    }
    #endregion

    #region Helper methods
    /// <summary>
    ///     Updates the sprite so that it matches the number.
    /// </summary>
    /// <remarks>
    ///     This works for whatever number was given. Negative numbers are set to 9 so that it easily loops:
    ///     <code>
    ///         1 -> 2 -> ... -> 9 -> 0 -> 1 -> ...
    ///         2 -> 1 -> 0 -> 9 -> 8 -> ...
    ///     </code>
    /// </remarks>
    private void ChangeSprite() {
        int index;
        switch (Digit) {
        case EDigit.First:
            index = Math.Abs(_number) % 10;
            break;
        case EDigit.Second:
            if (-10 < _number && _number < 0)
                index = 10;
            else
                index = (Math.Abs(_number) / 10) % 10;
            break;
        case EDigit.Third:
            if (-100 < _number && _number < -9)
                index = 10;
            else
                index = Math.Abs(_number) / 100;
            break;
        default:
            throw new ArgumentOutOfRangeException();
        }
        GetComponent<SpriteRenderer>().sprite = NumberSprites[index];
    }
    #endregion
}
