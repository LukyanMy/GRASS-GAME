using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            cam = objectInScene.GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rot = cam.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, rot.y, 0);
    }
}
