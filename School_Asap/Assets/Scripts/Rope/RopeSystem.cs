using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RopeSystem : MonoBehaviour
{
    #region Необходимые переменные
    [SerializeField]
    private LayerMask ropeLayerMask;
    [SerializeField]
    private float ropeMaxCastDistance = 8f;
    [SerializeField]
    private DistanceJoint2D ropeJoint;
    [SerializeField]
    private SoftPlayerController playerMovement;

    [SerializeField]
    private float timeBetweenFires = 0.3f; // Задержка между выстрелами (кулдаун)
    [SerializeField]
    private float timeTilNextFire = 0.0f; // Счетчик задержки между выстрелами

    private Vector2 playerPosition;

    [SerializeField]
    private Sound sound;

    #region Переменные необходимые для создания прицела
    [SerializeField]
    private Transform crosshair;
    [SerializeField]
    private SpriteRenderer crosshairSprite;
    [SerializeField]
    private float radiusCrosshair = 0.5f;

    private bool ropeAttached;
    #endregion

    #region Точка прицепления
    [SerializeField]
    private GameObject ropeHingeAnchor;

    private Rigidbody2D ropeHingeAnchorRb;
    private SpriteRenderer ropeHingeAnchorSprite;
    #endregion

    # region Отрисовка верёвки
    [SerializeField]
    private LineRenderer ropeRenderer;

    private List<Vector2> ropePositions = new List<Vector2>();
    private bool distanceSet;
    #endregion

    # region Словарь контактов верёвки
    private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();
    #endregion

    #region Спуск/подьём по верёвке
    [SerializeField]
    private float climbSpeed = 3f;
    #endregion
    #endregion

    void Awake()
    {
        sound = GameObject.FindWithTag("GameController").GetComponent<Sound>();
        ropeJoint.enabled = false;
        playerPosition = transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        HandleRopeLength();
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        // Вычисление угла прицеливания
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
            aimAngle = Mathf.PI * 2 + aimAngle;

        // Вектор направления прицела
        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        playerPosition = transform.position;

        // Определение прекреплена ли верёвка к поверхности
        if (!ropeAttached)
        {
            SetCrosshairPosition(aimAngle);
            playerMovement.isSwinging = false;
        }
        else
        {
            playerMovement.isSwinging = true;
            playerMovement.ropeHook = ropePositions.Last();
            crosshairSprite.enabled = false;

            if (ropePositions.Count > 0)
            {
                var lastRopePoint = ropePositions.Last();
                var playerToCurrentNextHit = Physics2D.Raycast(playerPosition, (lastRopePoint - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint) - 0.1f, ropeLayerMask);

                if (playerToCurrentNextHit)
                {
                    var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                    if (colliderWithVertices != null)
                    {
                        var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                        if (wrapPointsLookup.ContainsKey(closestPointToHit))
                        {
                            ResetRope();
                            return;
                        }

                        ropePositions.Add(closestPointToHit);
                        wrapPointsLookup.Add(closestPointToHit, 0);
                        distanceSet = false;
                    }
                }
            }
        }

        HandleInput(aimDirection);
        UpdateRopePositions();
        HandleRopeUnwrap();
    }

    private void SetCrosshairPosition(float aimAngle)
    {
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        // Преобразуем координаты прицела на основании угла и радиуса
        var x = transform.position.x + radiusCrosshair * Mathf.Cos(aimAngle);
        var y = transform.position.y + radiusCrosshair * Mathf.Sin(aimAngle);

        // Задаём преобразованную позицию прицела
        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetMouseButton(0) && timeTilNextFire < 0)
        {
            if (ropeAttached) return;

            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);

            if (hit.collider != null)
            {
                ropeAttached = true;
                if (!ropePositions.Contains(hit.point))
                {
                    sound.PlayClip(sound.ropeSound);
                    // Немного подпрыгивает над землёй, когда игрок к чему-то прицепится крюком.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    ropePositions.Add(hit.point);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    ropeHingeAnchorSprite.enabled = true;
                    HealthSystem.HealphCount -= 10;
                }
            }
            else
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }

        timeTilNextFire -= Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            timeTilNextFire = timeBetweenFires;
            ResetRope();
        }
    }

    public void ResetRope()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropePositions.Clear();
        ropeHingeAnchorSprite.enabled = false;
        wrapPointsLookup.Clear();
    }

    private void UpdateRopePositions()
    {
        // Выходим если верёвка не прикреплена
        if (!ropeAttached)
        {
            return;
        }

        // Количество точек верёвки
        ropeRenderer.positionCount = ropePositions.Count + 1;

        // присваиваем позиции вершины рендера линии
        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1) 
            {
                ropeRenderer.SetPosition(i, ropePositions[i]);

                // Присваиваем опорной точке верёвки вторую с конца позицию верёвки
                if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                {
                    var ropePosition = ropePositions[ropePositions.Count - 1];
                    if (ropePositions.Count == 1)
                    {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                    else
                    {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                // обрабатываем случай, когда текущая позиция верёвки в цикле является второй с конца
                else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                {
                    var ropePosition = ropePositions.Last();
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                // обрабатывает присвоение позицию последней вершины верёвки значения текущей позиции игрока.
                ropeRenderer.SetPosition(i, transform.position);
            }
        }
    }

    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        // преобразуем коллекцию точек полигонального коллайдера в словарь позиций Vector2
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        // Возвращается ближайшее расстояние
        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

    private void HandleRopeLength()
    {
        if (Input.GetAxis("Vertical") >= 1f && ropeAttached)
        {
            if(ropeJoint.distance > 0)
                ropeJoint.distance -= Time.deltaTime * climbSpeed;
        }
        else if (Input.GetAxis("Vertical") < 0f && ropeAttached)
        {
            if(ropeMaxCastDistance > Vector2.Distance(transform.position, ropePositions.First()))
                ropeJoint.distance += Time.deltaTime * climbSpeed;
        }
    }

    private void HandleRopeUnwrap()
    {
        if (ropePositions.Count <= 1)
        {
            return;
        }

        // Hinge = следующая точка вверх от позиции игрока
        // Anchor = следующая точка вверх от Hinge
        // Hinge Angle = угол между anchor и hinge
        // Player Angle = угол между anchor и player
        var anchorIndex = ropePositions.Count - 2;
        var hingeIndex = ropePositions.Count - 1;
        var anchorPosition = ropePositions[anchorIndex];
        var hingePosition = ropePositions[hingeIndex];
        var hingeDir = hingePosition - anchorPosition;
        var hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
        var playerDir = playerPosition - anchorPosition;
        var playerAngle = Vector2.Angle(anchorPosition, playerDir);

        if (!wrapPointsLookup.ContainsKey(hingePosition))
        {
            Debug.LogError("Мы не нашли hingePosition (" + hingePosition + ") при осмотре словаря.");
            return;
        }

        // гарантия обновления словаря точек прицепления
        if (playerAngle < hingeAngle)
        {
            if (wrapPointsLookup[hingePosition] == 1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            wrapPointsLookup[hingePosition] = -1;
        }
        else
        {
            if (wrapPointsLookup[hingePosition] == -1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            wrapPointsLookup[hingePosition] = 1;
        }
    }

    private void UnwrapRopePosition(int anchorIndex, int hingeIndex)
    {
        var newAnchorPosition = ropePositions[anchorIndex];
        wrapPointsLookup.Remove(ropePositions[hingeIndex]);
        ropePositions.RemoveAt(hingeIndex);

        ropeHingeAnchorRb.transform.position = newAnchorPosition;
        distanceSet = false;

        // Задаём новое расстояние joint distance для положения anchor, если оно еще не установлено.
        if (distanceSet)
        {
            return;
        }
        ropeJoint.distance = Vector2.Distance(transform.position, newAnchorPosition);
        distanceSet = true;
    }
}