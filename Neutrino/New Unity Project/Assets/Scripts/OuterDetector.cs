using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
//Liz Izatt
//Ayana Burkins
public class OuterDetector : MonoBehaviour {
	public Camera cam1;
	//Camera cam2;
	public ReadData rd;
	public Vector3 dir;
	public Vector3 v;
	public GameObject intersect;
	public bool first;
	public GameObject cone;
	static Material lineMaterial; 
	public Vector3[] vertices;
	//public Vector2[] newUV;
	public int[] lines;
	public int particleNum;

	// Use this for initialization
	void Start () {
		particleNum = 0;
		intersect = GameObject.Find ("Intersect");
		//go = GameObject.Find ("GameObject");
		//cam1 = GameObject.Find ("CameraStereo0.Left").GetComponent<Camera>();
		//cam2 = GameObject.Find ("CameraStereo0.Right").GetComponent<Camera>();

		/*
		Mesh mesh = new Mesh();
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		//mesh.uv = newUV;
		int k = 0;
		//calculate vertices
		for (int i = 0; i < 361; i++){
			float cx = 17 * Mathf.Cos(i*Mathf.Deg2Rad);
			float cy = 17 * Mathf.Sin(i*Mathf.Deg2Rad);
			Vector2 v1 = new Vector2(cx,cy);
			
			cx = 17 * Mathf.Cos(i*Mathf.Deg2Rad+Mathf.Deg2Rad);
			cy = 17 * Mathf.Sin(i*Mathf.Deg2Rad+Mathf.Deg2Rad);
			Vector2 v2 = new Vector2(cx,cy);
			for(int j = 0; j < 41; j+=5){
				vertices[k] = new Vector3(v1.x,v1.y,j);
				vertices[k+1] = new Vector3(v2.x,v2.y,j);
				k = k + 2;
			}
		}
		//set vertices
		mesh.vertices = vertices;
		//calculate lines
		for (int i = 0; i < vertices.Length-1; i+=2) {
			lines[i] = i;
			lines[i+1] = i+1;
		}
		//set lines
		mesh.SetIndices (lines, MeshTopology.Lines, 0);
		mesh.RecalculateNormals();
*/
	}


	
	// Update is called once per frame
	void Update () {
		//for debugging purposes. Draw a ray in the direction of a cone and plot the intersection with cylinder



	}

	Vector3 crossProduct(Vector3 a, Vector3 b){
		return new Vector3 ( (a[1]*b[2]-a[2]*b[1]) , ( a[2]*b[0]-a[0]*b[2]),  (a[0]*b[1]-a[1]*b[0]));
	}

