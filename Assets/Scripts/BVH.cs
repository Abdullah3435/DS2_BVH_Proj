using UnityEngine;
using System.Collections.Generic;

public class BVHTree
{
    private class Node
    {
        public Bounds bounds;
        public List<int> objectIndices;
        public Node leftChild;
        public Node rightChild;
    }

    private Node root;
    private List<GameObject> objects;

    public BVHTree(List<GameObject> objects)
    {
        this.objects = objects;
        ConstructBVHTree();
    }

    private void ConstructBVHTree()
    {
        List<Bounds> objectBounds = CalculateObjectBounds(objects);
        List<int> objectIndices = new List<int>();
        for (int i = 0; i < objects.Count; i++)
        {
            objectIndices.Add(i);
        }
        root = ConstructBVHTreeRecursive(objectBounds, objectIndices);
    }

    private Node ConstructBVHTreeRecursive(List<Bounds> objectBounds, List<int> objectIndices)
    {
        Node node = new Node();
        node.bounds = CalculateBounds(objectBounds, objectIndices);

        if (objectIndices.Count == 1)
        {
            node.objectIndices = objectIndices;
            return node;
        }

        // Split objects into two groups based on the longest axis of the bounding box
        int longestAxis = node.bounds.size.x > node.bounds.size.y ? (node.bounds.size.x > node.bounds.size.z ? 0 : 2) : (node.bounds.size.y > node.bounds.size.z ? 1 : 2);
        float splitPosition = (node.bounds.max[longestAxis] + node.bounds.min[longestAxis]) / 2f;

        List<int> leftIndices = new List<int>();
        List<int> rightIndices = new List<int>();

        foreach (int index in objectIndices)
        {
            if (objectBounds[index].max[longestAxis] < splitPosition)
            {
                leftIndices.Add(index);
            }
            else
            {
                rightIndices.Add(index);
            }
        }

        node.leftChild = ConstructBVHTreeRecursive(objectBounds, leftIndices);
        node.rightChild = ConstructBVHTreeRecursive(objectBounds, rightIndices);

        return node;
    }

    private List<Bounds> CalculateObjectBounds(List<GameObject> objects)
    {
        List<Bounds> objectBounds = new List<Bounds>();
        foreach (GameObject obj in objects)
        {
            objectBounds.Add(obj.GetComponent<Renderer>().bounds);
        }
        return objectBounds;
    }

    private Bounds CalculateBounds(List<Bounds> objectBounds, List<int> objectIndices)
    {
        Bounds bounds = new Bounds(objectBounds[objectIndices[0]].center, Vector3.zero);
        foreach (int index in objectIndices)
        {
            bounds.Encapsulate(objectBounds[index]);
        }
        return bounds;
    }

    public List<GameObject> GetCollisions(GameObject obj)
    {
        List<GameObject> collisions = new List<GameObject>();
        if (root != null)
        {
            GetCollisionsRecursive(obj, root, collisions);
        }
        return collisions;
    }

    private void GetCollisionsRecursive(GameObject obj, Node node, List<GameObject> collisions)
    {
        if (node.bounds.Intersects(obj.GetComponent<Renderer>().bounds))
        {
            if (node.leftChild != null)
            {
                GetCollisionsRecursive(obj, node.leftChild, collisions);
            }
            if (node.rightChild != null)
            {
                GetCollisionsRecursive(obj, node.rightChild, collisions);
            }
            if (node.objectIndices != null)
            {
                foreach (int index in node.objectIndices)
                {
                    if (objects[index] != obj && obj.GetComponent<Renderer>().bounds.Intersects(objects[index].GetComponent<Renderer>().bounds))
                    {
                        collisions.Add(objects[index]);
                    }
                }
            }
        }
    }
}
