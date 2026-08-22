using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class BezierUIConnection : MaskableGraphic
{
	[Header("Connection")]
	[SerializeField] private RectTransform m_From;
	[SerializeField] private RectTransform m_To;

	[SerializeField] private float m_Thickness = 4f;

	[Header("Corners")]
	[SerializeField] private float m_CornerRadius = 20f;
	[SerializeField] private int m_CornerSegments = 5;

	[Header("Arrow")]
	[SerializeField] private bool m_ShowArrow = true;
	[SerializeField] private float m_ArrowLength = 14f;
	[SerializeField] private float m_ArrowWidth = 10f;

	private readonly List<Vector2> m_Path = new();
	private readonly List<Vector2> m_RoundedPath = new();

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();

		if (m_From == null || m_To == null)
			return;

		GenerateConnection(vh);
	}

	public void SetEndpoints(RectTransform from, RectTransform to)
	{
		m_From = from;
		m_To = to;

		SetVerticesDirty();
	}

	public void Rebuild()
	{
		SetVerticesDirty();
	}

	private void GenerateConnection(VertexHelper vh)
	{
		m_Path.Clear();
		m_RoundedPath.Clear();

		// ALWAYS bottom of source -> top of destination
		Vector2 start = GetBottomCenter(m_From);
		Vector2 end = GetTopCenter(m_To);

		BuildVerticalResearchPath(start, end);

		RoundCorners();

		DrawPath(vh);

		if (m_ShowArrow)
			DrawArrow(vh);
	}

	// ---------------------------------------------------------
	// PATH CREATION
	// ---------------------------------------------------------
	private void BuildVerticalResearchPath(Vector2 start, Vector2 end)
	{
		m_Path.Clear();

		m_Path.Add(start);

		// Horizontal section exactly halfway
		float middleY = (start.y + end.y) * 0.5f;

		// First vertical section
		m_Path.Add(new Vector2(
			start.x,
			middleY
		));

		// Horizontal section
		m_Path.Add(new Vector2(
			end.x,
			middleY
		));

		// Final vertical section
		m_Path.Add(end);
	}

	// ---------------------------------------------------------
	// ROUNDED CORNERS
	// ---------------------------------------------------------

	private void RoundCorners()
	{
		if (m_Path.Count < 2)
			return;

		m_RoundedPath.Add(m_Path[0]);

		for (int i = 1; i < m_Path.Count - 1; i++)
		{
			Vector2 previous = m_Path[i - 1];
			Vector2 current = m_Path[i];
			Vector2 next = m_Path[i + 1];

			Vector2 incoming = (current - previous).normalized;

			Vector2 outgoing = (next - current).normalized;

			float incomingLength = Vector2.Distance(previous, current);

			float outgoingLength = Vector2.Distance(current, next);

			float radius = Mathf.Min(
				m_CornerRadius,
				incomingLength * 0.5f,
				outgoingLength * 0.5f
			);

			Vector2 cornerStart = current - incoming * radius;

			Vector2 cornerEnd = current + outgoing * radius;

			m_RoundedPath.Add(cornerStart);

			AddQuadraticCorner(
				cornerStart,
				current,
				cornerEnd
			);
		}

		m_RoundedPath.Add(m_Path[^1]);
	}

	private void AddQuadraticCorner(Vector2 start, Vector2 control, Vector2 end)
	{
		for (int i = 1; i <= m_CornerSegments; i++)
		{
			float t = i / (float)m_CornerSegments;

			float oneMinusT = 1f - t;

			Vector2 point =
				oneMinusT * oneMinusT * start +
				2f * oneMinusT * t * control +
				t * t * end;

			m_RoundedPath.Add(point);
		}
	}

	// ---------------------------------------------------------
	// DRAW LINE
	// ---------------------------------------------------------

	private void DrawPath(VertexHelper vh)
	{
		int lastSegment = m_RoundedPath.Count - 1;

		// Reserve space for the arrowhead.
		float arrowOffset = m_ShowArrow ? m_ArrowLength * 0.65f : 0f;

		for (int i = 0; i < lastSegment; i++)
		{
			Vector2 start = m_RoundedPath[i];
			Vector2 end = m_RoundedPath[i + 1];

			// Only shorten the final segment.
			if (i == lastSegment - 1 && m_ShowArrow)
			{
				Vector2 direction = (end - start).normalized;

				end -= direction * arrowOffset;
			}

			AddLineSegment(vh, start, end);
		}
	}

	private void AddLineSegment(VertexHelper vh, Vector2 start, Vector2 end)
	{
		Vector2 direction = (end - start).normalized;

		if (direction.sqrMagnitude < 0.0001f) return;

		Vector2 perpendicular =
			new(
				-direction.y,
				direction.x
			);

		float halfThickness = m_Thickness * 0.5f;

		Vector2 v0 = start + perpendicular * halfThickness;

		Vector2 v1 = start - perpendicular * halfThickness;

		Vector2 v2 = end - perpendicular * halfThickness;

		Vector2 v3 = end + perpendicular * halfThickness;

		int index = vh.currentVertCount;

		vh.AddVert(v0, color, Vector2.zero);
		vh.AddVert(v1, color, Vector2.zero);
		vh.AddVert(v2, color, Vector2.zero);
		vh.AddVert(v3, color, Vector2.zero);

		vh.AddTriangle(
			index,
			index + 1,
			index + 2
		);

		vh.AddTriangle(
			index,
			index + 2,
			index + 3
		);
	}

	// ---------------------------------------------------------
	// ARROW
	// ---------------------------------------------------------

	private void DrawArrow(VertexHelper vh)
	{
		if (m_RoundedPath.Count < 2) return;

		Vector2 tip = m_RoundedPath[^1];

		Vector2 previous = m_RoundedPath[^2];

		Vector2 direction = (tip - previous).normalized;

		if (direction.sqrMagnitude < 0.0001f) return;

		Vector2 perpendicular =
			new Vector2(
				-direction.y,
				direction.x
			);

		Vector2 basePoint = tip - direction * m_ArrowLength;

		Vector2 left = basePoint + perpendicular * (m_ArrowWidth * 0.5f);

		Vector2 right = basePoint - perpendicular * (m_ArrowWidth * 0.5f);

		int index = vh.currentVertCount;

		vh.AddVert(
			tip,
			color,
			Vector2.zero
		);

		vh.AddVert(
			left,
			color,
			Vector2.zero
		);

		vh.AddVert(
			right,
			color,
			Vector2.zero
		);

		vh.AddTriangle(
			index,
			index + 1,
			index + 2
		);
	}

	// ---------------------------------------------------------
	// NODE EDGE CALCULATION
	// ---------------------------------------------------------
	private Vector2 GetBottomCenter(RectTransform rect)
	{
		Vector3[] corners = new Vector3[4];
		rect.GetWorldCorners(corners);

		Vector3 bottomCenter = (corners[0] + corners[3]) * 0.5f;

		return transform.InverseTransformPoint(bottomCenter);
	}

	private Vector2 GetTopCenter(RectTransform rect)
	{
		Vector3[] corners = new Vector3[4];
		rect.GetWorldCorners(corners);

		Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

		return transform.InverseTransformPoint(topCenter);
	}
}