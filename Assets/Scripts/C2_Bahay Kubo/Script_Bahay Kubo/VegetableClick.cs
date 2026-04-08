using UnityEngine;

public class VegetableClick : MonoBehaviour
{
    [HideInInspector] public BahayKuboSequentialSpawner spawner;
    [HideInInspector] public int vegetableID; // Assigned by the Spawner

    private void OnMouseDown()
    {
        if (spawner != null)
        {
            // Instead of destroying itself, it asks the spawner: "Is it my turn?"
            spawner.TryHarvest(vegetableID, gameObject);
        }
    }
}