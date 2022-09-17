using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDoorEnd : MonoBehaviour
{
    public EndDoor OpenEnd;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && Collect.CollectKeyEnd)
        {
            OpenEnd.enabled = true;
        }
    }
}