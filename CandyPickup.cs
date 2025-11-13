using UnityEngine;

public class CandyPickup : MonoBehaviour
{
    public float pickupDistance = 8f;
    public int candyCount = 0;
    public LayerMask candylayer;


    public AudioClip pickupSound; 
    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        // basically if press E -> try pick up candy ig
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupCandy();
        }
    }

    void TryPickupCandy()
    {
        // based off mouse pos
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // if mouse on candy + dist check
        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, candylayer))
        {
            // we call hit and destroy candy and score++ ig
            GameObject candy = hit.collider.gameObject;


            if (pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            Destroy(candy);

            candyCount++;
            Debug.Log("Candy Picked Up! Candy Total = " + candyCount);
        }
    }
}
