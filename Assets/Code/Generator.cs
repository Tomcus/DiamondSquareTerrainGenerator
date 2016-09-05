using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Terrain{
	public class Generator : MonoBehaviour {
		public int maxExponent = 12;
		public float speed = 40f;
		public Text expText;
		public Canvas c;
		public Canvas r;
		private int exponent = 3;
		private float leftTopHeight = 0.0f;
		private float leftBottomHeight = 0.0f;
		private float rightTopHeight = 0.0f;
		private float rightBottomHeight = 0.0f;
		private float randElement = 0.5f;
		private bool snapValues = true;
		private Tile[,] world;
		public GameObject cube;
		private int length;
		private float rot;
		private List<Tile> existingTiles = new List<Tile> ();
		private List<Tile> newTiles = new List<Tile> ();

		private float getMaxCorner(){
			float max = leftTopHeight;
			if (max < leftBottomHeight)
				max = leftBottomHeight;
			if (max < rightTopHeight)
				max = rightTopHeight;
			if (max < rightBottomHeight)
				max = rightBottomHeight;
			return max;
		}

		public void prepareScene(){
			leftTopHeight = (float)Tile.rnd.NextDouble () * 100f;
			leftBottomHeight = (float)Tile.rnd.NextDouble () * 100f;
			rightTopHeight = (float)Tile.rnd.NextDouble () * 100f;
			rightBottomHeight = (float)Tile.rnd.NextDouble () * 100f;
			randElement = (float)Tile.rnd.NextDouble ();
			generateTerain ();
			c.gameObject.SetActive (false);
			r.gameObject.SetActive (true);
			Camera.main.transform.position = new Vector3 (-38f, getMaxCorner(), length/2f);
		}

		private void generateTerain(){
			length = (int)Mathf.Pow(2, exponent) + 1;
			world = new Tile[length, length];
			Debug.Log ("Generating: " + (length * length) + " tiles");
			nullTheWorld ();
			prepareTiles ();
			doDiamondSquare ();
			transform.position = new Vector3 (length / 2f, 0, length / 2f);
			createWorld ();
		}

		private void prepareTiles(){
			world [0, 0] = new Tile (0, 0, world, length, leftTopHeight);
			existingTiles.Add (world [0, 0]);
			world [0, length - 1] = new Tile (0, length - 1, world, length, leftBottomHeight);
			existingTiles.Add (world [0, length - 1]);
			world [length - 1, 0] = new Tile (length - 1, 0, world, length, rightTopHeight);
			existingTiles.Add (world [length - 1, 0]);
			world [length - 1, length - 1] = new Tile (length - 1, length - 1, world, length, rightBottomHeight);
			existingTiles.Add (world [length - 1, length - 1]);
		}

		private void nullTheWorld(){
			for (int x = 0; x < length; x++)
				for (int y = 0; y < length; y++){
					world [x, y] = null;
				}
		}

		public void onSizeChange(float newValue){
			exponent = (int)(newValue * (maxExponent - 3)) + 3;
			expText.text = "" + exponent;
		}
		public void onSeedChange(string newValue){
			if (newValue == "")
				Tile.rnd = new System.Random ();
			else {
				Tile.rnd = new System.Random (newValue.GetHashCode ());
				Debug.Log (newValue.GetHashCode ());
			}
		}
		public void onSnapChange(bool newValue){
			snapValues = newValue;
		}
		public void onRotationChange(float newValue){
			transform.eulerAngles = new Vector3 (0, newValue * 360f, 0);
		}
		public void onLinkClicked(){
			Application.OpenURL ("");
		}

		private void doDiamondSquare(){
			int len = length;
			HeightCounter hc = null;
			Thread countingThread = null;
			while (len > 1) {
				len /= 2;
				// Diamond
				newTiles = new List<Tile> ();
				foreach (Tile t in existingTiles) {
					newTiles.AddRange(t.doDiamond (len));
				}
				while (countingThread != null && hc != null && countingThread.IsAlive && hc.isRunning );
				hc = new HeightCounter (newTiles, len, randElement);
				countingThread = new Thread (new ThreadStart(hc.countHeights));
				countingThread.Start ();
				existingTiles.AddRange (newTiles);
				Debug.Log ("Generated: " + existingTiles.Count);
				// Square
				newTiles = new List<Tile> ();
				foreach (Tile t in existingTiles) {
					newTiles.AddRange(t.doSquare (len));
				}
				while (countingThread != null && hc != null && countingThread.IsAlive && hc.isRunning );
				hc = new HeightCounter (newTiles, len, randElement);
				countingThread = new Thread (new ThreadStart(hc.countHeights));
				countingThread.Start ();
				existingTiles.AddRange (newTiles);
				Debug.Log ("Generated: " + existingTiles.Count);
			}
			while (countingThread != null && hc != null && countingThread.IsAlive && hc.isRunning );
		}

		private void createWorld(){
			for (int x = 0; x < length; x++)
				for (int y = 0; y < length; y++) {
					GameObject go = (GameObject)Instantiate (cube, world [x, y].getPosition (snapValues), Quaternion.identity);
					go.name = "Tile_" + x + "_" + y;
					go.transform.SetParent (transform);
				}
		}
	}
}
