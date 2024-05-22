using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI fps, Selectedsystem, Total_Objs, memoryText;

    public TextMeshProUGUI collisions,collision_time;

    [Header("BVH")]
    public GameObject BVH_statspanel;
    public TextMeshProUGUI BVH_Const, BVH_Nodes, BVH_Traversal;

    [Header("SH")]
    public GameObject SH_statspanel;
    public TextMeshProUGUI SH_Const, SH_Cells,SH_Hashes;


    float FPS;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(updatestats(0.5f)); //update stats per second
    }

    // Update is called once per frame
    void Update()
    {
        Updatefps();

        Total_Objs.text = $"Total Objects  :  {SimulationMG.Instance.allobjs.Count}";
    }

    void Updatefps()
    {
        FPS = 1f / Time.deltaTime;

    }

    IEnumerator updatestats(float updatetime)
    {
        yield return new WaitForSeconds(updatetime);
        while (true)
        {


            Selectedsystem.text = SimulationMG.Instance.Selected_Collision_Detection.ToString();
            collisions.text = "Collisions :  " + SimulationMG.Instance.TotalCollisions;
            fps.text = "FPS: " + Mathf.Round(FPS);
            Total_Objs.text = $"Total Objects  :  {SimulationMG.Instance.allobjs.Count}";

            memoryText.text = "Memory: " + System.GC.GetTotalMemory(true) / (1024 * 1024) + " MB";
            //cpuText.text = "CPU Usage: " + (SystemInfo.processorFrequency * SystemInfo.processorCount).ToString() + " Hz";

            collision_time.text = $"Collision Time : {SimulationMG.Instance.COllision_Time}";

            BVH_Stats();
            SH_Stats();
            yield return new WaitForSeconds(updatetime);
        }
    }

    void BVH_Stats()
    {
        if (SimulationMG.Instance.Selected_Collision_Detection == Collision_System.BVH)
        {
            BVH_statspanel.SetActive(true);
            BVH_Nodes.text = $"Nodes  :  {SimulationMG.Instance.getbvhnodes()}";
            BVH_Const.text = $"Construction Time : {SimulationMG.Instance.BVH_Construction_Time}ms";
            BVH_Traversal.text = $"Traversal Time : {SimulationMG.Instance.COllision_Time}ms";

        }
        else { BVH_statspanel.SetActive(false) ;}
    }
    void SH_Stats()
    {
        if (SimulationMG.Instance.Selected_Collision_Detection == Collision_System.Spatial_Hashing)
        {
            SH_statspanel.SetActive(true);
            SH_Const.text = $"Rehash Time  :  {SimulationMG.Instance.SH_rehash_time}";
            SH_Cells.text = $"Occupied Cells  :  {SimulationMG.Instance.SH_grid.totalOccupiedCells}";
            SH_Hashes.text = $"Total Hashes  :  {SimulationMG.Instance.SH_grid.totalObjectPairs}";
        }

        else { SH_statspanel.SetActive(false); }
    }
}



