using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NPuzzle : MonoBehaviour {

    public float[] gridsY = {0, 0, 0,
                             1, 1, 1,
                             2, 2, 2};
    public float[] gridsX = {0, 1, 2,
                             0, 1, 2,
                             0, 1, 2};

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    public void UpdateGrid()
    {
        Debug.Log("GilLog - NPuzzle::UpdateGrid " + gridsY.Length);

        Shader.SetGlobalFloatArray("_GridY", gridsY);
        Shader.SetGlobalFloat("_GridYLength", gridsY.Length);

        Shader.SetGlobalFloatArray("_GridX", gridsX);
        Shader.SetGlobalFloat("_GridXLength", gridsX.Length);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NPuzzle))]
public class NPuzzleEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NPuzzle editorObj = target as NPuzzle;

        if (editorObj == null) return;

        if (GUILayout.Button("Update Grid"))
        {
            editorObj.UpdateGrid();
        }
    }

}
#endif
