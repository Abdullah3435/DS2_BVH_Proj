using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public enum Collision_System
{
    BVH,
    Unity_Collision_Detection,
    Spatial_Hashing,
    Brute_Force
}

public class SimulationMG : MonoBehaviour
{
    public Collision_System Selected_Collision_Detection;
    public int TotalCollisions;
    public static SimulationMG Instance;

    private int Collisioncalls;
    private Collision_System current_system;
    [SerializeField]
    private List<GameObject> allobjs;  // references  alto keep track of all created objs

    private BVHTree bvhTree; // BVH tree instance

    // Start is called before the first frame update
    void Start()
    {
        initMG();
        current_system = Selected_Collision_Detection;
        TotalCollisions = 0;
        Collisioncalls = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SelectCollisionSystem();
        // Update collisions counter
        TotalCollisions = Collisioncalls / 2;
    }

    public void SelectCollisionSystem() // handle switching for collision detection system
    {
        if (current_system != Selected_Collision_Detection)
        {
            current_system = Selected_Collision_Detection;

            Remove_Unity_Collisions();

            switch (current_system)
            {
                case Collision_System.BVH:
                    CreateBVHTree(); // create bvh
                    break;

                case Collision_System.Unity_Collision_Detection:
                    Setup_Unity_Collisions();
                    // functionality for Unity's COllision detection System
                    break;

                case Collision_System.Spatial_Hashing:
                    // functionality for Spatial hashing System
                    break;

                case Collision_System.Brute_Force:
                    // functionality for Spatial hashing System
                    break;

                default:

                    break;
            }
        }
    }
    public void RegisterCollision()
    {
        Collisioncalls++;
    }

    public void UnRegisterCollision()
    {
        Collisioncalls--;
    }

    public void RegisterObj(GameObject newobj) // registering objects typically from the spawners
    {
        allobjs.Add(newobj);

        // Update BVH tree if selected collision detection system is BVH
        if (Selected_Collision_Detection == Collision_System.BVH)
        {
            UpdateBVHTree();
        }
    }

    public void RemoveObj(GameObject obj) // registering objects typically from the spawners
    {
        allobjs.Remove(obj);

        // Update BVH tree if selected collision detection system is BVH
        if (Selected_Collision_Detection == Collision_System.BVH)
        {
            UpdateBVHTree();
        }
    }

    #region Lifetime
    void initMG()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region BVH Extension
    // Create BVH tree based on current objects in the scene
    private void CreateBVHTree()
    {
        bvhTree = new BVHTree(allobjs);
    }

    // Update BVH tree when objects are added or removed from the scene
    private void UpdateBVHTree()
    {
        bvhTree = new BVHTree(allobjs);
    }

    // Get collisions using BVH tree
    public List<GameObject> GetCollisionsBVH(GameObject obj)
    {
        if (bvhTree != null)
        {
            return bvhTree.GetCollisions(obj);
        }
        else
        {
            Debug.LogWarning("BVH tree not initialized.");
            return new List<GameObject>();
        }
    }
    #endregion


    #region Envronment_Setup_Based_On_Current_Collision_Detection

    public void Setup_BVH()
    {
    }

    public void Remove_BVH()
    {
        
    }

    public void Setup_Spatial_Hashing()
    {
    }

    public void Remove_Spatial_Hashing()
    {

    }

    public void Setup_Brute_Force()
    {
    }

    public void Remove_Brute_Force()
    {

    }

    public void Setup_Unity_Collisions()
    {
        foreach (GameObject obj in allobjs)
        {
            obj.GetComponent<Collider>().enabled = true;

        }
    }

    public void Remove_Unity_Collisions()
    {
        foreach (GameObject obj in allobjs)
        {
            obj.GetComponent<Collider>().enabled = false;
        }
    }


    #endregion
}
