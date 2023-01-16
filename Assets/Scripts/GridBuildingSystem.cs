using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Code.Utils;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private GridXZ<GridObject> grid;
    private PlacedObjectTypeSO.Dir dir;
    public static GridBuildingSystem Instance { get; private set; }
    public event EventHandler OnObjectPlaced;
    public event EventHandler OnSelectedChanged;
    public LineRenderer aimLineRenderer;
    public Transform obj1;
    public Transform obj2;
    [SerializeField] private GameObject visual;
    public float turnSpeed = 20f;


    private void Awake() {
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
        placedObjectTypeSO = placedObjectTypeSOList[0];
        visual = Instantiate(visual, Vector3.zero, Quaternion.identity);
        // visual.transform.parent = transform;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localEulerAngles = Vector3.zero;
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectTypeSO != null) {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        } else {
            return Quaternion.identity;
        }
    }

    public Vector2Int GetPosition(Vector3 worldPosition) {
        grid.GetXZ(worldPosition, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int y);
        if (placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public class GridObject {
        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        private PlacedObject placedObject;

        public GridObject(GridXZ<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetPlacedObject(PlacedObject placedObject) {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject GetPlacedObject() {
            return placedObject;
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public bool CanBuild() {
            return placedObject = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + placedObject;
        }
    }

    private void Update() {
        aimLineRenderer.enabled = true;
        aimLineRenderer.SetPosition(0, obj1.position);
        aimLineRenderer.sharedMaterial.color = Color.green;
    // aimLineRenderer.sharedMaterial.color = hitEnemy ? Color.green : Color.white;



        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        aimLineRenderer.SetPosition(1, mousePosition);

        visual.transform.localPosition = mousePosition;

        Vector3 direction = obj1.transform.position - visual.transform.localPosition;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(visual.transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        visual.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);


        if (Input.GetMouseButtonDown(0)) {
            grid.GetXZ(mousePosition, out int x, out int y);
            Vector2Int placedObjectOrigin = new Vector2Int(x, y);
            bool canBuild = true;
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            if (placedObjectTypeSO != null) {
                foreach (Vector2Int gridPosition in gridPositionList) {
                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                        canBuild = false;
                        break;
                    }
                }
            }
            if (canBuild) {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                if (placedObjectTypeSO != null) {
                    placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);
                    Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                    PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
                    foreach (Vector2Int gridPosition in gridPositionList) {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    }
                        OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                        //DeselectObjectType();
                    }
                }else {
                    UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
                }
            }
        if (Input.GetMouseButtonDown(1)) {
            if (grid.GetGridObject(mousePosition) != null) {
                PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                if (placedObject != null) {
                    placedObject.DestroySelf();
                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                    foreach (Vector2Int gridPosition in gridPositionList) {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
        }

    private void DeselectObjectType() {
        placedObjectTypeSO = null;
        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
}
