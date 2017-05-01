using System;
using UnityEngine;
using System.Collections;

/// <summary>
///     Handles the logic of the time counters
/// </summary>
public class TimeScript : MonoBehaviour {
    #region Enums/Structs
    /// <summary>
    ///     Different digits in the time counter
    /// </summary>
    public enum ETimeNumber {
        Hundred0,
        Hundred00,
        Second0,
        Second00,
        Minute0,
        Minute00,
        Hour0,
        Hour00
    }
    #endregion

    #region Fields
    /// <summary>
    ///     Which digit does this tile represent?
    /// </summary>
    public ETimeNumber Type;

    /// <summary>
    ///     Reference to all of the number sprites
    /// </summary>
    public Sprite[] NumberSprites;
    #endregion

    /// <summary>
    ///     Sets the correct sprite for this digit
    /// </summary>
    /// <param name="elapsed"></param>
    public void SetTimeSprite(TimeSpan elapsed) {
        var sr = GetComponent<SpriteRenderer>();
        switch (Type) {
        case ETimeNumber.Hundred0:
            sr.sprite = NumberSprites[(elapsed.Milliseconds / 10) % 10];
            break;
        case ETimeNumber.Hundred00:
            sr.sprite = NumberSprites[(elapsed.Milliseconds / 100) % 10];
            break;
        case ETimeNumber.Second0:
            sr.sprite = NumberSprites[elapsed.Seconds % 10];
            break;
        case ETimeNumber.Second00:
            sr.sprite = NumberSprites[(elapsed.Seconds / 10) % 10];
            break;
        case ETimeNumber.Minute0:
            sr.sprite = NumberSprites[elapsed.Minutes % 10];
            break;
        case ETimeNumber.Minute00:
            sr.sprite = NumberSprites[(elapsed.Minutes / 10) % 10];
            break;
        case ETimeNumber.Hour0:
            sr.sprite = NumberSprites[elapsed.Hours % 10];
            break;
        case ETimeNumber.Hour00:
            sr.sprite = NumberSprites[(elapsed.Hours / 10) % 10];
            break;
        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}
