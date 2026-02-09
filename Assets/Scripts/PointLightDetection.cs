using UnityEngine;
using System.Collections.Generic;

public class PointLightDetection : MonoBehaviour
{
    [Header("References")]
    public Light pointLight;

    [Header("Detection Settings")]
    public LayerMask targetMask;    // Layer des mobs
    public LayerMask obstacleMask;  // Murs / d�cors
    public float minIntensity = 0.2f;
    public float checkRadius = 20f;

    [Header("Debug")]
    public bool drawDebug = true;

    void Update()
    {
        if (pointLight == null)
            return;

        if (!pointLight.enabled || !gameObject.activeInHierarchy)
        {
            DisableAllTargets();
            return;
        }

        // R�cup�re tous les mobs dans le rayon de la lumi�re
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            pointLight.range,
            targetMask
        );

        foreach (Collider col in targets)
        {
            LightReceiver receiver = col.GetComponent<LightReceiver>();
            if (receiver == null)
                continue;

            Vector3 targetPos = col.bounds.center;
            float distance = Vector3.Distance(transform.position, targetPos);

            Vector3 direction = (targetPos - transform.position).normalized;

            // Obstacle ?
            if (Physics.Raycast(transform.position, direction, distance, obstacleMask))
            {
                receiver.SetIlluminated(false);
                if (drawDebug)
                    Debug.DrawRay(transform.position, direction * distance, Color.red);
                continue;
            }

            // Att�nuation de la lumi�re
            float intensity = pointLight.intensity / (distance * distance);

            bool lit = intensity > minIntensity;
            receiver.SetIlluminated(lit);

            if (drawDebug)
                Debug.DrawRay(
                    transform.position,
                    direction * distance,
                    lit ? Color.green : Color.yellow
                );
        }
    }


    void DisableAllTargets()
    {
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            pointLight.range,
            targetMask
        );

        foreach (Collider col in targets)
        {
            LightReceiver receiver = col.GetComponent<LightReceiver>();
            if (receiver != null)
                receiver.SetIlluminated(false);
        }
    }

}

