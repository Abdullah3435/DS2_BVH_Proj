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

    public int totalNodes = 0;

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
        totalNodes++;  // Increment the node counter each time a node is created

        // Base case: if there's one object or no objects, stop splitting.
        if (objectIndices.Count <= 1)
        {
            node.objectIndices = objectIndices;
            return node;
        }

        // Determine the longest axis to split the objects along.
        int longestAxis = node.bounds.size.x > node.bounds.size.y ? (node.bounds.size.x > node.bounds.size.z ? 0 : 2) : (node.bounds.size.y > node.bounds.size.z ? 1 : 2);
        float splitPosition = (node.bounds.max[longestAxis] + node.bounds.min[longestAxis]) / 2f;

        List<int> leftIndices = new List<int>();
        List<int> rightIndices = new List<int>();

        // Split objects into left and right groups based on their position relative to splitPosition.
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

        // Check if either side is empty and adjust if necessary to prevent infinite recursion.
        if (leftIndices.Count == 0 && rightIndices.Count > 0)
        {
            leftIndices.Add(rightIndices[rightIndices.Count - 1]);
            rightIndices.RemoveAt(rightIndices.Count - 1);
        }
        else if (rightIndices.Count == 0 && leftIndices.Count > 0)
        {
            rightIndices.Add(leftIndices[leftIndices.Count - 1]);
            leftIndices.RemoveAt(leftIndices.Count - 1);
        }

        // Recursively construct the left and right subtrees.
        node.leftChild = leftIndices.Count > 0 ? ConstructBVHTreeRecursive(objectBounds, leftIndices) : null;
        node.rightChild = rightIndices.Count > 0 ? ConstructBVHTreeRecursive(objectBounds, rightIndices) : null;

        return node;
    }

    private List<Bounds> CalculateObjectBounds(List<GameObject> objects)
    {
        List<Bounds> objectBounds = new List<Bounds>();
        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent<Renderer>())
            {
                objectBounds.Add(obj.GetComponent<Renderer>().bounds);
            }
        }
        return objectBounds;
    }

    private Bounds CalculateBounds(List<Bounds> objectBounds, List<int> objectIndices)
    {
        Debug.Log("The Indice is" + objectIndices[0]);
        Bounds bounds = new Bounds(objectBounds[objectIndices[0]].center, Vector3.zero);
        foreach (int index in objectIndices)
        {
            bounds.Encapsulate(objectBounds[index]);
        }
        return bounds;
    }

    public GameObject GetCollisions(GameObject obj)
    {

        if (root != null)
        {
            return GetCollisionsRecursive(obj, root);
        }
        return null; 
    }


    private GameObject GetCollisionsRecursive(GameObject obj, Node node)
    {
        if (node.bounds.Intersects(obj.GetComponent<Renderer>().bounds))
        {
            // Highlight the bounds of the current node to show the traversal path
            if (SimulationMG.Instance.VisualizeBVH)
            {
                SimulationMG.Instance.DrawBoundsWireframe(node.bounds, Color.yellow);
            }

            // Check left child
            if (node.leftChild != null)
            {
                GameObject leftCollision = GetCollisionsRecursive(obj, node.leftChild);
                if (leftCollision != null) return leftCollision;
            }

            // Check right child
            if (node.rightChild != null)
            {
                GameObject rightCollision = GetCollisionsRecursive(obj, node.rightChild);
                if (rightCollision != null) return rightCollision;
            }

            // Check current node's objects
            if (node.objectIndices != null)
            {
                foreach (int index in node.objectIndices)
                {
                    if (objects[index] != obj && obj.GetComponent<Renderer>().bounds.Intersects(objects[index].GetComponent<Renderer>().bounds))
                    {
                        // Highlight the collision
                        if (SimulationMG.Instance.VisualizeBVH_Collisions)
                        {
                            SimulationMG.Instance.DrawBoundsWireframe(objects[index].GetComponent<Renderer>().bounds, Color.red);
                            TraverseBVH(obj, root);
                        }
                        return objects[index];
                    }
                }
            }
        }
        return null; // No collision found in this path
    }

    void TraverseBVH(GameObject obj, Node node) // trverse the entire partial hierarchy of bvh and simulate it using boxes
    {
        if (node.bounds.Intersects(obj.GetComponent<Renderer>().bounds))
        {
            // Highlight the bounds of the current node to show the traversal path
            SimulationMG.Instance.DrawBoundsWireframe(node.bounds, Color.green);

            if (node.leftChild != null)
            {
                TraverseBVH(obj, node.leftChild);
            }
            if (node.rightChild != null)
            {
                TraverseBVH(obj, node.rightChild);
            }
        }
    }

}
