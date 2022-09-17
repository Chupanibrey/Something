using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Изменение скорости перемещения героя
    public float playerSpeed = 1.0f;
    // Текущая скорость перемещения
    private float currentSpeed = 0.0f;
    // Сохранение последнего перемещения
    private Vector3 lastMovement = new Vector3();

    // Ссылка на компонент Rigidbody
    private new Rigidbody2D rigidbody2D;
    // Ссылка на компонент анимаций
    private Animator anim;
    //переменная для определения направления персонажа вправо/влево
    private bool isFacingRight = true;

    // Создание переменных для кнопок
    public List<KeyCode> leftButton;
    public List<KeyCode> rightButton;
    public List<KeyCode> jumptButton;
    // Кнопка, которая используется для выстрела
    public List<KeyCode> shootButton;

    // Находится ли персонаж на земле или в прыжке?
    private bool isGrounded = false;
    // Ссылка на компонент Transform объекта
    // Для определения соприкосновения с землей
    public Transform groundCheck;
    // Радиус определения соприкосновения с землей
    private float groundRadius = 0.2f;
    // Ссылка на слой, представляющий землю
    public LayerMask whatIsGround;
    // Для определения использования верёвки
    public bool isSwinging;

    // Счетчик задержки между ударами
    private float timeTilNextFire = 0.0f;
    //Задержка между ударами(кулдаун)
    private float timeBetweenFires = 0.5f;
    private bool attached = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (attached)
            Attack();

        Jump();

        Movement();

        Turn();
    }

    private void FixedUpdate()
    {
        // Определяем, на земле ли персонаж
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        AnimateController();
    }

    public void Jump()
    {
        foreach (var key in jumptButton)
        {
            // Если персонаж на земле и нажат пробел
            if (isGrounded && Input.GetKeyDown(key))
            {
                // Устанавливаем в аниматоре переменную в false
                anim.SetBool("Ground", false);
                // Прикладываем силу вверх, чтобы персонаж подпрыгнул
                rigidbody2D.AddForce(new Vector2(0, 200));
            }
        }
    }

    public void AnimateController()
    {
        anim.SetBool("Ground", isGrounded);
        anim.SetFloat("vSpeed", rigidbody2D.velocity.y);
        anim.SetFloat("Speed", currentSpeed);
    }

    public void Turn()
    {
        // -1 возвращается при нажатии на клавиатуре стрелки влево (или клавиши А),
        // 1 возвращается при нажатии на клавиатуре стрелки вправо (или клавиши D)
        float move = Input.GetAxis("Horizontal");

        // Если нажали клавишу для перемещения вправо, а персонаж направлен влево
        if (move > 0 && !isFacingRight)
        {
            // Отражаем персонажа вправо
            Flip();
        }

        // Обратная ситуация. отражаем персонажа влево
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        if(isGrounded)
            anim.Play("Turn");
        // Меняем направление движения персонажа
        isFacingRight = !isFacingRight;
        // Получаем размеры персонажа
        Vector3 theScale = transform.localScale;
        // Зеркально отражаем персонажа по оси Х
        theScale.x *= -1;
        // Задаем новый размер персонажа, равный старому, но зеркально отраженный
        transform.localScale = theScale;
    }

    // Движение героя
    void Movement()
    {
        if (!isSwinging)
        {
            // Необходимое движение
            Vector3 movement = new Vector3();
            // Проверка нажатых клавиш
            movement += MoveIfPressed(leftButton, Vector3.left);
            movement += MoveIfPressed(rightButton, Vector3.right);
            // Если нажато несколько кнопок, обрабатываем это
            movement.Normalize();
            // Проверка нажатия кнопки
            if (movement.magnitude > 0)
            {
                attached = false;
                // После нажатия двигаемся в этом направлении
                currentSpeed = playerSpeed;
                this.transform.Translate(movement * Time.deltaTime * playerSpeed, Space.World);
                lastMovement = movement;
            }
            else
            {
                attached = true;
                // Если ничего не нажато
                this.transform.Translate(lastMovement * Time.deltaTime * currentSpeed, Space.World);
                // Замедление со временем
                currentSpeed *= 0.8f;
            }
        }
    }

    // Возвращает движение, если нажата кнопка
    Vector3 MoveIfPressed(List<KeyCode> keyList, Vector3 Movement)
    {
        // Проверяем кнопки из списка
        foreach (KeyCode element in keyList)
        {
            if (Input.GetKey(element))
            {
                // Если нажато, покидаем функцию
                return Movement;
            }
        }
        // Если кнопки не нажаты, то не двигаемся
        return Vector3.zero;
    }

    public void Attack()
    {
        foreach (var key in shootButton)
            if (Input.GetKey(key) && timeTilNextFire < 0)
            {
                timeTilNextFire = timeBetweenFires;
                anim.Play("Attack");
                break;
            }
        timeTilNextFire -= Time.deltaTime;
    }
}