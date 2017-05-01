#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// No worries about this one
/// </summary>
public class TestScript : MonoBehaviour {

    public void DoSomething() {
        Debug.Log("30s * 3 = " + TimeSpan.FromSeconds(30).Multiply(3).TotalSeconds + "s");
    }
}

[CustomEditor(typeof(TestScript))]
public class TestEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TestScript obj = (TestScript)target;
        if (GUILayout.Button("Click MEEE!"))
            obj.DoSomething();
    }
}
#endif