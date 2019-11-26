using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    protected virtual void OnEnable() {
        Camera.onPreCull += FaceCamera;
    }
    protected virtual void OnDisable() {
        Camera.onPreCull -= FaceCamera;
    }

    protected void FaceCamera(Camera camera) {
        transform.LookAt(transform.position + Vector3.ProjectOnPlane(transform.position - camera.transform.position, Vector3.up).normalized, Vector3.up);
    }
}
