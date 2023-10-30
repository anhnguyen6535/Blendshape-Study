using UnityEngine;

public class CameraCircleMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0, 0, 0);
    public float radius;
    public float rotationSpeed;

    private float currentAngle;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraCircleMovement: Target not set, defaulting to world origin.");
            target = new GameObject("CameraTarget").transform;
        }
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;

        float x = target.position.x + radius * Mathf.Cos(currentAngle);
        float z = target.position.z + radius * Mathf.Sin(currentAngle);
        Vector3 newPosition = new Vector3(x, transform.position.y, z);

        transform.position = newPosition;
        transform.LookAt(target.position + targetOffset);
    }
}