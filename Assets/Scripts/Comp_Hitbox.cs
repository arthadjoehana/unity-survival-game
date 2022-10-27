using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp_Hitbox : MonoBehaviour, IHitDetector
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private LayerMask _layerMask;

    private float thickness = 0.025f;
    private IHitResponder _hitResponder;

    IHitResponder IHitDetector.HitResponder { get => _hitResponder; set => _hitResponder = value; }

    public void CheckHit()
    {
        Vector3 scaledSize = new Vector3(
            _collider.size.x * transform.lossyScale.x,
            _collider.size.y * transform.lossyScale.y,
            _collider.size.z * transform.lossyScale.z);

        float distance = scaledSize.y - thickness;
        Vector3 direction = transform.up;
        Vector3 center = transform.TransformPoint(_collider.center);
        Vector3 start = center - direction * (distance / 2);
        Vector3 halfExtents = new Vector3(scaledSize.x, thickness, scaledSize.z) / 2;
        Quaternion orientation = transform.rotation;

        HitData hitdata = null;
        IHurtbox hurtbox = null;
        RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, orientation, distance, _layerMask);
        foreach (RaycastHit hit in hits)
        {
            hurtbox = hit.collider.GetComponent<IHurtbox>();
            if (hurtbox != null)
                if (hurtbox.Active)
                {
                    hitdata = new HitData
                    {
                        damage = _hitResponder == null ? 0 : _hitResponder.Damage,
                        hitPoint = hit.point == Vector3.zero ? center : hit.point,
                        hitNormal = hit.normal,
                        hurtbox = hurtbox,
                        hitDetector = this
                    };

                    if (hitdata.Validate())
                    {
                        hitdata.hitDetector.HitResponder?.Response(hitdata);
                        hitdata.hurtbox.HurtResponder?.Response(hitdata);
                    }
                }
        }
    }
}
