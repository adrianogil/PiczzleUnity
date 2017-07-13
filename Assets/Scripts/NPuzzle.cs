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

    [Range(0, 1)]
    public float moveValue;

    MeshCollider meshCollider;
    MeshRenderer meshRenderer;


    public Vector2 lastMouse;
    public Vector2 delta;

    public Vector2 lastCoord;

	// Use this for initialization
	void Start () {
        Reset();
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

        meshRenderer.sharedMaterial.SetFloat("_SelectionMove", moveValue);
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

        currentSelectedPosition = new GridPosition(-1, -1);


        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.SetVector("_SelectedPosition", new Vector4(currentSelectedPosition.x,
                                                                                       currentSelectedPosition.y, 0f, 0f));
        meshRenderer.sharedMaterial.SetVector("_EmptyPosition", new Vector4(currentEmptyPosition.x,
                                                                                    currentEmptyPosition.y, 0f, 0f));
        meshRenderer.sharedMaterial.SetFloat("_SelectionMove", 0f);

        UpdateGrid();
    }

    void VerifyTouchInValidGridItem()
    {
        lastMouse = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(lastMouse);
        RaycastHit hit;
        if (meshCollider.Raycast(ray, out hit, 100.0F))
        {
            Debug.Log("GilLog - NPuzzle::VerifyTouchInValidGridItem " + hit.textureCoord);

            Vector2 uv = hit.textureCoord;

            lastCoord = uv;

            uv.y = 1 - uv.y; // Reverse because Texture is mapped from bottom to top



            GridPosition selectedPosition = new GridPosition();
            selectedPosition.x = Mathf.FloorToInt(uv.x * gridSize.x);
            selectedPosition.y = Mathf.FloorToInt(uv.y * gridSize.y);

            Debug.Log("GilLog - NPuzzle::VerifyTouchInValidGridItem SelectedPosition " + selectedPosition.name);

            if (Mathf.Abs(selectedPosition.x - currentEmptyPosition.x) + Mathf.Abs(selectedPosition.y - currentEmptyPosition.y) == 1)
            {
                currentSelectedPosition = selectedPosition;

                // int index = selectedPosition.y * gridSize.x + selectedPosition.x;

                // positions[index] = new GridPosition() { x = -1, y = -1};

                movingItem = true;
                moveValue = 0f;

                meshRenderer.sharedMaterial.SetVector("_SelectedPosition", new Vector4(currentSelectedPosition.x,
                                                                                       currentSelectedPosition.y, 0f, 0f));
                meshRenderer.sharedMaterial.SetVector("_EmptyPosition", new Vector4(currentEmptyPosition.x,
                                                                                    currentEmptyPosition.y, 0f, 0f));

                UpdateGrid();
            }
        }
    }

    void MoveCurrentGridItem()
    {
        Vector2 currentMouse = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(currentMouse);
        RaycastHit hit;
        if (meshCollider.Raycast(ray, out hit, 100.0F))
        {
            Debug.Log("GilLog - NPuzzle::MoveCurrentGridItem " + hit.textureCoord);

            Vector2 uv = hit.textureCoord;

            delta = uv - new Vector2((0.5f + currentSelectedPosition.x)*(1/gridSize.x), 1 - (0.5f + currentSelectedPosition.y)*(1/gridSize.y));
            
            delta.y *= -1;

            if (currentEmptyPosition.x == currentSelectedPosition.x)
            {
                // moveValue += delta.x * Mathf.Sign(currentEmptyPosition.y - currentSelectedPosition.y);
                moveValue = delta.x;
            } else {
                moveValue += delta.y * Mathf.Sign(currentEmptyPosition.x - currentSelectedPosition.x);
                moveValue = delta.y;
            }
            
            moveValue = Mathf.Clamp01(moveValue);

            lastMouse = currentMouse;
            lastCoord = uv;
        }

        
    }

    int GetIndex(GridPosition p)
    {
        return p.y * gridSize.x + p.x;
    }

    void ReleaseCurrentGridItem()
    {
        float finalMove = Mathf.Round(moveValue);
        moveValue = 0f;

        if (finalMove == 0f)
        {

        } else if (finalMove == 1f)
        {
            positions[GetIndex(currentEmptyPosition)] = positions[GetIndex(currentSelectedPosition)];
            positions[GetIndex(currentSelectedPosition)] = new GridPosition(-1, -1);
            currentEmptyPosition = currentSelectedPosition;
        }

        currentSelectedPosition = new GridPosition(-1,-1);
        movingItem = false;

        meshRenderer.sharedMaterial.SetVector("_SelectedPosition", new Vector4(currentSelectedPosition.x,
                                                                                       currentSelectedPosition.y, 0f, 0f));
        meshRenderer.sharedMaterial.SetVector("_EmptyPosition", new Vector4(currentEmptyPosition.x,
                                                                                    currentEmptyPosition.y, 0f, 0f));

        UpdateGrid();
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
