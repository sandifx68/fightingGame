using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetmove : MonoBehaviour
{
    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private Camera cam = null;


    void Update()
    {
        float cx = -cam.transform.rotation.eulerAngles.x/Mathf.Rad2Deg;
        float cy = cam.transform.rotation.eulerAngles.y/Mathf.Rad2Deg; 
        transform.position =cam.transform.position + new Vector3(Mathf.Sin(cy)*radius -(radius - Mathf.Cos(cx) * radius)*Mathf.Sin(cy), 
                                                                 Mathf.Sin(cx)*radius, 
                                                                 Mathf.Cos(cy)*radius -(radius - Mathf.Cos(cx) * radius)*Mathf.Cos(cy));
    }
}
