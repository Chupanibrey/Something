using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    #region ���������
    private const float splineOffset = 0.5f;
    #endregion

    #region ����
    [SerializeField]
    private SpriteShapeController spriteShape;
    [SerializeField]
    private Transform[] points;
    #endregion

    #region ����� �������
    private void Awake()
    {
        UpdateVerticies();
    }
    private void Update()
    {
        UpdateVerticies();
    }
    #endregion

    #region ��������� ������
    private void UpdateVerticies()
    {
        for(int i = 0; i < points.Length -1; i++)
        {
            Vector2 vertex = points[i].localPosition;
            Vector2 towardCenter = (Vector2.zero - vertex).normalized;
            float colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;

            try
            {
                spriteShape.spline.SetPosition(i, (vertex - towardCenter * colliderRadius));
            }
            catch
            {
                Debug.Log("����� ����� ������� ������ � ���� �����, ������������");
                spriteShape.spline.SetPosition(i, (vertex - towardCenter * (colliderRadius + splineOffset)));
            }

            Vector2 lt = spriteShape.spline.GetLeftTangent(i);

            Vector2 newRt = Vector2.Perpendicular(towardCenter) * lt.magnitude;
            Vector2 newLt = Vector2.zero - (newRt);

            spriteShape.spline.SetRightTangent(i, newRt);
            spriteShape.spline.SetLeftTangent(i, newLt);
        }
    }
    #endregion
}
