using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiryUICustomAnimationElement : AiryUIAnimatedElement
{
    public ComponentsToAnimate componentsToAnimate_SHOW;
    public ComponentsToAnimate componentsToAnimate_HIDE;

    public List<TransformAnimationRecord> TransformAnimationRecords_SHOW = new List<TransformAnimationRecord>();
    public List<GraphicAnimationRecord> GraphicAnimationRecords_SHOW = new List<GraphicAnimationRecord>();
    public List<TransformAndGraphicAnimationRecord> TransformAndGraphicAnimationRecords_SHOW = new List<TransformAndGraphicAnimationRecord>();

    public List<TransformAnimationRecord> TransformAnimationRecords_HIDE = new List<TransformAnimationRecord>();
    public List<GraphicAnimationRecord> GraphicAnimationRecords_HIDE = new List<GraphicAnimationRecord>();
    public List<TransformAndGraphicAnimationRecord> TransformAndGraphicAnimationRecords_HIDE = new List<TransformAndGraphicAnimationRecord>();

    public float currentRecordDuration = 0.5f;
    public float currentRecordDelay = 0;

    public bool loop = false;

    private GraphicType graphicType;

    private Graphic graphic;
    private Image img;
    private Text txt;

    private TransformAnimationRecord initialRectValues;
    private GraphicAnimationRecord initialGraphicValues;

    private TransformAnimationRecord transformRecord_beforeRecrod;
    private GraphicAnimationRecord graphicRecord_beforeRecrod;
    private TransformAndGraphicAnimationRecord transformAndGraphicRecord_beforeRecrod;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();

        SetInitialValues();
    }

    private void SetInitialValues()
    {
        initialRectValues = new TransformAnimationRecord()
        {
            Position = rectTransform.position,
            Scale = rectTransform.localScale,
            RotationEuler = rectTransform.rotation.eulerAngles,
            Pivot = rectTransform.pivot,
            AnchorMin = rectTransform.anchorMin,
            AnchorMax = rectTransform.anchorMax
        };

        if (graphic)
        {
            if (graphic is Image)
            {
                graphicType = GraphicType.Image;
                img = GetComponent<Image>();
            }
            else if (graphic is Text)
            {
                graphicType = GraphicType.Text;
                txt = GetComponent<Text>();
            }

            initialGraphicValues = new GraphicAnimationRecord()
            {
                Color = graphic.color,
                Sprite = (graphicType == GraphicType.Image) ? img.sprite : null,
                Text = (graphicType == GraphicType.Text) ? txt.text : "",
            };
        }
    }

    public override void ShowElement()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();

        switch (componentsToAnimate_SHOW)
        {
            case (ComponentsToAnimate.Transform):
                StartCoroutine(AnimateTransform_SHOW());
                break;
            case (ComponentsToAnimate.Graphic):
                StartCoroutine(AnimateGraphic_SHOW());
                break;
            case (ComponentsToAnimate.Both):
                StartCoroutine(AnimateTransformAndGraphic_SHOW());
                break;
        }

        if (OnShow != null)
            OnShow.Invoke();
    }

    public override void HideElement()
    {
        OnHideComplete.AddListener(ResetAll);
        StopAllCoroutines();

        switch (componentsToAnimate_HIDE)
        {
            case (ComponentsToAnimate.Transform):
                StartCoroutine(AnimateTransform_HIDE());
                break;
            case (ComponentsToAnimate.Graphic):
                StartCoroutine(AnimateGraphic_HIDE());
                break;
            case (ComponentsToAnimate.Both):
                StartCoroutine(AnimateTransformAndGraphic_HIDE());
                break;
        }

        if (OnHide != null)
            OnHide.Invoke();
    }

    #region Show Coroutines

    private IEnumerator AnimateTransform_SHOW()
    {
        if (withDelay)
            yield return (new WaitForSeconds(showDelay));

        if (TransformAnimationRecords_SHOW.Count > 1)
        {
            if (loop)
            {
                while (true)
                {
                    for (int i = 1; i < TransformAnimationRecords_SHOW.Count; i++)
                    {
                        if (TransformAnimationRecords_SHOW[i - 1].Delay > 0)
                            yield return (new WaitForSeconds(TransformAnimationRecords_SHOW[i - 1].Delay));

                        float startTime = Time.time;
                        while (Time.time <= TransformAnimationRecords_SHOW[i - 1].Duration + startTime)
                        {
                            float t = (Time.time - startTime) / TransformAnimationRecords_SHOW[i - 1].Duration;

                            rectTransform.position = Vector3.Lerp(TransformAnimationRecords_SHOW[i - 1].Position, TransformAnimationRecords_SHOW[i].Position, t);
                            rectTransform.localScale = Vector3.Lerp(TransformAnimationRecords_SHOW[i - 1].Scale, TransformAnimationRecords_SHOW[i].Scale, t);
                            rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(TransformAnimationRecords_SHOW[i - 1].RotationEuler, TransformAnimationRecords_SHOW[i].RotationEuler, t));
                            rectTransform.pivot = Vector2.Lerp(TransformAnimationRecords_SHOW[i - 1].Pivot, TransformAnimationRecords_SHOW[i].Pivot, t);
                            rectTransform.anchorMin = Vector2.Lerp(TransformAnimationRecords_SHOW[i - 1].AnchorMin, TransformAnimationRecords_SHOW[i].AnchorMin, t);
                            rectTransform.anchorMax = Vector2.Lerp(TransformAnimationRecords_SHOW[i - 1].AnchorMax, TransformAnimationRecords_SHOW[i].AnchorMax, t);

                            yield return (null);
                        }

                        #region Set Final Values

                        rectTransform.position = TransformAnimationRecords_SHOW[i].Position;
                        rectTransform.localScale = TransformAnimationRecords_SHOW[i].Scale;
                        rectTransform.rotation = Quaternion.Euler(TransformAnimationRecords_SHOW[i].RotationEuler);
                        rectTransform.pivot = TransformAnimationRecords_SHOW[i].Pivot;
                        rectTransform.anchorMin = TransformAnimationRecords_SHOW[i].AnchorMin;
                        rectTransform.anchorMax = TransformAnimationRecords_SHOW[i].AnchorMax;

                        #endregion
                    }

                    if (OnShowComplete != null)
                        OnShowComplete.Invoke();
                }
            }
            else
            {
                for (int i = 1; i < TransformAnimationRecords_SHOW.Count; i++)
                {
                    if (TransformAnimationRecords_SHOW[i - 1].Delay > 0)
                        yield return (new WaitForSeconds(TransformAnimationRecords_SHOW[i - 1].Delay));

                    float startTime = Time.time;
                    while (Time.time <= TransformAnimationRecords_SHOW[i - 1].Duration + startTime)
                    {
                        float t = (Time.time - startTime) / TransformAnimationRecords_SHOW[i - 1].Duration;

                        rectTransform.position = Vector3.Lerp(TransformAnimationRecords_SHOW[i - 1].Position, TransformAnimationRecords_SHOW[i].Position, t);
                        rectTransform.localScale = Vector3.Lerp(TransformAnimationRecords_SHOW[i - 1].Scale, TransformAnimationRecords_SHOW[i].Scale, t);
                        rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(TransformAnimationRecords_SHOW[i - 1].RotationEuler, TransformAnimationRecords_SHOW[i].RotationEuler, t));
                        rectTransform.pivot = Vector2.Lerp(TransformAnimationRecords_SHOW[i - 1].Pivot, TransformAnimationRecords_SHOW[i].Pivot, t);
                        rectTransform.anchorMin = Vector2.Lerp(TransformAnimationRecords_SHOW[i - 1].AnchorMin, TransformAnimationRecords_SHOW[i].AnchorMin, t);
                        rectTransform.anchorMax = Vector2.Lerp(TransformAnimationRecords_SHOW[i - 1].AnchorMax, TransformAnimationRecords_SHOW[i].AnchorMax, t);

                        yield return (null);
                    }

                    #region Set Final Values

                    rectTransform.position = TransformAnimationRecords_SHOW[i].Position;
                    rectTransform.localScale = TransformAnimationRecords_SHOW[i].Scale;
                    rectTransform.rotation = Quaternion.Euler(TransformAnimationRecords_SHOW[i].RotationEuler);
                    rectTransform.pivot = TransformAnimationRecords_SHOW[i].Pivot;
                    rectTransform.anchorMin = TransformAnimationRecords_SHOW[i].AnchorMin;
                    rectTransform.anchorMax = TransformAnimationRecords_SHOW[i].AnchorMax;

                    #endregion
                }

                if (OnShowComplete != null)
                    OnShowComplete.Invoke();
            }
        }
    }

    private IEnumerator AnimateGraphic_SHOW()
    {
        if (withDelay)
            yield return (new WaitForSeconds(showDelay));
        if (GraphicAnimationRecords_SHOW.Count > 1)
        {
            if (loop)
            {
                while (true)
                {
                    for (int i = 1; i < GraphicAnimationRecords_SHOW.Count; i++)
                    {
                        if (GraphicAnimationRecords_SHOW[i - 1].Delay > 0)
                            yield return (new WaitForSeconds(GraphicAnimationRecords_SHOW[i - 1].Delay));

                        float startTime = Time.time;
                        while (Time.time <= startTime + GraphicAnimationRecords_SHOW[i - 1].Duration)
                        {
                            float t = (Time.time - startTime) / GraphicAnimationRecords_SHOW[i - 1].Duration;

                            if (graphicType == GraphicType.Image)
                            {
                                Image img = graphic as Image;
                                if (GraphicAnimationRecords_SHOW[i - 1].Sprite)
                                {
                                    img.sprite = GraphicAnimationRecords_SHOW[i - 1].Sprite;
                                    img.color = Color.Lerp(GraphicAnimationRecords_SHOW[i - 1].Color, GraphicAnimationRecords_SHOW[i].Color, t);
                                }
                            }

                            else if (graphicType == GraphicType.Text)
                            {
                                Text txt = graphic as Text;
                                if (!string.IsNullOrEmpty(GraphicAnimationRecords_SHOW[i - 1].Text))
                                {
                                    txt.text = GraphicAnimationRecords_SHOW[i - 1].Text;
                                    txt.color = Color.Lerp(GraphicAnimationRecords_SHOW[i - 1].Color, GraphicAnimationRecords_SHOW[i].Color, t);
                                }
                            }

                            yield return (null);
                        }

                        #region Set Final Values

                        if (graphicType == GraphicType.Image)
                        {
                            Image img = graphic as Image;
                            img.sprite = GraphicAnimationRecords_SHOW[i].Sprite;
                            img.color = GraphicAnimationRecords_SHOW[i].Color;
                        }
                        else if (graphicType == GraphicType.Text)
                        {
                            Text txt = graphic as Text;
                            txt.text = GraphicAnimationRecords_SHOW[i].Text;
                            txt.color = GraphicAnimationRecords_SHOW[i].Color;
                        }

                        #endregion
                    }
                }
            }
            else
            {
                for (int i = 1; i < GraphicAnimationRecords_SHOW.Count; i++)
                {
                    if (GraphicAnimationRecords_SHOW[i - 1].Delay > 0)
                        yield return (new WaitForSeconds(GraphicAnimationRecords_SHOW[i - 1].Delay));

                    float startTime = Time.time;
                    while (Time.time <= startTime + GraphicAnimationRecords_SHOW[i - 1].Duration)
                    {
                        float t = (Time.time - startTime) / GraphicAnimationRecords_SHOW[i - 1].Duration;

                        if (graphicType == GraphicType.Image)
                        {
                            Image img = graphic as Image;
                            if (GraphicAnimationRecords_SHOW[i - 1].Sprite)
                                img.sprite = GraphicAnimationRecords_SHOW[i - 1].Sprite;
                            img.color = Color.Lerp(GraphicAnimationRecords_SHOW[i - 1].Color, GraphicAnimationRecords_SHOW[i].Color, t);
                        }

                        else if (graphicType == GraphicType.Text)
                        {
                            Text txt = graphic as Text;
                            if (!string.IsNullOrEmpty(GraphicAnimationRecords_SHOW[i - 1].Text))
                                txt.text = GraphicAnimationRecords_SHOW[i - 1].Text;
                            txt.color = Color.Lerp(GraphicAnimationRecords_SHOW[i - 1].Color, GraphicAnimationRecords_SHOW[i].Color, t);
                        }

                        yield return (null);
                    }

                    #region Set Final Values

                    if (graphicType == GraphicType.Image)
                    {
                        Image img = graphic as Image;
                        if (GraphicAnimationRecords_SHOW[i].Sprite)
                            img.sprite = GraphicAnimationRecords_SHOW[i].Sprite;
                        img.color = GraphicAnimationRecords_SHOW[i].Color;
                    }
                    else if (graphicType == GraphicType.Text)
                    {
                        Text txt = graphic as Text;
                        if (!string.IsNullOrEmpty(GraphicAnimationRecords_SHOW[i].Text))
                            txt.text = GraphicAnimationRecords_SHOW[i].Text;
                        txt.color = GraphicAnimationRecords_SHOW[i].Color;
                    }

                    #endregion
                }

                if (OnShowComplete != null)
                    OnShowComplete.Invoke();
            }
        }
    }

    private IEnumerator AnimateTransformAndGraphic_SHOW()
    {
        if (TransformAndGraphicAnimationRecords_SHOW.Count > 1)
        {
            if (withDelay)
                yield return (new WaitForSeconds(showDelay));

            if (loop)
            {
                while (true)
                {
                    for (int i = 1; i < TransformAndGraphicAnimationRecords_SHOW.Count; i++)
                    {
                        if (TransformAndGraphicAnimationRecords_SHOW[i - 1].Delay > 0)
                            yield return (new WaitForSeconds(TransformAndGraphicAnimationRecords_SHOW[i - 1].Delay));

                        float startTime = Time.time;
                        while (Time.time <= TransformAndGraphicAnimationRecords_SHOW[i - 1].Duration + startTime)
                        {
                            float t = (Time.time - startTime) / TransformAndGraphicAnimationRecords_SHOW[i - 1].Duration;

                            rectTransform.position = Vector3.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Position, TransformAndGraphicAnimationRecords_SHOW[i].Position, t);
                            rectTransform.localScale = Vector3.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Scale, TransformAndGraphicAnimationRecords_SHOW[i].Scale, t);
                            rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].RotationEuler, TransformAndGraphicAnimationRecords_SHOW[i].RotationEuler, t));
                            rectTransform.pivot = Vector2.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Pivot, TransformAndGraphicAnimationRecords_SHOW[i].Pivot, t);
                            rectTransform.anchorMin = Vector2.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].AnchorMin, TransformAndGraphicAnimationRecords_SHOW[i].AnchorMin, t);
                            rectTransform.anchorMax = Vector2.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].AnchorMax, TransformAndGraphicAnimationRecords_SHOW[i].AnchorMax, t);

                            if (graphicType == GraphicType.Image)
                            {
                                Image img = graphic as Image;
                                if (TransformAndGraphicAnimationRecords_SHOW[i - 1].Sprite)
                                {
                                    img.sprite = TransformAndGraphicAnimationRecords_SHOW[i - 1].Sprite;
                                }
                                img.color = Color.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Color, TransformAndGraphicAnimationRecords_SHOW[i].Color, t);
                            }

                            else if (graphicType == GraphicType.Text)
                            {
                                Text txt = graphic as Text;
                                if (!string.IsNullOrEmpty(TransformAndGraphicAnimationRecords_SHOW[i - 1].Text))
                                {
                                    txt.text = TransformAndGraphicAnimationRecords_SHOW[i - 1].Text;
                                }
                                txt.color = Color.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Color, TransformAndGraphicAnimationRecords_SHOW[i].Color, t);
                            }

                            yield return (null);
                        }

                        #region Set Final Values

                        rectTransform.position = TransformAndGraphicAnimationRecords_SHOW[i].Position;
                        rectTransform.localScale = TransformAndGraphicAnimationRecords_SHOW[i].Scale;
                        rectTransform.rotation = Quaternion.Euler(TransformAndGraphicAnimationRecords_SHOW[i].RotationEuler);
                        rectTransform.pivot = TransformAndGraphicAnimationRecords_SHOW[i].Pivot;
                        rectTransform.anchorMin = TransformAndGraphicAnimationRecords_SHOW[i].AnchorMin;
                        rectTransform.anchorMax = TransformAndGraphicAnimationRecords_SHOW[i].AnchorMax;

                        if (graphicType == GraphicType.Image)
                        {
                            Image img = graphic as Image;
                            if (TransformAndGraphicAnimationRecords_SHOW[i].Sprite)
                                img.color = TransformAndGraphicAnimationRecords_SHOW[i].Color;
                        }
                        else if (graphicType == GraphicType.Text)
                        {
                            Text txt = graphic as Text;
                            if (!string.IsNullOrEmpty(TransformAndGraphicAnimationRecords_SHOW[i].Text))
                                txt.color = TransformAndGraphicAnimationRecords_SHOW[i].Color;
                        }

                        #endregion
                    }
                }
            }
            else
            {
                for (int i = 1; i < TransformAndGraphicAnimationRecords_SHOW.Count; i++)
                {
                    if (TransformAndGraphicAnimationRecords_SHOW[i - 1].Delay > 0)
                        yield return (new WaitForSeconds(TransformAndGraphicAnimationRecords_SHOW[i - 1].Delay));

                    float startTime = Time.time;
                    while (Time.time <= TransformAndGraphicAnimationRecords_SHOW[i - 1].Duration + startTime)
                    {
                        float t = (Time.time - startTime) / TransformAndGraphicAnimationRecords_SHOW[i - 1].Duration;

                        rectTransform.position = Vector3.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Position, TransformAndGraphicAnimationRecords_SHOW[i].Position, t);
                        rectTransform.localScale = Vector3.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Scale, TransformAndGraphicAnimationRecords_SHOW[i].Scale, t);
                        rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].RotationEuler, TransformAndGraphicAnimationRecords_SHOW[i].RotationEuler, t));
                        rectTransform.pivot = Vector2.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Pivot, TransformAndGraphicAnimationRecords_SHOW[i].Pivot, t);
                        rectTransform.anchorMin = Vector2.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].AnchorMin, TransformAndGraphicAnimationRecords_SHOW[i].AnchorMin, t);
                        rectTransform.anchorMax = Vector2.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].AnchorMax, TransformAndGraphicAnimationRecords_SHOW[i].AnchorMax, t);

                        if (graphicType == GraphicType.Image)
                        {
                            Image img = graphic as Image;
                            if (TransformAndGraphicAnimationRecords_SHOW[i - 1].Sprite)
                                img.sprite = TransformAndGraphicAnimationRecords_SHOW[i - 1].Sprite;

                            img.color = Color.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Color, TransformAndGraphicAnimationRecords_SHOW[i].Color, t);
                        }

                        else if (graphicType == GraphicType.Text)
                        {
                            Text txt = graphic as Text;
                            if (!string.IsNullOrEmpty(TransformAndGraphicAnimationRecords_SHOW[i - 1].Text))
                                txt.text = TransformAndGraphicAnimationRecords_SHOW[i - 1].Text;

                            txt.color = Color.Lerp(TransformAndGraphicAnimationRecords_SHOW[i - 1].Color, TransformAndGraphicAnimationRecords_SHOW[i].Color, t);
                        }

                        yield return (null);
                    }

                    #region Set Final Values

                    rectTransform.position = TransformAndGraphicAnimationRecords_SHOW[i].Position;
                    rectTransform.localScale = TransformAndGraphicAnimationRecords_SHOW[i].Scale;
                    rectTransform.rotation = Quaternion.Euler(TransformAndGraphicAnimationRecords_SHOW[i].RotationEuler);
                    rectTransform.pivot = TransformAndGraphicAnimationRecords_SHOW[i].Pivot;
                    rectTransform.anchorMin = TransformAndGraphicAnimationRecords_SHOW[i].AnchorMin;
                    rectTransform.anchorMax = TransformAndGraphicAnimationRecords_SHOW[i].AnchorMax;

                    if (graphicType == GraphicType.Image)
                    {
                        Image img = graphic as Image;
                        img.color = TransformAndGraphicAnimationRecords_SHOW[i].Color;
                    }
                    else if (graphicType == GraphicType.Text)
                    {
                        Text txt = graphic as Text;
                        txt.color = TransformAndGraphicAnimationRecords_SHOW[i].Color;
                    }

                    #endregion
                }

                if (OnShowComplete != null)
                    OnShowComplete.Invoke();
            }
        }
    }

    #endregion

    //==========================================================================================
    //==========================================================================================

    #region Hide Coroutines

    private IEnumerator AnimateTransform_HIDE()
    {
        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        for (int i = 1; i < TransformAnimationRecords_HIDE.Count; i++)
        {
            if (TransformAnimationRecords_HIDE[i - 1].Delay > 0)
                yield return (new WaitForSeconds(TransformAnimationRecords_HIDE[i - 1].Delay));

            float startTime = Time.time;
            while (Time.time <= TransformAnimationRecords_HIDE[i - 1].Duration + startTime)
            {
                float t = (Time.time - startTime) / TransformAnimationRecords_HIDE[i - 1].Duration;

                rectTransform.position = Vector3.Lerp(TransformAnimationRecords_HIDE[i - 1].Position, TransformAnimationRecords_HIDE[i].Position, t);
                rectTransform.localScale = Vector3.Lerp(TransformAnimationRecords_HIDE[i - 1].Scale, TransformAnimationRecords_HIDE[i].Scale, t);
                rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(TransformAnimationRecords_HIDE[i - 1].RotationEuler, TransformAnimationRecords_HIDE[i].RotationEuler, t));
                rectTransform.pivot = Vector2.Lerp(TransformAnimationRecords_HIDE[i - 1].Pivot, TransformAnimationRecords_HIDE[i].Pivot, t);
                rectTransform.anchorMin = Vector2.Lerp(TransformAnimationRecords_HIDE[i - 1].AnchorMin, TransformAnimationRecords_HIDE[i].AnchorMin, t);
                rectTransform.anchorMax = Vector2.Lerp(TransformAnimationRecords_HIDE[i - 1].AnchorMax, TransformAnimationRecords_HIDE[i].AnchorMax, t);

                yield return (null);
            }

            #region Set Final Values

            rectTransform.position = TransformAnimationRecords_HIDE[i].Position;
            rectTransform.localScale = TransformAnimationRecords_HIDE[i].Scale;
            rectTransform.rotation = Quaternion.LookRotation(TransformAnimationRecords_HIDE[i].RotationEuler);
            rectTransform.pivot = TransformAnimationRecords_HIDE[i].Pivot;
            rectTransform.anchorMin = TransformAnimationRecords_HIDE[i].AnchorMin;
            rectTransform.anchorMax = TransformAnimationRecords_HIDE[i].AnchorMax;

            #endregion
        }

        gameObject.SetActive(false);

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateGraphic_HIDE()
    {
        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        for (int i = 1; i < GraphicAnimationRecords_HIDE.Count; i++)
        {
            if (GraphicAnimationRecords_HIDE[i - 1].Delay > 0)
                yield return (new WaitForSeconds(GraphicAnimationRecords_HIDE[i - 1].Delay));

            float startTime = Time.time;
            while (Time.time <= startTime + GraphicAnimationRecords_HIDE[i - 1].Duration)
            {
                float t = (Time.time - startTime) / GraphicAnimationRecords_HIDE[i - 1].Duration;

                if (graphicType == GraphicType.Image)
                {
                    Image img = graphic as Image;
                    if (GraphicAnimationRecords_HIDE[i - 1].Sprite)
                        img.sprite = GraphicAnimationRecords_HIDE[i - 1].Sprite;
                    img.color = Color.Lerp(GraphicAnimationRecords_HIDE[i - 1].Color, GraphicAnimationRecords_HIDE[i].Color, t);
                }

                else if (graphicType == GraphicType.Text)
                {
                    Text txt = graphic as Text;
                    if (!string.IsNullOrEmpty(GraphicAnimationRecords_HIDE[i - 1].Text))
                        txt.text = GraphicAnimationRecords_HIDE[i - 1].Text;
                    txt.color = Color.Lerp(GraphicAnimationRecords_HIDE[i - 1].Color, GraphicAnimationRecords_HIDE[i].Color, t);
                }

                yield return (null);
            }

            #region Set Final Values

            if (graphicType == GraphicType.Image)
            {
                Image img = graphic as Image;
                img.color = GraphicAnimationRecords_HIDE[i].Color;
            }
            else if (graphicType == GraphicType.Text)
            {
                Text txt = graphic as Text;
                txt.color = GraphicAnimationRecords_HIDE[i].Color;
            }

            #endregion
        }

        gameObject.SetActive(false);

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    private IEnumerator AnimateTransformAndGraphic_HIDE()
    {
        if (withDelay)
            yield return (new WaitForSeconds(hideDelay));

        for (int i = 1; i < TransformAndGraphicAnimationRecords_HIDE.Count; i++)
        {
            if (TransformAndGraphicAnimationRecords_HIDE[i - 1].Delay > 0)
                yield return (new WaitForSeconds(TransformAndGraphicAnimationRecords_HIDE[i - 1].Delay));

            float startTime = Time.time;
            while (Time.time <= TransformAndGraphicAnimationRecords_HIDE[i - 1].Duration + startTime)
            {
                float t = (Time.time - startTime) / TransformAndGraphicAnimationRecords_HIDE[i - 1].Duration;

                rectTransform.position = Vector3.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].Position, TransformAndGraphicAnimationRecords_HIDE[i].Position, t);
                rectTransform.localScale = Vector3.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].Scale, TransformAndGraphicAnimationRecords_HIDE[i].Scale, t);
                rectTransform.rotation = Quaternion.Euler(Vector3.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].RotationEuler, TransformAndGraphicAnimationRecords_HIDE[i].RotationEuler, t));
                rectTransform.pivot = Vector2.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].Pivot, TransformAndGraphicAnimationRecords_HIDE[i].Pivot, t);
                rectTransform.anchorMin = Vector2.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].AnchorMin, TransformAndGraphicAnimationRecords_HIDE[i].AnchorMin, t);
                rectTransform.anchorMax = Vector2.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].AnchorMax, TransformAndGraphicAnimationRecords_HIDE[i].AnchorMax, t);

                if (graphicType == GraphicType.Image)
                {
                    Image img = graphic as Image;
                    if (TransformAndGraphicAnimationRecords_HIDE[i].Sprite)
                        img.sprite = TransformAndGraphicAnimationRecords_HIDE[i].Sprite;
                    img.color = Color.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].Color, TransformAndGraphicAnimationRecords_HIDE[i].Color, t);
                }

                else if (graphicType == GraphicType.Text)
                {
                    Text txt = graphic as Text;
                    if (!string.IsNullOrEmpty(TransformAndGraphicAnimationRecords_HIDE[i].Text))
                        txt.text = TransformAndGraphicAnimationRecords_HIDE[i].Text;
                    txt.color = Color.Lerp(TransformAndGraphicAnimationRecords_HIDE[i - 1].Color, TransformAndGraphicAnimationRecords_HIDE[i].Color, t);
                }

                yield return (null);
            }

            #region Set Final Values

            rectTransform.position = TransformAndGraphicAnimationRecords_HIDE[i].Position;
            rectTransform.localScale = TransformAndGraphicAnimationRecords_HIDE[i].Scale;
            rectTransform.rotation = Quaternion.LookRotation(TransformAndGraphicAnimationRecords_HIDE[i].RotationEuler);
            rectTransform.pivot = TransformAndGraphicAnimationRecords_HIDE[i].Pivot;
            rectTransform.anchorMin = TransformAndGraphicAnimationRecords_HIDE[i].AnchorMin;
            rectTransform.anchorMax = TransformAndGraphicAnimationRecords_HIDE[i].AnchorMax;

            if (graphicType == GraphicType.Image)
            {
                Image img = graphic as Image;
                if (TransformAndGraphicAnimationRecords_HIDE[i].Sprite)
                    img.sprite = TransformAndGraphicAnimationRecords_HIDE[i].Sprite;
                img.color = TransformAndGraphicAnimationRecords_HIDE[i].Color;
            }
            else if (graphicType == GraphicType.Text)
            {
                Text txt = graphic as Text;
                if (!string.IsNullOrEmpty(TransformAndGraphicAnimationRecords_HIDE[i].Text))
                    txt.text = TransformAndGraphicAnimationRecords_HIDE[i].Text;
                txt.color = TransformAndGraphicAnimationRecords_HIDE[i].Color;
            }

            #endregion
        }

        gameObject.SetActive(false);

        if (OnHideComplete != null)
            OnHideComplete.Invoke();
    }

    #endregion

    private void ResetAll()
    {
        rectTransform.position = initialRectValues.Position;
        rectTransform.localScale = initialRectValues.Scale;
        rectTransform.rotation = Quaternion.Euler(initialRectValues.RotationEuler);
        rectTransform.pivot = initialRectValues.Pivot;
        rectTransform.anchorMin = initialRectValues.AnchorMin;
        rectTransform.anchorMax = initialRectValues.AnchorMax;

        if (graphicType == GraphicType.Image)
        {
            Image img = graphic as Image;
            img.sprite = initialGraphicValues.Sprite;
            img.color = initialGraphicValues.Color;
        }

        else if (graphicType == GraphicType.Text)
        {
            Text txt = graphic as Text;
            txt.text = initialGraphicValues.Text;
            txt.color = initialGraphicValues.Color;
        }
    }

    public void Record(GameObject gObject, AnimationShowOrHide showOrHide)
    {
        rectTransform = gObject.GetComponent<RectTransform>();
        graphic = gObject.GetComponent<Graphic>();

        RecordValues(showOrHide, false);
    }

    public void EnterRecordMode(GameObject gObject, AnimationShowOrHide showOrHide)
    {
        rectTransform = gObject.GetComponent<RectTransform>();
        graphic = gObject.GetComponent<Graphic>();

        transformRecord_beforeRecrod = null;
        graphicRecord_beforeRecrod = null;
        transformAndGraphicRecord_beforeRecrod = null;

        RecordValues(showOrHide, true);
    }

    public void ExitRecordMode(GameObject gObject, AnimationShowOrHide showOrHide)
    {
        rectTransform = gObject.GetComponent<RectTransform>();
        graphic = gObject.GetComponent<Graphic>();

        ReturnValuesAfterRecord(gObject, showOrHide);
    }

    private void RecordValues(AnimationShowOrHide showOrHide, bool recordModeActive)
    {
        ComponentsToAnimate componentsToAnimate = (showOrHide == AnimationShowOrHide.Show) ? componentsToAnimate_SHOW : componentsToAnimate_HIDE;

        switch (componentsToAnimate)
        {
            case (ComponentsToAnimate.Transform):
                TransformAnimationRecord transformRecord = new TransformAnimationRecord()
                {
                    Duration = currentRecordDuration,
                    Delay = currentRecordDelay,

                    Position = rectTransform.position,
                    Scale = rectTransform.localScale,
                    RotationEuler = rectTransform.eulerAngles,
                    Pivot = rectTransform.pivot,
                    AnchorMin = rectTransform.anchorMin,
                    AnchorMax = rectTransform.anchorMax,
                };

                if (recordModeActive)
                {
                    transformRecord_beforeRecrod = transformRecord;
                    return;
                }

                if (showOrHide == AnimationShowOrHide.Show)
                    TransformAnimationRecords_SHOW.Add(transformRecord);
                else
                    TransformAnimationRecords_HIDE.Add(transformRecord);

                break;

            case (ComponentsToAnimate.Graphic):
                if (graphic is Image)
                {
                    img = graphic as Image;
                    graphicType = GraphicType.Image;
                }
                else if (graphic is Text)
                {
                    txt = graphic as Text;
                    graphicType = GraphicType.Text;
                }

                GraphicAnimationRecord graphicRecord = new GraphicAnimationRecord()
                {
                    Duration = currentRecordDuration,
                    Delay = currentRecordDelay,

                    Color = graphic.color,
                    Sprite = (graphicType == GraphicType.Image) ? img.sprite : null,
                    Text = (graphicType == GraphicType.Text) ? txt.text : ""
                };

                if (recordModeActive)
                {
                    graphicRecord_beforeRecrod = graphicRecord;
                    return;
                }

                if (showOrHide == AnimationShowOrHide.Show)
                    GraphicAnimationRecords_SHOW.Add(graphicRecord);
                else
                    GraphicAnimationRecords_HIDE.Add(graphicRecord);
                break;
            case (ComponentsToAnimate.Both):
                if (graphic is Image)
                {
                    img = graphic as Image;
                    graphicType = GraphicType.Image;
                }
                else if (graphic is Text)
                {
                    txt = graphic as Text;
                    graphicType = GraphicType.Text;
                }

                TransformAndGraphicAnimationRecord transformAndGrapihcRecord = new TransformAndGraphicAnimationRecord()
                {
                    Duration = currentRecordDuration,
                    Delay = currentRecordDelay,

                    Position = rectTransform.position,
                    Scale = rectTransform.localScale,
                    RotationEuler = rectTransform.eulerAngles,
                    Pivot = rectTransform.pivot,
                    AnchorMin = rectTransform.anchorMin,
                    AnchorMax = rectTransform.anchorMax,

                    Color = graphic.color,
                    Sprite = (graphicType == GraphicType.Image) ? img.sprite : null,
                    Text = (graphicType == GraphicType.Text) ? txt.text : ""
                };

                if (recordModeActive)
                {
                    transformAndGraphicRecord_beforeRecrod = transformAndGrapihcRecord;
                    return;
                }

                if (showOrHide == AnimationShowOrHide.Show)
                    TransformAndGraphicAnimationRecords_SHOW.Add(transformAndGrapihcRecord);
                else
                    TransformAndGraphicAnimationRecords_HIDE.Add(transformAndGrapihcRecord);

                break;
        }
    }

    private void ReturnValuesAfterRecord(GameObject gObject, AnimationShowOrHide showOrHide)
    {
        ComponentsToAnimate componentsToAnimate = (showOrHide == AnimationShowOrHide.Show) ? componentsToAnimate_SHOW : componentsToAnimate_HIDE;

        switch (componentsToAnimate)
        {
            case (ComponentsToAnimate.Transform):
                rectTransform.position = transformRecord_beforeRecrod.Position;
                rectTransform.localScale = transformRecord_beforeRecrod.Scale;
                rectTransform.pivot = transformRecord_beforeRecrod.Pivot;
                rectTransform.anchorMin = transformRecord_beforeRecrod.AnchorMin;
                rectTransform.anchorMax = transformRecord_beforeRecrod.AnchorMax;
                rectTransform.rotation = Quaternion.Euler(transformRecord_beforeRecrod.RotationEuler);

                break;
            case (ComponentsToAnimate.Graphic):
                graphic.color = graphicRecord_beforeRecrod.Color;

                if (graphic is Image)
                    gObject.GetComponent<Image>().sprite = graphicRecord_beforeRecrod.Sprite;
                else if (graphic is Text)
                    gObject.GetComponent<Text>().text = graphicRecord_beforeRecrod.Text;

                break;
            case (ComponentsToAnimate.Both):
                rectTransform.position = transformAndGraphicRecord_beforeRecrod.Position;
                rectTransform.localScale = transformAndGraphicRecord_beforeRecrod.Scale;
                rectTransform.pivot = transformAndGraphicRecord_beforeRecrod.Pivot;
                rectTransform.anchorMin = transformAndGraphicRecord_beforeRecrod.AnchorMin;
                rectTransform.anchorMax = transformAndGraphicRecord_beforeRecrod.AnchorMax;
                rectTransform.rotation = Quaternion.Euler(transformAndGraphicRecord_beforeRecrod.RotationEuler);

                graphic.color = transformAndGraphicRecord_beforeRecrod.Color;
                if (graphic is Image)
                    gObject.GetComponent<Image>().sprite = transformAndGraphicRecord_beforeRecrod.Sprite;
                else if (graphic is Text)
                    gObject.GetComponent<Text>().text = transformAndGraphicRecord_beforeRecrod.Text;

                break;
        }

        transformRecord_beforeRecrod = null;
        graphicRecord_beforeRecrod = null;
        transformAndGraphicRecord_beforeRecrod = null;
    }

    public enum ComponentsToAnimate
    {
        Transform, Graphic, Both
    }

    public enum AnimationShowOrHide
    {
        Show = 0, Hide = 1
    }

    private enum GraphicType
    {
        Image, Text
    }
}

[System.Serializable]
public class TransformAnimationRecord
{
    public float Delay;
    public float Duration = 0.5f;

    public Vector3 Position;
    public Vector3 Scale;
    public Vector3 RotationEuler;
    public Vector2 Pivot;
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;
}

[System.Serializable]
public class GraphicAnimationRecord
{
    public float Delay;
    public float Duration = 0.5f;

    [Tooltip("Only works if the game object has Image component")] public Sprite Sprite;
    [Tooltip("Only works if the game object has Text component")] public string Text;
    public Color Color;
}

[System.Serializable]
public class TransformAndGraphicAnimationRecord
{
    public float Delay;
    public float Duration = 0.5f;

    public Vector3 Position;
    public Vector3 Scale;
    public Vector3 RotationEuler;
    public Vector2 Pivot;
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;

    [Tooltip("Only works if the game object has Image component")] public Sprite Sprite;
    [Tooltip("Only works if the game object has Text component")] public string Text;
    public Color Color;
}