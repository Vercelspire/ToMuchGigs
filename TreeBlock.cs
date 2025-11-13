using UnityEngine;



// idk what im doing with this, this prob has no use

// goofy tree block
public class TreeBlock : MonoBehaviour
{
    [Header("Parent folders containing all tree prefabs")]
    public Transform TreesBlock;
    public Transform TreesThrough;

    [Header("Use convex mesh collider?")]
    public bool useConvex = false; // only set true if trees move or interact with rigidbody

    private void Start()
    {
        MakeSolidRecursive(TreesBlock);
        MakeSolidRecursive(TreesThrough);
    }

    private void MakeSolidRecursive(Transform parent)
    {
        if (parent == null) return;

        foreach (Transform t in parent)
        {
            // if it has children, process them recursively
            if (t.childCount > 0)
            {
                MakeSolidRecursive(t);
            }

            // skip if it already has a collider
            if (t.GetComponent<Collider>() != null) continue;

            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf != null)
            {
                MeshCollider meshCol = t.gameObject.AddComponent<MeshCollider>();
                meshCol.sharedMesh = mf.sharedMesh;
                meshCol.convex = useConvex;
            }
            else
            {
                // fallback if no mesh
                t.gameObject.AddComponent<BoxCollider>();
            }

            t.GetComponent<Collider>().isTrigger = false;
            t.gameObject.isStatic = true; // optimize physics for static trees
        }
    }
}
