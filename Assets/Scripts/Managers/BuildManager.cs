using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float gridSize = 1f;

    [Header("Ghost Visuals")]
    [SerializeField] private Color validColor = new Color(0, 1, 0, 0.5f); //Color (R, G, B, Alpha)
    [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.5f); //Color (R, G, B, Alpha)
    private BuildingData selectedBuilding;
    private GameObject ghostObject;
    private bool isBuildingMode;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if(GameManager.Instance != null) GameManager.Instance.OnGamePhaseChanged += GamePhaseChanged;
    }

    void Update()
    {
        if(GameManager.Instance.CurrentPhase != GamePhase.Preparation || isBuildingMode == false) return;
        
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CancelBuilding();
            return;
        }

        HandleBuildingPosition();
    }

    public void SelectBuildingToPlace(BuildingData data)
    {
        if(selectedBuilding == data) return; //Item is already selected

        CancelBuilding(); //Clears previous selection

        selectedBuilding = data;
        isBuildingMode = true;

        //Visual ghost preview
        ghostObject = Instantiate(selectedBuilding.PreviewPrefab);
    }

    void HandleBuildingPosition()
    {
        //Creates a ray from the camera to the mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Checks if the ray hit the ground
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            //Calculates position on Grid
            Vector3 targetPosition = SnapToGrid(hit.point);
            ghostObject.transform.position = targetPosition;

            //Check if the position is valid
            bool isValid = IsValidPosition(targetPosition);
            //Changes ghost colour in real time
            UpdateGhostColor(isValid);

            //Checks if clicking on UI (to avoid accidental building when clicking a button)
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (isValid)
                {
                    PlaceBuilding(targetPosition);
                }
                else
                {
                    Debug.Log($"Local invalido");
                }
            }
        }
    }

    Vector3 SnapToGrid(Vector3 originalPosition)
    {
        //Mathf.Round rounds float to int
        float x = Mathf.Round(originalPosition.x/gridSize) * gridSize;
        float z = Mathf.Round(originalPosition.z/gridSize) * gridSize;
        float y = 0;
        return new Vector3 (x, y, z);
    }

    bool IsValidPosition(Vector3 position)
    {
        //Sets collision box size based on the grid. Multiplies by 0.9f to make the box smaller than the grid square
        //Avoids false wall collisions from edge contact
        float checkSize = gridSize * 0.9f;

        Vector3 checkCenter = position + Vector3.up * 0.5f;

        Vector3 halfExtents = new Vector3 (checkSize/2, 0.45f, checkSize/2);    
        
        //Physics.CheckBox creates an invisible box in the world
        //checkCenter: where it located
        //halfExtents: the box radius for each side
        //obsctacleLayer: layer mask to detect only obstacles (excludes grounds)
        bool hitSomething = Physics.CheckBox(checkCenter, halfExtents, Quaternion.identity, obstacleLayer);
        return !hitSomething;
    }

    void PlaceBuilding(Vector3 position)
    {
        Instantiate(selectedBuilding.Prefab, position, Quaternion.identity);
        CancelBuilding();
    }

    void UpdateGhostColor(bool isValid)
    {
        if (ghostObject == null) return;

        Color targetColor;

        if (isValid == true) targetColor = validColor;
        else targetColor = invalidColor;

        //Gets all renderers (in case the object has multiple parts)
        MeshRenderer[] renderers = ghostObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.color = targetColor;
        }
    }

    void CancelBuilding()
    {
        isBuildingMode = false;
        selectedBuilding = null;
        if(ghostObject != null) Destroy(ghostObject);
    }

    void GamePhaseChanged(GamePhase newPhase)
    {
        if (newPhase != GamePhase.Preparation) CancelBuilding();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGamePhaseChanged -= GamePhaseChanged;
    }
}
