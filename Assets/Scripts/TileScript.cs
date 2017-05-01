using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///     Handles the logic of a single tile
/// </summary>
public class TileScript : MonoBehaviour {
    #region Enums
    /// <summary>
    ///     Type of tile (bomb or mine)
    /// </summary>
    public enum EType {
        Safe,
        Mine
    }

    /// <summary>
    ///     Number of the tile (can be correctly casted to an integer)
    /// </summary>
    public enum ENumber {
        Empty,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }
    #endregion

    #region Fields
    /// <summary>
    ///     References to unrevealed tiles
    /// </summary>
    public Sprite TileUnknown,
                  TilePressed;

    /// <summary>
    ///     References to revealed empty tiles
    /// </summary>
    public Sprite Tile0,
                  Tile1,
                  Tile2,
                  Tile3,
                  Tile4,
                  Tile5,
                  Tile6,
                  Tile7,
                  Tile8;

    /// <summary>
    ///     References to mine-related tiles
    /// </summary>
    public Sprite TileMine,
                  TileFlag,
                  TileQuestion,
                  TileQuestionPressed;

    /// <summary>
    ///     References to revealed or wrongly placed flag tiles
    /// </summary>
    public Sprite TileDead,
                  TileWrong;

    public float LocalScale = 1;

    /// <summary>
    ///     What type is this
    /// </summary>
    private EType _type = EType.Safe;

    private ENumber _num = ENumber.Empty;

    private bool _flagged;
    private bool _revealed;
    private Sprite _sprRevealed;

    private SpriteRenderer _sprRend;

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
    /// <summary>
    ///     What type is this tile (safe or not)?
    /// </summary>
    public EType Type {
        get { return _type; }
        set {
            _type = value;
            AssignTexture();
        }
    }

    /// <summary>
    ///     What number does this tile have? Only applicable if not a mine
    /// </summary>
    public ENumber Number {
        get { return _num; }
        set {
            _num = value;
            AssignTexture();
        }
    }

    /// <summary>
    ///     Reference to the control script
    /// </summary>
    public ControlScript Parent {
        get { return _parent; }
        set { _parent = value; }
    }

    /// <summary>
    ///     Reference to the tiles container
    /// </summary>
    public GameObject[,] Container {
        get { return _container; }
        set { _container = value; }
    }

    /// <summary>
    ///     Position of this tile within the grid
    /// </summary>
    public Vector2i GridPos {
        get { return _gridPos; }
        set { _gridPos = value; }
    }

    /// <summary>
    ///     Is the tile revealed? Setting it will apply its value.
    /// </summary>
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

    /// <summary>
    ///     Is the tile flagged? Setting it will apply or remove the flag
    /// </summary>
    public bool Flagged {
        get { return _flagged; }
        set {
            if (value)
                Flag();
            else
                Unflag();
        }
    }

    /// <summary>
    ///     All neighbours of this tile
    /// </summary>
    public List<TileScript> Neighbours {
        get { return _neighbours; }
    }

    /// <summary>
    ///     Adds a neighbour. Strange that this is a property...
    /// </summary>
    public TileScript NewNeighbour {
        set { _neighbours.Add(value); }
    }

    /// <summary>
    ///     Should the tile react to user input
    /// </summary>
    public bool Enabled {
        get { return _enabled; }
        set { _enabled = value; }
    }
    #endregion

    #region Unity Methods
    /// <summary>
    ///     First initailization, called before all Start methods
    /// </summary>
    void Awake() {
        _sprRevealed = Tile0; // default reveal tile
    }

    /// <summary>
    ///     Use this for initialization
    /// </summary>
    void Start() {
        _sprRend = GetComponent<SpriteRenderer>();
        _sprRend.sprite = TileUnknown;
        transform.localScale = new Vector3(LocalScale, LocalScale, LocalScale);
    }

