using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float cameraDepth = -10f;
    public float minX, minY, maxX, maxY;

    [SerializeField]
    private GameObject deathPanel;

    private void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Time.timeScale == 0f || deathPanel.active == true)
            Cursor.visible = true;
        else
            Cursor.visible = false;

        var find = GameObject.FindWithTag("Player");
        if (find == null)
            return;
        target = find.transform;
        var newPosition = Vector2.Lerp(transform.position, new Vector2(target.position.x, target.position.y + 3f), Time.deltaTime * speed);
        var camPosition = new Vector3(newPosition.x, newPosition.y, cameraDepth);
        var v3 = camPosition;
        var newX = Mathf.Clamp(v3.x, minX, maxX);
        var newY = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = new Vector3(newX, newY, cameraDepth);
    }
}