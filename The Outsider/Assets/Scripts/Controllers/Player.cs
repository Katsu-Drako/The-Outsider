using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(Rigidbody), typeof(GroundChecker), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    protected const float _jumpMovementDelay = 0.25f;
    public float MovementSpeed = 6f, JumpPower = 6f;
    protected float _currentJumpMovementDelay;
    protected InputManager _inputManager;
    protected Rigidbody _rigidbody;
    protected CapsuleCollider _capsuleCollider;
    protected GroundChecker _groundChecker;
    protected SpriteRenderer _spriteRenderer;
    protected Vector3 _movementDirection = new Vector3(), _cameraOffset = Vector3.up * 1.75f, _projectedVelocity;
    protected RaycastHit[] _raycastHits = new RaycastHit[10];
    protected List<Vector3> _pushForces = new List<Vector3>();
    protected void Awake() {
        _inputManager = Manager.Get<InputManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _groundChecker = GetComponent<GroundChecker>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    protected void Update() {
        _movementDirection = Vector3.ProjectOnPlane(_inputManager.GetAxis(EAxis.Horizontal) * Camera.main.transform.right + _inputManager.GetAxis(EAxis.Vertical) * Camera.main.transform.forward, Vector3.up).normalized;
        if (_inputManager.GetButtonDown(ECommand.Jump) && _groundChecker.bGrounded) {
            _pushForces.Add(Vector3.up * JumpPower);
        }
    }
    protected void FixedUpdate() {
        if (_currentJumpMovementDelay > 0) _currentJumpMovementDelay -= Time.deltaTime;
        _projectedVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, Vector3.up);
        if (_pushForces.Any()) {
            if (_movementDirection.magnitude > 0) {
                Physics.SphereCastNonAlloc(transform.position + 0.5f * _capsuleCollider.height * Vector3.up, _capsuleCollider.radius, _movementDirection.normalized, _raycastHits, 0.1f);
                if (_raycastHits.Where(x => x.transform != null).Where(x => !x.transform.IsChildOf(transform.root)).Any()) {
                    _currentJumpMovementDelay = _jumpMovementDelay;
                }
            }
            foreach (Vector3 pushForce in _pushForces) {
                _rigidbody.AddForce(pushForce, ForceMode.Impulse);
            }
            _pushForces.Clear();
        }
        else if (_movementDirection.magnitude > 0 && MovementSpeed > 0 && _currentJumpMovementDelay <= 0)
        {
            _rigidbody.AddForce(_movementDirection * Mathf.Lerp(MovementSpeed, MovementSpeed * Time.deltaTime, _projectedVelocity.magnitude / MovementSpeed), ForceMode.Impulse);
        }
        else {
            _rigidbody.AddForce(-_projectedVelocity.normalized * Mathf.Lerp(MovementSpeed, MovementSpeed * Time.deltaTime, Mathf.Clamp01(1f - _projectedVelocity.magnitude / MovementSpeed)), ForceMode.Impulse);
        }
    }
    protected void LateUpdate() {
        Camera.main.transform.rotation = Quaternion.Euler(-_inputManager.OrbitXY.y, _inputManager.OrbitXY.x, 0);
        Camera.main.transform.position = transform.position + _cameraOffset - Camera.main.transform.forward * _inputManager.Zoom;
        float _horizontalInput = _inputManager.GetAxis(EAxis.Horizontal);
        if (_horizontalInput < 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_horizontalInput > 0) {
            _spriteRenderer.flipX = true;
        }
    }
}
