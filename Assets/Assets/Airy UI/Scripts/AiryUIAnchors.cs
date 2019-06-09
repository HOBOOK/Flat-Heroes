using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AiryUIAnchors : MonoBehaviour
{
    private static float parentRectWidth, parentRectHeight;

    private static RectTransform rectTransform;

    private static float thisRectWidth, thisRectHeight;

    private static Vector3 rectInitialPosition;
    private static Vector2 minAnchors;
    private static Vector2 maxAnchors;
    private static Vector2 rectPositionRelativeToParent;
    private static Vector2 initialPivot;

    private static float rectWidthRatio;
    private static float rectHeightRatio;

    private static void SetInitialValues()
    {
        // Initial Position, because we will reset the position later.
        rectInitialPosition = rectTransform.localPosition;
        initialPivot = rectTransform.pivot;

        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Screen dimensions.
        // 15-12-2018 We do not use Screen.width and Screen.height here, because these variables are only set correctly in play mode, in edit mode, they return the size of the game window, it's a litte bit tricky -_- .
        // 16-12-2018 We do not use screen dimensions anymore. Instead we use the parent rect's dimensions.

        //string[] resolution = UnityEditor.UnityStats.screenRes.Split('x');
        //screenWidth = int.Parse(resolution[0]);
        //screenHeight = int.Parse(resolution[1]);

        parentRectWidth = rectTransform.parent.GetComponent<RectTransform>().rect.width;
        parentRectHeight = rectTransform.parent.GetComponent<RectTransform>().rect.height;

        // Rect dimensions.
        thisRectWidth = rectTransform.rect.width;
        thisRectHeight = rectTransform.rect.height;

        // Relative position, width, and height. We add 0.5 to X and Y coordinates becuase the center in local space is (0, 0) while the center in world space (0.5, 0.5).
        rectPositionRelativeToParent = new Vector2((rectTransform.localPosition.x / parentRectWidth) + 0.5f, (rectTransform.localPosition.y / parentRectHeight) + 0.5f);

        // rectWidthRatio and rectHeightRatio return the ratio of the current rect's dimensions relative to screen. e.g if parent rect's width = 400 and current rect's width = 100, then rectWidthRatio will be 0.25 
        rectWidthRatio = thisRectWidth / parentRectWidth;
        rectHeightRatio = thisRectHeight / parentRectHeight;

        float minX = rectPositionRelativeToParent.x - (rectWidthRatio / 2);
        float minY = rectPositionRelativeToParent.y - (rectHeightRatio / 2);
        float maxX = rectPositionRelativeToParent.x + (rectWidthRatio / 2);
        float maxY = rectPositionRelativeToParent.y + (rectHeightRatio / 2);

        minAnchors = new Vector2(minX, minY);
        maxAnchors = new Vector2(maxX, maxY);
    }

    public static void SetAnchorsToRect(RectTransform rect)
    {
        // Done in 17-12-2018  ----------- It took too much time.
        // First, We will get the current position, width, and height of the rect transform, because we will reset these values after setting the anchor.
        // That's because when the anchors positions change, they autmatically change the coordinates of the rect transform.
        // Second, we will get the width and height of the parent rect transform.
        // Second, we will get the width and height of this rect transform.
        // Third, we have to find the rect transform's position in relativity with parent width, and height. (rectTransform.position.x / parent.width)

        rectTransform = rect;

        // Here we calculate the min and max anchors.
        SetInitialValues();

        // And finally setting the anchors.
        rectTransform.anchorMin = minAnchors;
        rectTransform.anchorMax = maxAnchors;

        // Resetting the rect to its initial position, width, and height.
        rectTransform.pivot = initialPivot;
        rectTransform.localPosition = rectInitialPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisRectHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisRectWidth);
    }

    public static void SetAnchorsCenterOfRect(RectTransform rect)
    {
        rectTransform = rect;
        initialPivot = rectTransform.pivot;

        SetInitialValues();

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // Resetting the rect to its initial position, width, and height.
        rectTransform.pivot = initialPivot;
        rectTransform.localPosition = rectInitialPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisRectHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisRectWidth);
    }

    public static void SetAnchorsTopRight(RectTransform rect)
    {
        rectTransform = rect;
        initialPivot = rectTransform.pivot;

        SetInitialValues();

        rectTransform.anchorMin = Vector2.one;
        rectTransform.anchorMax = Vector2.one;

        // Resetting the rect to its initial position, width, and height.
        rectTransform.pivot = initialPivot;
        rectTransform.localPosition = rectInitialPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisRectHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisRectWidth);
    }

    public static void SetAnchorsTopLeft(RectTransform rect)
    {
        rectTransform = rect;
        initialPivot = rectTransform.pivot;

        SetInitialValues();

        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);

        // Resetting the rect to its initial position, width, and height.
        rectTransform.pivot = initialPivot;
        rectTransform.localPosition = rectInitialPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisRectHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisRectWidth);
    }

    public static void SetAnchorsBottomRight(RectTransform rect)
    {
        rectTransform = rect;
        initialPivot = rectTransform.pivot;

        SetInitialValues();

        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);

        // Resetting the rect to its initial position, width, and height.
        rectTransform.pivot = initialPivot;
        rectTransform.localPosition = rectInitialPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisRectHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisRectWidth);
    }

    public static void SetAnchorsBottomLeft(RectTransform rect)
    {
        rectTransform = rect;
        initialPivot = rectTransform.pivot;

        SetInitialValues();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;

        // Resetting the rect to its initial position, width, and height.
        rectTransform.pivot = initialPivot;
        rectTransform.localPosition = rectInitialPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisRectHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisRectWidth);
    }

    public static void SetRectToAnchor(RectTransform rect)
    {
        rectTransform = rect;
        initialPivot = rectTransform.pivot;

        rectTransform.pivot = initialPivot;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    #region Old
    private void SetInitialValues_OLD()
    {
        // (anchor in center of screen) = 0.5, 0.5
        // (center of rect for a value between 0 and 1) = (rect.position.x / screenWidth, rect.position.y / screenHeight).
        // To move the xMin and xMax, we move xMin by xC / 2 to the left, And move xMax by xC / 2 to the right away from the center.
        // To move the yMin and yMax, we move yMin by yC / 2 up, And move yMax by xC / 2 down away from the center.

        //float factor = 0.0000001f;
        //screenHeight = Screen.currentResolution.height;
        //screenWidth = Screen.currentResolution.width;

        //float heightRatio = screenHeight / screenWidth;
        //float widthRatio = screenWidth / screenHeight;

        //initialPosition = rectTransform.position;
        //Rect rectCoordinatess = rectTransform.rect;

        //rectwidth = rectTransform.rect.width;
        //rectheight = rectTransform.rect.height;

        //float diagonal = Mathf.Sqrt(Mathf.Pow(rectheight, 2) + Mathf.Pow(rectwidth, 2));

        //float xC = (rectwidth / 2) * (Screen.width * factor);
        //float yC = (rectheight / 2) * (Screen.height * factor);
    }
    #endregion

}