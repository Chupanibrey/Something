using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollision : MonoBehaviour
{
    [SerializeField]
    private VFXDeath death;
    [SerializeField]
    private Sound sound;

    private void Awake()
    {
        death = GameObject.FindWithTag("GameController").GetComponent<VFXDeath>();
        sound = GameObject.FindWithTag("GameController").GetComponent<Sound>();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "StaticEnemy")
        {
            HealthSystem.HealphCount -= 10;
            death.BloodEffect(transform.position);

            if (HealthSystem.HealphCount <= 0)
            {
                death.DeathEffect(transform.position);
                death.Death();
            }
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Jelly") && HealthSystem.HealphCount <= 99)
        {
            sound.PlayClip(sound.eatSound);
            Destroy(collision.gameObject);
            HealthSystem.HealphCount += 10;
        }
        else if (collision.gameObject.tag == "Key")
        {
            sound.PlayClip(sound.keySound);
            Destroy(collision.gameObject);
            Collect.CollectKey = true;
        }
        else if (collision.gameObject.tag == "KeyEnd")
        {
            sound.PlayClip(sound.keySound);
            Destroy(collision.gameObject);
            Collect.CollectKeyEnd = true;
        }
        else if (collision.gameObject.tag == "End")
        {
            Collect.End = true;
        }
    }
}
