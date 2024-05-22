using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectmovement : MonoBehaviour
{
    public Vector3 velocity;
    public bool isColliding;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestruct", 10);
        InitRandomSize();
        InitVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object in the direction of its velocity vector scaled by deltaTime
        transform.Translate(velocity * Time.deltaTime);
    }

    void SelfDestruct()
    {
        if (isColliding)
        {
            SimulationMG.Instance.UnRegisterCollision();
        }

        SimulationMG.Instance.RemoveObj(gameObject); // have to debug it later on if its working or not 
        Destroy(gameObject);
    }

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");
        isColliding = true;
        SimulationMG.Instance.RegisterCollision();
    }

    private void OnCollisionExit(Collision collision)
    {
        SimulationMG.Instance.UnRegisterCollision();
        isColliding = false;
    }
    #endregion

    #region Initialization
    void InitRandomSize()
    {
        float sc_multiplier = Random.Range(1f, 5f);
        transform.localScale = new Vector3(sc_multiplier, sc_multiplier, sc_multiplier);
    }

    void InitVelocity()
    {
        // Generate a random velocity vector
        float speed = Random.Range(1f, 5f); // Speed magnitude
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
    }
    #endregion
}