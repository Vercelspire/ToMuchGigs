using UnityEngine;

[RequireComponent(typeof(InventoryController))]
public class AttackController : MonoBehaviour
{
    [Header("References")]
    public Transform playerCameraTransform;   // Camera for aiming
    public LineRenderer trajectoryLine;       // LineRenderer component

    [Header("Trajectory Settings")]
    public int lineSegments = 30;             // Number of points for smooth curve
    public float throwForce = 15f;            // Throw speed

    [Header("Line Width Settings")]
    public float startWidth = 0.3f;
    public float endWidth = 0.3f;

    public GameObject rockPrefab;

    private InventoryController inventory;

    void Start()
    {
        inventory = GetComponent<InventoryController>();

        if (!trajectoryLine)
        {
            Debug.LogError("Assign a LineRenderer to trajectoryLine!");
            return;
        }

        // Setup line renderer
        trajectoryLine.positionCount = lineSegments;
        trajectoryLine.startWidth = startWidth;
        trajectoryLine.endWidth = endWidth;
        trajectoryLine.enabled = false;
    }

    void Update()
    {
        // Show trajectory while holding right-click
        if (Input.GetMouseButton(1) && inventory.GetItemCount("Rock") > 0)
        {
            trajectoryLine.enabled = true;
            SimulateTrajectory();
        }
        else if (trajectoryLine)
        {
            trajectoryLine.enabled = false;
        }

        // Throw rock on left-click
        if (Input.GetMouseButtonDown(0))
        {
            ThrowRock();
        }
    }

    void SimulateTrajectory()
    {
        Vector3 startPos = playerCameraTransform.position + playerCameraTransform.forward * 0.5f;
        Vector3 velocity = playerCameraTransform.forward * throwForce;

        for (int i = 0; i < lineSegments; i++)
        {
            float t = (i / (float)lineSegments) * 2f; // scale t for proper arc
            Vector3 pos = startPos + velocity * t + 0.5f * Physics.gravity * t * t;
            trajectoryLine.SetPosition(i, pos);
        }
    }
    void ThrowRock()
    {
        InventoryItem rockItem = inventory.GetItem("Rock");
        if (rockItem == null || rockItem.quantity <= 0)
            return;
        GameObject rock = Instantiate(rockItem.prefab,
            playerCameraTransform.position + playerCameraTransform.forward * 0.5f,
            Quaternion.identity);

        rock.SetActive(true);

        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (!rb) rb = rock.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.velocity = playerCameraTransform.forward * throwForce;
        Destroy(rock, 5f);

        rockItem.quantity--;
    }
}
