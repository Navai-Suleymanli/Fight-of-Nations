using UnityEngine;

public class BulletLayerManager : MonoBehaviour
{
    private int defaultLayer;
    private int cullLayer;
    private Camera mainCamera;
    public float cullDistance = 15f;

    void Awake()
    {
        // Cache the layers
        defaultLayer = LayerMask.NameToLayer("Default");
        cullLayer = LayerMask.NameToLayer("bullet");

        // Find the main camera
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        // When the bullet is activated, check the distance and set the layer
        if (Vector3.Distance(mainCamera.transform.position, transform.position) < cullDistance)
        {
            gameObject.layer = cullLayer;
        }
        else
        {
            gameObject.layer = defaultLayer;
        }
    }

    void OnDisable()
    {
        // When the bullet is deactivated, reset to default layer
        gameObject.layer = defaultLayer;
    }
}
