using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI fps;

    public TextMeshProUGUI collisions;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Updatefps();
        UpdateCollisions();
    }

    void Updatefps()
    {
        float FPS = 1f / Time.deltaTime;
        fps.text = "FPS: " + Mathf.Round(FPS);
    }

    void UpdateCollisions()
    {
        collisions.text = "Collisions " + SimulationMG.Instance.TotalCollisions;
    }
}
