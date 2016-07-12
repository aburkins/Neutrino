using UnityEngine;
using System.Collections;
//using MiddleVR_Unity3D;
using UnityEngine.UI;
//#include <math.h>;
//Liz Izatt
//Ayana Burkins

public class ReadData : MonoBehaviour {
	string line;
	ArrayList data = new ArrayList(); //will hold the dot structs
	ArrayList allEvents = new ArrayList(); //will hold dotVector structs
	public dotVector currentDots;
	int eventNumber;
	public GameObject tube;
	public bool colorByCharge;
	public GameObject cylinder;
	public bool odReady;
	public Camera cam1;
	public Camera cam2;
	public bool doVideo;
	public Button button;
	public GameObject menu; 
	private bool isShowing;
	OuterDetector od;

	//structure for holding detector data
	public struct dot {
		public string ID,number;
		public float x,y,z,time,charge;


		public dot(string s1, string s2, string s3, string s4, string s5, string s6, string s7){
			ID = s1;
			number = s2;
			x = float.Parse (s3) / 100.0f;
			y = float.Parse (s4) / 100.0f;
			z = float.Parse (s5) *  20.0f / 1810.0f + 20.0f;
			charge = float.Parse (s6);
			time = float.Parse (s7);

		}
	}
	//each dotVector holds an array of dots for a single event + particle info
	public struct dotVector{
		public int eventNum;
		public ArrayList dots;
		public ArrayList particles;
		public Vector3 vertex;

		public dotVector(int num, ArrayList vector, ArrayList p, Vector3 v){
			eventNum = num;
			dots = vector;
			particles = p;
			float v1,v2,v3;
			v1 = v.x / 100.0f;
			v2 = v.y/ 100.0f;
			v3 = v.z *  20.0f / 1810.0f + 20.0f;
			vertex = new Vector3(v1,v2,v3);
		}
	}

	public struct particle{
		public string type;
		public Vector3 coneDirection;
		public float momentum;

		public particle(string t, string x, string y, string z, string m){
			type = t;
			coneDirection = new Vector3(float.Parse (x),float.Parse (y),float.Parse (z));
			momentum = float.Parse (m);
		}
	}
	public void changeColor(){

		GameObject[] ids = GameObject.FindGameObjectsWithTag ("id");
		GameObject[] ods = GameObject.FindGameObjectsWithTag ("od");
		foreach(GameObject o in ids){
			Destroy (o);
		}
		foreach(GameObject o in ods){
			Destroy (o);
		}
		if (colorByCharge) {
			colorByCharge = false;

			plotDots(currentDots);
			button.GetComponentInChildren<Text>().text = "Color by Charge";
			} else {
					colorByCharge = true;
					plotDots (currentDots);
					button.GetComponentInChildren<Text>().text = "Color by Time";
			}

	}
	//reads the data file, separates data into events, and populates an array of all eventss
	public void readEvents(){
		int num = 1; //event number
		ArrayList cones = new ArrayList ();
		Vector3 vertex = new Vector3 (0, 0, 0);
		System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Ayana Burkins\Documents\Neutrino\tour.txt");
		while ((line = file.ReadLine()) != null) {
			string[] split = line.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
			if(split[1] == "NEXTEVENT"){
				dotVector dv = new dotVector(num, data,cones,vertex);
				allEvents.Add (dv);
				data = new ArrayList();
				cones = new ArrayList();
				num+=1;
			}
			else{
				if(split[1] == "ID" || split[1] == "OD"){
					dot d = new dot(split[1],split[2],split[3],split[4],split[5],split[6],split[7]);
					data.Add (d);
				}
			}

			if(split[1] == "VERTEX"){
				vertex = new Vector3(float.Parse(split[2]),float.Parse(split[3]),float.Parse(split[4]));
			}
		
			if(split[1] == "PARTICLE"){
				particle p = new particle(split[2],split[3],split[4],split[5],split[6]);
				cones.Add(p);
			}


		}
	}
	
