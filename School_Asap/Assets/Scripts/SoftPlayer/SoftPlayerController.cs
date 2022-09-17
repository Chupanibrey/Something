using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPlayerController : MonoBehaviour
{
    #region Переменные
    [SerializeField]
    public bool isSwinging = false; // Находится ли игрок на верёвке
    [SerializeField]
    private VFXDeath death;
    [SerializeField]
    private Sound sound;

    [SerializeField]
    private float playerSpeed = 4.0f; // Изменение скорости перемещения героя
    [SerializeField]
    private float maxSpeedX = 15f; // Масимальная скорость по оси X
    [SerializeField]
    private float jumpForce = 6.0f; // Изменение силы прыжка героя
    [SerializeField]
    private new Rigidbody2D[] rigidbody2D; // Ссылка на компонент Rigidbody всех точек героя
    [SerializeField]
    private Transform groundCheck; // Для определения соприкосновения с землей
    [SerializeField]
    private float groundRadius = 0.2f; // Радиус определения соприкосновения с землей
    [SerializeField]
    private LayerMask whatIsGround; // Ссылка на слой, к которому мы можем прицепиться

    // Находится ли персонаж на земле или в прыжке?
    private bool isGrounded = false;
    // Текущая скорость перемещения
    private float currentSpeed = 0.0f;

    #region Расскачивание
    public Vector2 ropeHook;

    [SerializeField]
    private float swingForce = 3f;
    [SerializeField]
    private Transform playerPosition;
    [SerializeField]
    private float inverseSwingForce = 0.025f;

    private float horizontalInput;
    private Vector3 pastPosition;
    #endregion

    #region Создание переменных для кнопок
    [SerializeField]
    private List<KeyCode> leftButton;
    [SerializeField]
    private List<KeyCode> rightButton;
    [SerializeField]
    private List<KeyCode> jumptButton;
    #endregion
#endregion

    private void Awake()
    {
        pastPosition = playerPosition.position;
        death = GameObject.FindWithTag("GameController").GetComponent<VFXDeath>();
        sound = GameObject.FindWithTag("GameController").GetComponent<Sound>();
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        Movement();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        Jump();
    }

    // Движение героя
    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            death.DeathEffect(rigidbody2D[rigidbody2D.Length - 1].transform.position);
            death.Death();
        }

        if (!isSwinging)
        {
            // Смерть при падении с большой высоты
            if (isGrounded && rigidbody2D[rigidbody2D.Length - 1].velocity.y < -22)
            {
                death.DeathEffect(rigidbody2D[rigidbody2D.Length - 1].transform.position);
                death.Death();
            }
            else if(HealthSystem.HealphCount <= 0)
            {
                death.DeathEffect(rigidbody2D[rigidbody2D.Length - 1].transform.position);
                death.Death();
            }

            foreach (var rb in rigidbody2D)
            {
                // Необходимое движение
                Vector2 movement = new Vector2();
                // Проверка нажатых клавиш
                movement += MoveIfPressed(leftButton, Vector2.left);
                movement += MoveIfPressed(rightButton, Vector2.right);
                // Если нажато несколько кнопок, обрабатываем это
                movement.Normalize();

                // Проверка нажатия кнопки
                if (movement.magnitude > 0)
                {
                    // После нажатия двигаемся в этом направлении
                    currentSpeed = playerSpeed;
                    rb.AddForce(new Vector2(movement.x * playerSpeed, 0f), ForceMode2D.Force);
                }
                else
                {
                    // Если ничего не нажато
                    rb.AddForce(new Vector2(movement.x * currentSpeed, 0f), ForceMode2D.Force);
                    // Замедление со временем
                    currentSpeed *= 0.95f;
                }

                // Ограничение скорости
                if (Mathf.Abs(rb.velocity.x) > maxSpeedX)
                {
                    rb.velocity = new Vector2(maxSpeedX * rb.velocity.normalized.x, rb.velocity.y);
                }
            }
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput != 0)
            {
                // 1 - получаем нормализованный вектор направления от игрока к точке крюка
                var playerToHookDirection = (ropeHook - (Vector2)playerPosition.position).normalized;
                // 2 - Инвертируем направление, чтобы получить перпендикулярное направление
                Vector2 perpendicularDirection;

                if (horizontalInput < 0)
                {
                    perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                    var leftPerpPos = (Vector2)playerPosition.position - perpendicularDirection * -2f;
                    Debug.DrawLine(playerPosition.position, leftPerpPos, Color.green, 0f);
                }
                else
                {
                    perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                    var rightPerpPos = (Vector2)playerPosition.position + perpendicularDirection * 2f;
                    Debug.DrawLine(playerPosition.position, rightPerpPos, Color.green, 0f);
                }

                var force = perpendicularDirection * swingForce;
                foreach (var rb in rigidbody2D)
                {
                    rb.AddForce(force, ForceMode2D.Force);
                }
            }
            else
                InverseRocking();
        }
    }

    public void Jump()
    {
        // Определяем, на земле ли персонаж
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        foreach (var key in jumptButton)
        {
            // Если персонаж на земле и нажат пробел
            if (isGrounded && Input.GetKeyDown(key))
            {
                sound.PlayClip(sound.jumpPlayerSound);
                foreach (var rb in rigidbody2D)
                {
                    rb.AddForce(new Vector2(0, jumpForce));
                }
            }

        }
    }

    public void InverseRocking()
    {
        if (playerPosition.position == pastPosition)
            return;

        var playerToHookDirection = (ropeHook - (Vector2)playerPosition.position).normalized;
        Vector2 perpendicularDirection;

        if (playerPosition.position.x - pastPosition.x > 0)
        {
            perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
            var leftPerpPos = (Vector2)playerPosition.position - perpendicularDirection * -2f;
            Debug.DrawLine(playerPosition.position, leftPerpPos, Color.green, 0f);
            pastPosition = playerPosition.position;
        }
        else
        {
            perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
            var rightPerpPos = (Vector2)playerPosition.position + perpendicularDirection * 2f;
            Debug.DrawLine(playerPosition.position, rightPerpPos, Color.green, 0f);
            pastPosition = playerPosition.position;
        }

        var force = perpendicularDirection * inverseSwingForce;
        foreach (var rb in rigidbody2D)
        {
            rb.AddForce(force, ForceMode2D.Force);
        }
    }

    // Возвращает движение, если нажата кнопка
    Vector2 MoveIfPressed(List<KeyCode> keyList, Vector2 Movement)
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
        return Vector2.zero;
    }
}