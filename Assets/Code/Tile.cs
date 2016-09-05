using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TerrainGen{
	public class Tile {
		private float height;
		private int worldSize;
		private List<Tile> parrents;
		private Tile [,] world;
		public int x, y;
		public static System.Random rnd = new System.Random ();
		public static float maxHeight = 0;
		public static float minHeight = 0;

		public Tile(int x, int y, Tile[,] world, int ws){
			parrents = new List<Tile> ();
			height = Mathf.Infinity;
			this.world = world;
			this.worldSize = ws;
			this.x = x;
			this.y = y;
			//Debug.Log ("x: " + x + "; y: " + y);
		}
		public Tile(int x, int y, Tile[,] world, int ws, float height){
			parrents = null;
			this.height = height;
			this.world = world;
			this.worldSize = ws;
			this.x = x;
			this.y = y;
			//Debug.Log ("x: " + x + "; y: " + y);
		}

		public List<Tile> doDiamond(int radius){
			List<Tile> res = new List<Tile> ();
			Tile t = workOnTile (x + radius, y + radius);
			if (t != null)
				res.Add (t);
			t = workOnTile (x - radius, y + radius);
			if (t != null)
				res.Add (t);
			t = workOnTile (x + radius, y - radius);
			if (t != null)
				res.Add (t);
			t = workOnTile (x - radius, y - radius);
			if (t != null)
				res.Add (t);
			return res;
		}
		public List<Tile> doSquare(int radius){
			List<Tile> res = new List<Tile> ();
			Tile t = workOnTile (x + radius, y);
			if (t != null)
				res.Add (t);
			t = workOnTile (x - radius, y);
			if (t != null)
				res.Add (t);
			t = workOnTile (x, y + radius);
			if (t != null)
				res.Add (t);
			t = workOnTile (x, y - radius);
			if (t != null)
				res.Add (t);
			return res;
		}

		public void generateHeigth(int radius, float randRadius = 0.0f){
			if (parrents == null)
				return;
			float avg = 0.0f;
			foreach (Tile t in parrents) {
				avg += t.height;
			}
			avg /= parrents.Count;
			float randAdd = ((float)rnd.NextDouble () * 2 * randRadius) - randRadius;
			avg += randAdd * radius;
			height = avg;
			if (height > maxHeight)
				maxHeight = height;
			if (height < minHeight)
				minHeight = height;
		}

		public void addParrent(Tile t){
			parrents.Add (t);
		}

		public static bool isValidPosition(int x, int y, int wSize){
			return x >= 0 && x < wSize && y >= 0 && y < wSize;
		}
		public bool isValidPosition(int x, int y){
			return isValidPosition (x, y, worldSize);
		}

		private Tile workOnTile(int x, int y){
			if (!isValidPosition (x, y))
				return null;
			Tile toReturn = null;
			if (world [x, y] == null) {
				world [x, y] = new Tile (x, y, world, worldSize);
				toReturn = world [x, y];
			}
			world [x, y].addParrent (this);
			return toReturn;
		}

		public Vector3 getPosition(bool floorVal){
			if (floorVal)
				return new Vector3 ((float)x, Mathf.Floor(height), (float)y);
			return new Vector3 ((float)x, height, (float)y);
		}
		public float getPosition(float mHeight){
			return (height - minHeight) / mHeight;
		}
	}
}
