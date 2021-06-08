#region

using Tank;
using UnityEngine;

#endregion

namespace Shell
{
	public class ShellExplosion : MonoBehaviour
	{
		[SerializeField]
		private LayerMask tankMask;
		[SerializeField]
		private ParticleSystem explosionParticles;
		[SerializeField]
		private AudioSource explosionAudio;
		private readonly float explosionForce = 1000f;
		private readonly float explosionRadius = 5f;

		private readonly float maxDamage = 100f;
		private readonly float maxLifeTime = 2f;

		private float time = 0;

		private void Start() => Destroy(gameObject, maxLifeTime);

		private void Update() => time += Time.deltaTime;

		// Find all the tanks in an area around the shell and damage them.
		private void OnTriggerEnter(Collider other)
		{
			Collider[] colliders = Physics.OverlapSphere(
				transform.position,
				explosionRadius,
				tankMask);

			for (int i = 0; i < colliders.Length; ++i)
			{
				Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

				if (targetRigidbody)
				{
					targetRigidbody.isKinematic = false;

					targetRigidbody.AddExplosionForce(
						explosionForce,
						transform.position,
						explosionRadius);

					TankHealth targetHealth =
						targetRigidbody.GetComponent<TankHealth>();

					if (targetHealth)
					{
						float damage = CalculateDamage(targetRigidbody.position);
						targetHealth.TakeDamage(damage);
					}
				}
			}

			explosionParticles.transform.parent = null;
			explosionParticles.Play();
			explosionAudio.Play();

			Destroy(
				explosionParticles.gameObject,
				explosionParticles.main.duration);

			Destroy(gameObject);
		}

		// Calculate the amount of damage a target should take based on it's position.
		private float CalculateDamage(Vector3 targetPosition)
		{
			Vector3 explosionToTarget = targetPosition - transform.position;
			float explosionDistance = explosionToTarget.magnitude;
			float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;
			float damage = relativeDistance * maxDamage;

			damage = Mathf.Max(0f, damage);

			return damage;
		}
	}
}