using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] private Transform camPos;

    void Update() {
        transform.position = camPos.position;
    }
}
