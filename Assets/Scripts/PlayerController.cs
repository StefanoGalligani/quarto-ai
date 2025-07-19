using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _rotSpeed;
    [SerializeField] float _movSpeed;
    [SerializeField] float _minDistance;
    [SerializeField] float _maxDistance;
    public void Start()
    {
        transform.LookAt(Vector3.zero);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = -Input.GetAxis("Mouse Y");
            transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * horizontal * _rotSpeed);

            float angleCos = -Vector3.Dot(transform.forward, Vector3.up);
            if (vertical > 0 && angleCos < 0.9f || vertical < 0 && angleCos > 0.1f)
                transform.RotateAround(Vector3.zero, transform.right, Time.deltaTime * vertical * _rotSpeed);
        }
        float movement = -Input.mouseScrollDelta.y;
        float currentDist = transform.position.magnitude;
        float newDistance = Mathf.Clamp(currentDist + movement * _movSpeed, _minDistance, _maxDistance);
        transform.position *= newDistance / currentDist;
    }
    
}
