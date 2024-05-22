using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using System.Diagnostics;

public enum Collision_System
{
    BVH,
    Unity_Collisions,
    Spatial_Hashing,
    Brute_Force
}

public class SimulationMG : MonoBehaviour
{
    public Collision_System Selected_Collision_Detection;
    public int TotalCollisions;
    public static SimulationMG Instance;

    [Header("BVH")]
    public bool VisualizeBVH;
    public bool VisualizeBVH_Collisions;

    [Header("Spatial Hash")]
    [Range(2, 1000)]
    public int cellsize = 10;



    private int Collisioncalls;
    private Collision_System current_system;

    [SerializeField]
    public List<GameObject> allobjs;  // references  alto keep track of all created objs

    private BVHTree bvhTree; // BVH tree instance
    public SpatialHashing SH_grid;

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
        UpdateCollisions();
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

                case Collision_System.Unity_Collisions:
                    Setup_Unity_Collisions();
                    // functionality for Unity's COllision detection System
                    break;

                case Collision_System.Spatial_Hashing:
                    initSpatialHashing();
                    // functionality for Spatial hashing System
                    break;

                case Collision_System.Brute_Force:
                    Setup_Brute_Force();
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

    public void RemoveObj(GameObject obj) // removing objects typically from the spawners
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
    public double BVH_Construction_Time,COllision_Time,SH_rehash_time;
    void UpdateCollisions()
    {
        Stopwatch stopwatch = new Stopwatch();
        switch (current_system)
        {
            case Collision_System.BVH:
                stopwatch.Start(); // Start timing
                UpdateBVHTree();
                stopwatch.Stop(); // Stop timing
                BVH_Construction_Time = stopwatch.Elapsed.TotalMilliseconds; // Get elapsed time in milliseconds

                int bvhColls = 0;
                foreach (GameObject obj in allobjs)
                {
                    stopwatch.Restart(); // Reset and start the stopwatch

                    GameObject collidedObj = bvhTree.GetCollisions(obj);
                    stopwatch.Stop(); // Stop timing
                    COllision_Time = stopwatch.Elapsed.TotalMilliseconds; // Get elapsed time in milliseconds

                    if (collidedObj)
                    {
                        bvhColls++;
                        UnityEngine.Debug.Log("BVH Collision detected");
                    }
                }
                TotalCollisions = bvhColls;
                break;

            case Collision_System.Unity_Collisions:
                // doing nothing here as unity itself is registering collisions using the register and unregister methods
                // functionality for Unity's COllision detection System
                break;

            case Collision_System.Spatial_Hashing:
                SH_grid.cellSize = cellsize;
                stopwatch.Start();
                SH_grid.RehashAllObjects(allobjs);
                stopwatch.Stop();
                SH_rehash_time = stopwatch.Elapsed.TotalMilliseconds;
                int sp_Colls = 0;
                foreach (GameObject obj in allobjs)
                {
                    stopwatch.Restart();
                    GameObject collidedobj = SH_grid.GetFirstCollision(obj);
                    stopwatch.Stop() ;
                    COllision_Time = stopwatch.Elapsed.TotalMilliseconds;

                    if (collidedobj)
                    {
                        sp_Colls++;
                        UnityEngine.Debug.Log("SpatialHash Collision detected");
                    }
                }
                SH_grid.DrawAllCellBounds();
                TotalCollisions = sp_Colls;
                // functionality for Spatial hashing System
                break;

            case Collision_System.Brute_Force:
                stopwatch.Start();
                CheckCollisions_Using_BF();
                stopwatch.Stop();
                COllision_Time = stopwatch.Elapsed.TotalMilliseconds;
                // functionality for Spatial hashing System
                break;

            default:

                break;
        }

    }
    public void CheckCollisions_Using_BF()
    {
        TotalCollisions = BruteForceCollisionDetection(); // also update the number of collisions accordingly
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
    public GameObject GetCollisionsBVH(GameObject obj)
    {
        if (bvhTree != null)
        {
            return bvhTree.GetCollisions(obj);
        }
        else
        {
            UnityEngine.Debug.LogWarning("BVH tree not initialized.");
            return null;
        }
    }
    #endregion

    #region SpatialHAshing
    void initSpatialHashing()
    {
        SH_grid = new SpatialHashing(20);
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
        foreach (GameObject obj in allobjs)
        {
            obj.GetComponent<Collider>().enabled = false;
        }
    }

    public void Remove_Brute_Force()
    {
        foreach (GameObject obj in allobjs)
        {
            obj.GetComponent<Collider>().enabled = true;
        }
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

    #region Brute_force

    // Brute force collision detection
    private int BruteForceCollisionDetection()
    {
        int totalcols = 0;
        for (int i = 0; i < allobjs.Count; i++)
        {
            GameObject obj1 = allobjs[i];
            Bounds bound1 = obj1.GetComponent<Renderer>().bounds;




            for (int j = 0; j < allobjs.Count; j++)
            {
                GameObject obj2 = allobjs[j];

                if (i != j) // dont check self collisions
                {
                    Bounds bound2 = obj2.GetComponent<Renderer>().bounds;


                    //Debug.Log($"checking from bounds {collider1.bounds.center}");
                    //Debug.Log($"To bounds {collider2.bounds.center}");


                    if (bound1.Intersects(bound2))
                    {
                        UnityEngine.Debug.Log($"Object {obj1.name} Bounding Volume:");
                        UnityEngine.Debug.Log($"Center: {bound1.center}");
                        UnityEngine.Debug.Log($"Size: {bound1.size}");
                        UnityEngine.Debug.Log($"Min: {bound1.min}");
                        UnityEngine.Debug.Log($"Max: {bound1.max}");
                        DrawBoundsWireframe(bound1, Color.red);

                        UnityEngine.Debug.Log($"Object {obj1.name} Bounding Volume:");
                        UnityEngine.Debug.Log($"Center: {bound2.center}");
                        UnityEngine.Debug.Log($"Size: {bound2.size}");
                        UnityEngine.Debug.Log($"Min: {bound2.min}");
                        UnityEngine.Debug.Log($"Max: {bound2.max}");
                        DrawBoundsWireframe(bound2, Color.red);

                        // Perform collision resolution
                        totalcols++;
                        ResolveCollision(obj1, obj2);
                    }
                }
            }
        }
        return totalcols;
    }

    // Resolve collision between two objects
    private void ResolveCollision(GameObject obj1, GameObject obj2)
    {
        // Perform collision resolution based on physics properties
        // For example, adjust velocities, apply forces, etc.
        // You can access Rigidbody components and other physics properties of the objects here.
        Rigidbody rb1 = obj1.GetComponent<Rigidbody>();
        Rigidbody rb2 = obj2.GetComponent<Rigidbody>();

        // Example: Swap velocities of the objects
        if (rb1 != null && rb2 != null)
        {
            UnityEngine.Debug.Log("Attempting to swap velocities");
            Vector3 tempVelocity = rb1.velocity;
            rb1.velocity = rb2.velocity;
            rb2.velocity = tempVelocity;
        }
    }

    public void DrawBoundsWireframe(Bounds bounds, Color color)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // Calculate the corner points of the bounding box
        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        corners[1] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[3] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[4] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[5] = center + new Vector3(-extents.x, extents.y, extents.z);
        corners[6] = center + new Vector3(extents.x, extents.y, extents.z);
        corners[7] = center + new Vector3(extents.x, extents.y, -extents.z);

        // Draw lines between the corner points
        UnityEngine.Debug.DrawLine(corners[0], corners[1], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[1], corners[2], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[2], corners[3], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[3], corners[0], color, Time.deltaTime);

        UnityEngine.Debug.DrawLine(corners[4], corners[5], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[5], corners[6], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[6], corners[7], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[7], corners[4], color, Time.deltaTime);

        UnityEngine.Debug.DrawLine(corners[0], corners[4], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[1], corners[5], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[2], corners[6], color, Time.deltaTime);
        UnityEngine.Debug.DrawLine(corners[3], corners[7], color, Time.deltaTime);
    }
    #endregion


    #region ADDons
    public int getbvhnodes()
    {
        return bvhTree.totalNodes;
    }
    #endregion
}
