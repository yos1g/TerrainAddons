using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Terrain/Terrain Assets")]
[ExecuteInEditMode]
public class TerrainAssets : MonoBehaviour {
	public GameObject Asset;
	
	public float pointDistance = 5.0f;
	public float offsetRotation = 90.0f;
	public Vector3 groundOffset = Vector3.zero;
	public float maxScaleFactor = 0f;

	[HideInInspector]
	public List<Vector3> points;
	
	void OnDrawGizmos() {
		Gizmos.DrawRay(transform.position, transform.up);
		Gizmos.DrawWireCube(transform.position, transform.up);
	}
	
	void OnDrawGizmosSelected(){
		if (points.Count < 2)
			return;

		Spline.DrawPathHelper(points.ToArray(),Color.red);
	}
	
	public void GenerateAssets(bool withGameObject) {
		
		for (var i = 0; i < transform.childCount; ++i) {
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
		
		if (transform.childCount !=0) {
			GenerateAssets(withGameObject);
			return;
		}
		
		if (points.Count == 0)
			return;

		if (!withGameObject)
			return;


		for(int i = 1; i < points.Count - 1; i++) {
			GameObject clone = Instantiate(Asset, points[i] + groundOffset, Asset.transform.rotation) as GameObject;
			clone.transform.parent = transform;
			if (maxScaleFactor != 0) {
				float rndScale = UnityEngine.Random.Range(0, maxScaleFactor);
				clone.transform.localScale += new Vector3(rndScale, rndScale, rndScale);
			}

			if (i > 0 && i < points.Count - 1) {
				Vector3 Direction = (points[i-1] - points[i+1]).normalized;
				float angle = Vector3.Angle(clone.transform.forward, Direction);
				Quaternion rot = Quaternion.LookRotation(points[i-1] - points[i+1]);
				clone.transform.rotation = rot * Quaternion.Euler(0, offsetRotation, 0);
			}
		}
	}
}
