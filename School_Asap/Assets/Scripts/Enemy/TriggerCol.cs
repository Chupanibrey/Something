using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCol : MonoBehaviour
{
    private Collider2D col;
    [SerializeField]
    private FolowPath move;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            move.enabled = true;
        }
    }
}