	Vector3 intersectCylinder(Vector3 rayOrigin, Vector3 ray, float radius, float height){  //assumes cylinder aligned along z-axis and centered on 0,0,0, returns a point of intersection
		/*
	RAYCASTING ALGORITHM
	(vx+dx*t)^2 + (vy+dy*t)^2 = r^2
	vx^2 + 2*vx*dx*t + dx^2*t^2 + vy^2 + 2*vy*dy*t + dy^2*t^2 = r^2
	*****	t^2 * (dx^2 + dy^2) + t * (2*vx*dx+2*vy*dy) + (vx^2 + vy^2 - r^2) = 0 ****
	solve for t
	first positive t will be the intersection we want (since we're starting inside the cylinder in all cases)
	plug in t to (vx+dx*t) for x coord, (vy+dx*t) for y coord, t*dz will give delta z to generate z coord
	if z > h or z < 0, then we hit a disk, backtrack
	solve (vz + dz*t) = h or (vz+dz*t) = 0
	thus t = ([h/0]-vz)/dz
	plug in t for x and y coords
	fin
	*/
		
		float a = Mathf.Pow(ray[0],2)+Mathf.Pow(ray[1],2);
		float b = 2*rayOrigin[0]*ray[0] + 2*rayOrigin[1]*ray[1];
		float c = Mathf.Pow(rayOrigin[0],2) + Mathf.Pow(rayOrigin[1],2) - Mathf.Pow(radius,2);
		float radical = Mathf.Pow(b,2) - 4 * a * c;
		Vector3 point = new Vector3 (0, 0, 0);
		if(radical < 0){
			return point;  //we should never get here, this is the case where there is no intersection
		}
		float t = (-b + Mathf.Sqrt(radical))/(2*a);  //quadratic formula

		if(t <= 0){								// we should get out a positive and negative t for each case since we're doing an intersection from inside the cylinder									//
			t = (-b + Mathf.Sqrt(radical))/(2*a);    //  so we can assume it's the "+" version of quadratic formula
		}
	
		//here we should have a value of t
		//so extrapolate that and find our point on hte cylinder
		point = t*ray;  //this is relative to the vertexPosition
		
		//rayOrigin[2] = rayOrigin[2]-height/2;  //here, vertex position is stored ranging from 0 - 40 (eg, bottom of the cylinder is at 0), whereas the stored geometry at this point puts the bottom at -20 and top at 20.  Changing it before would mess up calculations, so change it here (and only temporarily)
		Vector3 finalPoint = point + rayOrigin;  //this is the actual point of contact with the cylinder
		/*
		if(finalPoint[2] > height/2){ //if we hit the top
			t = ((height/2) - rayOrigin[2]) / Mathf.Abs(ray[2]);	
			point = t * ray;
		}
		if(finalPoint[2] < (-height/2)){ //we hit the bottom
			t = (rayOrigin[2]+(height/2)) / Mathf.Abs(ray[2]);
			point = t * ray;
		}
		*/
		return finalPoint;
	}
	static void CreateLineMaterial ()
	{
		if (!lineMaterial)
		{

			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			var shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn on depth writes
			lineMaterial.SetInt ("_ZWrite", 1);
			lineMaterial.SetInt ("_ZTest",(int)UnityEngine.Rendering.CompareFunction.NotEqual);



		}
	}

	

