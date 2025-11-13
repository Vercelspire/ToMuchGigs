using UnityEngine;
public class MoggerDamage : MonoBehaviour
{
    private Mogger parentMogger;
    void Start()
    {
        parentMogger = GetComponentInParent<Mogger>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);

        Transform t = collision.transform;
        while (t != null)
        {
            Debug.Log("Checking: " + t.name);

            // idk why just safety net ig, so checks if name contains rock or Rock
            if (t.name.Contains("Rock") || t.name.Contains("rock"))
            {
                if (parentMogger)
                {
                    parentMogger.TakeDamage(2);
                    Debug.Log("HIT BY ROCK (COLLISION) - Dealt 2 damage!");
                }
                break;
            }
            t = t.parent;
        }
    }
}