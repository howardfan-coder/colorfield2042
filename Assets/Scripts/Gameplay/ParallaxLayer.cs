using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    
    [Tooltip("0 = 不动, 1 = 跟相机一起动, 小于1 = 远景, 大于1 = 近景")]
    public float parallaxFactor = 0.5f;

    private Vector3 lastCameraPosition;
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;

        // 只做 X 轴的话可以用 new Vector3(delta.x * factor, 0, 0);
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);

        lastCameraPosition = cameraTransform.position;
    }
}
