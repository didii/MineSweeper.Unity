using System;
using UnityEngine;
using System.Collections;

public class BombNumberScript : MonoBehaviour {

    public enum EDigit {
        First, Second, Third
    }

    #region Fields

    public EDigit Digit;
    public Sprite[] NumberSprites;
    private int _number;
    private ControlScript _parent;
    private bool _leftDown, _rightDown;
    #endregion

    #region Properties

    public ControlScript Parent {
        set { _parent = value; }
    }
    #endregion

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void LateUpdate () {
	    if (Input.GetMouseButtonUp(0))
	        _leftDown = false;
	    if (Input.GetMouseButtonUp(1))
	        _rightDown = false;
	}

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

    public void SetNumber(int number) {
        _number = number;
        ChangeSprite();
    }

    public void ChangeNumber(int diff) {
        _number += diff;
        ChangeSprite();
    }

    private void ChangeSprite() {
        int index;
        switch (Digit) {
            case EDigit.First:
                index = Math.Abs(_number)%10;
                break;
            case EDigit.Second:
                if (-10 < _number && _number < 0)
                    index = 10;
                else
                    index = (Math.Abs(_number)/10)%10;
                break;
            case EDigit.Third:
                if (-100 < _number && _number < -9)
                    index = 10;
                else
                    index = Math.Abs(_number)/100;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        this.GetComponent<SpriteRenderer>().sprite = NumberSprites[index];
    }
}
