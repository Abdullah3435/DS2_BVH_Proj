using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Range(0,2)]
    public float SpawnSpeed;
    public GameObject[] prefabs;
    public GameObject[] spawnPoints;
    public bool StopSpawning;

    [Range(3,20)]
    public int Size_Variance;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnSpeed);
            if (!StopSpawning)
                CreateObj();
        }

    }
    void CreateObj()
    {
        int prefab_i = Random.Range(0, prefabs.Length);
        int spanwn_i = Random.Range(0, spawnPoints.Length);

        int size_multiplier = Random.Range(1, Size_Variance); // adjust variance in sizes 

        GameObject obj = Instantiate(prefabs[prefab_i], spawnPoints[spanwn_i].transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(1,1,1)*size_multiplier;
        if (SimulationMG.Instance.Selected_Collision_Detection != Collision_System.Unity_Collisions)
        {
            obj.GetComponent<Collider>().enabled= false;
        }

        SimulationMG.Instance.RegisterObj(obj); // register objects to simulation manager

    }
}
