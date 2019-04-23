using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using PrimitiveUI;

namespace PrimitiveUI.Editor {
	public class PrimitiveUIEditor {
		//Would have just used UnityEditor.UI.PlaceUIElementRoot(...), but there's a linking bug for that namespace...
		//Instead, use a modified version of
		//https://bitbucket.org/Unity-Technologies/ui/src/3de6943aaebc66eb8284d63140f64abdca277228/UnityEditor.UI/UI/MenuOptions.cs?at=5.0
		//and live with the shortcomings...

		static public GameObject GetGraphicParentGameObject(){
			GameObject selectedGo = Selection.activeGameObject;

			Graphic graphic = (selectedGo != null) ? selectedGo.GetComponentInParent<Graphic>() : null;
			if(graphic != null && graphic.gameObject.activeInHierarchy) return graphic.gameObject;
			
			Canvas canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
			if(canvas != null && canvas.gameObject.activeInHierarchy) return canvas.gameObject;

			return null;
		}

		//And now the actual menu item
		[MenuItem("GameObject/UI/Primitive Canvas")]
		private static void AddPrimitiveCanvasToScene(MenuCommand menuCommand){
			GameObject graphicRoot = GetGraphicParentGameObject();

			if(graphicRoot == null){
				Debug.LogError("Please ensure at least one active Canvas exists in the scene.");
				return;
			}

			string uniqueName = GameObjectUtility.GetUniqueNameForSibling(graphicRoot.transform, "Primitive Canvas");

			GameObject pCanvas = new GameObject(uniqueName);
			
			Undo.RegisterCreatedObjectUndo(pCanvas, "Create " + uniqueName);
			Undo.SetTransformParent(pCanvas.transform, graphicRoot.transform, "Parent " + pCanvas.name);
			GameObjectUtility.SetParentAndAlign(pCanvas, graphicRoot);
			
			RectTransform rectTransform = pCanvas.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;
			
			pCanvas.AddComponent<PrimitiveCanvas>();

			Selection.activeGameObject = pCanvas;
		}
	}
}