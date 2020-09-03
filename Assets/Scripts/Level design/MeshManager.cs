using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    public float TunnelRadius;
	public int LanesCount;

	public float RingDistance;
	
	public float SegmentRadius;
	public int SegmentRingsCount;

	private Mesh _mesh;
	private Vector3[] _vertices;
	private Vector2[] _uv;
	private int[] _triangles;

	private float _segmentAngle;
	private float _relativeRotation;

	private void Awake () {
		GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
		_mesh.name = "Pipe";
	}

	public void Generate () {
		GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
		_mesh.name = "Pipe";
		_mesh.Clear();
		SetVertices();
		SetUV();
		SetTriangles();
		_mesh.RecalculateNormals();

		for (int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	private void SetVertices () {
		_vertices = new Vector3[LanesCount * SegmentRingsCount * 4];

		float uStep = RingDistance / SegmentRadius;
		_segmentAngle = uStep * SegmentRingsCount * (360f / (2f * Mathf.PI));
		CreateFirstQuadRing(uStep);
		int iDelta = LanesCount * 4;
		for (int u = 2, i = iDelta; u <= SegmentRingsCount; u++, i += iDelta) {
			CreateQuadRing(u * uStep, i);
		}
		_mesh.vertices = _vertices;
	}

	private void CreateFirstQuadRing (float u) {
		float vStep = (2f * Mathf.PI) / LanesCount;

		Vector3 vertexA = GetPointOnTorus(0f, 0f);
		Vector3 vertexB = GetPointOnTorus(u, 0f);
		for (int v = 1, i = 0; v <= LanesCount; v++, i += 4) {
			_vertices[i] = vertexA;
			_vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
			_vertices[i + 2] = vertexB;
			_vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
		}
	}

	private void CreateQuadRing (float u, int i) {
		float vStep = (2f * Mathf.PI) / LanesCount;
		int ringOffset = LanesCount * 4;

		Vector3 vertex = GetPointOnTorus(u, 0f);
		for (int v = 1; v <= LanesCount; v++, i += 4) {
			_vertices[i] = _vertices[i - ringOffset + 2];
			_vertices[i + 1] = _vertices[i - ringOffset + 3];
			_vertices[i + 2] = vertex;
			_vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
		}
	}

	private void SetUV () {
		_uv = new Vector2[_vertices.Length];
		for (int i = 0; i < _vertices.Length; i+= 4) {
			_uv[i] = Vector2.zero;
			_uv[i + 1] = Vector2.right;
			_uv[i + 2] = Vector2.up;
			_uv[i + 3] = Vector2.one;
		}
		_mesh.uv = _uv;
	}

	private void SetTriangles () {
		_triangles = new int[LanesCount * SegmentRingsCount * 6];
		for (int t = 0, i = 0; t < _triangles.Length; t += 6, i += 4) {
			_triangles[t] = i;
			_triangles[t + 1] = _triangles[t + 4] = i + 2;
			_triangles[t + 2] = _triangles[t + 3] = i + 1;
			_triangles[t + 5] = i + 3;
		}
		_mesh.triangles = _triangles;
	}

	private Vector3 GetPointOnTorus (float u, float v) {
		Vector3 p;
		float r = (SegmentRadius + TunnelRadius * Mathf.Cos(v));
		p.x = r * Mathf.Sin(u);
		p.y = r * Mathf.Cos(u);
		p.z = TunnelRadius * Mathf.Sin(v);
		return p;
	}

	public void AlignWith (Pipe pipe) {
		_relativeRotation =
			Random.Range(0, SegmentRingsCount) * 360f / LanesCount;

		transform.SetParent(pipe.transform, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.CurveAngle);
		transform.Translate(0f, pipe.CurveRadius, 0f);
		transform.Rotate(_relativeRotation, 0f, 0f);
		transform.Translate(0f, -SegmentRadius, 0f);
		transform.SetParent(pipe.transform.parent);
		transform.localScale = Vector3.one;
	}
}
