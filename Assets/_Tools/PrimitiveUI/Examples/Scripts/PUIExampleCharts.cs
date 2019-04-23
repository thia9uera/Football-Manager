using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveUI.Examples {
	public class PUIExampleCharts : MonoBehaviour {
		public PrimitiveCanvas primitiveCanvas;
		public Slider sliderAka;
		public Slider sliderBlauw;
		public Slider sliderGelb;
		public Slider sliderVerde;

		[Range(0.1f, 0.25f)]
		public float barChartBarWidth = 0.2f;

		Rect barChartBounds = new Rect(0.45f, 0.05f, 0.5f, 0.5f);
		Vector2 pieChartCenter = new Vector2(0.7f, 0.8f);
		float pieChartRadius = 0.1f;

		Color red = new Color(1f, 0.196f, 0f);
		Color blue = new Color(0f, 0.47f, 1f);
		Color yellow = new Color(1f, 0.92f, 0f);
		Color green = new Color(0f, 1f, 0.176f);
		StrokeStyle barChartAxisStroke;
		StrokeStyle barChartBgStroke;

		void Start(){
			//hook up the sliders so that the chart will redraw whenever a stat is updated
			sliderAka.onValueChanged.AddListener(f => Draw());
			sliderBlauw.onValueChanged.AddListener(f => Draw());
			sliderGelb.onValueChanged.AddListener(f => Draw());
			sliderVerde.onValueChanged.AddListener(f => Draw());
			
			//set up some StrokeStyles and colors
			barChartAxisStroke = new StrokeStyle(Color.black, 0.006f, StrokeScaleMode.Relative);
			barChartBgStroke = new StrokeStyle(new Color(0f, 0f, 0f, 0.2f), 0.0025f, StrokeScaleMode.Relative);

			//do an initial draw of the charts
			Draw();
		}

		void Draw(){
			primitiveCanvas.Clear(); //clear the canvas for new drawing

			//bars first, pie is for dessert
			//let's start with the helper lines at 10% intervals in the background
			float bgLineSpacing = barChartBounds.height / 9f;

			for(int i=0; i<8; i++){
				float height = barChartBounds.yMin + (i+1) * bgLineSpacing;
				primitiveCanvas.DrawLine(new Vector2(barChartBounds.xMin, height), new Vector2(barChartBounds.xMax, height), barChartBgStroke);
			}

			//and now the bars themselves
			float realBarWidth = barChartBounds.width * barChartBarWidth;
			float barSpacing = (barChartBounds.width - realBarWidth * 4) / 5;
			Rect bar = new Rect(barChartBounds.xMin + barSpacing * 1, barChartBounds.yMin,
			                    realBarWidth, sliderAka.value * barChartBounds.height * 0.01f);
			primitiveCanvas.DrawRectangle(bar, red);
			bar = new Rect(barChartBounds.xMin + barSpacing * 2 + realBarWidth * 1, barChartBounds.yMin,
			               realBarWidth, sliderBlauw.value * barChartBounds.height * 0.01f);
			primitiveCanvas.DrawRectangle(bar, blue);
			bar = new Rect(barChartBounds.xMin + barSpacing * 3 + realBarWidth * 2, barChartBounds.yMin,
			               realBarWidth, sliderGelb.value * barChartBounds.height * 0.01f);
			primitiveCanvas.DrawRectangle(bar, yellow);
			bar = new Rect(barChartBounds.xMin + barSpacing * 4 + realBarWidth * 3, barChartBounds.yMin,
			               realBarWidth, sliderVerde.value * barChartBounds.height * 0.01f);
			primitiveCanvas.DrawRectangle(bar, green);

			//and lastly, draw the main axes
			Vector2[] mainAxisPoints = new Vector2[]{
				new Vector2(barChartBounds.xMin, barChartBounds.yMax),
				barChartBounds.min,
				new Vector2(barChartBounds.xMax, barChartBounds.yMin)
			};
			
			primitiveCanvas.DrawPath(mainAxisPoints, barChartAxisStroke, false);

			//and now for the pie!
			float totalValue = sliderAka.value + sliderBlauw.value + sliderGelb.value + sliderVerde.value;
			float startAngle = 0f;
			float sliceSize = (sliderAka.value / totalValue) * 360f;
			primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1, startAngle, startAngle + sliceSize, red);
			startAngle += sliceSize;
			sliceSize = (sliderBlauw.value / totalValue) * 360f;
			primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1, startAngle, startAngle + sliceSize, blue);
			startAngle += sliceSize;
			sliceSize = (sliderGelb.value / totalValue) * 360f;
			primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1, startAngle, startAngle + sliceSize, yellow);
			startAngle += sliceSize;
			sliceSize = (sliderVerde.value / totalValue) * 360f;
			primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1, startAngle, startAngle + sliceSize, green);

			//because the circle is drawn using a "step size", the edge between green and red may "jiggle" a bit
			//as the sliders are manipulated. To mask this, we can draw a 1 degree red slice on top of it.
			primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1, 0f, 1f, red);
		}
	}
}