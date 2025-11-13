using UnityEngine;

namespace MVsToolkit.Utils
{
    public static class RotationUtils
    {
        public static Quaternion QuaternionSmoothDamp(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            if (Time.deltaTime == 0) return current;
            if (smoothTime == 0) return target;

            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
                Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
                Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
                Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static void LookAtSmoothDamp(this Transform transform, Vector3 targetPosition, ref Vector3 refVelocity, float smoothTime)
        {
            // Direction vers la cible
            Vector3 direction = (targetPosition - transform.position).normalized;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            // Rotation désirée
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Convertir en euler pour appliquer SmoothDampAngle
            Vector3 targetEuler = targetRotation.eulerAngles;
            Vector3 currentEuler = transform.rotation.eulerAngles;

            currentEuler.x = Mathf.SmoothDampAngle(currentEuler.x, targetEuler.x, ref refVelocity.x, smoothTime);
            currentEuler.y = Mathf.SmoothDampAngle(currentEuler.y, targetEuler.y, ref refVelocity.y, smoothTime);
            currentEuler.z = Mathf.SmoothDampAngle(currentEuler.z, targetEuler.z, ref refVelocity.z, smoothTime);

            transform.rotation = Quaternion.Euler(currentEuler);
        }
    }
}