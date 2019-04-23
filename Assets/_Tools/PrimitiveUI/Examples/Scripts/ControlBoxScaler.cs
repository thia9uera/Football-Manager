using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PrimitiveUI.Examples {
	//quick and dirty solution to make text look consistent and nice
	public class ControlBoxScaler : MonoBehaviour {
		public Text[] textElements;
		Vector2 screenSize;

		void Start(){
			screenSize = new Vector2(Screen.width, Screen.height);

			foreach(Text text in textElements){
				text.fontSize = Mathf.FloorToInt(Screen.height / 20);
			}
		}
		
		void Update(){
			if(Screen.width != screenSize.x || Screen.height != screenSize.y){
				foreach(Text text in textElements){
					text.fontSize = Mathf.FloorToInt(Screen.height / 20);
				}

				screenSize = new Vector2(Screen.width, Screen.height);
			}
		}
	}
}