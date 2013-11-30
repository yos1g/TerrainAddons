using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TerrainAssets))]
[HideInInspector]
public class TerrainAssetsEditor : Editor {

	public Vector3 origPosition = Vector3.zero;
	
	public void OnSceneGUI() {
		TerrainAssets ta = (TerrainAssets)target;
		if(!ta.Asset) {
			return;
		}
		
		Terrain terrain = Terrain.activeTerrain;
		if (terrain == null) {
			return;
		}
		

		
		// Assign to terrain
		Vector3 height = ta.transform.position;
		height.y = terrain.SampleHeight(height);
		ta.transform.position = height;

		if (!Event.current.shift) {
			SplineHandlers(ta);
			return; // cancel draws
		} else {
			Reset(ta);
		}

		if (Event.current.type != EventType.mouseDrag)
			return;

		if (ta.points.Count == 0) {
			ta.points.Add(ta.transform.position);
		}
		
		if (Vector3.Distance(ta.points[ta.points.Count - 1], ta.transform.position) > ta.pointDistance) {
			ta.points.Add(ta.transform.position);
		}
	}
	
	
	public void SplineHandlers(TerrainAssets ta)
	{
		if(ta.points.Count < 4)
			return;
		
		for (int i = 1; i < ta.points.Count - 1; i++) {
			Vector3 v3 = Handles.PositionHandle(ta.points [i], ta.transform.rotation);
			if (v3 != ta.points [i]) {
					Undo.RegisterUndo(ta, "Spline Point Modified");
					Terrain terrain = Terrain.activeTerrain;
					v3.y = terrain.SampleHeight(v3);
					ta.points [i] = v3;
					ta.GenerateAssets(true);
					EditorUtility.SetDirty(target);
			}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		TerrainAssets ta = (TerrainAssets)target;
		
		EditorGUIUtility.LookLikeInspector ();
		DrawDefaultInspector ();
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Reset")) {
			ta.points.Clear();
			Reset(ta);
		}
		
		if(GUILayout.Button("Generate")) {
			ta.GenerateAssets(true);
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	void Reset(TerrainAssets ta) {
		if (Event.current.type == EventType.mouseDrag) {
			origPosition = Vector3.zero;
		} else {
			if (!origPosition.Equals(Vector3.zero)) 
				ta.transform.position = origPosition;
			origPosition = Vector3.zero;
		}
		ta.GenerateAssets(false);
	}
}
