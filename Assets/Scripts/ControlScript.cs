using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

/// <summary>
///     The control script of the game, defines the logic. Should have been a singleton.
/// </summary>
public class ControlScript : MonoBehaviour {
    #region Structs/Enums
    /// <summary>
    ///     States of the game
    /// </summary>
    public enum EGameState {
        Uninitialized,
        Playing,
        Win,
        Lose
    }
    #endregion

    #region Fields
    /// <summary>
    ///     Width of the next generated grid
    /// </summary>
    [Header("Field properties")]
    public int GridSizeX;

    /// <summary>
    ///     Height of the next generated grid
    /// </summary>
    public int GridSizeY;

    /// <summary>
    ///     Number of mines for the next game
    /// </summary>
    [Range(0, 999)]
    public int TotalMines;

    /// <summary>
    ///     Bounds so the field does not get too small or too big
    /// </summary>
    public float MinScale,
                 MaxScale;

    /// <summary>
    ///     Reference to the scrolling (game view) camera
    /// </summary>
    [Header("Object references")]
    public GameObject ScrollingCamera;

    /// <summary>
    ///     Reference to the screen overlay canvas (doesn't do anything)
    /// </summary>
    public GameObject OverlayCanvas;

    /// <summary>
    ///     Reference to the button that shows the current state of the game (playing or dead)
    /// </summary>
    public Image StateButton;

    /// <summary>
    ///     Reference to all bomb number objects
    /// </summary>
    public BombNumberScript[] BombNumbers;

    /// <summary>
    ///     Reference to all time number objects
    /// </summary>
    public TimeScript[] TimeNumbers;

    /// <summary>
    ///     Prefab of a single tile
    /// </summary>
    public GameObject TilePrefab;

    /// <summary>
    ///     Prefab of some information
    /// </summary>
    public GameObject PersistentInfoPrefab;

    /// <summary>
    ///     Prefab for the options form when the user clicks on the cog
    /// </summary>
    public GameObject OptionsFormPrefab;

    // Private vars
    private PersistentInfoScript _persistentInfo;

    private GameObject OptionsForm;

    private GameObject[,] _tiles;
    private int _numRevealedTiles;
    private int _numBombsLeft;
    private EGameState _gameState = EGameState.Uninitialized;

    private float _startTime;
    private TimeSpan _showTime;
    private bool _updateTime;
    #endregion

    #region Properties
    /// <summary>
    ///     Current state of the game. When set, also changes the actual state
    /// </summary>
    public EGameState GameState {
        set {
            _gameState = value;
            OnGameStateChange();
        }
        get { return _gameState; }
    }

    /// <summary>
    ///     Number of bombs that are left on the field
    /// </summary>
    public int NumBombsLeft {
        get { return _numBombsLeft; }
        set { _numBombsLeft = value; }
    }

    /// <summary>
    ///     Rectangle that gives the current visible part of the field
    /// </summary>
    public Rect VisibleField {
        get {
            var cam = ScrollingCamera.GetComponent<Camera>();
            var bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
            return new Rect(bottomLeft, topRight - bottomLeft);
        }
    }

    /// <summary>
    ///     The controls
    /// </summary>
    public ActionMap<EControls> Controls {
        get { return _persistentInfo.Controls; }
    }
    #endregion

    #region Unity methods
    /// <summary>
    ///     Use this for initialization
    /// </summary>
    void Start() {
        // Check if PersistentInfo exists
        var info = GameObject.FindGameObjectsWithTag("PersistentInfo");
        switch (info.Length) {
        case 0:
            _persistentInfo = Instantiate(PersistentInfoPrefab).GetComponent<PersistentInfoScript>();
            break;
        case 1:
            _persistentInfo = info[0].GetComponent<PersistentInfoScript>();
            CopyPersistentInfo();
            break;
        default:
            //TODO: fix it
            throw new MissingComponentException("Multiple copies of PersistentInfo were found");
        }

        // Assign some variables
        BombNumbers[0].Parent = this;
        BombNumbers[1].Parent = this;
        BombNumbers[2].Parent = this;

        // Set camera bounds
        ScrollingCamera.GetComponent<CameraScript>().Reset();
        CreateTiles();
    }

    /// <summary>
    ///     Update is called once per frame
    /// </summary>
    void Update() {
        // Update timer
        if (_updateTime) {
            _showTime = TimeSpan.FromSeconds(Time.time - _startTime);
            Array.ForEach(TimeNumbers, tnum => tnum.SetTimeSprite(_showTime));
        }
    }
    #endregion

