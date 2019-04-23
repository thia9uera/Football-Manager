using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PrimitiveUI.Examples {
	public class PUIExampleSineWaves : MonoBehaviour {
		public PrimitiveCanvas primitiveCanvas;
		public Slider sliderPeriod;
		public Slider sliderAmplitude;
		public Slider sliderFrequency;
		public Color bgLinesColor;
		public Color centerLinesColor;
		public int numBgLinesHorizontal;

		StrokeStyle sinStrokeStyle;
		StrokeStyle bgLinesStrokeStyle;
		StrokeStyle centerLinesStrokeStyle;

		float lastSineProgress = 0f;
		
		void Start(){
			//hook up the sliders so that the sine wave will redraw whenever a parameter is updated
			sliderPeriod.onValueChanged.AddListener(f => Draw());
			sliderAmplitude.onValueChanged.AddListener(f => Draw());
			sliderFrequency.onValueChanged.AddListener(f => Draw());

			//set up some StrokeStyles and colors
			sinStrokeStyle = new StrokeStyle(Color.white, 0.025f, StrokeScaleMode.Relative);
			bgLinesStrokeStyle = new StrokeStyle(bgLinesColor, 0.004f, StrokeScaleMode.Relative);
			centerLinesStrokeStyle = new StrokeStyle(centerLinesColor, 0.01f, StrokeScaleMode.Relative);
		}

		void Update(){
			Draw();
		}
		
		void Draw(){
			//clear the canvas for new drawing
			primitiveCanvas.Clear();
			//draw the background lines, evenly spaced
			//first, the horizontal ones
			float totalLineThickess = bgLinesStrokeStyle.thickness * numBgLinesHorizontal;
			float lineSpacing = (1f - totalLineThickess) / (numBgLinesHorizontal - 1);
			float lineOffset = bgLinesStrokeStyle.thickness/2f * (numBgLinesHorizontal - 1);

			for(int i = 0; i<numBgLinesHorizontal; i++){
				float lineX = lineOffset + i * lineSpacing;
				primitiveCanvas.DrawLine(new Vector2(lineX, 0f), new Vector2(lineX, 1f), bgLinesStrokeStyle);
			}
			//next, the verticals. We need to keep the aspect ratio in mind here
			float lineThicknessVertical = bgLinesStrokeStyle.thickness * primitiveCanvas.aspectRatio;
			lineSpacing = lineSpacing * primitiveCanvas.aspectRatio;
			int numBgLinesVertical = Mathf.CeilToInt(1f / (lineThicknessVertical + lineSpacing));
			lineOffset = lineThicknessVertical/2f * (numBgLinesVertical - 1);

			for(int i = 0; i<numBgLinesVertical; i++){
				float lineY = lineOffset + i * lineSpacing;
				primitiveCanvas.DrawLine(new Vector2(0f, lineY), new Vector2(1f, lineY), bgLinesStrokeStyle);
			}

			//draw the center lines
			primitiveCanvas.DrawLine(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), centerLinesStrokeStyle);
			primitiveCanvas.DrawLine(new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), centerLinesStrokeStyle);

			//and now for the fun part, the sine wave!
			float overdraw = 0.05f; //draw a bit too much to prevent visible line caps on thicker stroke styles
			float sampleSize = 0.01f;
			List<Vector2> points = new List<Vector2>();

			for(float i = -overdraw; i < 1f + overdraw; i += sampleSize){
				float period = Mathf.PI*2 / sliderPeriod.value;
				float sin = Mathf.Sin(period * i*Mathf.PI + lastSineProgress) * sliderAmplitude.value / 2 + 0.5f;
				points.Add(new Vector2(i, sin));
			}
			
				           primitiveCanvas.DrawPath(points.ToArray(), sinStrokeStyle, false);

			//and finally, update the value that allows a smooth, continuous, editable sine wave
			lastSineProgress += Time.deltaTime * sliderFrequency.value;
		}
	}
}