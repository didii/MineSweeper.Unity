using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

static public class Utility {

    public enum EDirection {
        Up,
        Right,
        Down,
        Left
    }

    static public Vector2i Index1DTo2D(int index, int width) {
        return new Vector2i(index%width,index/width);
    }

    static public int Index2DTo1D(Vector2i index, int width) {
        return index.X + index.Y*width;
    }

    static public Vector3 Divide(this Vector3 obj1, Vector3 obj2) {
        return new Vector3(obj1.x/obj2.x, obj1.y/obj2.y, obj1.z == 0 && obj2.z == 0 ? 0 : obj1.z/obj2.z);
    }


    static public bool IsBetween(float val, float lower, float upper) {
        return lower < val && val < upper;
    }

    // Bool conversion to int (false=0, true=1)
    static public int ToInt(this bool val) {
        return val ? 1 : 0;
    }
    // Bool conversion to int (false=-1, true=1)
    static public int ToIntSign(this bool val) {
        return val ? 1 : -1;
    }

    // Multiplies the TimeSpan with the given integer
    static public TimeSpan Multiply(this TimeSpan ts, int num) {
        return TimeSpan.FromTicks(ts.Ticks*num);
    }
    // Multiplier the TimeSpan with the given float
    static public TimeSpan Multiply(this TimeSpan ts, float num) {
        return TimeSpan.FromTicks((long)(ts.Ticks*num));
    }
}