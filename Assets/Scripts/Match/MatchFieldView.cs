using UnityEngine;
using UnityEngine.UI;

public class MatchFieldView : MonoBehaviour
{
	[SerializeField] private GameObject[] fieldAreas = null;
	[SerializeField] private RectTransform ball = null;

	private Vector3 ballPosition;

	private void Start()
    {
        ballPosition = fieldAreas[15].transform.position;
    }

    public void UpdateFieldArea(int _area)
    {
        for(int i = 0; i < 31; i++)
        {
            GameObject obj = fieldAreas[i];

            if (i == _area)
            {
                Image img = obj.GetComponent<Image>();
                Color col = img.color;
                img.color = new Color(col.r, col.g, col.b, col.a + (1f / 20f));
                ballPosition = obj.transform.position;
            }
        }        
    }

    public void ResetField()
    {
        ballPosition = fieldAreas[15].transform.position;
        foreach(GameObject obj in fieldAreas)
        {
            Image img = obj.GetComponent<Image>();
            Color col = img.color;
            img.color = new Color(col.r, col.g, col.b, 0f);
        }
    }

    private void Update()
    {
        if (ball.position != ballPosition) ball.position = Vector3.Lerp(ball.position, ballPosition, Time.deltaTime*2);
    }
}
