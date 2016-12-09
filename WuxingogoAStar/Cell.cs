using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {

	public List<Cell> neighbors = new List<Cell>();
	public Cell cameFrom = null;
	public bool isObstacle = false;
	public bool isGoal = false;
	public int x = 0;
	public int y = 0;
	public void AddNeighbors(Cell neighbor)
	{
		if(!neighbors.Contains(neighbor))
		{
			neighbor.neighbors.Add(this);
			neighbors.Add(neighbor);
		}

	}
	public static Cell Create(int x, int y)
	{
		var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//var go = new GameObject();
		var cell = go.AddComponent<Cell>();
		cell.x = x;
		cell.y = y;
		return cell;
	}

	public void Log()
	{
		XLogger.Log("Tranvers " + name, gameObject); 
	}
	
	public Color color = Color.red;

//	void OnDrawGizmos()
//	{
//		Gizmos.color = color;
//		for (int i = 0; i < neighbors.Count; i++) {
//			Gizmos.DrawCube(neighbors[i].transform.position, Vector3.one);
//		}
//
//	}
	void Start()
	{
		if (isObstacle)
			SetColor(Color.black);
	}

	public void SetColor(Color color)
	{
		if(isObstacle)
			GetComponent<MeshRenderer>().material.color = color + new Color(0.5f,0f,0f);
		else
			GetComponent<MeshRenderer>().material.color = color;
		if(isGoal)
			GetComponent<MeshRenderer>().material.color = Color.red;

	}

	public int Cost()
	{
		return isObstacle ? 2 : 1;
	}
}


