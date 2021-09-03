using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool canSpawn;
    public List<GameObject> entitiesIn;

    private void Start()
    {
        entitiesIn = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<EnemyBehaviour>() != null || other.GetComponentInParent<PlayerController>() != null)
        {
            entitiesIn.Add(other.gameObject);
            canSpawn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        entitiesIn.Remove(other.gameObject);
        if (entitiesIn.Count == 0)
        {
            canSpawn = true;
        }
    }
}
