using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    //[SerializeField] private Weapon currentWeapon;
    private float currentSpeed;

    //Layer
    [SerializeField] private LayerMask groundLayer;

    //References
    EntityStats stats;
    Inventory inventory;

    //Events
    public static event Action OnShootPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = GetComponent<EntityStats>();
        inventory = GetComponent<Inventory>();  
        currentSpeed = stats.MoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerRotation();
        PlayerShootingInput();  
        HandleWeaponInput();
        PauseGame();
    }

    void PlayerMove()
    {
        //GetAxisRaw returns immediately -1,0 or 1. Avoids sliding.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Normalizes the vector to 1 to limit the speed
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f && controller.enabled)
        {
            controller.Move(direction * currentSpeed * Time.deltaTime);
        }
    }

    void PlayerRotation()
    {
        //Creates a ray from the camera to the mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Checks if the ray hit the ground
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            //Gets the point where the mouse touched the ground
            Vector3 targetPosition = hit.point;

            //Keeps the point at the same height as the player (important to avoid tilting)
            targetPosition.y = transform.position.y;

            //Calculates the direction and smoothly rotates the character
            Vector3 lookDirection = targetPosition - transform.position;

            if(lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    void PlayerShootingInput()
    {
        Weapon activeWeapon = inventory.GetCurrentWeapon();
        if (activeWeapon == null) return;

        if (activeWeapon.IsAutomatic)
        {
            if (Input.GetMouseButton(0))
            {
                activeWeapon.TryShoot();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                activeWeapon.TryShoot();
            }
        }
    }

    void HandleWeaponInput()
    {
        //Keys 1-4
        if (Input.GetKeyDown(KeyCode.Alpha1)) inventory.EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) inventory.EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) inventory.EquipWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) inventory.EquipWeapon(3);

        //Mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel"); //Nome exato do eixo definido no InputManager
        if (scroll > 0f) inventory.NextWeapon();
        if (scroll < 0f) inventory.PreviousWeapon();

    }

    void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.TogglePause();
        }
    }
}
