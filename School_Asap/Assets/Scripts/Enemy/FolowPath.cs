using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowPath : MonoBehaviour
{
    public enum MovementType
    {
        Moveing,
        Lerping
    }

    public MovementType Type = MovementType.Moveing; // вид движения
    public MovementPath MyPath; // путь движения
    public float speed = 1f;
    public float maxDistance = 0.1f; // расстояние до опознования точки

    public IEnumerator<Transform> pointInPath; // проверка точек

    private void Start()
    {
        if (MyPath == null) // проверка прикрепления пути
        {
            Debug.Log("Путь не инициализирован");
            return;
        }

        pointInPath = MyPath.GetNextPathPoint();
        pointInPath.MoveNext();

        if (pointInPath.Current == null) // проверка количества точек
        {
            Debug.Log("Не достаточно точек");
            return;
        }

        transform.position = pointInPath.Current.position; // инициализация стартовой точки в пути
    }

    private void Update()
    {
        if (pointInPath == null || pointInPath.Current == null)
            return; // нету пути

        if(Type == MovementType.Moveing)
        {
            // перемещение обьекта к выбранной точке
            transform.position = Vector2.MoveTowards(transform.position, pointInPath.Current.position, Time.deltaTime * speed);
        }
        else if(Type == MovementType.Lerping)
        {
            // перемещение обьекта к выбранной точке рывками
            transform.position = Vector2.Lerp(transform.position, pointInPath.Current.position, Time.deltaTime * speed);
        }

        var distanceSquare = (transform.position - pointInPath.Current.position).sqrMagnitude; // проверка, достаточно ли мы близко к точке, чтобы сменить точку движения

        if(distanceSquare < maxDistance * maxDistance)
        {
            pointInPath.MoveNext(); // смена точки
        }
    }
}
