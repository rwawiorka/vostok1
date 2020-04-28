 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Den.Tools
{
	
	public class GeometryTest : MonoBehaviour
	{
		public enum TestCase { lineFormula, pointLineHeight, closestPointBetweenLines, distanceToLine }
		public TestCase testCase;

		public Vector3 line1start = new Vector3(-10,0,0);
		public Vector3 line1end = new Vector3(10,0,0);

		public Vector3 line2start = new Vector3(-10,0,5);
		public Vector3 line2end = new Vector3(10,0,5);

		public Vector3 point = new Vector3(0,10,0);
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(GeometryTest))]
	public class GeometryTestEditor : Editor
	{
		public void OnSceneGUI ()
		{
			GeometryTest test = (GeometryTest)target;

			//drawing point
			Handles.color = Color.white;
			test.point = Handles.PositionHandle(test.point, Quaternion.identity);
			Handles.DotHandleCap(0, test.point, Quaternion.identity, HandleUtility.GetHandleSize(test.point)/15f, EventType.Repaint);

			//line 1
			Handles.color = Color.white;
			test.line1start = Handles.PositionHandle(test.line1start, Quaternion.identity);
			test.line1end = Handles.PositionHandle(test.line1end, Quaternion.identity);
			Handles.DrawLine(test.line1start, test.line1end);

			//line 2
			Handles.color = new Color(0.6f, 0.6f, 0.6f, 1);
			test.line2start = Handles.PositionHandle(test.line2start, Quaternion.identity);
			test.line2end = Handles.PositionHandle(test.line2end, Quaternion.identity);
			Handles.DrawLine(test.line2start, test.line2end);

			if (test.testCase == GeometryTest.TestCase.lineFormula)
			{
				Vector3 p = test.line1start;
				Vector3 d = (test.line1end - test.line1start).normalized;

				for (int x=-100; x<100; x+=1)
				{
					float y = (x/d.x - p.x/d.x + p.y/d.y) * d.y;
					float z = (x/d.x - p.x/d.x + p.z/d.z) * d.z;

					Vector3 pos = new Vector3(x,y,z);
					Handles.DotHandleCap(0, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos)/15f, EventType.Repaint);
				}
			}

			if (test.testCase == GeometryTest.TestCase.pointLineHeight)
			{
				Vector3 pos = Geometry.ClosestPointOnLine2(test.line1start, test.line1end, test.point);
				
				Handles.color = Color.green;
				Handles.DotHandleCap(0, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos)/15f, EventType.Repaint);
			}

			if (test.testCase == GeometryTest.TestCase.closestPointBetweenLines)
			{
				Vector3 pos = Geometry.ClosestPointBetweenLines(test.line1start, test.line1end, test.line2start, test.line2end);

				Handles.color = Color.green;
				Handles.DotHandleCap(0, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos)/15f, EventType.Repaint);
			}

			if (test.testCase == GeometryTest.TestCase.distanceToLine)
			{
				float dist = Geometry.DistanceToLine(test.line1start, test.line1end, test.point);

				Handles.color = Color.green; 
				Handles.SphereHandleCap(0, test.point, Quaternion.identity, dist*2, EventType.Repaint);  //HandleUtility.GetHandleSize( new Vector3(dist, dist, dist) ), EventType.Repaint);
			}

			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) SceneView.RepaintAll();
		}

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector();

		}
	}
	#endif
}
