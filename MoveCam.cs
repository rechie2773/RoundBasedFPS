using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveCam : MonoBehaviour
{
    public Transform camerapos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camerapos.position;
    }
}