    /// <summary>
    ///     Update is called once per frame
    /// </summary>
    void Update() {
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

    /// <summary>
    ///     Is called every frame that the mouse is over this objects bounding box
    /// </summary>
    void OnMouseOver() {
        if (!_enabled)
            return;

        // Left mouse button behaviour
        if (!_revealed && _parent.Controls[EControls.TilePressed].IsActive()) {
            _leftDown = true;
            TilePress();
        }
        if (_parent.Controls[EControls.TileReleased].IsActive() && _leftDown)
            _parent.OnTileClick(gameObject);

        // Right mouse button behaviour
        if (!_revealed && _parent.Controls[EControls.TileFlagPressed].IsActive())
            _rightDown = true;
        if (_parent.Controls[EControls.TileFlagReleased].IsActive() && _rightDown)
            _parent.OnTileRightClick(gameObject);

        // Middle mouse button behaviour
        if (_revealed && _parent.Controls[EControls.TileSquarePressed].IsActive()) {
            _middleDown = true;
            foreach (var neighbour in _neighbours.Where(neighbour => !neighbour._flagged && !neighbour._revealed))
                neighbour.TilePress();
        }
        if (_parent.Controls[EControls.TileSquareReleased].IsActive() && _middleDown)
            _parent.OnTileMiddleClick(gameObject);
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Removes all neighbours
    /// </summary>
    public void ClearNeighbours() {
        _neighbours.Clear();
    }

    /// <summary>
    ///     Reveals current tile and nearby ones if ENumber == Empty
    /// </summary>
    public void Reveal() {
        // skip if already revealed or flagged
        if (_revealed || _flagged)
            return;

        _revealed = true;
        _sprRend.sprite = _sprRevealed;
        if (_type != EType.Mine && _num == ENumber.Empty)
            Neighbours.ForEach(neighbour => neighbour.Revealed = true);

        _parent.OnTileReveal(gameObject);
    }

    /// <summary>
    ///     Used when the user reveals this mine
    /// </summary>
    public void SetAsDeadMine() {
        _sprRend.sprite = TileDead;
    }

    /// <summary>
    ///     Hides the tile again
    /// </summary>
    public void Unreveal() {
        // Skip if already hidden
        if (!_revealed)
            return;

        _revealed = false;
        _sprRend.sprite = TileUnknown;
    }

    /// <summary>
    ///     Reveals the tile as if the game was lost (only mines are revealed, empty tiles and correctly flagged mines are
    ///     unaffected, incorrectly flagged mines are tagged)
    /// </summary>
    public void RevealAsLoseState() {
        if (_type == EType.Mine && !_flagged && !_revealed) {
            _sprRend.sprite = TileMine;
        } else if (_type != EType.Mine && _flagged) {
            _sprRend.sprite = TileWrong;
        }
    }

    /// <summary>
    ///     Swap the flag state
    /// </summary>
    public void FlagSwap() {
        if (_flagged)
            Unflag();
        else
            Flag();
    }

    /// <summary>
    ///     Forces a flag on the tile
    /// </summary>
    public void Flag() {
        // skip if already flagged or revealed
        if (_flagged || _revealed)
            return;

        _flagged = true;
        _sprRend.sprite = TileFlag;
    }

    /// <summary>
    ///     Forces to remove the flag
    /// </summary>
    public void Unflag() {
        // skip if already unflagged
        if (!_flagged)
            return;

        _flagged = false;
        _sprRend.sprite = TileUnknown;
    }
    #endregion

    #region Helper functions
    /// <summary>
    ///     Visual feedback when the user clicks a mine
    /// </summary>
    private void TilePress() {
        if (!_revealed && !_flagged)
            _sprRend.sprite = TilePressed;
    }

    /// <summary>
    ///     Visual feedback for when the user releases the click
    /// </summary>
    private void TileRelease() {
        if (!_revealed && !_flagged)
            _sprRend.sprite = TileUnknown;
    }

    /// <summary>
    ///     Assigns the correct texture for when the tile is revealed
    /// </summary>
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
