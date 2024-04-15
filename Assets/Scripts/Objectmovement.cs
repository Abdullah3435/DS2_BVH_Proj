using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectmovement : MonoBehaviour
{
    public float speed = 1;
    public bool iscolliding;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestruct", 10);
        InitRandomSize();
        InitSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed * 5);
    }

    void SelfDestruct()
    {
        if (iscolliding)
        {
            SimulationMG.Instance.UnRegisterCollision();
        }

        //SimulationMG.Instance.RemoveObj(this.gameObject); // have to debug it later on if its working or not 
        Destroy(gameObject);
    }

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");
        iscolliding = true;
        SimulationMG.Instance.RegisterCollision();
    }

    private void OnCollisionExit(Collision collision)
    {
        SimulationMG.Instance.UnRegisterCollision();
        iscolliding = false;
    }

    #endregion

    #region Initialization
    void InitRandomSize()
    {
        float sc_multiplier = Random.Range(1, 5);
        transform.localScale = new Vector3(1, 1, 1) * sc_multiplier;
    }

    void InitSpeed()
    {
        speed = Random.Range(1, 5);
    }

    #endregion
}
