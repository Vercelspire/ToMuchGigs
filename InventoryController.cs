using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryController : MonoBehaviour
{
    private List<InventoryItem> inventory = new List<InventoryItem>();

    private int selectedIndex = 0;
    private InventoryItem selectedItem;

    void Update()
    {
        // 1 = selects rust_key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectItem(0);
        }

        // 2 = selects rocks
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectItem(1);
        }

        // Use the selected item
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseSelectedItem();
        }
    }

    public void AddItem(string itemName, GameObject prefab = null, int amount = 1)
    {
        InventoryItem existing = inventory.Find(i => i.itemName == itemName);
        if (existing != null)
            existing.quantity += amount;
        else
            inventory.Add(new InventoryItem(itemName, prefab, amount));

        Debug.Log($"Added {amount} {itemName}(s). Total: {GetItemCount(itemName)}");
    }


    // get inv count
    public int GetItemCount(string itemName)
    {

        // iv item
        InventoryItem item = inventory.Find(i => i.itemName == itemName);

        // if exist then we return amount otherwise 0
        if (item != null)
        {
            return item.quantity;
        }
        else
        {
            return 0;
        }
    }


    // gets item ig
    public InventoryItem GetItem(string itemName)
    {
        return inventory.Find(i => i.itemName == itemName);
    }

    // get inv count
    public int GetInventoryCount()
    {
        return inventory.Count;
    }

    public InventoryItem GetInventoryItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
            return inventory[index];
        return null;
    }

    // select item
    void SelectItem(int index)
    {
        InventoryItem item = GetInventoryItemAt(index);
        if (item != null)
        {
            selectedIndex = index;
            selectedItem = item;
            Debug.Log("Selected: " + selectedItem.itemName);
        }
        else
        {
            Debug.Log("No item in that slot!");
        }
    }

    // use (this is for key)
    void UseSelectedItem()
    {
        if (selectedItem == null)
        {
            Debug.Log("No item selected!");
            return;
        }

        // Use the Key -> from butchercontroller
        if (selectedItem.itemName == "Key")
        {
            // Find nearest Easy_Door 2 within 5 units (escape)


            // escape tag
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Escape");

            GameObject nearestDoor = null;
            float closestDist = 5f;

            // loops ig
            foreach (GameObject door in doors)
            {
                if (door.name != "Easy_Door 2") continue; // only target this specific door
                float dist = Vector3.Distance(transform.position, door.transform.position);
                if (dist <= closestDist)
                {
                    nearestDoor = door;
                    closestDist = dist;
                }
            }

            if (nearestDoor != null)
            {
                // "Unlock" the door -> win screen
                Debug.Log("Used the Key on Easy_Door 2!");
                nearestDoor.SetActive(false);

                // Remove key
                inventory.Remove(selectedItem);
                selectedItem = null;

                // Destroys the key
                Camera playerCamera = Camera.main;
                if (playerCamera != null)
                {
                    // Look for the key as a child of the camera
                    foreach (Transform child in playerCamera.transform)
                    {
                        if (child.name.Contains("rust_key"))
                        {
                            Destroy(child.gameObject);
                            Debug.Log("Key visual removed!");
                            break;
                        }
                    }
                }
                SceneManager.LoadScene("WinScreen");
            }
            else
            {
                Debug.Log("No Easy_Door 2 nearby to use the key!");
            }
        }
    }

    // use rock ig
    public void UseRock(Transform spawnPoint, float throwForce = 15f)
    {
        InventoryItem rock = inventory.Find(i => i.itemName == "Rock");

        // if rock exist
        if (rock != null && rock.quantity > 0)
        {

            // use 1
            rock.quantity--;

            if (rock.prefab != null)
            {
                GameObject thrownRock = GameObject.Instantiate(rock.prefab, spawnPoint.position, Quaternion.identity);
                Rigidbody rb = thrownRock.GetComponent<Rigidbody>();

                // adds force
                if (rb != null)
                    rb.AddForce(spawnPoint.forward * throwForce, ForceMode.Impulse);
            }

            Debug.Log("Threw 1 rock. Remaining: " + rock.quantity);
        }
        else
        {
            Debug.Log("No rocks to throw!");
        }
    }
}
