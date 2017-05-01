using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Behaviour for the camera
/// </summary>
public class CameraScript : MonoBehaviour {

    #region Fields
    /// <summary>
    /// The speed at which the camera scrolls through the field
    /// </summary>
    public float ScrollSpeed;
    /// <summary>
    /// How wide the area at the side of the screen should be before the camera moves
    /// </summary>
    public int ScrollAreaSize;

    private bool _allowScrollUp, _allowScrollDown, _allowScrollLeft, _allowScrollRight;
    private float _maxUp, _maxDown, _maxLeft, _maxRight;
    private Vector3 _initialPosition;
    #endregion

    #region Properties
    /// <summary>
    /// Is scrolling up allowed?
    /// </summary>
    public bool AllowScrollUp {
        get { return _allowScrollUp; }
        set {
            _allowScrollUp = value;
            //UpperScrollBox.GetComponent<BoxCollider2D>().enabled = value;
        }
    }

    /// <summary>
    /// Is scrolling down allowed?
    /// </summary>
    public bool AllowScrollDown {
        get { return _allowScrollDown; }
        set {
            _allowScrollDown = value;
            //LowerScrollBox.GetComponent<BoxCollider2D>().enabled = value;
        }
    }

    /// <summary>
    /// Is scrolling left allowed?
    /// </summary>
    public bool AllowScrollLeft {
        get { return _allowScrollLeft; }
        set {
            _allowScrollLeft = value;
            //LeftScrollBox.GetComponent<BoxCollider2D>().enabled = value;
        }
    }

    /// <summary>
    /// Is scrolling right allowed?
    /// </summary>
    public bool AllowScrollRight {
        get { return _allowScrollRight; }
        set {
            _allowScrollRight = value;
            //RightScrollBox.GetComponentInParent<BoxCollider2D>().enabled = value;
        }
    }

    /// <summary>
    /// Highest possible value to scroll up to
    /// </summary>
    public float MaxUpperView {
        get { return _maxUp; }
        set {
            _maxUp = value;
            AllowAllScrollingDirections();
            FitViewToBoundaries();
        }
    }

    /// <summary>
    /// Lowest possible value to scroll down to
    /// </summary>
    public float MaxLowerView {
        get { return _maxDown; }
        set {
            _maxDown = value;
            AllowAllScrollingDirections();
            FitViewToBoundaries();
        }
    }

    /// <summary>
    /// Lowest possible value to scroll left to
    /// </summary>
    public float MaxLeftView {
        get { return _maxLeft; }
        set {
            _maxLeft = value;
            AllowAllScrollingDirections();
            FitViewToBoundaries();
        }
    }

    /// <summary>
    /// Highest possible value to scroll right to
    /// </summary>
    public float MaxRightView {
        get { return _maxRight; }
        set {
            _maxRight = value;
            AllowAllScrollingDirections();
            FitViewToBoundaries();
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets default values needed before handling the object before anything else
    /// </summary>
    void Awake() {
        var cam = this.GetComponentInParent<Camera>();
        _initialPosition = cam.transform.position;
        MaxLeftView = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        MaxLowerView = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).y;
        MaxRightView = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).x;
        MaxUpperView = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).y;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        AllowScrollUp = true;
        AllowScrollDown = true;
        AllowScrollLeft = true;
        AllowScrollRight = true;
    }

    /// <summary>
    /// Update is called once per frame. Primarily handles input.
    /// </summary>
    void Update() {
        // Get some values
        var mPos = Input.mousePosition;
        var cam = GetComponent<Camera>();
        var bottomLeft = cam.ViewportToScreenPoint(new Vector3(0, 0, cam.nearClipPlane)); //get bounds
        var topRight = cam.ViewportToScreenPoint(new Vector3(1, 1, cam.nearClipPlane));   //^
        var speed = ScrollSpeed * Time.deltaTime;

        // Check input
        if (bottomLeft.x < mPos.x && mPos.x < bottomLeft.x + ScrollAreaSize && _allowScrollLeft) {
            this.transform.Translate(-speed, 0f, 0f);
            AllowScrollRight = true;
        }
        if (bottomLeft.y < mPos.y && mPos.y < bottomLeft.y + ScrollAreaSize && _allowScrollDown) {
            this.transform.Translate(0f, -speed, 0);
            AllowScrollUp = true;
        }
        if (topRight.x - ScrollAreaSize < mPos.x && mPos.x < topRight.x && _allowScrollRight) {
            this.transform.Translate(speed, 0f, 0f);
            AllowScrollLeft = true;
        }
        if (topRight.y - ScrollAreaSize < mPos.y && mPos.y < topRight.y && _allowScrollUp) {
            this.transform.Translate(0f, speed, 0f);
            AllowScrollDown = true;
        }
        FitViewToBoundaries();
    }

    /// <summary>
    /// Moves the camera to its initial position
    /// </summary>
    public void Reset() {
        this.GetComponent<Camera>().transform.position = _initialPosition;
        //UnityEditor.PrefabUtility.ResetToPrefabState(this.gameObject);
    }

    #endregion

    #region Helper Methods
    /// <summary>
    /// Less copy-pastas
    /// </summary>
    private void AllowAllScrollingDirections() {
        _allowScrollDown = true;
        _allowScrollLeft = true;
        _allowScrollRight = true;
        _allowScrollUp = true;
    }

    /// <summary>
    /// Makes sure the view does not go out of bounds
    /// </summary>
    private void FitViewToBoundaries() {
        // Get some needed values
        var camera = GetComponent<Camera>();
        var bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        var topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        if (bottomLeft.x < _maxLeft) {
            camera.transform.Translate(new Vector3(_maxLeft - bottomLeft.x, 0f, 0f));
            AllowScrollLeft = false;
        }
        if (bottomLeft.y < _maxDown) {
            camera.transform.Translate(new Vector3(0f, _maxDown - bottomLeft.y, 0f));
            AllowScrollDown = false;
        }
        if (topRight.x > _maxRight) {
            camera.transform.Translate(new Vector3(_maxRight - topRight.x, 0f, 0f));
            AllowScrollRight = false;
        }
        if (topRight.y > _maxUp) {
            camera.transform.Translate(new Vector3(0f, _maxUp - topRight.y, 0f));
            AllowScrollUp = false;
        }
    }

    #endregion

}
