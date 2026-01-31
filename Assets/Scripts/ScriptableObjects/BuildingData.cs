using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Building/ Building Data")]
public class BuildingData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private int price;
    [SerializeField] private GameObject prefab; //Real Object
    [SerializeField] private GameObject previewPrefab; //A version without scripts, purely visual (transparent material)
    [SerializeField] private Vector2Int size = new Vector2Int(1, 1); 

    public string DisplayName => displayName;
    public int Price => price;
    public GameObject Prefab => prefab;
    public GameObject PreviewPrefab => previewPrefab;
    public Vector2Int Size => size;
}