	// Use this for initialization
	void Start () {
		odReady = false;
		doVideo = false;
		//attach a script to the camera for rendering lines
		//GameObject camera1 = GameObject.Find ("CameraStereo0.Left");
		//camera1.AddComponent<OuterDetector> ();
		//GameObject camera2 = GameObject.Find ("CameraStereo0.Right");
		//camera2.AddComponent<OuterDetector> ();
		//cam1 = camera1.GetComponent<Camera>();
		//cam2 = camera2.GetComponent<Camera>();
		colorByCharge = true;
		eventNumber = 0;
		//read the data file and populate an array of dot structs
			/*
		System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\amb136\Neutrino\small.txt");
		while ((line = file.ReadLine()) != null) {
			string[] split = line.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
			if(split[1] == "ID" || split[1] == "OD"){
				dot d = new dot(split[1],split[2],split[3],split[4],split[5],split[6],split[7]);
				data.Add (d);
			}
		}
		*/

		readEvents ();
		dotVector dv = (dotVector) allEvents [eventNumber];
		currentDots = dv;
		plotDots(dv);
		//InvokeRepeating("AutoEvent", 1, 1.0f);

	}
	// color the tubes and place them in the cylinder. Reads a single event.
	void plotDots(dotVector dv){
		GameObject clone;
		ArrayList dots = dv.dots;

		foreach(dot d in dots){
            //clone the dummy tube
			clone = Instantiate (tube, new Vector3(d.x,d.y,d.z), Quaternion.identity) as GameObject;
			//clone.tag = "dot";
			//color the dot
			Vector3 color = dotColor (d);
			//wprint (color);
			Color matColor = clone.GetComponent<Renderer>().material.color;
			matColor.r = color.x;
			matColor.g = color.y;
			matColor.b = color.z;
			clone.GetComponent<Renderer>().material.color = matColor;
		
			//calculate rotations
			if (d.z == 40 || d.z == 0) {
				clone.transform.Rotate (Vector3.forward);
				clone.transform.Rotate (Vector3.right, 90);
			} 
			else 	{
				float theta = Mathf.Atan2 (d.y, d.x);
				clone.transform.Rotate (Vector3.up, 90);
				clone.transform.Rotate (Vector3.right, -theta * 180 / Mathf.PI);
				clone.transform.Rotate (Vector3.right, 90);
			}
			//keep track of the outer detectors
			if(d.ID == "OD"){
				clone.tag = "od";
			}
			else{
				clone.tag = "id";
			}

		}
		odReady = true;


	}


	
	void drawBox(GameObject dot){
		//ArrayList ods = go.GetComponent<ReadData> ().ODS ();
		//print (ods);


	}
	//returns a list of outer detectors (for drawing OD boxes)

	

	int[] red_values =		{132	,74,	22,		4,		4,		6,		40,		115,	193,	249,	253,	249,	219,	219,	219,	219};  //r
	int[] green_values =	{4		,12,	4,		124,	175,	184,	183,	114,	193,	249,	213,	191,	143,	129,	102,	33};    //g
	int[] blue_values =	{186	,178,	186,	186,	186,	86,		7,		0,		6,		21,		80,		1,		12,		12,		12,		12};   //b


	public Vector3 dotColor(dot d){ //returns rgb values for tube color based on charge
		float red, green, blue;
		//float numDivs = 16.0f;
		int section = 0;
		if(colorByCharge) {
			float value = d.charge;
			//print (value);
			if(value > 26.7f) section = 15;
			else if (value > 23.3f) section = 14;
			else if (value > 20.2f) section = 13;
			else if (value > 17.3f) section = 12;
			else if (value > 14.7f) section = 11;
			else if (value > 12.2f) section = 10;
			else if (value > 10.0f) section = 9;
			else if (value > 8.0f) section = 8;
			else if (value > 6.2f) section = 7;
			else if (value > 4.7f) section = 6;
			else if (value > 3.3f) section = 5;
			else if (value > 2.2f) section = 4;
			else if (value > 1.3f) section = 3;
			else if (value > 0.7f) section = 2;
			else if (value > 0.2f) section = 1;
			
			red = red_values[section] / 255.0f;
			blue = blue_values[section] / 255.0f;
			green = green_values[section] / 255.0f;
		} else {
			float value = d.time;
			if(value < 893) section = 15;
			else if (value < 909) section = 14;
			else if (value < 925) section = 13;
			else if (value < 941) section = 12;
			else if (value < 957) section = 11;
			else if (value < 973) section = 10;
			else if (value < 989) section = 9;
			else if (value < 1005) section = 8;
			else if (value < 1021) section = 7;
			else if (value < 1037) section = 6;
			else if (value < 1053) section = 5;
			else if (value < 1069) section = 4;
			else if (value < 1085) section = 3;
			else if (value < 1101) section = 2;
			else if (value < 1117) section = 1;
			
			//now we will access an array of pre-picked values corresponding with the superscan mode, based on the SECTION
			red = red_values[section] / 255.0f;
			blue = blue_values[section] / 255.0f;
			green = green_values[section] / 255.0f;


		}
		return new Vector3(red,green,blue);
	}


