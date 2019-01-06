#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class AvatarEditor : MonoBehaviour
{
    public RectTransform avatar;

    public TMP_InputField inputField;

    public Camera camera;

    public void TakeScreenshot()
    {
        StartCoroutine("SaveScreenshot");
    }

    public IEnumerator SaveScreenshot()
    {
        int width = System.Convert.ToInt32(avatar.rect.width);
        int height = System.Convert.ToInt32(avatar.rect.height);

        yield return new WaitForEndOfFrame();

        Vector2 temp = avatar.transform.position;
        var startX = temp.x - width / 2;
        var startY = temp.y - height / 2;

        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToPNG();
        Destroy(tex);

        if (string.IsNullOrEmpty(inputField.text))
        {
            print("!!!!!!!! NAME FIELD EMPTY !!!!!!!!!!!");
        }
        else
        {
            string file = inputField.text;
            string path = Application.dataPath + "/Resources/Avatars/" + file + ".png";
            File.WriteAllBytes(path, bytes);
            print("SAVED TO : " + path);
            //AssetDatabase.Refresh();
        }
    }
}
#endif
