using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupController : MonoBehaviour
{
    public Transform playerCameraTransform;
    public LayerMask rockLayerMask;
    public float pickupRange = 5f;
    public float pickupRadius = 0.5f;

    public GameObject rockPrefab;

    public AudioClip pickupSound;

    private InventoryController playerInventory;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerInventory = GetComponent<InventoryController>();
        if (!playerInventory)
            Debug.LogError("InventoryController not found on player!");
    }

    void Update()
    {
        CheckForPickup();
    }

    private void CheckForPickup()
    {
        // Cast sphere from lplr's camera forward to detect pickups
        if (Physics.SphereCast(playerCameraTransform.position, pickupRadius, playerCameraTransform.forward, out RaycastHit hit, pickupRange, rockLayerMask))
        {
            // if sphere hits collider
            if (hit.collider != null)
            {
                Debug.Log("Press E to pick up a rock!");
                // if we hold down E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // calls pickup rock func
                    PickupRock(hit.collider.gameObject);
                }
            }
        }
    }
    private void PickupRock(GameObject rock)
    {
        // adds rock prefab into inv
        playerInventory.AddItem("Rock", rockPrefab);

        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        rock.SetActive(false); // Remove rock
    }
}

