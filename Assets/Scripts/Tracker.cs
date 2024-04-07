using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    [SerializeField] List<Transform> destinations = new List<Transform>();
    int destinationIndex;

    private void Update()
    {
        if (destinationIndex < destinations.Count)
        {
            Vector3 destinationPos = destinations[destinationIndex].position;
            transform.position = Vector3.MoveTowards(transform.position, destinations[0].position, 2 * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinationPos) < 0.01f)
            {
                destinationIndex++;
            }
        }
        
    }
}
