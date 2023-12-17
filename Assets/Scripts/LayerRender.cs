using UnityEngine;

public class LayerRender : MonoBehaviour
{
    public Camera mainCamera;
    public string targetTag = "bullet";
    public float cullDistance = 15f;
    private int defaultLayer;
    private int cullLayer;

    void Start()
    {
        // Cache the default layer and the cull layer
        defaultLayer = LayerMask.NameToLayer("Default");
        cullLayer = LayerMask.NameToLayer("bullet");
    }

    void Update()
    {
        // Find all objects with the target tag
        foreach (var obj in GameObject.FindGameObjectsWithTag(targetTag))
        {
            if (Vector3.Distance(mainCamera.transform.position, obj.transform.position) < cullDistance)
            {
                // If close and in the direction, change to cull layer
                obj.layer = cullLayer;
            }
            else
            {
                // Otherwise, use the default layer
                obj.layer = defaultLayer;
            }
        }
    }
}
