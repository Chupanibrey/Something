using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
    public enum PathTypes // Линейный или зацикленный путь
    {
        linear,
        loop
    }

    public PathTypes PathType; // Тип пути
    public int movementDirection = 1; // направление вперёд или назад
    public int moveingTo = 0; // номер точки, к которой рисовать линию
    public Transform[] PathElements; // массив из точек движения

    public void OnDrawGizmos() // отображение линий между точками пути
    {
        if (PathElements == null || PathElements.Length < 2) // проверка хотябы на две точки в пути
            return;

        for (int i = 1; i < PathElements.Length; i++) // цикл по всем точкам массива
        {
            // команда рисования линий между точками
            Gizmos.DrawLine(PathElements[i - 1].position, PathElements[i].position);
        }

        if (PathType == PathTypes.loop) // замыкание пути, рисуем линии между первой и последней точкой
        {
            Gizmos.DrawLine(PathElements[0].position, PathElements[PathElements.Length - 1].position);
        }
    }

    public IEnumerator<Transform> GetNextPathPoint() // получает положение следующей точки
    {
        if (PathElements == null || PathElements.Length < 1) //проверка на наличие точек в массиве
        {
            yield break;
        }

        while (true)
        {
            yield return PathElements[moveingTo]; // текущие положение точки

            if (PathElements.Length == 1) // если точка всего одна, выйти
            {
                continue;
            }

            if (PathType == PathTypes.linear)
            {
                if (moveingTo <= 0) // движение по нарастаюшей
                {
                    movementDirection = 1;
                }
                else if (moveingTo >= PathElements.Length - 1) // движение по убывающей
                {
                    movementDirection = -1;
                }

                moveingTo = moveingTo + movementDirection; // диапазон движения от 1 до -1

                if(PathType == PathTypes.loop)
                {
                    if(moveingTo >= PathElements.Length) // инверсия движения при достижении последней точки
                    {
                        moveingTo = 0;
                    }

                    if(moveingTo < 0) // инверсия движения при достижении первой точки
                    {
                        moveingTo = PathElements.Length - 1;
                    }
                }
            }
        }
    }
}