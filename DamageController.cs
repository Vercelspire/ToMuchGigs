using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageController : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;         // player settings
    public int maxHP = 4;   // health
    private int currentHP;   // current health

    // damage cooldown
    [Header("Damage Settings")]
    public float damageCooldown = 1f; // seconds -> taking damage
    private bool canTakeDamage = true;


    // detections
    [Header("Enemy Detection Settings")]
    public string enemyTag = "Enemy"; // Tag for Shadowlop 
    public float detectionRadius = 1.5f; // distance to take damage

    [Header("Audio Settings")] 
    public AudioClip hitsounds;    
    public AudioSource audioSource;

    // what happens when start
    private void Start()
    {
        // the 4 hp var = maxHP
        currentHP = maxHP;
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // some goofy ahh debug
        if (!player)
            Debug.LogError("Player not found! Assign player Transform.");
    }

    private void Update()
    {
        if (!player || !canTakeDamage) return;


        // Check all enemies in the scene, which for ours it is shadowlop

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance <= detectionRadius)
            {
                // calls TakeDamage function
                TakeDamage(1);

                break; // Take damage from one enemy at a time per cooldown
            }
        }
    }

    // takes damage
    private void TakeDamage(int amount)
    {
        if (!canTakeDamage) return;

        currentHP -= amount;

        // hits
        Debug.Log("Player HP: " + currentHP);

        if (hitsounds != null)
        {
            if (audioSource)
                audioSource.PlayOneShot(hitsounds);
            else
                AudioSource.PlayClipAtPoint(hitsounds, player.position);
        }
        // if lplr hp is 0 or lower then load death screen we die.
        if (currentHP <= 0)
        {

            // dead
            Debug.Log("Player dead! Freezing game.");

            SceneManager.LoadScene("DeathScreen");

            return;
        }

        canTakeDamage = false;
        StartCoroutine(ResetDamageCooldown());
    }

    // resets
    private IEnumerator ResetDamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}
