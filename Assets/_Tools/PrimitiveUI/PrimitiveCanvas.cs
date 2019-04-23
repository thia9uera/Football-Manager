using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PrimitiveUI{
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Primitive Canvas")]
	public class PrimitiveCanvas : MaskableGraphic {
		public bool setDirtyOnDraw = true;
		public float aspectRatio {
			get{return rectTransform.rect.width / rectTransform.rect.height;}
		}

		List<PUIElement> elements = new List<PUIElement>();

		protected override void OnPopulateMesh(VertexHelper vh){
			vh.Clear();

			Vector2 absSize = rectTransform.rect.size;
			Vector3 pivotOffset = new Vector3(-rectTransform.pivot.x, -rectTransform.pivot.y, 0f);
			List<List<UIVertex>> uiVertexTriangleStreams = new List<List<UIVertex>>(elements.Count);

			for(int i=0; i<elements.Count; i++){
				uiVertexTriangleStreams.Add(elements[i].GetUIVertexTriangleStream(pivotOffset, absSize, color));
			}

			vh.AddUIVertexTriangleStream(uiVertexTriangleStreams.SelectMany(l => l).ToList());
		}

		public void Clear(){
			elements.Clear();
			SetAllDirty();
		}

		protected override void OnRectTransformDimensionsChange(){
			base.OnRectTransformDimensionsChange();

			PUIStrokeElement current;

			foreach(PUIElement el in elements){
				if(el.GetType() == typeof(PUIStrokeElement)){
					current = (PUIStrokeElement)el;

					if(current.strokeStyle.scaleMode == StrokeScaleMode.Absolute){
						float relativeThickness = current.strokeStyle.thickness / rectTransform.rect.width;

						if(current.rawPoints.Length == 2){
							current.UpdatePoints(PUIUtils.GetLinePoints(current.rawPoints[0], current.rawPoints[1],
							                                            relativeThickness,
							                                            aspectRatio));
						}
						else{
							current.UpdatePoints(PUIUtils.GetPathPoints(current.rawPoints,
							                                            current.isClosedPath,
							                                            relativeThickness,
							                                            aspectRatio));
						}
					}
				}
			}
		}

		#region DrawSquare()
		public void DrawSquare(Vector2 center, float size, Color fillColor){
			DrawSquare(center, size, 0f, fillColor, null);
		}

		public void DrawSquare(Vector2 center, float size, StrokeStyle strokeStyle){
			DrawSquare(center, size, 0f, null, strokeStyle);
		}

		public void DrawSquare(Vector2 center, float size, Color fillColor, StrokeStyle strokeStyle){
			DrawSquare(center, size, 0f, fillColor, strokeStyle);
		}

		public void DrawSquare(Vector2 center, float size, float rotation=0f, Color? fillColor=null, StrokeStyle strokeStyle=null){
			DrawRectangle(new Rect(center.x - size*0.5f, center.y - size*0.5f*aspectRatio, size, size * aspectRatio), rotation, fillColor, strokeStyle);
		}
		#endregion DrawSquare()

		#region DrawRectangle()
		public void DrawRectangle(float x, float y, float width, float height, Color fillColor){
			DrawRectangle(new Rect(x, y, width, height), 0f, fillColor, null);
		}

		public void DrawRectangle(float x, float y, float width, float height, StrokeStyle strokeStyle){
			DrawRectangle(new Rect(x, y, width, height), strokeStyle);
		}

		public void DrawRectangle(float x, float y, float width, float height, Color fillColor, StrokeStyle strokeStyle){
			DrawRectangle(new Rect(x, y, width, height), fillColor, strokeStyle);
		}
		
		public void DrawRectangle(float x, float y, float width, float height, float rotation=0f, Color? fillColor=null, StrokeStyle strokeStyle=null){
			DrawRectangle(new Rect(x, y, width, height), rotation, fillColor, strokeStyle);
		}
		
		public void DrawRectangle(Rect rect, Color fillColor){
			DrawRectangle(rect, 0f, fillColor, null);
		}

		public void DrawRectangle(Rect rect, StrokeStyle strokeStyle){
			DrawRectangle(rect, 0f, null, strokeStyle);
		}

		public void DrawRectangle(Rect rect, Color fillColor, StrokeStyle strokeStyle){
			DrawRectangle(rect, 0f, fillColor, strokeStyle);
		}

		public void DrawRectangle(Rect rect, float rotation=0f, Color? fillColor=null, StrokeStyle strokeStyle=null){
			Vector2 pivot = rect.center;
			Vector2[] points = new Vector2[4];

			if(rotation != 0f){
				rect.width *= aspectRatio;
			}

			points[0] = new Vector2(rect.min.x, rect.min.y);
			points[1] = new Vector2(rect.min.x, rect.max.y);
			points[2] = new Vector2(rect.max.x, rect.max.y);
			points[3] = new Vector2(rect.max.x, rect.min.y);

			if(rotation != 0f){
				rotation *= Mathf.Deg2Rad;

				float cos = Mathf.Cos(rotation);
				float sin = Mathf.Sin(rotation);
				float dx;
				float dy;
				for(int i=0; i<points.Length; i++){
					dx = points[i].x - rect.center.x;
					dy = points[i].y - rect.center.y;
					points[i] = new Vector2((dx * cos + dy * sin) / aspectRatio + pivot.x,
					                         dy * cos - dx * sin + pivot.y);
				}
			}

			if(fillColor != null){
				elements.Add(new PUIFillElement(points, new int[]{0, 1, 2, 2, 3, 0}, fillColor.Value));
			}

			if(strokeStyle != null){
				DrawPath(points, strokeStyle, true);
			}

			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawRectangle()

		#region DrawCircle()
		public void DrawCircle(Vector2 center, float radius, Color fillColor){
			DrawCircle(center, radius, 1f, 0f, 360f, fillColor, null);
		}

		public void DrawCircle(Vector2 center, float radius, StrokeStyle strokeStyle){
			DrawCircle(center, radius, 1f, 0f, 360f, Color.white, strokeStyle);
		}

		public void DrawCircle(Vector2 center, float radius, Color fillColor, StrokeStyle strokeStyle){
			DrawCircle(center, radius, 1f, 0f, 360f, fillColor, strokeStyle);
		}

		public void DrawCircle(Vector2 center, float radius, float stepSize=1f, float startAngle=0f, float endAngle=360f, Color? fillColor=null, StrokeStyle strokeStyle=null){
			if(endAngle - startAngle < 0f){
				Debug.LogWarning("DrawCircle() only works in the clockwise-direction; please ensure endAngle > startAngle.");
				return;
			}

			DrawEllipse(center, new Vector2(radius, radius * aspectRatio), stepSize, 0f, startAngle, endAngle, fillColor, strokeStyle);
		}
		#endregion DrawCircle()

		#region DrawEllipse()
		public void DrawEllipse(Vector2 center, Vector2 radii, Color fillColor){
			DrawEllipse(center, radii, 1f, 0f, 0f, 360f, fillColor, null);
		}

		public void DrawEllipse(Vector2 center, Vector2 radii, StrokeStyle strokeStyle){
			DrawEllipse(center, radii, 1f, 0f, 0f, 360f, null, strokeStyle);
		}

		public void DrawEllipse(Vector2 center, Vector2 radii, Color fillColor, StrokeStyle strokeStyle){
			DrawEllipse(center, radii, 1f, 0f, 0f, 360f, fillColor, strokeStyle);
		}

		public void DrawEllipse(Vector2 center, Vector2 radii, float stepSize=1f, float rotation=0f, float startAngle=0f, float endAngle=360f, Color? fillColor=null, StrokeStyle strokeStyle=null){
			if(endAngle - startAngle == 0f){
				return;
			}
			else if(endAngle - startAngle < 0f){
				Debug.LogWarning("DrawEllipse() only works in the clockwise-direction; please ensure endAngle > startAngle.");
				return;
			}

			Vector2 pivot = center;

			if(rotation != 0f){
				radii.x *= aspectRatio;
			}

			float correctionAngle = Mathf.PI * 0.5f - startAngle * Mathf.Deg2Rad; //rotate the shape 90 deg. ccw by default to align more naturally
			float angRad = Mathf.PI*2 / (360f / stepSize);
			
			int sides = Mathf.CeilToInt((endAngle - startAngle) / stepSize) + 1;

			List<Vector2> points = new List<Vector2>();
			points.Add(center);
			
			for(int i=0; i<sides; i++){
				points.Add(new Vector2(Mathf.Cos(correctionAngle - angRad * i) * radii.x + center.x,
				                       Mathf.Sin(correctionAngle - angRad * i) * radii.y + center.y));
			}
			
			if(rotation != 0f){
				rotation *= Mathf.Deg2Rad;

				float _aspectRatio = aspectRatio;
				float cos = Mathf.Cos(rotation);
				float sin = Mathf.Sin(rotation);
				float dx;
				float dy;
			
				for(int i=0; i<points.Count; i++){
					dx = points[i].x - center.x;
					dy = points[i].y - center.y;
					points[i] = new Vector2((dx * cos + dy * sin) / _aspectRatio + pivot.x,
					                         dy * cos - dx * sin + pivot.y);
				}
			}
			
			if(fillColor != null){
				int[] triangles;
				if(endAngle - startAngle == 0f){
					triangles = new int[(points.Count - 1) * 3];
					for(int i=0, j=1; i<triangles.Length; i+=3, j++){
						triangles[i] = 0;
						triangles[i+1] = j;
						triangles[i+2] = j+1;
					}
					triangles[triangles.Length-1] = 1;
				}
				else{
					triangles = new int[(points.Count - 2) * 3];
					for(int i=0, j=1; i<triangles.Length; i+=3, j++){
						triangles[i] = 0;
						triangles[i+1] = j;
						triangles[i+2] = j+1;
					}
				}
				elements.Add(new PUIFillElement(points.ToArray(), triangles, fillColor.Value));
			}
			
			if(strokeStyle != null){
				DrawPath(points.GetRange(1, points.Count-2).ToArray(), strokeStyle, true);
			}
			
			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawEllipse()

		#region DrawRegularSolid()
		public void DrawRegularSolid(Vector2 center, float radius, int sides, Color fillColor){
			DrawRegularSolid(center, radius, sides, 0f, fillColor, null);
		}

		public void DrawRegularSolid(Vector2 center, float radius, int sides, StrokeStyle strokeStyle){
			DrawRegularSolid(center, radius, sides, 0f, null, strokeStyle);
		}

		public void DrawRegularSolid(Vector2 center, float radius, int sides, Color fillColor, StrokeStyle strokeStyle){
			DrawRegularSolid(center, radius, sides, 0f, fillColor, strokeStyle);
		}

		public void DrawRegularSolid(Vector2 center, float radius, int sides, float rotation=0f, Color? fillColor=null, StrokeStyle strokeStyle=null){
			if(sides < 3){
				Debug.LogError("DrawRegularSolid() requires at least 3 sides.");
				return;
			}

			DrawCircle(center, radius, 360f / sides, rotation, 360f + rotation, fillColor, strokeStyle);
		}
		#endregion DrawRegularSolid()

		#region DrawIrregularSolid()
		public void DrawIrregularSolid(Vector2 center, float[] radii, Color fillColor){
			DrawIrregularSolid(center, radii, 0f, fillColor, null);
		}
		
		public void DrawIrregularSolid(Vector2 center, float[] radii, StrokeStyle strokeStyle){
			DrawIrregularSolid(center, radii, 0f, null, strokeStyle);
		}
		
		public void DrawIrregularSolid(Vector2 center, float[] radii, Color fillColor, StrokeStyle strokeStyle){
			DrawIrregularSolid(center, radii, 0f, fillColor, strokeStyle);
		}
		
		public void DrawIrregularSolid(Vector2 center, float[] radii, float rotation, Color? fillColor, StrokeStyle strokeStyle){
			int sides = radii.Length;

			if(sides < 3){
				Debug.LogError("DrawIrregularSolid() requires at least 3 radii.");
				return;
			}

			float correctionAngle = Mathf.PI * 0.5f - rotation * Mathf.Deg2Rad; //rotate the shape 90 deg. ccw by default to align more naturally
			float angRad = Mathf.PI*2 / sides;
			
			float _aspectRatio = aspectRatio;

			Vector2 evenRadius;

			List<Vector2> points = new List<Vector2>();
			points.Add(center);

			for(int i=0; i<sides; i++){
				evenRadius = new Vector2(radii[i], radii[i] * _aspectRatio);

				points.Add(new Vector2(Mathf.Cos(correctionAngle - angRad * i) * evenRadius.x + center.x,
				                       Mathf.Sin(correctionAngle - angRad * i) * evenRadius.y + center.y));
			}
			
			if(fillColor != null){
				int[] triangles = new int[(points.Count - 1) * 3];
				for(int i=0, j=1; i<triangles.Length; i += 3, j++){
					triangles[i] = 0;
					triangles[i+1] = j;
					triangles[i+2] = j+1;
				}
				triangles[triangles.Length-1] = 1;
				elements.Add(new PUIFillElement(points.ToArray(), triangles, fillColor.Value));
			}
			
			if(strokeStyle != null){
				DrawPath(points.GetRange(1, points.Count-2).ToArray(), strokeStyle, true);
			}

			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawIrregularSolid()

		#region DrawPolygon()
		public void DrawPolygon(Vector2[] points, Color fillColor){
			DrawPolygon(points, fillColor, null);
		}

		public void DrawPolygon(Vector2[] points, StrokeStyle strokeStyle){
			DrawPath(points, strokeStyle, true);
		}
		
		public void DrawPolygon(Vector2[] points, Color fillColor, StrokeStyle strokeStyle){
			if(points.Length < 3){
				Debug.LogError("DrawPolygon() requires at least 3 vertices");
				return;
			}
			else if(points.Length == 3){
				points = new Vector2[]{points[0], points[1], points[2]};
				elements.Add(new PUIFillElement(points, new int[]{0, 1, 2}, fillColor));
			}
			else if(points.Length == 4){
				elements.Add(new PUIFillElement(points, new int[]{0, 1, 2, 2, 3, 0}, fillColor));
			}
			else{
				//fill into tris
				int[] triangles = new int[(points.Length-2)*3];
				int currentTriangleIndex = 0;
				
				LinkedList<Vector2> allVertices = new LinkedList<Vector2>(points);
				List<LinkedListNode<Vector2>> ears = new List<LinkedListNode<Vector2>>();
				List<LinkedListNode<Vector2>> convexVertices = new List<LinkedListNode<Vector2>>();
				List<LinkedListNode<Vector2>> reflexVertices = new List<LinkedListNode<Vector2>>();
				
				LinkedListNode<Vector2> currentVertex = allVertices.First;
				LinkedListNode<Vector2> previousVertex;
				LinkedListNode<Vector2> nextVertex;
				LinkedListNode<Vector2> adjPreviousVertex; //adjacent
				LinkedListNode<Vector2> adjNextVertex; //adjacent
				LinkedListNode<Vector2> leftMostVertex = allVertices.First;
				
				//find left-most vertex to determine vertex order direction (CW or CCW)
				while(currentVertex.Next != null){
					currentVertex = currentVertex.Next;
					if(currentVertex.Value.x < leftMostVertex.Value.x){
						leftMostVertex = currentVertex;
					}
				}
				
				previousVertex = leftMostVertex.Previous ?? leftMostVertex.List.Last;
				nextVertex = leftMostVertex.Next ?? leftMostVertex.List.First;
				
				bool clockwise = nextVertex.Value.y > previousVertex.Value.y;
				
				//set up direction-specifics
				Func<Vector2, Vector2, Vector2 , bool> IsConvex; //prev, cur, next
				if(clockwise){
					IsConvex = (prev, cur, next) => {
						Vector2 edge1 = cur - prev;
						Vector2 edge2 = next - cur;
						return Vector2.Dot(new Vector2(-edge1.y, edge1.x), edge2) < 0; //right turn
					};
				}
				else{
					IsConvex = (prev, cur, next) => {
						Vector2 edge1 = cur - prev;
						Vector2 edge2 = next - cur;
						return Vector2.Dot(new Vector2(-edge1.y, edge1.x), edge2) > 0; //left turn
					};
				}
				
				//initialize initial convex, concave and ear lists
				currentVertex = allVertices.First;
				for(int i=0; i<allVertices.Count; i++){
					previousVertex = currentVertex.Previous ?? currentVertex.List.Last;
					nextVertex = currentVertex.Next ?? currentVertex.List.First;
					
					if(IsConvex(previousVertex.Value, currentVertex.Value, nextVertex.Value)){
						convexVertices.Add(currentVertex);
						
						float triArea = PUIUtils.GetTriangleArea(previousVertex.Value, currentVertex.Value, nextVertex.Value);
						bool isEar = true;
						foreach(LinkedListNode<Vector2> vert in reflexVertices){
							if(PUIUtils.PointInTriangle(vert.Value, previousVertex.Value, currentVertex.Value, nextVertex.Value, triArea)){
								isEar = false;
								break;
							}
						}
						if(isEar){
							ears.Add(currentVertex);
						}
					}
					else{
						reflexVertices.Add(currentVertex);
					}
					
					currentVertex = currentVertex.Next;
				}
				
				while(allVertices.Count > 3){
					currentVertex = ears[0];
					previousVertex = currentVertex.Previous ?? currentVertex.List.Last;
					nextVertex = currentVertex.Next ?? currentVertex.List.First;
					
					triangles[currentTriangleIndex] = Array.IndexOf(points, previousVertex.Value);
					triangles[currentTriangleIndex+1] = Array.IndexOf(points, currentVertex.Value);
					triangles[currentTriangleIndex+2] = Array.IndexOf(points, nextVertex.Value);
					currentTriangleIndex += 3;
					
					ears.Remove(currentVertex);
					allVertices.Remove(currentVertex);
					
					LinkedListNode<Vector2>[] adjacentVerts = new LinkedListNode<Vector2>[]{previousVertex, nextVertex};
					foreach(LinkedListNode<Vector2> adjacentVert in adjacentVerts){
						adjPreviousVertex = adjacentVert.Previous ?? adjacentVert.List.Last;
						adjNextVertex = adjacentVert.Next ?? adjacentVert.List.First;
						
						if(IsConvex(adjPreviousVertex.Value, adjacentVert.Value, adjNextVertex.Value)){
							if(reflexVertices.Contains(adjacentVert)){
								reflexVertices.Remove(adjacentVert);
								convexVertices.Add(adjacentVert);
							}
							
							//check earness
							float triArea = PUIUtils.GetTriangleArea(adjPreviousVertex.Value, adjacentVert.Value, adjNextVertex.Value);
							bool isEar = true;
							foreach(LinkedListNode<Vector2> vert in reflexVertices){
								if(PUIUtils.PointInTriangle(vert.Value, adjPreviousVertex.Value, adjacentVert.Value, adjNextVertex.Value, triArea)){
									isEar = false;
									break;
								}
							}
							if(isEar && !ears.Contains(adjacentVert)){
								ears.Add(adjacentVert);
							}
							else if(!isEar && ears.Contains(adjacentVert)){
								ears.Remove(adjacentVert);
							}
						}
						else{ //reflex
							convexVertices.Remove(adjacentVert);
							reflexVertices.Add(adjacentVert);
						}
					}
				}
				
				triangles[currentTriangleIndex] = Array.IndexOf(points, allVertices.First.Value);
				triangles[currentTriangleIndex+1] = Array.IndexOf(points, allVertices.First.Next.Value);
				triangles[currentTriangleIndex+2] = Array.IndexOf(points, allVertices.First.Next.Next.Value);
				
				elements.Add(new PUIFillElement(points, triangles, fillColor));
			}
			
			if(strokeStyle != null){
				DrawPath(points, strokeStyle, true);
			}
			
			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawPolygon()
		
		#region DrawRawMesh()
		public void DrawRawMesh(Vector2[] points, int[] triangles, Color fillColor){
			elements.Add(new PUIFillElement(points, triangles, fillColor));
			
			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawRawMesh()

		#region DrawLine()
		public void DrawLine(Vector2 point1, Vector2 point2){
			DrawLine(point1, point2, StrokeStyle.defaultStrokeStyle);
		}

		public void DrawLine(Vector2 point1, Vector2 point2, StrokeStyle strokeStyle){
			float relativeThickness = strokeStyle.scaleMode == StrokeScaleMode.Absolute ?
									  strokeStyle.thickness / rectTransform.rect.width :
									  strokeStyle.thickness;

			elements.Add(new PUIStrokeElement(new Vector2[]{point1, point2},
											  PUIUtils.GetLinePoints(point1, point2, relativeThickness, aspectRatio),
											  strokeStyle,
											  false));
			
			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawLine()

		#region DrawPath()
		public void DrawPath(Vector2[] points){
			DrawPath(points, StrokeStyle.defaultStrokeStyle, false);
		}

		public void DrawPath(Vector2[] points, StrokeStyle strokeStyle){
			DrawPath(points, strokeStyle, false);
		}

		public void DrawPath(Vector2[] points, StrokeStyle strokeStyle, bool closePath){
			if(points.Length < 2){
				Debug.LogError("DrawPath() needs at least two points to draw");
				return;
			}
			else if(points.Length == 2){
				DrawLine(points[0], points[1], strokeStyle);
				if(closePath){
					Debug.LogWarning("DrawPath() can't close a path with only two points. 'closePath' parameter ignored.");
				}
				return;
			}

			float relativeThickness = strokeStyle.scaleMode == StrokeScaleMode.Absolute ?
									  strokeStyle.thickness / rectTransform.rect.width :
									  strokeStyle.thickness;
//			Debug.Log(String.Join(", ", PUIUtils.GetPathPoints(points, closePath, relativeThickness, aspectRatio).Select(x => x.ToString("f5")).ToArray()));
			elements.Add(new PUIStrokeElement(points,
			                                  PUIUtils.GetPathPoints(points, closePath, relativeThickness, aspectRatio),
			                                  strokeStyle,
			                                  closePath));
			
			if(setDirtyOnDraw){
				SetVerticesDirty();
			}
		}
		#endregion DrawPath()
	
		#region Child Classes
		abstract class PUIElement {
			protected Color32 color;
			protected Vector2[] points;
			protected UIVertex[] uiVerts;
			protected List<UIVertex> uiVertexTriangleStream;

			public abstract List<UIVertex> GetUIVertexTriangleStream(Vector2 offset, Vector2 scale, Color32 color);
		}
		
		class PUIFillElement : PUIElement {
			int[] triangles;

			public PUIFillElement(Vector2[] points, int[] triangles, Color32 color){
				uiVerts = new UIVertex[points.Length];
				uiVertexTriangleStream = new List<UIVertex>(triangles.Length);

				this.points = points;
				this.triangles = triangles;
				this.color = color;

				for(int i=0; i<uiVerts.Length; i++){
					UIVertex vert = UIVertex.simpleVert;
					vert.color = color;
					uiVerts[i] = vert;
				}
			}

			public override List<UIVertex> GetUIVertexTriangleStream(Vector2 offset, Vector2 scale, Color32 color){
				uiVertexTriangleStream.Clear();

				color = (Color)color * (Color)this.color;
				
				if(color.Equals(uiVerts[0].color)){ //no change in color, skip setting it
					for(int i=0; i<uiVerts.Length; i++){
						uiVerts[i].position = new Vector3((points[i].x + offset.x) * scale.x,
						                                  (points[i].y + offset.y) * scale.y,
						                                  0f);
					}
				}
				else{
					for(int i=0; i<uiVerts.Length; i++){
						uiVerts[i].color = color;
						uiVerts[i].position = new Vector3((points[i].x + offset.x) * scale.x,
						                                  (points[i].y + offset.y) * scale.y,
						                                  0f);
					}
				}
				
				for(int i=0; i<triangles.Length; i++){
					uiVertexTriangleStream.Add(uiVerts[triangles[i]]);
				}

				return uiVertexTriangleStream;
			}
		}
		
		class PUIStrokeElement : PUIElement {
			public Vector2[] rawPoints{get; private set;} //path the stroke tesselates		
			public StrokeStyle strokeStyle{get; private set;}
			public bool isClosedPath{get; private set;}

			public PUIStrokeElement(Vector2[] rawPoints, Vector2[] points, StrokeStyle strokeStyle, bool isClosedPath){
				uiVerts = new UIVertex[points.Length];
				uiVertexTriangleStream = new List<UIVertex>((points.Length-1)*6);

				this.rawPoints = rawPoints;
				this.strokeStyle = strokeStyle;
				this.isClosedPath = isClosedPath;
				this.color = strokeStyle.color;

				for(int i=0; i<uiVerts.Length; i++){
					UIVertex vert = UIVertex.simpleVert;
					vert.color = color;
					uiVerts[i] = vert;
				}
				
				UpdatePoints(points);
			}
			
			public void UpdatePoints(Vector2[] newPoints){
				this.points = newPoints;
			}

			public override List<UIVertex> GetUIVertexTriangleStream(Vector2 offset, Vector2 scale, Color32 color){
				uiVertexTriangleStream.Clear();
				
				color = (Color)color * (Color)this.color;
				
				if(color.Equals(uiVerts[0].color)){ //no change in color, skip setting it
					for(int i=0; i<uiVerts.Length; i++){
						uiVerts[i].position = new Vector3((points[i].x + offset.x) * scale.x,
						                                  (points[i].y + offset.y) * scale.y,
						                                  0f);
					}
				}
				else{
					for(int i=0; i<uiVerts.Length; i++){
						uiVerts[i].color = color;
						uiVerts[i].position = new Vector3((points[i].x + offset.x) * scale.x,
						                                  (points[i].y + offset.y) * scale.y,
						                                  0f);
					}
				}
				
				for(int i=0; i<uiVerts.Length; i+=4){
					uiVertexTriangleStream.Add(uiVerts[i]);
					uiVertexTriangleStream.Add(uiVerts[i+1]);
					uiVertexTriangleStream.Add(uiVerts[i+2]);
					uiVertexTriangleStream.Add(uiVerts[i+2]);
					uiVertexTriangleStream.Add(uiVerts[i+3]);
					uiVertexTriangleStream.Add(uiVerts[i]);
				}
				
				return uiVertexTriangleStream;
			}
		}

		class PUIUtils {
			public static float Cross2D(Vector2 lhs, Vector2 rhs){
				return lhs.x * rhs.y - lhs.y * rhs.x;
			}
			
			public static float GetTriangleArea(Vector2 tri0, Vector2 tri1, Vector2 tri2){
				return Mathf.Abs(-tri1.y * tri2.x + tri0.y * (-tri1.x + tri2.x) + tri0.x * (tri1.y - tri2.y) + tri1.x * tri2.y);
			}
			
			public static bool PointInTriangle(Vector2 point, Vector2 tri0, Vector2 tri1, Vector2 tri2, float triAarea){
				float s = (tri0.y * tri2.x - tri0.x * tri2.y + (tri2.y - tri0.y) * point.x + (tri0.x - tri2.x) * point.y);
				float t = (tri0.x * tri1.y - tri0.y * tri1.x + (tri0.y - tri1.y) * point.x + (tri1.x - tri0.x) * point.y);
				
				if (s <= 0f || t <= 0f)
					return false;
				
				return s + t < triAarea;
			}
			
			public static Vector2? GetLineIntersection(Vector2 line1P1, Vector2 line1P2, Vector2 line2P1, Vector2 line2P2){
				//Adapted from http://www.codeproject.com/Articles/226569/Drawing-polylines-by-tessellation
				//Original by Paul Bourke: http://paulbourke.net/geometry/pointlineplane/#i2l
				
				float denomenator = (line2P2.y - line2P1.y) * (line1P2.x - line1P1.x) - (line2P2.x - line2P1.x) * (line1P2.y - line1P1.y);
				float numerator1 = (line2P2.x - line2P1.x) * (line1P1.y - line2P1.y) - (line2P2.y - line2P1.y) * (line1P1.x - line2P1.x);
				float numerator2 = (line1P2.x - line1P1.x) * (line1P1.y - line2P1.y) - (line1P2.y - line1P1.y) * (line1P1.x - line2P1.x);
				
				if(Mathf.Abs(numerator1) < Mathf.Epsilon && Mathf.Abs(numerator2) < Mathf.Epsilon && Mathf.Abs(denomenator) < Mathf.Epsilon){
					return new Vector2((line1P1.x + line1P2.x) * 0.5f, (line1P1.y + line1P2.y) * 0.5f); //Houston, we have a collision!
				}
				
				if(Mathf.Abs(denomenator) < Mathf.Epsilon){
					return null; //parallel
				}
				
				float decimal1 = numerator1 / denomenator;
				
				return new Vector2(line1P1.x + decimal1 * (line1P2.x - line1P1.x), line1P1.y + decimal1 * (line1P2.y - line1P1.y));
			}
			
			public static Vector2[] GetLinePoints(Vector2 point1, Vector2 point2, float strokeThickness, float aspectRatio){
				Vector2 evenThickness = new Vector2(strokeThickness, strokeThickness * aspectRatio);
				
				Vector2 dir = (point2 - point1).normalized;
				Vector2 orthogonalOffset = new Vector2(-dir.y * evenThickness.x, dir.x * evenThickness.y);
				
				Vector2[] points = new Vector2[4];
				points[0] = point1 - orthogonalOffset;
				points[1] = point1 + orthogonalOffset;
				points[2] = point2 + orthogonalOffset;
				points[3] = point2 - orthogonalOffset;
				return points;
			}
			
			public static Vector2[] GetPathPoints(Vector2[] points, bool closePath, float strokeThickness, float aspectRatio){
				//	For-loop points map:
				//	count - 8	inner 1 start		count - 5	inner 1 end
				//	count - 1	inner 2 start		count - 4	inner 2 end
				//	count - 7	outer 1 start		count - 6	outer 1 end
				//	count - 3	outer 2 start		count - 2	outer 2 end
				
				Vector2 evenThickness = new Vector2(strokeThickness, strokeThickness * aspectRatio);
				
				Vector2 dir = (points[1] - points[0]).normalized;
				Vector2 orthogonalOffset = new Vector2(-dir.y * evenThickness.x, dir.x * evenThickness.y);
				Vector2? intersectionInner;
				Vector2? intersectionOuter;
				Vector2 intersection;
				
				List<Vector2> _points = new List<Vector2>();
				_points.Add(points[0] - orthogonalOffset);
				_points.Add(points[0] + orthogonalOffset);
				_points.Add(points[1] + orthogonalOffset);
				_points.Add(points[1] - orthogonalOffset);
				
				for(int i=1; i<points.Length - 1; i++){
					dir = (points[i+1] - points[i]).normalized;
					orthogonalOffset = new Vector2(-dir.y * evenThickness.x, dir.x * evenThickness.y);
					
					_points.Add(points[i] - orthogonalOffset);
					_points.Add(points[i] + orthogonalOffset);
					_points.Add(points[i+1] + orthogonalOffset);
					_points.Add(points[i+1] - orthogonalOffset);
					
					intersectionInner = PUIUtils.GetLineIntersection(_points[_points.Count-8], _points[_points.Count-5],
					                                                 _points[_points.Count-4], _points[_points.Count-1]);
					intersectionOuter = PUIUtils.GetLineIntersection(_points[_points.Count-7], _points[_points.Count-6],
					                                                 _points[_points.Count-3], _points[_points.Count-2]);
					
					if(intersectionInner != null){
						intersection = intersectionInner.Value;
						_points[_points.Count-5] = _points[_points.Count-4] = intersection;
					}
					
					if(intersectionOuter != null){
						intersection = intersectionOuter.Value;

						_points[_points.Count-6] = _points[_points.Count-3] = intersection;
					}
				}
				
				if(closePath){
					dir = (points[points.Length-1] - points[0]).normalized;
					orthogonalOffset = new Vector2(-dir.y * evenThickness.x, dir.x * evenThickness.y);
					
					_points.Add(points[points.Length-1] - orthogonalOffset);
					_points.Add(points[points.Length-1] + orthogonalOffset);
					_points.Add(points[0] + orthogonalOffset);
					_points.Add(points[0] - orthogonalOffset);
					
					//last -> closing
					intersectionInner = PUIUtils.GetLineIntersection(_points[_points.Count-8], _points[_points.Count-5],
					                                                 _points[_points.Count-3], _points[_points.Count-2]);
					intersectionOuter = PUIUtils.GetLineIntersection(_points[_points.Count-7], _points[_points.Count-6],
					                                                 _points[_points.Count-4], _points[_points.Count-1]);

					if(intersectionInner != null){
						intersection = intersectionInner.Value;
						_points[_points.Count-5] = _points[_points.Count-3] = intersection;
					}
//					
					if(intersectionOuter != null){
						intersection = intersectionOuter.Value;
						_points[_points.Count-6] = _points[_points.Count-4] = intersection;
					}

					//closing -> first
					intersectionInner = PUIUtils.GetLineIntersection(_points[3], _points[0],
					                                                 _points[_points.Count-3], _points[_points.Count-2]);
					intersectionOuter = PUIUtils.GetLineIntersection(_points[2], _points[1],
					                                                 _points[_points.Count-4], _points[_points.Count-1]);
					
					if(intersectionInner != null){
						intersection = intersectionInner.Value;
						_points[0] = _points[_points.Count-2] = intersection;
					}
					
					if(intersectionOuter != null){
						intersection = intersectionOuter.Value;
						_points[1] = _points[_points.Count-1] = intersection;
					}
				}
				
				return _points.ToArray();
			}
		}
		#endregion Child Classes
	}

	public enum StrokeScaleMode {Relative, Absolute}

	public class StrokeStyle {
		public Color color;
		public float thickness;
		public StrokeScaleMode scaleMode;

		static StrokeStyle defaultStrokeStyleInstance;

		public static StrokeStyle defaultStrokeStyle{
			get{
				if(defaultStrokeStyleInstance == null){
					defaultStrokeStyleInstance = new StrokeStyle(Color.white, 0.04f, StrokeScaleMode.Relative);
				}
				return defaultStrokeStyleInstance;
			}
		}

		public StrokeStyle(Color color, float thickness, StrokeScaleMode scaleMode){
			this.color = color;
			this.thickness = thickness;
			this.scaleMode = scaleMode;
		}
	}
}