using UnityEngine;

public class StormParticleFollower : MonoBehaviour
{
	[SerializeField] private Transform m_Target;

	private void LateUpdate()
	{
		Vector3 position = m_Target.position;

		position.z = transform.position.z;

		transform.position = position;
	}
}