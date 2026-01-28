using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float gridSize = 1f;

    private BuildingData selectedBuilding;
    private GameObject ghostObject;
    private bool isBuildingMode;

 void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if(GameManager.Instance.CurrentPhase != GamePhase.Preparation || isBuildingMode == false) return;
        
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CancelBuilding();
            return;
        }
    }

    public void ButtonBuildingToPlace(BuildingData data)
    {
        if(selectedBuilding == data) return; //Já está com ele selecionado

        CancelBuilding(); //Limpa seleção anterior

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
            //Vector3 targetPosition = SnapToGrid
        }
    }

   /* Vector3 SnapToGrid(Vector3 originalPosition)
    {
        //Mathf.Round arrendonda float para int
        loat x 
    }*/
    void CancelBuilding()
    {
        isBuildingMode = false;
        selectedBuilding = null;
        if(ghostObject != null) Destroy(ghostObject);
    }
}
