using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    // —сылка на компонент анимаций
    private Animator anim;
    public float speed;
    // “екуща€ скорость перемещени€
    private bool move = false;
    private GameObject player;
    //переменна€ дл€ определени€ направлени€ персонажа вправо/влево
    public bool chase = false;
    public Transform startingPoint;
    [SerializeField]
    private VFXDeath death;
    [SerializeField]
    private Sound sound;


    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        sound = GameObject.FindWithTag("GameController").GetComponent<Sound>();
    }

    void Update()
    { 
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }
            
        if (chase == true)
            Chase();
        else
            ReturnStartPosition();
        // ”станавливаем в аниматоре значение скорости взлета/падени€
        anim.SetBool("move", move);
    }

    public void Chase()
    {

        Flip();
        move = true;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    public void ReturnStartPosition()
    {
        if(transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);
        if (transform.position == startingPoint.position)
            move = false;
    }

    public void Flip()
    {
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "StaticEnemy")
        {
            death.DeathEffect(this.transform.position);
            sound.PlayClip(sound.deathSound);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            HealthSystem.HealphCount -= 1;

        if (HealthSystem.HealphCount <= 0 && player != null)
        {
            death.DeathEffect(death.player.transform.position);
            death.Death();
        }
    }
}