	public Vector3 intersectCylinder(Vector3 rayOrigin, Vector3 ray, float radius, float height){  //assumes cylinder aligned along z-axis and centered on 0,0,0, returns a point of intersection
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
		Vector3 point;
		//if(radical < 0){
			//return point;  //we should never get here, this is the case where there is no intersection
		//}
		float t = (-b + Mathf.Sqrt(radical))/(2*a);  //quadratic formula
		/*
	if(t <= 0){								// we should get out a positive and negative t for each case since we're doing an intersection from inside the cylinder									//
	t = (-b + sqrt(radical))/(2*a);    //  so we can assume it's the "+" version of quadratic formula
	}
	*/
		//here we should have a value of t
		//so extrapolate that and find our point on hte cylinder
		point = t*ray;  //this is relative to the vertexPosition
		
		rayOrigin[2] = rayOrigin[2]-height/2;  //here, vertex position is stored ranging from 0 - 40 (eg, bottom of the cylinder is at 0), whereas the stored geometry at this point puts the bottom at -20 and top at 20.  Changing it before would mess up calculations, so change it here (and only temporarily)
		Vector3 finalPoint = point + rayOrigin;  //this is the actual point of contact with the cylinder
		
		if(finalPoint[2] > height/2){ //if we hit the top
			t = ((height/2) - rayOrigin[2]) / Mathf.Abs(ray[2]);	
			point = t * ray;
		}
		if(finalPoint[2] < (-height/2)){ //we hit the bottom
			t = (rayOrigin[2]+(height/2)) / Mathf.Abs(ray[2]);
			point = t * ray;
		}
		return point;
	}




	//remove current dots and move to the next event
	public void changeEvent(int num){
		if((!(eventNumber >= allEvents.Count))){
			if(num == 1)eventNumber +=1;
			if(num == -1){
				if(eventNumber != 0) eventNumber -=1;
			}
			if(eventNumber < allEvents.Count && eventNumber > -1){
				odReady = false;
				GameObject[] ids = GameObject.FindGameObjectsWithTag ("id");
				GameObject[] ods = GameObject.FindGameObjectsWithTag ("od");
				foreach(GameObject o in ids){
					Destroy (o);
				}
				foreach(GameObject o in ods){
					Destroy (o);
				}

				//eventNumber += 1;


				dotVector dv = (dotVector) allEvents [eventNumber];
				currentDots = dv;
				plotDots(dv);
			}
		}
    }
    // Update is called once per frame
    
    void Update () {


        /*
		bool wandButtonPressed3 = MiddleVR.VRDeviceMgr.IsWandButtonPressed(3);
		bool wandButtonPressed0 = MiddleVR.VRDeviceMgr.IsWandButtonPressed(0);
		bool wandButtonPressed1 = MiddleVR.VRDeviceMgr.IsWandButtonToggled(1);
       
        if (wandButtonPressed1) {
			isShowing = !isShowing;
			menu.SetActive(isShowing);
		}
        */


        if (Input.GetButtonDown("Fire1"))
        {
            print ("Moving to next event");
            if (!(eventNumber >= allEvents.Count))
            {
                changeEvent(1);
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!(eventNumber == 0))
            {
                //eventNumber -=1;
                changeEvent(-1);
                print ("Moving to previous event");
            }
        }

    }

    void AutoEvent(){

		changeEvent (1);
	}

	public void setParticle(){
		od.changeParticle (1);
	}

}
