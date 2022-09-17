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

    public MovementType Type = MovementType.Moveing; // ��� ��������
    public MovementPath MyPath; // ���� ��������
    public float speed = 1f;
    public float maxDistance = 0.1f; // ���������� �� ����������� �����

    public IEnumerator<Transform> pointInPath; // �������� �����

    private void Start()
    {
        if (MyPath == null) // �������� ������������ ����
        {
            Debug.Log("���� �� ���������������");
            return;
        }

        pointInPath = MyPath.GetNextPathPoint();
        pointInPath.MoveNext();

        if (pointInPath.Current == null) // �������� ���������� �����
        {
            Debug.Log("�� ���������� �����");
            return;
        }

        transform.position = pointInPath.Current.position; // ������������� ��������� ����� � ����
    }

    private void Update()
    {
        if (pointInPath == null || pointInPath.Current == null)
            return; // ���� ����

        if(Type == MovementType.Moveing)
        {
            // ����������� ������� � ��������� �����
            transform.position = Vector2.MoveTowards(transform.position, pointInPath.Current.position, Time.deltaTime * speed);
        }
        else if(Type == MovementType.Lerping)
        {
            // ����������� ������� � ��������� ����� �������
            transform.position = Vector2.Lerp(transform.position, pointInPath.Current.position, Time.deltaTime * speed);
        }

        var distanceSquare = (transform.position - pointInPath.Current.position).sqrMagnitude; // ��������, ���������� �� �� ������ � �����, ����� ������� ����� ��������

        if(distanceSquare < maxDistance * maxDistance)
        {
            pointInPath.MoveNext(); // ����� �����
        }
    }
}
