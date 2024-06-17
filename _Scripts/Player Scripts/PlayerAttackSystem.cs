using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttackSystem : MonoBehaviour
{
    public PlayerStats playerStats;
    public Transform wandTip; // Assign the wand tip transform here
    public Camera playerCamera; // Assign the main camera here
    public Vector2 crosshairOffset = Vector2.zero; // Offset for the crosshair position
    public SpellSystem spellSystem;
    public PlayerLoadoutSystem playerLoadoutSystem;

    private Animator animator;
    private float nextAttackTime = 0f;
    private bool isAttacking = false; // Flag to indicate if attacking

    private PlayerCameraSystem cameraController;
    private PlayerInput playerInput;
    private InputAction attackAction;

    void Awake()
    {
        animator = GetComponent<Animator>();
        cameraController = FindObjectOfType<PlayerCameraSystem>();
        playerInput = GetComponent<PlayerInput>();

        attackAction = playerInput.actions["Fire"];
    }

    void OnEnable()
    {
        attackAction.started += OnAttackStarted;
        attackAction.canceled += OnAttackCanceled;
    }

    void OnDisable()
    {
        attackAction.started -= OnAttackStarted;
        attackAction.canceled -= OnAttackCanceled;
    }

    void OnAttackStarted(InputAction.CallbackContext context)
    {
        isAttacking = true;
    }

    void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }

    void Update()
    {
        if (isAttacking && Time.time >= nextAttackTime && SceneManager.GetActiveScene().name != "Camp")
        {
            bool isJumping = animator.GetBool("isJumping");
            if (!isJumping)
            {
                Attack();
                nextAttackTime = Time.time + spellSystem.baseSpell.attackCooldown - playerStats.GetAttackCooldown(); // Set the next attack time
            }
        }
    }

    void Attack()
    {
        // Play attack animation
        animator.SetTrigger(spellSystem.baseSpell.animationTrigger);

        // Calculate the shooting direction
        Vector3 direction = Vector3.zero;
        if (cameraController.currentStyle == PlayerCameraSystem.CameraStyle.Topdown)
        {
            Vector3 mousePosition = cameraController.GetMouseWorldPosition();
            direction = (mousePosition - wandTip.position).normalized;
            // if lower then wand tip make it level with wand tip
            if (mousePosition.y < wandTip.position.y)
                direction.y = 0;
            //if (mousePosition.y > wandTip.position.y + 1)
             //   direction.y = wandTip.position.y + 1;
            
        }
        else if (cameraController.currentStyle == PlayerCameraSystem.CameraStyle.Combat)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f + crosshairOffset.x, 0.5f + crosshairOffset.y, 0));
            direction = ray.direction;
        }

        // Instantiate and shoot the magic missile after a short delay to match the animation
        StartCoroutine(ShootMagicMissile(direction));
    }

    IEnumerator ShootMagicMissile(Vector3 direction)
    {
        // Wait for a small delay to sync the missile firing with the animation
        yield return new WaitForSeconds(spellSystem.baseSpell.attackCooldown - playerStats.GetAttackCooldown() / 2); // Adjust the timing if necessary

        // Apply loadout and active cards before shooting the missile
        // OLD AND DEPRECIATED CALL -->  playerLoadoutSystem.ApplyLoadoutModifiers(spellSystem);
        if (playerStats.activeSpellCards != null) {
            spellSystem.ApplyCards();
        }
        

        // Get modified spell from SpellSystem
        (GameObject prefab, float damage, float speed, float range, bool isHoming, int splitCount) = spellSystem.GetModifiedSpell();

        // Calculate final damage based on player intelligence and cards
        float finalDamage = damage + playerStats.GetMagicDamage();
        float finalSpeed = speed + playerStats.GetSpellSpeed();
        float finalRange = range + playerStats.GetSpellRange();

        // Instantiate and shoot the magic missile
        GameObject magicMissile = Instantiate(prefab, wandTip.position, Quaternion.LookRotation(direction));
        MagicMissile missileComponent = magicMissile.GetComponent<MagicMissile>();
        if (missileComponent != null)
        {
            missileComponent.Initialize((int)finalDamage, finalSpeed, finalRange, isHoming, splitCount, prefab);
        }
    }
    
}
