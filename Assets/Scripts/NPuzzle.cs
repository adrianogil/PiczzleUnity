using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class GridPosition
{
    public string name { get { return "(" + x + ", " + y + ")"; }}

    public int x, y;

    public GridPosition() {}
    public GridPosition(int x, int y) { this.x = x; this.y = y;}
}

public class NPuzzle : MonoBehaviour {

    public GridPosition gridSize;

    public GridPosition currentEmptyPosition;

    public GridPosition[] positions;

    public GridPosition currentSelectedPosition = new GridPosition(1,1);

    public bool movingItem = false;

    MeshCollider meshCollider;


	// Use this for initialization
	void Start () {
        meshCollider = GetComponent<MeshCollider>();
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            VerifyTouchInValidGridItem();
        } else if (Input.GetMouseButtonUp(0) && movingItem)
        {
            ReleaseCurrentGridItem();
        } else if (Input.GetMouseButton(0) && movingItem)
        {
            MoveCurrentGridItem();
        }
	}

    public void UpdateGrid()
    {
        int totalGridSize = gridSize.x * gridSize.y;

        Debug.Log("GilLog - NPuzzle::UpdateGrid " + totalGridSize);

        float[] gridsY = new float[totalGridSize];
        float[] gridsX = new float[totalGridSize];

        int index = 0;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                index = y * gridSize.x + x;

                gridsX[index] = positions[index].x;
                gridsY[index] = positions[index].y;
            }
        }  

        Shader.SetGlobalFloatArray("_GridY", gridsY);
        Shader.SetGlobalFloat("_GridYLength", totalGridSize);

        Shader.SetGlobalFloatArray("_GridX", gridsX);
        Shader.SetGlobalFloat("_GridXLength", totalGridSize);
    }

    void Reset()
    {
        gridSize = new GridPosition() { x = 3, y = 3};

        currentEmptyPosition = new GridPosition() { x = 2, y = 2};

        positions = new GridPosition[gridSize.x * gridSize.y];

        int index = 0;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                index = y * gridSize.x + x;

                if (x == currentEmptyPosition.x && y == currentEmptyPosition.y)
                {
                    positions[index] = new GridPosition() { x = -1, y = -1};
                } else {
                    positions[index] = new GridPosition() { x = x, y = y};
                }
            }
        }    
    }

    void VerifyTouchInValidGridItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (meshCollider.Raycast(ray, out hit, 100.0F))
        {
            Debug.Log("GilLog - NPuzzle::VerifyTouchInValidGridItem " + hit.textureCoord);

            Vector2 uv = hit.textureCoord;
            uv.y = 1 - uv.y; // Reverse because Texture is mapped from bottom to top

            GridPosition selectedPosition = new GridPosition();
            selectedPosition.x = Mathf.FloorToInt(uv.x * gridSize.x);
            selectedPosition.y = Mathf.FloorToInt(uv.y * gridSize.y);

            Debug.Log("GilLog - NPuzzle::VerifyTouchInValidGridItem SelectedPosition " + selectedPosition.name);

            if (Mathf.Abs(selectedPosition.x - currentEmptyPosition.x) + Mathf.Abs(selectedPosition.y - currentEmptyPosition.y) == 1)
            {
                currentSelectedPosition = selectedPosition;

                movingItem = true;
            }
        }
    }

    void MoveCurrentGridItem()
    {

    }

    void ReleaseCurrentGridItem()
    {
        currentSelectedPosition = new GridPosition(-1,-1);
        movingItem = false;
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
