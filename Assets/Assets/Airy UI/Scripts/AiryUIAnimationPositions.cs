using UnityEngine;
using System.Collections;

public class AiryUIAnimationPositions : MonoBehaviour
{
    /// <summary>
    /// Used for initializing the position from a certian corner.
    /// </summary>
    /// <param name="initialPosition">the start position of the transform</param>
    /// <param name="rect">the rect transform</param>
    /// <param name="animationFromCornerType">BottomRight, TopRight, BottomLeft, TopLeft, Up, Bottom, Left, Or Right</param>
    /// <param name="animationFromCornerStartFromType">Screen or Rect</param>
    /// <returns></returns>
    public static Vector3 GetStartPositionFromCorner
        (Vector3 initialPosition, RectTransform rect, AnimationStartPosition animationFromCornerType, AnimationFromCornerStartFromType animationFromCornerStartFromType)
    {
        float startPositionX = 0;
        float startPositionY = 0;

        if (animationFromCornerStartFromType == AnimationFromCornerStartFromType.ScreenCorner)
        {
            switch (animationFromCornerType)
            {
                case (AnimationStartPosition.BottomRight):
                    startPositionX = Screen.width + (rect.rect.width / 2);
                    startPositionY = 0 - (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.BottomLeft):
                    startPositionX = 0 - (rect.rect.width / 2);
                    startPositionY = 0 - (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.TopRight):
                    startPositionX = Screen.width + (rect.rect.width / 2);
                    startPositionY = Screen.height + (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.TopLeft):
                    startPositionX = 0 - (rect.rect.width / 2);
                    startPositionY = Screen.height + (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.Top):
                    startPositionX = initialPosition.x;
                    startPositionY = Screen.height + (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.Bottom):
                    startPositionX = initialPosition.x;
                    startPositionY = 0 - (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.Left):
                    startPositionX = 0 - (rect.rect.width / 2);
                    startPositionY = initialPosition.y;
                    break;
                case (AnimationStartPosition.Right):
                    startPositionX = Screen.width + (rect.rect.width / 2);
                    startPositionY = initialPosition.y;
                    break;
            }
        }
        else if (animationFromCornerStartFromType == AnimationFromCornerStartFromType.ParentRectCorner)
        {
            switch (animationFromCornerType)
            {
                case (AnimationStartPosition.Top):
                    startPositionX = initialPosition.x;
                    startPositionY = rect.parent.position.y + (rect.parent.GetComponent<RectTransform>().rect.height / 2) + (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.Bottom):
                    startPositionX = initialPosition.x;
                    startPositionY = rect.parent.position.y - (rect.parent.GetComponent<RectTransform>().rect.height / 2) - (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.Left):
                    startPositionX = rect.parent.position.x - (rect.parent.GetComponent<RectTransform>().rect.width / 2) - (rect.rect.width / 2);
                    startPositionY = initialPosition.y;
                    break;
                case (AnimationStartPosition.Right):
                    startPositionX = rect.parent.position.x + (rect.parent.GetComponent<RectTransform>().rect.width / 2) + (rect.rect.width / 2);
                    startPositionY = initialPosition.y;
                    break;
                case (AnimationStartPosition.BottomRight):
                    startPositionX = rect.parent.position.x + (rect.parent.GetComponent<RectTransform>().rect.width / 2) + (rect.rect.width / 2);
                    startPositionY = rect.parent.position.y - (rect.parent.GetComponent<RectTransform>().rect.height / 2) - (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.BottomLeft):
                    startPositionX = rect.parent.position.x - (rect.parent.GetComponent<RectTransform>().rect.width / 2) - (rect.rect.width / 2);
                    startPositionY = rect.parent.position.y - (rect.parent.GetComponent<RectTransform>().rect.height / 2) - (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.TopRight):
                    startPositionX = rect.parent.position.x + (rect.parent.GetComponent<RectTransform>().rect.width / 2) + (rect.rect.width / 2);
                    startPositionY = rect.parent.position.y + (rect.parent.GetComponent<RectTransform>().rect.height / 2) + (rect.rect.height / 2);
                    break;
                case (AnimationStartPosition.TopLeft):
                    startPositionX = rect.parent.position.x - (rect.parent.GetComponent<RectTransform>().rect.width / 2) - (rect.rect.width / 2);
                    startPositionY = rect.parent.position.y + (rect.parent.GetComponent<RectTransform>().rect.height / 2) + (rect.rect.height / 2);
                    break;
            }
        }

        Vector3 startPos = new Vector3(startPositionX, startPositionY, 0);

        return (startPos);
    }
}

// TODO: Move this enum to AnimationPElement script when you go home.
public enum AnimationFromCornerStartFromType
{
    // Screen is used to start the animation from bordres of the screen.
    // Rect is used to start the animation from bordres of the current rect.
    ScreenCorner, ParentRectCorner
}