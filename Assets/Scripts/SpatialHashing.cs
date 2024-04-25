using UnityEngine;
using System.Collections.Generic;

public class SpatialHashing
{
    private Dictionary<Vector3Int, List<GameObject>> hashTable;
    public float cellSize;

    public SpatialHashing(float size)
    {
        hashTable = new Dictionary<Vector3Int, List<GameObject>>();
        cellSize = size;  // Size of each cell in the spatial hash grid
    }

    // Hashes a GameObject based on its position
    private Vector3Int CalculateHashKey(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize));
    }

    // Add a GameObject to the hash table
    public void AddObject(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        Vector3Int minKey = CalculateHashKey(bounds.min);
        Vector3Int maxKey = CalculateHashKey(bounds.max);

        for (int x = minKey.x; x <= maxKey.x; x++)
        {
            for (int y = minKey.y; y <= maxKey.y; y++)
            {
                for (int z = minKey.z; z <= maxKey.z; z++)
                {
                    Vector3Int key = new Vector3Int(x, y, z);
                    if (!hashTable.ContainsKey(key))
                    {
                        hashTable[key] = new List<GameObject>();
                    }
                    hashTable[key].Add(obj);
                }
            }
        }
    }

    // Remove a GameObject from the hash table
    public void RemoveObject(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        Vector3Int minKey = CalculateHashKey(bounds.min);
        Vector3Int maxKey = CalculateHashKey(bounds.max);

        for (int x = minKey.x; x <= maxKey.x; x++)
        {
            for (int y = minKey.y; y <= maxKey.y; y++)
            {
                for (int z = minKey.z; z <= maxKey.z; z++)
                {
                    Vector3Int key = new Vector3Int(x, y, z);
                    if (hashTable.ContainsKey(key))
                    {
                        hashTable[key].Remove(obj);
                        if (hashTable[key].Count == 0)
                        {
                            hashTable.Remove(key);
                        }
                    }
                }
            }
        }
    }


    // Check if any objects in the same cell are colliding
    public GameObject GetFirstCollision(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        Vector3Int minKey = CalculateHashKey(bounds.min);
        Vector3Int maxKey = CalculateHashKey(bounds.max);

        for (int x = minKey.x; x <= maxKey.x; x++)
        {
            for (int y = minKey.y; y <= maxKey.y; y++)
            {
                for (int z = minKey.z; z <= maxKey.z; z++)
                {
                    Vector3Int key = new Vector3Int(x, y, z);
                    if (hashTable.ContainsKey(key))
                    {
                        foreach (GameObject otherObj in hashTable[key])
                        {
                            if (otherObj != obj && obj.GetComponent<Renderer>().bounds.Intersects(otherObj.GetComponent<Renderer>().bounds))
                            {
                                return otherObj; // Return the first object that collides with the given object
                            }
                        }
                    }
                }
            }
        }

        return null; // Return null if no collision is found
    }

    // Simple collision detection (intersecting bounding boxes)
    private bool IsColliding(GameObject obj1, GameObject obj2)
    {
        Collider col1 = obj1.GetComponent<Collider>();
        Collider col2 = obj2.GetComponent<Collider>();
        if (col1 != null && col2 != null)
        {
            return col1.bounds.Intersects(col2.bounds);
        }
        return false;
    }

    // Update the object's position in the hash table
    public void RehashAllObjects(List<GameObject> allObjects)
    {
        // Clear the existing hash table completely
        hashTable.Clear();

        // Re-add each object to the hash table
        foreach (GameObject obj in allObjects)
        {
            if (obj.activeInHierarchy && obj.GetComponent<Renderer>())  // Ensure the object is active and has a Renderer
            {
                AddObject(obj);
            }
        }
    }

    public void DrawAllCellBounds()
    {
        foreach (var entry in hashTable)
        {
            Vector3Int key = entry.Key;
            Vector3 cellCenter = new Vector3(key.x * cellSize, key.y * cellSize, key.z * cellSize) + new Vector3(cellSize / 2f, cellSize / 2f, cellSize / 2f);
            Bounds bounds = new Bounds(cellCenter, new Vector3(cellSize, cellSize, cellSize));
            SimulationMG.Instance.DrawBoundsWireframe(bounds, Color.blue);
        }
    }
}
