using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundChecker : MonoBehaviour//PersistentComponent
{
    [Range(0, 90f)]
    public float MaxStepAngle = 45f;
    public float GroundRaycastDistance = 0.05f;
    [Range(0, 1f)]
    public float GroundedFriction = 0.6f, GroundedBounciness, AirborneFriction, AirborneBounciness = 0.1f;
    public PhysicMaterialCombine GroundedFrictionCombine = PhysicMaterialCombine.Average, GroundedBounceCombine = PhysicMaterialCombine.Average, AirborneFrictionCombine = PhysicMaterialCombine.Minimum, AirborneBounceCombine = PhysicMaterialCombine.Maximum;
    protected RaycastHit[] _groundHits = new RaycastHit[100];
    protected Collider _collider;
    protected bool _bGrounded;
    public bool bGrounded {
        get {
            return _bGrounded;
        }
        set {
            if (_bGrounded != value) {
                if (value)
                {
                    BroadcastMessage("Landing", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    BroadcastMessage("Falling", SendMessageOptions.DontRequireReceiver);
                }
            }
            _bGrounded = value;
            // _animator.SetBool("bGrounded", _animator.applyRootMotion = _bGrounded = value);
            if (value) {
                // _animator.ResetTrigger(_jumpHash);
                _collider.material.dynamicFriction = _collider.material.staticFriction = GroundedFriction;
                _collider.material.frictionCombine = GroundedFrictionCombine;
                _collider.material.bounciness = GroundedBounciness;
                _collider.material.bounceCombine = GroundedBounceCombine;
            }
            else {
                _collider.material.dynamicFriction = _collider.material.staticFriction = AirborneFriction;
                _collider.material.frictionCombine = AirborneFrictionCombine;
                _collider.material.bounciness = AirborneBounciness;
                _collider.material.bounceCombine = AirborneBounceCombine;
            }
        }
    }
    protected void Awake() {
        //base.Awake();
        _collider = GetComponent<Collider>();
    }
    protected void Start() {
        //base.Start();
        bGrounded = bGrounded;
    }
    protected virtual void FixedUpdate() {
        GroundCheck();
    }
    protected float GetColliderRadius() {
        if (_collider is CapsuleCollider _capsuleCollider) {
            return _capsuleCollider.radius;
        }
        else if (_collider is SphereCollider _sphereCollider) {
            return _sphereCollider.radius;
        }
        else if (_collider is BoxCollider _boxCollider) {
            return Mathf.Min(0.5f * _boxCollider.size.x, 0.5f * _boxCollider.size.z);
        }
        else return 0.25f;
    }
    protected void GroundCheck() {
        Array.Clear(_groundHits, 0, _groundHits.Length);
        Physics.SphereCastNonAlloc(transform.position + Vector3.up * GroundRaycastDistance, GetColliderRadius(), Vector3.down, _groundHits, 2f * GroundRaycastDistance);
        bGrounded = false;
        foreach (RaycastHit groundHit in _groundHits.Where(x => x.transform != null))
        {
            if (!groundHit.transform.root.IsChildOf(transform.root))
            {
                if (groundHit.normal.y >= (1f - MaxStepAngle / 90f))
                {
                    if (Physics.Raycast(transform.position + Vector3.up * GroundRaycastDistance, Vector3.down, 2f * GroundRaycastDistance)) {
                        bGrounded = true;
                        break;
                    }
                }
            }
        }
    }
}
