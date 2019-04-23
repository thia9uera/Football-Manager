using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveUI.Examples {
	public class PUIExampleRPGStats : MonoBehaviour {
		public PrimitiveCanvas primitiveCanvas;
		public Slider sliderStr;
		public Slider sliderWis;
		public Slider sliderAgi;
		public Slider sliderDex;
		public Slider sliderCha;
		public Slider sliderCon;
		public Color chartColorBackground;
		public Color chartColorFill;
		public Color chartColorStroke;

		Vector2 chartCenter = new Vector2(0.7f, 0.5f);
		float chartRadius = 0.2f;
		Color chartColorStrokeInner;
		StrokeStyle chartOuterStroke;
		StrokeStyle chartMarksStroke;
		StrokeStyle chartSegmentsStroke;

		void Start(){
			//hook up the sliders so that the chart will redraw whenever a stat is updated
			sliderStr.onValueChanged.AddListener(f => Draw());
			sliderWis.onValueChanged.AddListener(f => Draw());
			sliderAgi.onValueChanged.AddListener(f => Draw());
			sliderDex.onValueChanged.AddListener(f => Draw());
			sliderCha.onValueChanged.AddListener(f => Draw());
			sliderCon.onValueChanged.AddListener(f => Draw());
			

			//set up some StrokeStyles and colors
			chartColorStrokeInner = new Color(chartColorStroke.r, chartColorStroke.g, chartColorStroke.b, chartColorStroke.a * 0.5f);
			chartOuterStroke = new StrokeStyle(chartColorStroke, 4f, StrokeScaleMode.Absolute);
			chartMarksStroke = new StrokeStyle(chartColorStrokeInner, 2f, StrokeScaleMode.Absolute);
			chartSegmentsStroke = new StrokeStyle(chartColorStroke, 2f, StrokeScaleMode.Absolute);

			//do an initial draw of the chart
			Draw();
		}

		void Draw(){
			float[] statsPoints = new float[]{
				(sliderStr.value / sliderStr.maxValue) * chartRadius,
				(sliderWis.value / sliderWis.maxValue) * chartRadius,
				(sliderAgi.value / sliderAgi.maxValue) * chartRadius,
				(sliderDex.value / sliderDex.maxValue) * chartRadius,
				(sliderCha.value / sliderCha.maxValue) * chartRadius,
				(sliderCon.value / sliderCon.maxValue) * chartRadius,
			};

			primitiveCanvas.Clear(); //clear the canvas for new drawing
			primitiveCanvas.DrawRegularSolid(chartCenter, chartRadius, 6, chartColorBackground, chartOuterStroke); //background + outer stroke
			primitiveCanvas.DrawRegularSolid(chartCenter, chartRadius/3, 6, chartMarksStroke); //inner stroke 1/3rd mark
			primitiveCanvas.DrawRegularSolid(chartCenter, chartRadius/3*2, 6, chartMarksStroke); //inner stroke 2/2rd mark

			//Next, let's draw some center-outgoing lines as well, to mark the sections!
			Vector2 evenRadius = new Vector2(chartRadius, chartRadius * primitiveCanvas.aspectRatio); //needed if canvas' width != height
			float angRad = Mathf.PI*2 / 6;
			float correctionAngle = Mathf.PI * 0.5f - 60 * Mathf.Deg2Rad; //used to align our lines with our graph's corner points

			for(int i=0; i<6; i++){
				Vector2 lineEnd = new Vector2(Mathf.Cos(correctionAngle - angRad * i) * evenRadius.x + chartCenter.x,
				                              Mathf.Sin(correctionAngle - angRad * i) * evenRadius.y + chartCenter.y);
				primitiveCanvas.DrawLine(chartCenter, lineEnd, chartSegmentsStroke);
			}

			primitiveCanvas.DrawIrregularSolid(chartCenter, statsPoints, chartColorFill); //the graph fill
		}
	}
}