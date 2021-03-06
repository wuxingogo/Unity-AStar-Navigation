﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Obstacle{
	public int x;
	public int y;
}

public enum PathFinderType
{
	AStar,
	EarlyExit,
	Heuristic,
	Dijkstra,
}
public class Graph : MonoBehaviour {

	public int Size = 40;
	private Cell[][] totalCell = new Cell[40][];
	public int blank = 1;
	public Obstacle[] obstaclesX;
	public PathFinderType finderType = PathFinderType.AStar;
	void Start()
	{
		for (int i = 0; i < Size; i++) {
			totalCell[i] = new Cell[Size];
			for (int j = 0; j < Size; j++) {

				totalCell[i][j] = Cell.Create(i,j);
				totalCell[i][j].transform.SetParent(transform);
				totalCell[i][j].name = string.Format("{0}_{1}",i,j);
				totalCell[i][j].transform.localPosition = new Vector3(i *blank, j * blank, 0);
			}
		}

		for (int i = 0; i < obstaclesX.Length; i++) {
			int x = obstaclesX[i].x;
			int y = obstaclesX[i].y;
			totalCell[x][y].isObstacle = true;
		}

		for (int i = 0; i < Size; i++) {
			for (int j = 0; j < Size; j++) {
//				if(totalCell[i][j].isObstacle)
//					continue;
				if(i <= Size - 2)
					totalCell[i][j].AddNeighbors(totalCell[i + 1][j]);
				if(j <= Size - 2)
					totalCell[i][j].AddNeighbors(totalCell[i][j + 1]);
			}
		}

		SetStartCell(totalCell[10][2], totalCell[18][30]);
	}

	public int heuristic(Cell lhs, Cell rhs)
	{
		return Mathf.Abs(lhs.x - rhs.x) + Mathf.Abs(lhs.x - rhs.x);
	}
	public float delay = 0.01f;
	void SetStartCell(Cell startCell, Cell targetCell)
	{
		targetCell.isGoal = true;
		startCell.isGoal = true;
		targetCell.SetColor(Color.red);
		startCell.SetColor(Color.red);

		switch (finderType) {
			case PathFinderType.AStar:
				StartCoroutine(AStar(startCell, targetCell));
				break;
			case PathFinderType.EarlyExit:
				StartCoroutine(EarlyExit(startCell, targetCell));
				break;
			case PathFinderType.Heuristic:
				break;
			case PathFinderType.Dijkstra:
				StartCoroutine(Dijkstra(startCell, targetCell));
				break;
			default:
				break;
		}
//		Debug.Log("Came From : " + came_from.Count);
//		while (came_from.Count > 0) {
//			Debug.Log( came_from.Dequeue().name);
//		}
//		Debug.Log("frontier : " + frontier.Count);
//		while (frontier.Count > 0) {
//			Debug.Log( frontier.Dequeue().name);
//		}
	}
	IEnumerator EarlyExit(Cell startCell, Cell targetCell)
	{
        PriorityQueue<Cell> frontier = new PriorityQueue<Cell>();
        frontier.Enqueue(startCell, 0);
        Dictionary<Cell, int> cost_so_far = new Dictionary<Cell, int>();
        startCell.cameFrom = null;
        Cell current = null;
        cost_so_far.Add(startCell, 0);

        int newCost = 0;
        while (frontier.Count != 0) {
            current = frontier.Dequeue();

            current.Log();
            //current.color = Color.green;
            current.SetColor(Color.green);
            if(current == targetCell)
                break;
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < current.neighbors.Count; i++) {
                var next = current.neighbors[i];
                newCost = cost_so_far[current] + next.Cost();
                if(!cost_so_far.ContainsKey(next) || newCost < cost_so_far[next])
                {
                    if(!cost_so_far.ContainsKey(next))
                        cost_so_far.Add(next,newCost);
                    else
                        cost_so_far[next] = newCost;
                    int priority = newCost;
                    frontier.Enqueue(next, priority);
                    //came_from.Enqueue(next);
                    next.cameFrom = current;
                }
            }
        }