    #region Methods
    /// <summary>
    ///     Places mines randomly on the field
    /// </summary>
    public void PlaceMines() {
        PlaceMines(-1, -1);
    }

    /// <summary>
    ///     Places mines on the field and make sure that the given position is an empty tile
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    public void PlaceMines(int posX, int posY) {
        // Create list of all possible mine locations
        HashSet<Vector2i> tileList = new HashSet<Vector2i>(new Vector2iComparer());
        for (int i = 0; i < GridSizeX; i++) {
            for (int j = 0; j < GridSizeY; j++) {
                tileList.Add(new Vector2i(i, j));
            }
        }

        // Create safe zone if specified
        if (posX >= 0 && posY >= 0) {
            for (int i = posX - 1; i <= posX + 1; i++) {
                for (int j = posY - 1; j <= posY + 1; j++) {
                    tileList.Remove(new Vector2i(i, j));
                }
            }
        }

        // Place rest of mines in field
        int minesPlaced = 0;
        while (minesPlaced < TotalMines && tileList.Count > 0) {
            var r = tileList.ElementAt(Random.Range(0, tileList.Count));
            var tile = _tiles[r.X, r.Y].GetComponent<TileScript>();
            tile.Type = TileScript.EType.Mine;

            // Set numbers around it accordingly
            foreach (var neighbour in tile.Neighbours) {
                neighbour.Number++;
            }

            // Remove placed mine from list
            tileList.Remove(r);
            minesPlaced++;
        }

        // Update number of mines and gamestate
        TotalMines = minesPlaced;
        _numBombsLeft = minesPlaced;
        UpdateBombNumber();
        GameState = EGameState.Playing;
        StartTimer();
    }

    /// <summary>
    ///     Check if field is initialized, place mines if not
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    public void CheckInitialization(int posX, int posY) {
        if (_gameState == EGameState.Uninitialized)
            PlaceMines(posX, posY);
    }

    /// <summary>
    ///     Check for win conditions
    /// </summary>
    /// <returns></returns>
    public EGameState CheckWin() {
        if (_numRevealedTiles >= GridSizeX * GridSizeY - TotalMines)
            return EGameState.Win;
        return _gameState;
    }

    /// <summary>
    ///     Set state as losing (reveals all mines)
    /// </summary>
    public void SetLoseState() {
        _gameState = EGameState.Lose;
        RevealAllMines();
    }

