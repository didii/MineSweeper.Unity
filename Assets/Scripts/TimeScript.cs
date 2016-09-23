using System;
using UnityEngine;
using System.Collections;

public class TimeScript : MonoBehaviour {

    #region Enums/Structs

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
    // Public fields
    public ETimeNumber Type;
    public Sprite[] NumberSprites;
    #endregion

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void SetTimeSprite(TimeSpan elapsed) {
        var sr = this.GetComponent<SpriteRenderer>();
        switch (Type) {
            case ETimeNumber.Hundred0:
                sr.sprite = NumberSprites[(elapsed.Milliseconds/10)%10];
                break;
            case ETimeNumber.Hundred00:
                sr.sprite = NumberSprites[(elapsed.Milliseconds/100)%10];
                break;
            case ETimeNumber.Second0:
                sr.sprite = NumberSprites[elapsed.Seconds%10];
                break;
            case ETimeNumber.Second00:
                sr.sprite = NumberSprites[(elapsed.Seconds/10)%10];
                break;
            case ETimeNumber.Minute0:
                sr.sprite = NumberSprites[elapsed.Minutes%10];
                break;
            case ETimeNumber.Minute00:
                sr.sprite = NumberSprites[(elapsed.Minutes/10)%10];
                break;
            case ETimeNumber.Hour0:
                sr.sprite = NumberSprites[elapsed.Hours%10];
                break;
            case ETimeNumber.Hour00:
                sr.sprite = NumberSprites[(elapsed.Hours/10)%10];
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
