using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
///     Utility functions
/// </summary>
public static class Utility {
    /// <summary>
    ///     Bool conversion to int (false=-1, true=1)
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static int ToIntSign(this bool val) {
        return val ? 1 : -1;
    }

    /// <summary>
    ///     Multiplies the TimeSpan with the given integer
    /// </summary>
    /// <param name="ts"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static TimeSpan Multiply(this TimeSpan ts, int num) {
        return TimeSpan.FromTicks(ts.Ticks * num);
    }

    /// <summary>
    ///     Multiplier the TimeSpan with the given float
    /// </summary>
    /// <param name="ts"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static TimeSpan Multiply(this TimeSpan ts, float num) {
        return TimeSpan.FromTicks((long)(ts.Ticks * num));
    }
}