    /// <summary>
    ///     Restart the game
    /// </summary>
    public void Restart() {
        foreach (var tile in _tiles) {
            Destroy(tile);
        }
        _gameState = EGameState.Uninitialized;
        _numRevealedTiles = 0;
        CreateTiles();
        ResetTimer();
        StateButton.sprite = StateButton.GetComponent<StateButtonScript>().DefaultSprite;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    ///     Copy all info from PersistentInfo
    /// </summary>
    private void CopyPersistentInfo() {
        GridSizeX = _persistentInfo.DefaultFieldSizeX;
        GridSizeY = _persistentInfo.DefaultFieldSizeY;
        TotalMines = _persistentInfo.DefaultMines;
    }

    /// <summary>
    ///     Removes any interaction with all tiles
    /// </summary>
    private void DisablePlayField() {
        StopTimer();
        foreach (var tile in _tiles) {
            tile.GetComponent<TileScript>().enabled = false;
            tile.GetComponent<TileScript>().Enabled = false;
        }
    }

    /// <summary>
    ///     Re-enables all interaction with tiles
    /// </summary>
    private void EnablePlayField() {
        foreach (var tile in _tiles) {
            tile.GetComponent<TileScript>().Enabled = true;
        }
        if (_gameState == EGameState.Playing)
            ResumeTimer();
    }

    /// <summary>
    ///     Initializes <see cref="_tiles"/> and places them centralized around (0,0)
    /// </summary>
    private void CreateTiles() {
        // Reset scrolling camera
        var camScript = ScrollingCamera.GetComponent<CameraScript>();
        camScript.Reset();

        // Find scale to draw tiles with
        var tileRenderSize = TilePrefab.GetComponent<Renderer>().bounds.size;
        var fieldRenderSize = new Vector3(GridSizeX * tileRenderSize.x, GridSizeY * tileRenderSize.y);
        var cameraWorldViewSize = VisibleField;
        // get scale to draw tiles with
        var scale = Mathf.Clamp(Math.Min(cameraWorldViewSize.width / fieldRenderSize.x,
                                         cameraWorldViewSize.height / fieldRenderSize.y),
                                MinScale,
                                MaxScale);
        tileRenderSize *= scale;
        fieldRenderSize *= scale;

        // Update scrolling camera
        camScript.MaxUpperView = 0f;
        camScript.MaxLeftView = Math.Min(-fieldRenderSize.x / 2f, cameraWorldViewSize.xMin);
        camScript.MaxRightView = Math.Max(fieldRenderSize.x / 2f, cameraWorldViewSize.xMax);
        camScript.MaxLowerView = Math.Min(-fieldRenderSize.y, cameraWorldViewSize.yMin);

        // Create new container
        _tiles = new GameObject[GridSizeX, GridSizeY];
        for (int i = 0; i < GridSizeX; i++) {
            for (int j = 0; j < GridSizeY; j++) {
                // Instantiate tiles
                _tiles[i, j] = Instantiate(TilePrefab,
                                           new Vector3((i + .5f) * tileRenderSize.x - fieldRenderSize.x / 2f,
                                                       -j * tileRenderSize.y),
                                           new Quaternion());
                var tile = _tiles[i, j].GetComponent<TileScript>();
                tile.Parent = this;
                tile.Container = _tiles;
                tile.GridPos = new Vector2i(i, j);
                tile.LocalScale = scale;
            }
        }
        // Give every tile their neighbours
        for (int i = 0; i < GridSizeX; i++) {
            for (int j = 0; j < GridSizeY; j++) {
                for (int ii = i - 1; ii <= i + 1; ii++) {
                    for (int jj = j - 1; jj <= j + 1; jj++) {
                        if (i == ii && j == jj)
                            continue;
                        try {
                            _tiles[i, j].GetComponent<TileScript>().NewNeighbour =
                                _tiles[ii, jj].GetComponent<TileScript>();
                        }
                        catch (IndexOutOfRangeException) { } //ignore
                    }
                }
            }
        }

        // Set number of mines back to original value
        _numBombsLeft = TotalMines;
        UpdateBombNumber();
    }

    /// <summary>
    ///     Starts the timer
    /// </summary>
    private void StartTimer() {
        _startTime = Time.time;
        ResetTimer();
    }

    /// <summary>
    ///     Resumes the timer from a pauzed state
    /// </summary>
    private void ResumeTimer() {
        _startTime += (float)(Time.time - (_startTime + _showTime.TotalSeconds));
        _updateTime = true;
    }

    /// <summary>
    ///     Stops the timer
    /// </summary>
    private void StopTimer() {
        _showTime = TimeSpan.FromSeconds(Time.time - _startTime);
        _updateTime = false;
    }

    /// <summary>
    ///     Sets the timer to 0 without starting it
    /// </summary>
    private void ResetTimer() {
        _showTime = new TimeSpan();
        _updateTime = false;
    }

    /// <summary>
    ///     Reveals all the mines
    /// </summary>
    private void RevealAllMines() {
        foreach (var tile in _tiles) {
            tile.GetComponent<TileScript>().RevealAsLoseState();
        }
    }

    /// <summary>
    ///     Updates the bomb number
    /// </summary>
    private void UpdateBombNumber() {
        Array.ForEach(BombNumbers, bnum => bnum.SetNumber(_numBombsLeft));
    }
    #endregion

    #region Events
    /// <summary>
    ///     Is called whenever GameState is altered
    /// </summary>
    private void OnGameStateChange() {
        switch (_gameState) {
        case EGameState.Lose: // Actions to do when lost
            //StateButton.LoseState();
            StateButton.sprite = StateButton.GetComponent<StateButtonScript>().LoseSprite;
            RevealAllMines();
            DisablePlayField();
            break;
        case EGameState.Win: // Actions to do when won
            //StateButton.WinState();
            StateButton.sprite = StateButton.GetComponent<StateButtonScript>().WinSprite;
            DisablePlayField();
            break;
        }
    }

    /// <summary>
    ///     What to do when a tile has been clicked
    /// </summary>
    /// <param name="sender"></param>
    public void OnTileClick(GameObject sender) {
        var tile = sender.GetComponent<TileScript>();

        // Place mines if field is uninitialized
        if (_gameState == EGameState.Uninitialized)
            PlaceMines(tile.GridPos.X, tile.GridPos.Y);

        // Don't do anything unless the game is playing
        if (_gameState != EGameState.Playing)
            return;

        tile.Revealed = true;
        if (tile.Revealed && tile.Type == TileScript.EType.Mine) {
            GameState = EGameState.Lose;
            sender.GetComponent<TileScript>().SetAsDeadMine();
        }
        GameState = CheckWin();
    }

    /// <summary>
    ///     What to do when a tile was revealed
    /// </summary>
    /// <param name="sender"></param>
    public void OnTileReveal(GameObject sender) {
        _numRevealedTiles++;
    }

    /// <summary>
    ///     What to do when a tile has been right clicked
    /// </summary>
    /// <param name="sender"></param>
    public void OnTileRightClick(GameObject sender) {
        var tile = sender.GetComponent<TileScript>();

        _numBombsLeft += tile.Flagged.ToIntSign();
        tile.Flagged = !tile.Flagged;
        UpdateBombNumber();
    }

    /// <summary>
    ///     What to do when a tile has been middle clicked
    /// </summary>
    /// <param name="sender"></param>
    public void OnTileMiddleClick(GameObject sender) {
        var tile = sender.GetComponent<TileScript>();

        if ((int)tile.Number == tile.Neighbours.Count(neighbour => neighbour.Flagged))
            tile.Neighbours.ForEach(neighbour => neighbour.Revealed = true);

        GameState = CheckWin();
    }

    /// <summary>
    ///     What to do if a bomb number was clicked
    /// </summary>
    /// <param name="digit"></param>
    public void OnBombNumberClick(BombNumberScript.EDigit digit) {
        switch (digit) {
        case BombNumberScript.EDigit.First:
            TotalMines += (TotalMines % 10 != 9 ? 1 : -9);
            break;
        case BombNumberScript.EDigit.Second:
            TotalMines += ((TotalMines / 10) % 10 != 9 ? 10 : -90);
            break;
        case BombNumberScript.EDigit.Third:
            TotalMines += (TotalMines / 100 != 9 ? 100 : -900);
            break;
        default:
            throw new ArgumentOutOfRangeException("digit");
        }
        _numBombsLeft = TotalMines;
        UpdateBombNumber();
    }

    /// <summary>
    ///     What to do if a bomb number was right clicked
    /// </summary>
    /// <param name="digit"></param>
    public void OnBombNumberRightClick(BombNumberScript.EDigit digit) {
        switch (digit) {
        case BombNumberScript.EDigit.First:
            TotalMines -= (TotalMines % 10 != 0 ? 1 : -9);
            break;
        case BombNumberScript.EDigit.Second:
            TotalMines -= ((TotalMines / 10) % 10 != 0 ? 10 : -90);
            break;
        case BombNumberScript.EDigit.Third:
            TotalMines -= (TotalMines / 100 != 0 ? 100 : -900);
            break;
        default:
            throw new ArgumentOutOfRangeException("digit");
        }
        _numBombsLeft = TotalMines;
        UpdateBombNumber();
    }

    /// <summary>
    ///     What to do when the options button is clicked
    /// </summary>
    public void OnOptionsButtonClick() {
        // Check if an options form is already present
        if (OptionsForm != null) {
            OnOptionsButtonCancelClick();
            return;
        }
        // Disable playing field
        DisablePlayField();
        OptionsForm = Instantiate(OptionsFormPrefab);
        OptionsForm.transform.SetParent(OverlayCanvas.transform, false);
        var buttons = OptionsForm.GetComponentsInChildren<Button>();
        foreach (var button in buttons) {
            switch (button.name) {
            case "ButtonOK":
                button.onClick.AddListener(OnOptionsButtonOKClick);
                break;
            case "ButtonCancel":
                button.onClick.AddListener(OnOptionsButtonCancelClick);
                break;
            default:
                throw new MissingComponentException("Object does not belong to OptionsForm");
            }
        }
    }

    /// <summary>
    ///     What to do if the OK button was pressed on the options screen
    /// </summary>
    public void OnOptionsButtonOKClick() {
        var ofs = OptionsForm.GetComponent<OptionsFormScript>();
        TotalMines = ofs.Mines;
        GridSizeX = ofs.FieldWidth;
        GridSizeY = ofs.FieldHeight;

        Destroy(OptionsForm);
        EnablePlayField();
    }

    /// <summary>
    ///     What to do when the Cancel button was pressed on the options screen
    /// </summary>
    public void OnOptionsButtonCancelClick() {
        Destroy(OptionsForm);
        EnablePlayField();
    }
    #endregion
}
