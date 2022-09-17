using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRespawn : MonoBehaviour
{
    public Transform firstPoint;
    public Transform secondPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            firstPoint.transform.position = secondPoint.transform.position;
        }
    }
}
