using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class AiryUIAnimatedElement : MonoBehaviour
{
    public UnityEvent OnShow;
    public UnityEvent OnHide;
    public UnityEvent OnShowComplete;
    public UnityEvent OnHideComplete;

    [Tooltip("Show Item Animation On Menu Enable")] public bool showItemOnMenuEnable = true;

    public AnimationType showAnimationType;
    public AnimationType hideAnimationType;
    public AnimationStartPosition animationFromCornerType;
    public AnimationStartPosition animationToCornerType;
    public AnimationStartPosition pivotWhileRotating;
    public AnimationFromCornerStartFromType animationFromCornerStartFromType;

    public bool fadeChildren;

    public float animationShowDuration = 0.5f;
    public float animationHideDuration = 0.5f;

    public bool withDelay;
    public float showDelay;
    public float hideDelay;

    public float elasticityPower = 1;

    public Vector3 rotateFrom;
    public Vector3 rotateTo;

    public Vector3 initialWorldPosition;
    public Vector3 initialLocalPosition;
    public Color[] initialColorsOfChildren;
    public Vector3 initialScale;
    public Quaternion initialRotation;
    public Vector2 initialPivot;

    protected RectTransform rectTransform;

    private Graphic[] graphics;

    private bool isReady = false;

    private void Start()
    {
        if (!isReady)
            SetReady();
    }

    private void SetReady()
    {
        rectTransform = GetComponent<RectTransform>();
        graphics = GetComponentsInChildren<Graphic>();
        initialColorsOfChildren = new Color[graphics.Length];
        initialWorldPosition = transform.position;
        initialLocalPosition = transform.localPosition;
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
        initialPivot = rectTransform.pivot;

        for (int i = 0; i < graphics.Length; i++)
            initialColorsOfChildren[i] = graphics[i].color;

        isReady = true;
    }

    public virtual void ShowElement()
    {
        if (!isReady)
            SetReady();

        gameObject.SetActive(true);

        switch (showAnimationType)
        {
            case (AnimationType.Scale):
                StartCoroutine(AnimateScale_SHOW());
                break;
            case (AnimationType.Rotate):
                StartCoroutine(AnimateRotation_SHOW());
                break;
            case (AnimationType.ScaleElastic):
                StartCoroutine(AnimateElasticScale_SHOW());
                break;
            case (AnimationType.FadeColor):
                StartCoroutine(AnimateFadeIn_SHOW());
                break;
            case (AnimationType.FromCornerWithScale):
                StartCoroutine(AnimateFromCornerWithScale_SHOW());
                break;
            case (AnimationType.FromCornerWithoutScale):
                StartCoroutine(AnimateFromCornerWithoutScale_SHOW());
                break;
        }

        if (OnShow != null)
            OnShow.Invoke();
    }

    public virtual void HideElement()
    {
        OnHideComplete.AddListener(ResetDefaults);

        switch (hideAnimationType)
        {
            case (AnimationType.Scale):
                StartCoroutine(AnimateScale_HIDE());
                break;
            case (AnimationType.Rotate):
                StartCoroutine(AnimateRotation_HIDE());
                break;
            case (AnimationType.ScaleElastic):
                StartCoroutine(AnimateElasticScale_HIDE());
                break;
            case (AnimationType.FadeColor):
                StartCoroutine(AnimateFadeIn_HIDE());
                break;
            case (AnimationType.FromCornerWithScale):
                StartCoroutine(AnimateToCornerWithScale_HIDE());
                break;
            case (AnimationType.FromCornerWithoutScale):
                StartCoroutine(AnimateFromCornerWithoutScale_HIDE());
                break;
        }

        if (OnHide != null)
            OnHide.Invoke();
    }

    #region Show Coroutines

    private IEnumerator AnimateFromCornerWithScale_SHOW()
    {
        if (animationShowDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.Zero);
        ResetColor(ResetOptions.Zero);

        #region Initialization Part

        Graphic[] imagesInMenu = GetComponentsInChildren<Graphic>();

        // Here we get start color to a color with alpha = 0, and the end color to a color with alpha = 1.
        Color[] startColors = new Color[imagesInMenu.Length];
        Color[] endColors = new Color[imagesInMenu.Length];

        for (int i = 0; i < graphics.Length; i++)
        {
            startColors[i] = graphics[i].color;
            startColors[i].a = 0;

            endColors[i] = initialColorsOfChildren[i];
        }

        Vector3 startPos = AiryUIAnimationPositions.GetStartPositionFromCorner(initialWorldPosition, rectTransform, animationFromCornerType, animationFromCornerStartFromType);

        Vector3 targetPosition = initialWorldPosition;
        rectTransform.position = startPos;

        #endregion

        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        // Starting animating.
        rectTransform.position = startPos;

        float startTime = Time.time;

        while (Time.time <= startTime + animationShowDuration)
        {
            float t = (Time.time - startTime) / animationShowDuration;

            // Here we Lerp the position and the scale as well.
            rectTransform.position = Vector3.Lerp(startPos, targetPosition, t);
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            // Here we lerp the color from totally transparent to totally visible.
            for (int i = 0; i < imagesInMenu.Length; i++)
                imagesInMenu[i].color = Color.Lerp(startColors[i], endColors[i], t);

            yield return (null);
        }

        // Here we set the values to their end so we avoid the missing final step.
        rectTransform.position = targetPosition;
        rectTransform.localScale = Vector3.one;

        for (int i = 0; i < imagesInMenu.Length; i++)
            imagesInMenu[i].color = endColors[i];

        if (OnShowComplete != null)
            OnShowComplete.Invoke();
    }

    private IEnumerator AnimateFromCornerWithoutScale_SHOW()
    {
        if (animationShowDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.Zero);
        ResetColor(ResetOptions.Zero);

        #region Initialization Part


        Vector3 startPos = AiryUIAnimationPositions.GetStartPositionFromCorner(initialWorldPosition, rectTransform, animationFromCornerType, animationFromCornerStartFromType);
        Vector3 targetPosition = initialWorldPosition;

        rectTransform.position = startPos;

        #endregion

        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        ResetPosition();
        ResetScale(ResetOptions.One);
        ResetColor(ResetOptions.One);

        // Starting animating.
        float startTime = Time.time;

        rectTransform.position = startPos;

        while (Time.time <= startTime + animationShowDuration)
        {
            float t = (Time.time - startTime) / animationShowDuration;

            // Here we Lerp the position and the scale as well.
            rectTransform.position = Vector3.Lerp(startPos, targetPosition, t);

            yield return (null);
        }

        // Here we set the values to their end so we avoid the missing final step.
        rectTransform.position = targetPosition;

        if (OnShowComplete != null)
            OnShowComplete.Invoke();
    }

    private IEnumerator AnimateElasticScale_SHOW()
    {
        if (animationShowDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.Zero);
        ResetColor(ResetOptions.One);

        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        float startTime = Time.time;

        while (Time.time <= startTime + animationShowDuration)
        {
            float t = (Time.time - startTime) / animationShowDuration;

            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one + new Vector3(0.1f, 0.1f, 0.1f) * elasticityPower, t);

            yield return (null);
        }

        rectTransform.localScale = Vector3.one + new Vector3(0.1f, 0.1f, 0.1f) * elasticityPower;

        startTime = Time.time;
        while (Time.time <= startTime + animationShowDuration / 5)
        {
            float t = (Time.time - startTime) / (animationShowDuration / 5);
            rectTransform.localScale = Vector3.Lerp(Vector3.one + new Vector3(0.1f, 0.1f, 0.1f) * elasticityPower, Vector3.one, t);
            yield return (null);
        }

        rectTransform.localScale = Vector3.one;

        if (OnShowComplete != null)
            OnShowComplete.Invoke();
    }

    private IEnumerator AnimateScale_SHOW()
    {
        if (animationShowDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.Zero);
        ResetColor(ResetOptions.One);

        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        Vector3 startPosition = rectTransform.position;
        float startTime = Time.time;

        while (Time.time <= startTime + animationShowDuration)
        {
            float t = (Time.time - startTime) / animationShowDuration;

            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            yield return (null);
        }

        rectTransform.localScale = Vector3.one;

        if (OnShowComplete != null)
            OnShowComplete.Invoke();
    }

    private IEnumerator AnimateFadeIn_SHOW()
    {
        if (animationShowDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.One);

        #region Initialization Part

        Color[] startColors = new Color[graphics.Length], endColors = new Color[graphics.Length];

        for (int i = 0; i < graphics.Length; i++)
        {
            startColors[i] = graphics[i].color;
            startColors[i].a = 0;

            endColors[i] = initialColorsOfChildren[i];
        }

        ResetColor(ResetOptions.Zero);

        #endregion

        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        if (fadeChildren)
        {
            float startTime = Time.time;
            while (Time.time < startTime + animationShowDuration)
            {
                float t = (Time.time - startTime) / animationShowDuration;
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = Color.Lerp(startColors[i], endColors[i], t);
                }

                yield return (null);
            }
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].color = endColors[i];
            }
        }
        else
        {
            float startTime = Time.time;
            while (Time.time < startTime + animationShowDuration)
            {
                float t = (Time.time - startTime) / animationShowDuration;
                graphics[0].color = Color.Lerp(startColors[0], endColors[0], t);

                yield return (null);
            }

            graphics[0].color = endColors[0];
        }

        if (OnShowComplete != null)
            OnShowComplete.Invoke();
    }

    private IEnumerator AnimateRotation_SHOW()
    {
        if (animationShowDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.One);
        ResetColor(ResetOptions.One);
        SetPivot(pivotWhileRotating);
        ResetRotation(ResetOptions.One, true);

        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        float startTime = Time.time;
        while (Time.time <= startTime + animationShowDuration)
        {
            float t = (Time.time - startTime) / animationShowDuration;

            rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(rotateFrom, Quaternion.ToEulerAngles(initialRotation), t));

            yield return (null);
        }

        rectTransform.rotation = initialRotation;
        ResetRotation(ResetOptions.Zero, true);
        ResetPivot();

        if (OnShowComplete != null)
            OnShowComplete.Invoke();
    }

    #endregion

    //=======================================================================================
    //=======================================================================================

    #region Hide Coroutines

    private IEnumerator AnimateFadeIn_HIDE()
    {
        if (animationHideDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.One);
        ResetColor(ResetOptions.One);

        #region Initialization Part

        Color[] startColors = new Color[graphics.Length], endColors = new Color[graphics.Length];

        for (int i = 0; i < graphics.Length; i++)
        {
            startColors[i] = initialColorsOfChildren[i];

            endColors[i] = initialColorsOfChildren[i];
            endColors[i].a = 0;
        }

        #endregion

        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        float startTime = Time.time;

        if (fadeChildren)
        {
            while (Time.time < startTime + animationHideDuration)
            {
                float t = (Time.time - startTime) / animationHideDuration;
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = Color.Lerp(startColors[i], endColors[i], t);
                }

                yield return (null);
            }
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].color = endColors[i];
            }
        }
        else
        {
            while (Time.time < startTime + animationHideDuration)
            {
                float t = (Time.time - startTime) / animationHideDuration;
                graphics[0].color = Color.Lerp(startColors[0], endColors[0], t);

                yield return (null);
            }

            graphics[0].color = endColors[0];
        }

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateScale_HIDE()
    {
        if (animationHideDuration <= 0)
            yield break;

        Vector3 startPosition = rectTransform.position;
        rectTransform.localScale = Vector3.one;

        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        float startTime = Time.time;

        while (Time.time <= startTime + animationHideDuration)
        {
            float t = (Time.time - startTime) / animationHideDuration;

            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return (null);
        }

        rectTransform.localScale = Vector3.zero;

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateElasticScale_HIDE()
    {
        if (animationHideDuration <= 0)
            yield break;

        ResetPosition();
        ResetScale(ResetOptions.One);
        ResetColor(ResetOptions.One);

        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        rectTransform.localScale = Vector3.one;

        float startTime = Time.time;

        while (Time.time <= startTime + animationHideDuration / 5)
        {
            float t = (Time.time - startTime) / (animationHideDuration / 5);

            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one + new Vector3(0.1f, 0.1f, 0.1f) * elasticityPower, t);

            yield return (null);
        }

        startTime = Time.time;

        while (Time.time <= startTime + animationHideDuration)
        {
            float t = (Time.time - startTime) / (animationHideDuration);

            rectTransform.localScale = Vector3.Lerp(Vector3.one + new Vector3(0.1f, 0.1f, 0.1f) * elasticityPower, Vector3.zero, t);

            yield return (null);
        }

        rectTransform.localScale = Vector3.zero;

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateToCornerWithScale_HIDE()
    {
        if (animationHideDuration <= 0)
            yield break;

        Graphic[] images;

        if (fadeChildren)
            images = GetComponentsInChildren<Graphic>();
        else
            images = GetComponents<Graphic>();

        Color[] startColors = new Color[images.Length];
        Color[] endColors = new Color[images.Length];

        for (int i = 0; i < images.Length; i++)
        {
            startColors[i] = initialColorsOfChildren[i];

            endColors[i] = images[i].color;
            endColors[i].a = 0;
        }


        Vector3 startPos = initialWorldPosition;
        Vector3 targetPosition = AiryUIAnimationPositions.GetStartPositionFromCorner(initialWorldPosition, rectTransform, animationToCornerType, animationFromCornerStartFromType);
        rectTransform.position = startPos;

        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        float startTime = Time.time;

        while (Time.time <= startTime + animationHideDuration)
        {
            float t = (Time.time - startTime) / animationHideDuration;
            rectTransform.position = Vector3.Lerp(startPos, targetPosition, t);
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = Color.Lerp(startColors[i], endColors[i], t);
            }

            yield return (null);
        }

        rectTransform.position = targetPosition;
        rectTransform.localScale = Vector3.zero;

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = endColors[i];
        }

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateFromCornerWithoutScale_HIDE()
    {
        if (animationHideDuration <= 0)
            yield break;

        Vector3 startPos = initialWorldPosition;
        Vector3 targetPosition = AiryUIAnimationPositions.GetStartPositionFromCorner(initialWorldPosition, rectTransform, animationToCornerType, animationFromCornerStartFromType);
        rectTransform.position = startPos;

        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        float startTime = Time.time;

        while (Time.time <= startTime + animationHideDuration)
        {
            float t = (Time.time - startTime) / animationHideDuration;
            rectTransform.position = Vector3.Lerp(startPos, targetPosition, t);

            yield return (null);
        }

        rectTransform.position = targetPosition;

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateRotation_HIDE()
    {
        if (animationHideDuration <= 0)
            yield break;

        ResetPosition();
        SetPivot(pivotWhileRotating);
        ResetScale(ResetOptions.One);
        ResetColor(ResetOptions.One);
        ResetRotation(ResetOptions.Zero, true);

        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        float startTime = Time.time;
        while (Time.time < startTime + animationHideDuration)
        {
            float t = (Time.time - startTime) / animationHideDuration;

            rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(Quaternion.ToEulerAngles(initialRotation), rotateTo, t));

            yield return (null);
        }

        ResetRotation(ResetOptions.One, false);

        ResetPivot();

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    #endregion

    private void ResetDefaults()
    {
        ResetPosition();
        ResetPivot();
        ResetScale(ResetOptions.One);
        ResetColor(ResetOptions.One);
        ResetRotation(ResetOptions.Zero, true);

        gameObject.SetActive(false);
    }

    private void ResetPosition()
    {
        transform.position = initialWorldPosition;
        //transform.localPosition = initialLocalPosition;
    }

    private void ResetPivot()
    {
        rectTransform.pivot = initialPivot;
    }

    private void ResetColor(ResetOptions resetOption)
    {
        if (resetOption == ResetOptions.Zero)
        {
            if (fadeChildren)
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = new Color(initialColorsOfChildren[i].r, initialColorsOfChildren[i].g, initialColorsOfChildren[i].b, 0);
                }
            }
            else
            {
                // because the index 0 is for the componet on this game object.
                graphics[0].color = new Color(initialColorsOfChildren[0].r, initialColorsOfChildren[0].g, initialColorsOfChildren[0].b, 0);
            }
        }
        else if (resetOption == ResetOptions.One)
        {
            if (fadeChildren)
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = initialColorsOfChildren[i];
                }
            }
            else
            {
                graphics[0].color = initialColorsOfChildren[0];
            }
        }
    }

    private void ResetScale(ResetOptions resetOption)
    {
        if (resetOption == ResetOptions.Zero)
        {
            transform.localScale = Vector3.zero;
        }
        else if (resetOption == ResetOptions.One)
        {
            transform.localScale = initialScale;
        }
    }

    private void ResetRotation(ResetOptions resetOption, bool show)
    {
        if (resetOption == ResetOptions.Zero)
        {
            transform.rotation = initialRotation;
        }
        else if (resetOption == ResetOptions.One && show)
        {
            transform.rotation = Quaternion.Euler(rotateFrom);
        }
        else if (resetOption == ResetOptions.One && !show)
        {
            transform.rotation = Quaternion.Euler(rotateTo);
        }
    }

    private void SetPivot(AnimationStartPosition pivotPosition)
    {
        switch (pivotPosition)
        {
            case (AnimationStartPosition.Bottom):
                rectTransform.pivot = new Vector2();
                break;
            case (AnimationStartPosition.Top):
                rectTransform.pivot = new Vector2(0.5f, 1);
                break;
            case (AnimationStartPosition.Right):
                rectTransform.pivot = new Vector2(1, 0.5f);
                break;
            case (AnimationStartPosition.Left):
                rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case (AnimationStartPosition.TopRight):
                rectTransform.pivot = Vector2.one;
                break;
            case (AnimationStartPosition.TopLeft):
                rectTransform.pivot = new Vector2(0, 1);
                break;
            case (AnimationStartPosition.BottomRight):
                rectTransform.pivot = new Vector2(1, 0);
                break;
            case (AnimationStartPosition.BottomLeft):
                rectTransform.pivot = Vector2.zero;
                break;
            case (AnimationStartPosition.Center):
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }

    private enum ResetOptions
    {
        Zero, One
    }
}

public enum AnimationType
{
    Scale, ScaleElastic, Rotate, FadeColor, FromCornerWithScale, FromCornerWithoutScale
}

public enum AnimationStartPosition
{
    Top, Bottom, Left, Right, Center, BottomRight, TopRight, BottomLeft, TopLeft
}