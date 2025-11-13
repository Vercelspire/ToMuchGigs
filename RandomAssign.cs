using UnityEngine;

public class RandomAssign : MonoBehaviour
{
    private int wallsLayer;
    private int walkThroughLayer;

    void Start()
    {
        wallsLayer = LayerMask.NameToLayer("whatIsGround");
        walkThroughLayer = LayerMask.NameToLayer("WalkThrough");

        // Repeat the AssignLayers function every 1 second
        InvokeRepeating("AssignLayers", 0f, 1f);
    }

    void AssignLayers()
    {
        // Randomly assign the parent
        gameObject.layer = (Random.value > 0.5f) ? wallsLayer : walkThroughLayer;

        // Randomly assign the children
        foreach (Transform child in transform)
        {
            child.gameObject.layer = (Random.value > 0.5f) ? wallsLayer : walkThroughLayer;
        }
    }
}
