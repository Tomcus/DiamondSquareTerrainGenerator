using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain{
	public class HeightCounter {
		private List<Tile> newTiles;
		private int len;
		private float randElement;
		public bool isRunning = true;
		public HeightCounter(List<Tile> newTiles, int len, float randElement){
			this.newTiles = newTiles;
			this.len = len;
			this.randElement = randElement;
		}
		public void countHeights(){
			foreach (Tile t in newTiles){
				t.generateHeigth (len, randElement);
			}
			isRunning = false;
		}
	}
}