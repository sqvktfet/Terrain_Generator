using UnityEngine;
using System;

public class TerrainScript : MonoBehaviour {

	public int dimension;
	private int size;
	public int seed;
	public float rangeOfNoise; 
//	public float[] rangeOfNoiseList;
	private float[,] heightMap;
	private Vector3[] vertices;
	private int scale = 10;
	// Use this for initialization

	private MeshFilter terrainMesh;

	public Shader shader;
	private Color[] colorlist;
	private int numberOfDegrees;

	void Start () {

		dimension = 7;
		size = (int)(int)Math.Pow(2,dimension)+1;
		seed = 500;
		rangeOfNoise = 500.0f;
//		rangeOfNoiseList = new float[]{30.0f,15.0f,10.0f,7.0f,5.0f,4.0f};
		heightMap = new float[size, size];
		vertices = new Vector3[size * size];
		colorlist = new Color[]{ 
			converColor(153f, 76f, 0f, 1f),
			converColor(153f, 153f, 0f, 1f),
			converColor(102f, 204f, 0f, 1f),
			converColor(0f, 204f, 0f, 1f),
			converColor(178f, 255f, 102f, 1f),
			converColor(153f, 255f, 153f, 1f),
			converColor(255f, 255f, 255f, 1f)
		};
		numberOfDegrees = colorlist.Length;

		terrainMesh = GetComponent<MeshFilter> ();
		terrainMesh.mesh = this.CreateTerrainMesh ();
	}
	
	// Update is called once per frame
	void Update () {
		MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

		// Set blend uniform parameter in an oscillating fashion to demonstrate 
		// blending shader (challenge question)
		renderer.material.SetFloat("_BlendFct", ((int)Mathf.Sin(Time.time) + 1.0f) / 2.0f);	

		// Add collider to mesh 
		GetComponent<MeshCollider> ().sharedMesh = terrainMesh.mesh; 

	}

	private Mesh CreateTerrainMesh(){
		Mesh m = new Mesh();
		m.name = "Terrain demo";


		/***** Generate HeightMap *****/
		// Four cornors 
		heightMap [0,0] = (float)(seed + addError());
		heightMap [0, size - 1] = (float)(seed + addError());
		heightMap [size - 1, 0] = (float)(seed + addError());
		heightMap [size - 1, size - 1] = (float)(seed + addError());

		// "dimension" times iterations
		for (int i = 0; i < dimension; i++) {
			int step = size / (int)Math.Pow (2, i + 1);
			int sqrt = (int)Math.Pow (2, i); // sqrt of number of cornors

			// Diamond 
			for (int j = 0; j < (int)Math.Pow(4, i); j++){
				int x = 2 * (j / sqrt) * step + step;
				int y = 2 * (j % sqrt) * step + step;
				float tempHeight = 0;
				heightMap [x, y] = (
					heightMap [x - step, y - step] +
					heightMap [x - step, y + step] +
					heightMap [x + step, y - step] +
					heightMap [x + step, y + step]) / 4 + addError();

				// "square" surrounding each "diamond" 
				float temp = 0;

				// up
				temp =
					heightMap [x - step, y - step] +
					heightMap [x - step, y + step] +
					heightMap [x, y];
				if (x - 2 * step > 0)
					heightMap [x - step, y] = (temp + heightMap [x - 2 * step, y]) / 4 + addError();
				else
					heightMap [x - step, y] = temp / 3 + addError();
				
				// down
				temp = 
					heightMap [x + step, y - step] +
					heightMap [x + step, y + step] +
					heightMap [x, y];
				if (x + 2 * step < size)
					heightMap [x + step, y] = (temp + heightMap [x + 2 * step, y]) / 4 + addError();
				else
					heightMap [x + step, y] = temp / 3 + addError();
				
				// left
				temp =
					heightMap [x - step, y - step] +
					heightMap [x + step, y - step] +
					heightMap [x, y]; 
				if (y - 2 * step > 0)
					heightMap [x, y - step] = (temp + heightMap [x, y - 2 * step]) / 4 + addError();
				else
					heightMap [x, y - step] = temp / 3 + addError();
				
				// right
				temp = 
					heightMap [x - step, y + step] +
					heightMap [x + step, y + step] +
					heightMap [x, y];
				if (y + 2 * step < size)
					heightMap [x, y + step] = (temp + heightMap [x, y + 2 * step]) / 4 + addError();
				else
					heightMap [x, y + step] = temp / 3 + addError();
			}

			// Reduce range of noise for each iteration
			//rangeOfNoise = rangeOfNoise - rangeOfNoise / (float)dimension / 2;
//			rangeOfNoise = rangeOfNoiseList[i];
			rangeOfNoise *= 0.5f;
		}
		/***** Generate HeightMap *****/

		// get highest point and lowest point
		float top, bottom;
		top = bottom = heightMap[0,0];
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				if (top < heightMap [i,j])
					top = heightMap [i,j];
				if (bottom > heightMap [i,j])
					bottom = heightMap [i,j];
			}
		}

		// Define the vertices. 
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = new Vector3 ((float)(i % size * scale), heightMap [i / size, i % size], (float)((size - i / size) * scale));
//			Debug.Log (heightMap [i / size, i % size]);
		}
		m.vertices = vertices;

		// Add color to vertices
		Color[] colors = new Color[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			colors [i] = colorlist[degreeOfColor (top, bottom, vertices [i].y)];
		}
		m.colors = colors;

		// Automatically define the triangles based on the number of vertices
		int numOfSquares = (int)Math.Pow(4,dimension);
		int sqrtOfNumOfSquares = (int)Math.Pow (2, dimension);
		int[] triangles = new int[numOfSquares * 2 * 3];
		for (int i = 0; i < numOfSquares; i++) {
			int topLeftCornorIndex = (i / sqrtOfNumOfSquares) * size + i % sqrtOfNumOfSquares;
			triangles[2 * 3 * i + 0] = topLeftCornorIndex;
			triangles[2 * 3 * i + 1] = topLeftCornorIndex + 1;
			triangles[2 * 3 * i + 2] = topLeftCornorIndex + size;
			triangles[2 * 3 * i + 3] = topLeftCornorIndex + size;
			triangles[2 * 3 * i + 4] = topLeftCornorIndex + 1;
			triangles[2 * 3 * i + 5] = topLeftCornorIndex + size + 1;

		}

		m.triangles = triangles;

		return m;
	
	}

	private float addError(){
		System.Random rnd = new System.Random();
		float result;
		result = (float)rnd.NextDouble() * rangeOfNoise - rangeOfNoise / 2;
		return result;
	}

	public int GetSize() {
		return size;
	}

	public int GetScale() {
		return scale;
	}

	private int degreeOfColor(float top, float bottom, float height){
		int degree;
		float difference = top - bottom;
		float relativeHeight = height - bottom;
		float percentile = relativeHeight / difference;

		if (percentile <= 0.1f)
			degree = 0;
		else if (percentile <= 0.23f)
			degree = 1;
		else if (percentile <= 0.40f)
			degree = 2;
		else if (percentile <= 0.58f)
			degree = 3;
		else if (percentile <= 0.75f)
			degree = 4;
		else if (percentile <= 0.90f)
			degree = 5;
		else 
			degree = 6;

		return degree;
	}

	private Color converColor(float r, float g, float b, float a){
		Color result;
		result = new Color (r / 255.0f, g / 255.0f, b / 255.0f, a);
		return result;
	}

	public float GetHeight(int x, int z) {
		Debug.Log("x"+x);
		Debug.Log("z"+z);
			
		return heightMap [x, z];
	}
}
