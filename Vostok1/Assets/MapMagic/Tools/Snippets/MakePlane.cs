using UnityEngine;
using System.Collections;
using Den.Tools;

public class MakePlane 
{

	void MakePlaneFn (Rect rect, int resolution) 
	{
		float step = 1f * rect.size.x / resolution;
		
		Vector3[] verts = new Vector3[(resolution+1)*(resolution+1)];
		int[] tris = new int[resolution*resolution*2*3];

		int vertCounter = 0;
		int triCounter = 0;
		for (float x=0; x<rect.size.x+0.001f; x+=step) //including max
			for (float z=0; z<rect.size.y+0.001f; z+=step)
		{
			verts[vertCounter] = new Vector3(x, 0, z);
			
			if (x>0.001f && z>0.001f)
			{
				tris[triCounter] = vertCounter-(resolution+1);		tris[triCounter+1] = vertCounter-1;					tris[triCounter+2] = vertCounter-resolution-2;
				tris[triCounter+3] = vertCounter-1;					tris[triCounter+4] = vertCounter-(resolution+1);	tris[triCounter+5] = vertCounter;
				triCounter += 6;
			}

			vertCounter++;
		}
	}
}
