using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField]
    private Sound sound;
    private bool oneCollision = true;

    private void Awake()
    {
        sound = GameObject.FindWithTag("GameController").GetComponent<Sound>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && oneCollision)
        {
            oneCollision = false;
            Destroy(this.gameObject);
            sound.PlayClip(sound.takeCrystalSound);
            Collect.CollectCrystal++;
        }
    }
}