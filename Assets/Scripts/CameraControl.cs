#region

using UnityEngine;

#endregion

public class CameraControl : MonoBehaviour
{
	public float dampTime = 0.2f;
	public float screenEdgeBuffer = 4f;
	public float minSize = 6.5f;
	[HideInInspector]
	public Transform[] targets;

	private Camera camera;
	private Vector3 m_DesiredPosition;
	private Vector3 moveVelocity;
	private float zoomSpeed;

	private void Awake() => camera = GetComponentInChildren<Camera>();

	private void FixedUpdate()
	{
		Move();
		Zoom();
	}

	private void Move()
	{
		FindAveragePosition();

		transform.position = Vector3.SmoothDamp(
			transform.position,
			m_DesiredPosition,
			ref moveVelocity,
			dampTime);
	}

	private void FindAveragePosition()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;

		for (int i = 0; i < targets.Length; i++)
		{
			if (!targets[i].gameObject.activeSelf)
			{
				continue;
			}

			averagePos += targets[i].position;
			numTargets++;
		}

		if (numTargets > 0)
		{
			averagePos /= numTargets;
		}

		averagePos.y = transform.position.y;

		m_DesiredPosition = averagePos;
	}

	private void Zoom()
	{
		float requiredSize = FindRequiredSize();

		camera.orthographicSize = Mathf.SmoothDamp(
			camera.orthographicSize,
			requiredSize,
			ref zoomSpeed,
			dampTime);
	}

	private float FindRequiredSize()
	{
		Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

		float size = 0f;

		for (int i = 0; i < targets.Length; i++)
		{
			if (!targets[i].gameObject.activeSelf)
			{
				continue;
			}

			Vector3 targetLocalPos = transform.InverseTransformPoint(targets[i].position);

			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			size = Mathf.Max(
				size,
				Mathf.Abs(desiredPosToTarget.y));

			size = Mathf.Max(
				size,
				Mathf.Abs(desiredPosToTarget.x) / camera.aspect);
		}

		size += screenEdgeBuffer;

		size = Mathf.Max(size, minSize);

		return size;
	}

	public void SetStartPositionAndSize()
	{
		FindAveragePosition();

		transform.position = m_DesiredPosition;

		camera.orthographicSize = FindRequiredSize();
	}
}