	void OnRenderObject(){
		float PI = Mathf.PI;
		CreateLineMaterial ();
		
		//draw the cylindrical detector
		lineMaterial.SetPass (0);
		GL.Begin(GL.LINES);
		GL.Color (Color.white);

		//draws circles every 5 units

		int k = 0;
		for (int i = 0; i < 361; i++){
			
			
			float cx = 17 * Mathf.Cos(i*Mathf.Deg2Rad);
			float cy = 17 * Mathf.Sin(i*Mathf.Deg2Rad);
			Vector2 v1 = new Vector2(cx,cy);

			cx = 17 * Mathf.Cos(i*Mathf.Deg2Rad+Mathf.Deg2Rad);
			cy = 17 * Mathf.Sin(i*Mathf.Deg2Rad+Mathf.Deg2Rad);
			Vector2 v2 = new Vector2(cx,cy);

			for(int j = 0; j < 41; j+=5){
				GL.Vertex3(v1.x,v1.y,j);
				GL.Vertex3(v2.x,v2.y,j);


			}
	

		}

		//draws the "sides" of the tube, one line every 30 degrees
		for (int i = 0; i<361; i+=30) {
			float cx = 17 * Mathf.Cos(i*Mathf.Deg2Rad);
			float cy = 17 * Mathf.Sin(i*Mathf.Deg2Rad);
			GL.Vertex3(cx,cy,0);

			cx = 17 * Mathf.Cos(i*Mathf.Deg2Rad+Mathf.Deg2Rad);
			cy = 17 * Mathf.Sin(i*Mathf.Deg2Rad+Mathf.Deg2Rad);
			GL.Vertex3(cx,cy,40);
		}

		
		GL.End();






		//draw the boxes around outer detectors
		GameObject[] dots = GameObject.FindGameObjectsWithTag ("od");
		foreach (GameObject dot in dots){
			Vector3 center = dot.transform.position;
			Vector3 up = center + dot.transform.forward / 4;
			Vector3 down = center - dot.transform.forward / 4;
			Vector3 upLeft = up - dot.transform.right / 4;
			Vector3 upRight = up + dot.transform.right / 4;
			Vector3 lowRight = down + dot.transform.right / 4;
			Vector3 lowLeft = down - dot.transform.right / 4;
		
			GL.PushMatrix ();
			GL.LoadProjectionMatrix (cam1.projectionMatrix);
			//GL.LoadProjectionMatrix (cam2.projectionMatrix);
			GL.MultMatrix (dot.transform.localToWorldMatrix);
		
			GL.Begin (GL.LINES);
			GL.Color (dot.GetComponent<Renderer>().material.color);
		
			GL.Vertex (upLeft);
			GL.Vertex (upRight);
		
			GL.Vertex (lowRight);
			GL.Vertex (lowLeft);
		
			GL.Vertex (lowLeft);
			GL.Vertex (upLeft);
		
			GL.Vertex (lowRight);
			GL.Vertex (upRight);
			GL.PopMatrix ();
			GL.End ();
		}

		//cherenkov cone testing
		//bool doTimeCompressed = false;
		ReadData.dotVector cD = GameObject.Find ("GameObject").GetComponent<ReadData> ().currentDots;
		ReadData.particle p = (ReadData.particle) cD.particles [particleNum];
		Vector3 coneDirection = Vector3.Normalize(p.coneDirection);
		Vector3 arbitraryDirection = new Vector3(0,0,1); 
		Vector3 perpCompX = Vector3.Normalize(crossProduct(coneDirection, arbitraryDirection));
		Vector3 perpCompY = Vector3.Normalize(crossProduct(coneDirection, perpCompX));
		Vector3 perpendicularComponent = perpCompX;
		Vector3 rayDirection = coneDirection + Mathf.Tan(42*Mathf.PI/180)*perpendicularComponent;  //coneDirection should be normalized too
		v = cD.vertex;
		Vector3 lastPoint = intersectCylinder (v, Vector3.Normalize (rayDirection), 17, 40);
		Vector3 newPoint = new Vector3 (0, 0, 0);
		//GameObject clone = Instantiate (intersect, v, Quaternion.identity) as GameObject;
		//dir = Vector3.Normalize (rayDirection);
		//cone = GameObject.Find ("Cone");
	
		//GameObject c = Instantiate (cone, v, Quaternion.LookRotation (dir)) as GameObject;
		//Debug.DrawRay (v, dir,Color.red,300.0f,false);
		//Vector3 intersection = intersectCylinder (v, dir, 17, 40);
		//intersect.transform.position = intersection;

		//dir[1] += 1;
		//GameObject clone = Instantiate (intersect, v, Quaternion.identity) as GameObject;
		//Debug.DrawRay (v, dir,Color.red,300.0f,false);
		for(int i = 0; i < 361; i+=5){
			perpendicularComponent = Vector3.Normalize (Mathf.Cos (i*PI/180)*perpCompX + Mathf.Sin(i*PI/180)*perpCompY);
			rayDirection = coneDirection + Mathf.Tan (42*PI/180)*perpendicularComponent;
			newPoint = intersectCylinder(v,Vector3.Normalize(rayDirection),17,40);
			GL.Begin(GL.LINES);
			GL.Color (Color.yellow);
			GL.Vertex(v);
			GL.Vertex (newPoint);
			GL.End ();
			//Debug.DrawRay (new Vector3(0,0,0),newPoint,Color.red, 300.0f, false);
			//Debug.DrawRay(newPoint,new Vector3(0,0,0));
		}
		lastPoint = newPoint;




	}

	public void changeParticle(int num){
		ReadData.dotVector cD = GameObject.Find ("GameObject").GetComponent<ReadData> ().currentDots;
		if((!(particleNum >= cD.particles.Count))){
			if(num == 1)particleNum +=1;
			if(num == -1){
				if(particleNum != 0) particleNum -=1;
			}
		}
	}


}
