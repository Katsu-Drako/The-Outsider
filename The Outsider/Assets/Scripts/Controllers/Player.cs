using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody), typeof(GroundChecker))]
public class Player : MonoBehaviour
{
    public float MovementSpeed = 6f, JumpPower = 6f;
    protected InputManager _inputManager;
    protected Rigidbody _rigidbody;
    protected GroundChecker _groundChecker;
    protected Vector3 _movementDirection = new Vector3(), _cameraOffset = Vector3.up * 2f;
    protected List<Vector3> _pushForces = new List<Vector3>();
    protected void Awake() {
        _inputManager = Manager.Get<InputManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _groundChecker = GetComponent<GroundChecker>();
    }
    protected void Update() {
        if (MovementSpeed > 0)
        {
            _movementDirection = Vector3.ProjectOnPlane(_inputManager.GetAxis(EAxis.Horizontal) * Camera.main.transform.right + _inputManager.GetAxis(EAxis.Vertical) * Camera.main.transform.forward, Vector3.up).normalized;
            if (_movementDirection.magnitude > 0)
            {
                _rigidbody.AddForce(_movementDirection * Mathf.Lerp(MovementSpeed, MovementSpeed * Time.deltaTime, _rigidbody.velocity.magnitude / MovementSpeed), ForceMode.Impulse);
            }
        }
        if (_inputManager.GetButtonDown(ECommand.Jump) && _groundChecker.bGrounded) {
            _pushForces.Add(Vector3.up * JumpPower);
        }
    }
    protected void FixedUpdate() {
        foreach (Vector3 pushForce in _pushForces) {
            _rigidbody.AddForce(pushForce, ForceMode.Impulse);
        }
        _pushForces.Clear();
    }
    protected void LateUpdate() {
        Camera.main.transform.rotation = Quaternion.Euler(-_inputManager.OrbitXY.y, _inputManager.OrbitXY.x, 0);
        Camera.main.transform.position = transform.position + _cameraOffset - Camera.main.transform.forward * _inputManager.Zoom;
    }
}
