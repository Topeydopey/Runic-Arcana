using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GestureRecognizer
{

	/// <summary>
	/// Captures player drawing and call the Recognizer to discover which gesture player id.
	/// Calls 'OnRecognize' event when something is recognized.
	/// </summary>
	public class DrawDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
	{

		public Recognizer recognizer;

		public UILineRenderer line;
		private List<UILineRenderer> lines;

		[Range(0f, 1f)]
		public float scoreToAccept = 0.8f;

		[Range(1, 10)]
		public int minLines = 1;
		public int MinLines { set { minLines = Mathf.Clamp(value, 1, 10); } }

		[Range(1, 10)]
		public int maxLines = 2;
		public int MaxLines { set { maxLines = Mathf.Clamp(value, 1, 10); } }

		public enum RemoveStrategy { RemoveOld, ClearAll }
		public RemoveStrategy removeStrategy;

		public bool clearNotRecognizedLines;

		public bool fixedArea = false;

		GestureData data = new GestureData();

		[System.Serializable]
		public class ResultEvent : UnityEvent<RecognitionResult> { }
		public ResultEvent OnRecognize;

		RectTransform rectTransform;


		void Start()
		{
			line.relativeSize = true;
			line.LineList = false;
			lines = new List<UILineRenderer>() { line };
			rectTransform = transform as RectTransform;
			UpdateLines();
		}

		void OnValidate()
		{
			maxLines = Mathf.Max(minLines, maxLines);
		}

		public void UpdateLines()
		{
			while (lines.Count < data.lines.Count)
			{
				var newLine = Instantiate(line, line.transform.parent);
				lines.Add(newLine);
			}
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].Points = new Vector2[] { };
				lines[i].SetAllDirty();
			}
			int n = Mathf.Min(lines.Count, data.lines.Count);
			for (int i = 0; i < n; i++)
			{
				lines[i].Points = data.lines[i].points.Select(p => RealToLine(p)).ToArray();
				lines[i].SetAllDirty();
			}
		}

		Vector2 RealToLine(Vector2 position)
		{
			var local = rectTransform.InverseTransformPoint(position);
			var normalized = Rect.PointToNormalized(rectTransform.rect, local);
			return normalized;
		}

		Vector2 FixedPosition(Vector2 position)
		{
			return position;
			//var local = rectTransform.InverseTransformPoint (position);
			//var normalized = Rect.PointToNormalized (rectTransform.rect, local);
			//return normalized;
		}

		public void ResetDrawingState()
		{
			// Clear the lines from the drawing area
			ClearLines();
		}

		public void ClearLines()
		{
			data.lines.Clear();
			UpdateLines();
		}

		public void OnPointerClick(PointerEventData eventData)
		{

		}
		// This variable will keep track of the time since the last stroke was drawn
		private float timeSinceLastStroke = 0f;

		// The delay after which the drawing should disappear
		public float completionDelay = 2f;

		// Whether the player is currently drawing
		private bool isDrawing = false;

		void Update()
		{
			// If the player isn't drawing, keep updating the time since the last stroke
			if (!isDrawing)
			{
				timeSinceLastStroke += Time.unscaledDeltaTime;
			}
		}
		public void OnBeginDrag(PointerEventData eventData)
		{
			// The player has started drawing
			isDrawing = true;
			// Reset the time since the last stroke
			timeSinceLastStroke = 0f;
			if (data.lines.Count >= maxLines)
			{
				switch (removeStrategy)
				{
					case RemoveStrategy.RemoveOld:
						data.lines.RemoveAt(0);
						break;
					case RemoveStrategy.ClearAll:
						data.lines.Clear();
						break;
				}
			}

			data.lines.Add(new GestureLine());

			var fixedPos = FixedPosition(eventData.position);
			if (data.LastLine.points.Count == 0 || data.LastLine.points.Last() != fixedPos)
			{
				data.LastLine.points.Add(fixedPos);
				UpdateLines();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			var fixedPos = FixedPosition(eventData.position);
			if (data.LastLine.points.Count == 0 || data.LastLine.points.Last() != fixedPos)
			{
				data.LastLine.points.Add(fixedPos);
				UpdateLines();
			}
		}



		public void OnEndDrag(PointerEventData eventData)
		{
			isDrawing = false; // Player has stopped drawing
			StartCoroutine(OnEndDragCoroutine(eventData));
			// Start the coroutine to clear the drawing after a delay
			StartCoroutine(ClearDrawingAfterDelay());
		}
		private IEnumerator ClearDrawingAfterDelay()
		{
			float elapsed = 0f;
			while (elapsed < completionDelay)
			{
				elapsed += Time.unscaledDeltaTime;
				yield return null;
			}

			if (timeSinceLastStroke >= completionDelay)
			{
				ClearLines();  // Clears the current lines on the drawing UI
			}
		}

		IEnumerator OnEndDragCoroutine(PointerEventData eventData)
		{
			data.LastLine.points.Add(FixedPosition(eventData.position));
			UpdateLines();

			// Introduce a delay before recognition to prevent premature recognition using unscaled time
			float recognitionDelay = 0.7f;  // Delay before starting recognition
			float elapsedTime = 0f;       // Timer to track the elapsed time using unscaled time

			while (elapsedTime < recognitionDelay)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			// Proceed with the recognition process after the delay
			for (int size = data.lines.Count; size >= 1 && size >= minLines; size--)
			{
				// Extract the last 'size' number of lines for recognition
				var sizedData = new GestureData()
				{
					lines = data.lines.GetRange(data.lines.Count - size, size)
				};

				var sizedNormalizedData = sizedData;

				// Normalize data if required
				if (fixedArea)
				{
					var rect = this.rectTransform.rect;
					sizedNormalizedData = new GestureData()
					{
						lines = sizedData.lines.Select(line => new GestureLine()
						{
							closedLine = line.closedLine,
							points = line.points.Select(p => Rect.PointToNormalized(rect, this.rectTransform.InverseTransformPoint(p))).ToList()
						}).ToList()
					};
				}

				RecognitionResult result = null;

				// Run recognition in another thread to not block the main Unity thread
				var thread = new System.Threading.Thread(() =>
				{
					result = recognizer.Recognize(sizedNormalizedData, normalizeScale: !fixedArea);
				});
				thread.Start();
				while (thread.IsAlive)
				{
					yield return null;
				}

				// Invoke recognition result event and handle spell casting if the gesture is recognized
				if (result != null && result.gesture != null && result.score.score >= scoreToAccept)
				{
					OnRecognize.Invoke(result);

					// Invoke spell casting based on the recognized gesture
					var spellManager = FindObjectOfType<SpellManager>(); // Find the SpellManager in the scene
					if (spellManager != null)
					{
						spellManager.CastSpell(result.gesture.id); // Cast the spell using the gesture ID
					}
					if (clearNotRecognizedLines)
					{
						data = sizedData;
						UpdateLines();
					}
					break;
				}
				else
				{
					OnRecognize.Invoke(RecognitionResult.Empty);
				}
			}
		}
	}
}
