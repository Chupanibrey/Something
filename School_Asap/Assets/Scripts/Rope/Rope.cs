using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rope : MonoBehaviour
{
    #region Реализация интеграции Верле
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    // Длина 1 сегмента верёвки
    private float ropeSegLen = 0.25f;
    // Колличество сегментов
    private int segmentsLength = 70;
    // Ширина верёвки
    private float lineWidht = 0.125f;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    // Механика коллайдера
    //EdgeCollider2D edgeCollider;
    //Vector2 pointsCollider;
    //Vector2[] pointsColliderArray;

    // Фиксированный шаг
    private float stepTime = 0.01f;
    private float maxStep = 0.1f;
    private float timeAccum;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for(int i = 0; i < segmentsLength; i++)
        {
            ropeSegments.Add(new RopeSegment(startPoint));
            startPoint.y -= ropeSegLen;
        }

        // Выделяем коллизионные структуры.
        collisionInfos = new CollisionInfo[MAX_ROPE_COLLISIONS];
        for (int i = 0; i < collisionInfos.Length; i++)
        {
            // Каждый коллайдер может столкнуться с таким количеством узлов, сколько есть в веревке.
            collisionInfos[i] = new CollisionInfo(segmentsLength);
        }

        // Буфер для OverlapCircleNonAlloc.
        colliderBuffer = new Collider2D[COLLIDER_BUFFER_SIZE];

        // Механика коллайдера
        //pointsColliderArray = new Vector2[segmentsLength];
        //edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        //newPositionsCollider();
    }

    void Update()
    {
        if (shouldSnapshotCollision)
        {
            SnapshotCollisions();
        }

        // Фиксированный шаг
        timeAccum += Time.deltaTime;
        timeAccum = Mathf.Min(timeAccum, maxStep);
        while (timeAccum >= stepTime)
        {
            Simulate();

            //newPositionsCollider();

            DrawRope();

            timeAccum -= stepTime;
        }
    }

    private void FixedUpdate()
    {
        shouldSnapshotCollision = true;
    }

    // Обновление точек коллайдера
    //void newPositionsCollider()
    //{
    //    for (int i = 0; i < lineRenderer.positionCount; i++)
    //    {
    //        pointsCollider = lineRenderer.GetPosition(i);
    //        pointsColliderArray[i] = new Vector2(pointsCollider.x, pointsCollider.y);
    //    }

    //    edgeCollider.points = pointsColliderArray;
    //}

    private void Simulate()
    {
        // Симуляция
        Vector2 forceGravity = new Vector2(0f, -0.36f);

        int segmentsLenght = this.segmentsLength;
        for (int i = 0; i < segmentsLenght; i++)
        {
            RopeSegment currentSegment = ropeSegments[i];
            Vector2 velocity = currentSegment.currentPos - currentSegment.pastPos;
            currentSegment.pastPos = currentSegment.currentPos;
            currentSegment.currentPos += velocity;
            currentSegment.currentPos += forceGravity * Time.deltaTime;
            ropeSegments[i] = currentSegment;
        }

        
        for(int i = 0; i < 50; i++)
        {
            // Ограничение
            ApplyConstraint();

            // Столкновение
            AdjustCollisions();
        }
    }

    private void ApplyConstraint()
    {
        // Первый сегмент копирует положение startPoint
        RopeSegment segments = ropeSegments[0];
        segments.currentPos = startPoint.position;
        ropeSegments[0] = segments;

        // startPoint копирует положение первого сегмента привязанного к позиции мыши
        //RopeSegment segments = ropeSegments[0];
        //segments.currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //ropeSegments[0] = segments;
        //startPoint.position = segments.currentPos;


        int segmentsLength = this.segmentsLength;
        // Последний сегмент копирует положение endPoint
        //segments = ropeSegments[segmentsLength - 1];
        //segments.currentPos = endPoint.position;
        //ropeSegments[segmentsLength - 1] = segments;

        // endPoint копирует положение последнего сегмента
        segments = ropeSegments[segmentsLength - 1];
        endPoint.position = segments.currentPos;

        for (int i = 0; i < segmentsLength - 1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firstSeg.currentPos - secondSeg.currentPos).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;
            

            if (dist > ropeSegLen)
                changeDir = (firstSeg.currentPos - secondSeg.currentPos).normalized;
            else if (dist < ropeSegLen)
                changeDir = (secondSeg.currentPos - firstSeg.currentPos).normalized;

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.currentPos -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.currentPos += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.currentPos += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidht = this.lineWidht;
        lineRenderer.startWidth = lineWidht;
        lineRenderer.endWidth = lineWidht;

        int segmentsLength = this.segmentsLength;
        Vector3[] ropePositions = new Vector3[segmentsLength];
        for(int i = 0; i < segmentsLength; i++)
        {
            ropePositions[i] = ropeSegments[i].currentPos;
        }

        lineRenderer.positionCount = segmentsLength;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 currentPos;
        public Vector2 pastPos;

        public RopeSegment(Vector2 pos)
        {
            currentPos = pos;
            pastPos = pos;
        }
    }
    #endregion

    #region Снимок коллайдеров при пересечении
    // Максимальное количество коллайдеров, которых может коснуться веревка.
    private const int MAX_ROPE_COLLISIONS = 32;
    // Размер буфера коллайдера; максимальное количество коллайдеров, которых может коснуться один узел.
    private const int COLLIDER_BUFFER_SIZE = 8;

    public float collisionRadius = .5f; // Радиус моментального снимка вокруг каждого узла.
    private int numCollisions;
    private bool shouldSnapshotCollision;
    private CollisionInfo[] collisionInfos;
    private Collider2D[] colliderBuffer;

    private void SnapshotCollisions()
    {
        numCollisions = 0;
        // Проходим по каждому узлу и получаем коллизии в пределах радиуса.
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            int collisions =
                Physics2D.OverlapCircleNonAlloc(ropeSegments[i].currentPos, collisionRadius, colliderBuffer);

            for (int j = 0; j < collisions; j++)
            {
                Collider2D col = colliderBuffer[j];
                int id = col.GetInstanceID();

                int idx = -1;
                for (int k = 0; k < numCollisions; k++)
                {
                    if (collisionInfos[k].id == id)
                    {
                        idx = k;
                        break;
                    }
                }

                // Если коллайдера нету, нам нужно его добавить.
                if (idx < 0)
                {
                    // Записываем все данные, которые нам нужно использовать, в наш класс.
                    CollisionInfo ci = collisionInfos[numCollisions];
                    ci.id = id;
                    ci.wtl = col.transform.worldToLocalMatrix;
                    ci.ltw = col.transform.localToWorldMatrix;
                    ci.scale.x = ci.ltw.GetColumn(0).magnitude;
                    ci.scale.y = ci.ltw.GetColumn(1).magnitude;
                    ci.position = col.transform.position;
                    ci.numCollisions = 1; 
                    ci.collidingNodes[0] = i;
                    ci.isTrigger = col.isTrigger;

                    switch (col)
                    {
                        case CircleCollider2D c:
                            ci.colliderType = ColliderType.Circle;
                            ci.colliderSize.x = ci.colliderSize.y = c.radius;
                            break;
                        case BoxCollider2D b:
                            ci.colliderType = ColliderType.Box;
                            ci.colliderSize = b.size;
                            break;
                        default:
                            ci.colliderType = ColliderType.None;
                            break;
                    }

                    numCollisions++;
                    if (numCollisions >= MAX_ROPE_COLLISIONS)
                    {
                        return;
                    }
                    // Если мы нашли коллайдер, то нам просто нужно связать коллизии и добавить наш узел.
                }
                else
                {
                    CollisionInfo ci = collisionInfos[idx];
                    if (ci.numCollisions >= segmentsLength)
                    {
                        continue;
                    }

                    ci.collidingNodes[ci.numCollisions++] = i;
                }
            }
        }

        shouldSnapshotCollision = false;
    }
    #endregion

    #region Регулирование столкновений
    private void AdjustCollisions()
    {
        for (int i = 0; i < numCollisions; i++)
        {
            CollisionInfo ci = collisionInfos[i];

            if (!ci.isTrigger)
            {
                switch (ci.colliderType)
                {
                    case ColliderType.Circle:
                        {
                            float radius = ci.colliderSize.x * Mathf.Max(ci.scale.x, ci.scale.y);

                            for (int j = 0; j < ci.numCollisions; j++)
                            {
                                RopeSegment node = ropeSegments[ci.collidingNodes[j]];
                                float distance = Vector2.Distance(ci.position, node.currentPos);

                                // Продолжаем если столкновения нету.
                                if (distance - radius > 0)
                                {
                                    continue;
                                }

                                // Толкаем точку за пределы круга.
                                Vector2 dir = (node.currentPos - ci.position).normalized;
                                Vector2 hitPos = ci.position + dir * radius;
                                node.currentPos = hitPos;
                                ropeSegments[ci.collidingNodes[j]] = node;
                            }
                        }
                        break;

                    case ColliderType.Box:
                        {
                            for (int j = 0; j < ci.numCollisions; j++)
                            {
                                RopeSegment node = ropeSegments[ci.collidingNodes[j]];
                                Vector2 localPoint = ci.wtl.MultiplyPoint(node.currentPos);

                                // Если расстояние от центра больше "радиуса" коробки, то мы не можем столкнуться.
                                Vector2 half = ci.colliderSize * .5f;
                                Vector2 scalar = ci.scale;
                                float dx = localPoint.x;
                                float px = half.x - Mathf.Abs(dx);
                                if (px <= 0)
                                {
                                    continue;
                                }

                                float dy = localPoint.y;
                                float py = half.y - Mathf.Abs(dy);
                                if (py <= 0)
                                {
                                    continue;
                                }

                                // Вытолкнуть узел вдоль ближайшего края.
                                // Нужно умножить расстояние на масштаб, иначе мы испортим масштабированные углы коробки.
                                if (px * scalar.x < py * scalar.y)
                                {
                                    float sx = Mathf.Sign(dx);
                                    localPoint.x = half.x * sx;
                                }
                                else
                                {
                                    float sy = Mathf.Sign(dy);
                                    localPoint.y = half.y * sy;
                                }

                                Vector2 hitPos = ci.ltw.MultiplyPoint(localPoint);
                                node.currentPos = hitPos;
                                ropeSegments[ci.collidingNodes[j]] = node;
                            }
                        }
                        break;
                }
            }
        }
    }
    #endregion
}



enum ColliderType
{
    Circle,
    Box,
    None,
}

class CollisionInfo
{
    public int id;

    public ColliderType colliderType;
    public Vector2 colliderSize;
    public Vector2 position;
    public Vector2 scale;
    public Matrix4x4 wtl;
    public Matrix4x4 ltw;
    public int numCollisions;
    public int[] collidingNodes;
    public bool isTrigger;

    public CollisionInfo(int maxCollisions)
    {
        this.id = -1;
        this.colliderType = ColliderType.None;
        this.colliderSize = Vector2.zero;
        this.position = Vector2.zero;
        this.scale = Vector2.zero;
        this.wtl = Matrix4x4.zero;
        this.ltw = Matrix4x4.zero;

        this.numCollisions = 0;
        this.collidingNodes = new int[maxCollisions];
    }
}