        current = targetCell;
        current.SetColor(Color.blue);
        Queue<Cell> path = new Queue<Cell>();
        path.Enqueue(current);
        while (current != startCell) {
            yield return new WaitForSeconds(delay);
            current = current.cameFrom;
            path.Enqueue(current);
            current.SetColor(Color.blue);
        }
        path.Enqueue(startCell);
 
	}

	IEnumerator AStar(Cell startCell, Cell targetCell)
	{

		
		//		Queue<Cell> frontier = new Queue<Cell>();
		//		frontier.Enqueue(startCell);
		PriorityQueue<Cell> frontier = new PriorityQueue<Cell>();
		frontier.Enqueue(startCell, 0);
		Dictionary<Cell, int> cost_so_far = new Dictionary<Cell, int>();
		startCell.cameFrom = null;
		Cell current = null;
		cost_so_far.Add(startCell, 0);
		
		int newCost = 0;
		while (frontier.Count != 0) {
			current = frontier.Dequeue();
			
			current.Log();
			//current.color = Color.green;
			current.SetColor(Color.green);
			if(current == targetCell)
				break;
			yield return new WaitForEndOfFrame();
			for (int i = 0; i < current.neighbors.Count; i++) {
				var next = current.neighbors[i];
				newCost = cost_so_far[current] + next.Cost();
				if(!cost_so_far.ContainsKey(next) || newCost < cost_so_far[next])
				{
					if(!cost_so_far.ContainsKey(next))
						cost_so_far.Add(next,newCost);
					else
						cost_so_far[next] = newCost;
					int priority = newCost + heuristic(targetCell, next);
					frontier.Enqueue(next, priority);
					//came_from.Enqueue(next);
					next.cameFrom = current;
				}
			}
		}
		
		current = targetCell;
		current.SetColor(Color.blue);
		Queue<Cell> path = new Queue<Cell>();
		path.Enqueue(current);
		while (current != startCell) {
			yield return new WaitForSeconds(delay);
			current = current.cameFrom;
			path.Enqueue(current);
			current.SetColor(Color.blue);
		}
		path.Enqueue(startCell);
	}

	IEnumerator Dijkstra(Cell startCell, Cell targetCell)
	{
		
		//        Queue<Cell> frontier = new Queue<Cell>();
		//        frontier.Enqueue(startCell);
		PriorityQueue<Cell> frontier = new PriorityQueue<Cell>();
		frontier.Enqueue(startCell, 0);
		Dictionary<Cell, int> cost_so_far = new Dictionary<Cell, int>();
		startCell.cameFrom = null;
		Cell current = null;
		cost_so_far.Add(startCell, 0);
		
		int newCost = 0;
		while (frontier.Count != 0) {
			current = frontier.Dequeue();
			
			current.Log();
			//current.color = Color.green;
			current.SetColor(Color.green);
			if(current == targetCell)
				break;
			yield return new WaitForEndOfFrame();
			for (int i = 0; i < current.neighbors.Count; i++) {
				var next = current.neighbors[i];
				newCost = cost_so_far[current] + next.Cost();
				if(!cost_so_far.ContainsKey(next) || newCost < cost_so_far[next])
				{
					if(!cost_so_far.ContainsKey(next))
						cost_so_far.Add(next,newCost);
					else
						cost_so_far[next] = newCost;
					int priority = newCost;
					frontier.Enqueue(next, priority);
					//came_from.Enqueue(next);
					next.cameFrom = current;
				}
			}
		}
		
		current = targetCell;
		current.SetColor(Color.blue);
		Queue<Cell> path = new Queue<Cell>();
		path.Enqueue(current);
		while (current != startCell) {
			yield return new WaitForSeconds(delay);
			current = current.cameFrom;
			path.Enqueue(current);
			current.SetColor(Color.blue);
		}
		path.Enqueue(startCell);

	}
	
}
