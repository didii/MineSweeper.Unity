using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileScript : MonoBehaviour {

    #region Enums
    public enum EType {
        Safe, Mine
    }

    public enum ENumber {
        Empty, One, Two, Three, Four, Five, Six, Seven, Eight
    }
    #endregion

    #region Fields
    // Textures
    public Sprite TileUnknown, TilePressed;
    public Sprite Tile0, Tile1, Tile2, Tile3, Tile4, Tile5, Tile6, Tile7, Tile8;
    public Sprite TileMine, TileFlag, TileQuestion, TileQuestionPressed;
    public Sprite TileDead, TileWrong;
    public float LocalScale = 1;

    // Tile properties
    private EType _type = EType.Safe;
    private ENumber _num = ENumber.Empty;

    private bool _flagged = false;
    private bool _revealed = false;
    private Sprite _sprRevealed;

    private SpriteRenderer _sprRend = null;

    private bool _enabled = true;

    // Tile container
    private ControlScript _parent;
    private GameObject[,] _container;
    private Vector2i _gridPos;
    private List<TileScript> _neighbours = new List<TileScript>();

    // Tile mouse fields
    private bool _leftDown;
    private bool _rightDown;
    private bool _middleDown;
    #endregion

    #region Properties
    // Tile properties
    public EType Type {
        get { return _type; }
        set {
            _type = value;
            AssignTexture();
        }
    }

    public ENumber Number {
        get { return _num; }
        set {
            _num = value;
            AssignTexture();
        }
    }

    // Tile container
    public ControlScript Parent {
        get { return _parent; }
        set { _parent = value; }
    }

    public GameObject[,] Container {
        get { return _container; }
        set { _container = value; }
    }

    public Vector2i GridPos {
        get { return _gridPos; }
        set { _gridPos = value; }
    }

    public bool Revealed {
        get { return _revealed; }
        set {
            if (_revealed && !value) {
                Unreveal();
                _revealed = false;
            } else if (!_revealed && value) {
                Reveal();
            }
        }
    }

    public bool Flagged {
        get { return _flagged; }
        set {
            if (value)
                Flag();
            else
                Unflag();
        }
    }

    public List<TileScript> Neighbours {
        get { return _neighbours; }
    }

    public TileScript NewNeighbour {
        set { _neighbours.Add(value); }
    }

    public bool Enabled {
        get { return _enabled; }
        set { _enabled = value; }
    }
    #endregion

    #region Unity Methods
    // First initailization
    void Awake() {
        _sprRevealed = Tile0; // default reveal tile
    }

    // Use this for initialization
    void Start () {
	    _sprRend = GetComponent<SpriteRenderer>();
	    _sprRend.sprite = TileUnknown;
        this.transform.localScale = new Vector3(LocalScale, LocalScale, LocalScale);
	}
	
	// Update is called once per frame
	void Update () {
        // Left mouse button released
	    if (_parent.Controls[EControls.TileReleased].IsActive() && _leftDown) {
	        _leftDown = false;
            if (!_revealed)
                TileRelease();
	    }

        // Right mouse button released
	    if (_parent.Controls[EControls.TileFlagReleased].IsActive() && _rightDown)
	        _rightDown = false;

        // Middle mouse button released
	    if (_parent.Controls[EControls.TileSquareReleased].IsActive() && _middleDown) {
	        _middleDown = false;
	        foreach (var neighbour in _neighbours.Where(neighbour => !neighbour._flagged && !neighbour._revealed))
	            neighbour.TileRelease();
	    }
	}

    // Mouse actions
    void OnMouseOver() {
        if (!_enabled)
            return;

        // Left mouse button behaviour
        if (!_revealed && _parent.Controls[EControls.TilePressed].IsActive()) {
            _leftDown = true;
            TilePress();
        }
        if (_parent.Controls[EControls.TileReleased].IsActive() && _leftDown)
            _parent.OnTileClick(this.gameObject);

        // Right mouse button behaviour
        if (!_revealed && _parent.Controls[EControls.TileFlagPressed].IsActive())
            _rightDown = true;
        if (_parent.Controls[EControls.TileFlagReleased].IsActive() && _rightDown)
            _parent.OnTileRightClick(this.gameObject);

        // Middle mouse button behaviour
        if (_revealed && _parent.Controls[EControls.TileSquarePressed].IsActive()) {
            _middleDown = true;
            foreach (var neighbour in _neighbours.Where(neighbour => !neighbour._flagged && !neighbour._revealed))
                neighbour.TilePress();
        }
        if (_parent.Controls[EControls.TileSquareReleased].IsActive() && _middleDown)
            _parent.OnTileMiddleClick(this.gameObject);
    }
    #endregion

    #region Methods
    public void ClearNeighbours() {
        _neighbours.Clear();
    }

    // Reveals current tile and nearby ones if ENumber == Empty
    public void Reveal() {
        // skip if already revealed or flagged
        if (_revealed || _flagged)
            return;

        _revealed = true;
        _sprRend.sprite = _sprRevealed;
        if (_type != EType.Mine && _num == ENumber.Empty)
            Neighbours.ForEach(neighbour => neighbour.Revealed = true);

        _parent.OnTileReveal(this.gameObject);
    }

    public void SetAsDeadMine() {
        _sprRend.sprite = TileDead;
    }

    public void Unreveal() {
        // Skip if already hidden
        if (!_revealed)
            return;

        _revealed = false;
        _sprRend.sprite = TileUnknown;
    }

    public void RevealAsLoseState() {
        if (_type == EType.Mine && !_flagged && !_revealed) {
            _sprRend.sprite = TileMine;
        } else if (_type != EType.Mine && _flagged) {
            _sprRend.sprite = TileWrong;
        }
    }

    // Swap the flag state
    public void FlagSwap() {
        if (_flagged)
            Unflag();
        else
            Flag();
    }

    public void Flag() {
        // skip if already flagged or revealed
        if (_flagged || _revealed) return;

        _flagged = true;
        _sprRend.sprite = TileFlag;
    }

    public void Unflag() {
        // skip if already unflagged
        if (!_flagged) return;

        _flagged = false;
        _sprRend.sprite = TileUnknown;
    }
    #endregion

    #region Helper functions
    private void TilePress() {
        if (!_revealed && !_flagged)
            _sprRend.sprite = TilePressed;
    }

    private void TileRelease() {
        if (!_revealed && !_flagged)
            _sprRend.sprite = TileUnknown;
    }

    private void AssignTexture() {
        switch (_type) {
            case EType.Safe:
                switch (_num) {
                    case ENumber.Empty:
                        _sprRevealed = Tile0;
                        break;
                    case ENumber.One:
                        _sprRevealed = Tile1;
                        break;
                    case ENumber.Two:
                        _sprRevealed = Tile2;
                        break;
                    case ENumber.Three:
                        _sprRevealed = Tile3;
                        break;
                    case ENumber.Four:
                        _sprRevealed = Tile4;
                        break;
                    case ENumber.Five:
                        _sprRevealed = Tile5;
                        break;
                    case ENumber.Six:
                        _sprRevealed = Tile6;
                        break;
                    case ENumber.Seven:
                        _sprRevealed = Tile7;
                        break;
                    case ENumber.Eight:
                        _sprRevealed = Tile8;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case EType.Mine:
                _sprRevealed = TileMine;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}
