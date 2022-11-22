using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour {
    [SerializeField] private CharacterController CC;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private GameObject player;

    [SerializeField] int speed;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        Rotate();
        Move();

    }

    [SerializeField] Vector3 hitPoint;

    void Rotate() {
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, player.transform.position);

        if (plane.Raycast(mouseRay, out float hitDistance)) {
            hitPoint = mouseRay.GetPoint(hitDistance);
            player.transform.LookAt(hitPoint);
        }
    }

    void Move() {

        if (!CC.isGrounded) {
            return;
        }

        if (Input.GetKey(KeyCode.W)) {
            CC.Move(new Vector3(0, 0, .1f) * speed);
        }

        if (Input.GetKey(KeyCode.A)) {
            CC.Move(new Vector3(-.1f, 0, 0) * speed);
        }

        if (Input.GetKey(KeyCode.S)) {
            CC.Move(new Vector3(0, 0, -.1f) * speed);
        }

        if (Input.GetKey(KeyCode.D)) {
            CC.Move(new Vector3(.1f, 0, 0) * speed);
        }
    }
}
