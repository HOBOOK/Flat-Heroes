using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AiryUIAnimatedElement))]
public class AiryUIBackButton : MonoBehaviour
{
    [Tooltip("if false, it will work only on devices that has back button, and ESC button on standalone devices. If true, there will be a button on the screen")]
    public bool withGraphic;

    public float showDelay = 1;

    public Positions position;

    public GraphicType graphicType;

    public Color imageColor = Color.white;
    public Color textColor = Color.white;

    [Range(0.1f, 5)] public float scale = 0.5f;

    public float offsetX = 0, offsetY = 0;

    public string buttonText = "Back";
    public Sprite graphicSprite;

    public Font font;

    [HideInInspector] public AiryUIBackButtonManager backButtonManager;

    public Button backButton;

    private Button backButtonPrefab;

    private AiryUIAnimatedElement animatedElement;
    private AiryUIAnimatedElement backBtnAnimatedElement;

    private RectTransform rectTransform;

    private Image img;
    private Text txt;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animatedElement = GetComponent<AiryUIAnimatedElement>();

        InstantiateManager();
    }

    private void Start()
    {
        if (withGraphic)
        {
            InstantiateBackButton();

            animatedElement.OnShow.AddListener(SetPositionAndScale);

            animatedElement.OnShow.AddListener(() => backButton.gameObject.SetActive(true));
            animatedElement.OnShow.AddListener(() => SetGraphics());
            animatedElement.OnShow.AddListener(() => SetPositionAndScale());
            animatedElement.OnShow.AddListener(() => backBtnAnimatedElement.ShowElement());
        }

        animatedElement.OnShow.AddListener(() => AiryUIBackButtonManager.Instance.AddButtonToList(this));
        animatedElement.OnHide.AddListener(() => AiryUIBackButtonManager.Instance.RemoveButtonFromList(this));
    }

    private void InstantiateManager()
    {
        backButtonManager = Resources.Load<AiryUIBackButtonManager>("Back Button Manager");

        if (AiryUIBackButtonManager.Instance == null)
        {
            string backButtonManagerName = backButtonManager.gameObject.name;
            backButtonManager = Instantiate(backButtonManager);
            backButtonManager.gameObject.name = backButtonManagerName;
        }
    }

    private void InstantiateBackButton()
    {
        // Instantiate Button and make it child of this game object

        if (backButton == null)
        {
            backButtonPrefab = Resources.Load<Button>("Back Button");

            backButton = Instantiate(backButtonPrefab, transform, false);
            backButton.onClick.AddListener(DoBackOnThisObject);
            backButton.name = backButtonPrefab.name;

            backBtnAnimatedElement = backButton.GetComponent<AiryUIAnimatedElement>();
            img = backButton.transform.Find("Image").GetComponent<Image>();
            txt = backButton.transform.Find("Text").GetComponent<Text>();

            backBtnAnimatedElement.withDelay = true;
            backBtnAnimatedElement.showDelay = showDelay;

            backBtnAnimatedElement.ShowElement();

            SetPositionAndScale();
            SetGraphics();
        }
        else
        {
            SetGraphics();
            SetPositionAndScale();
        }
    }

    private void OnEnable()
    {
        AiryUIBackButtonManager.Instance.AddButtonToList(this);
    }

    private void OnDisable()
    {
        AiryUIBackButtonManager.Instance.RemoveButtonFromList(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DoBack();
        }
    }

    public void DoBack()
    {
        AiryUIBackButtonManager.Instance.DoBack(this);
    }

    public void DoBackOnThisObject()
    {
        AiryUIBackButtonManager.Instance.DoBackOnCurrentObject(this);
    }

    private void SetPositionAndScale()
    {
        backButton.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
        backBtnAnimatedElement.initialScale = new Vector3(scale, scale, scale);

        switch (position)
        {
            case (Positions.TopRight):
                // Instantiate at top right
                backButton.GetComponent<RectTransform>().localPosition = new Vector3(rectTransform.rect.width / 2 - offsetX, rectTransform.rect.height / 2 - offsetY, 0);
                break;
            case (Positions.TopLeft):
                // Instantiate at top left
                backButton.GetComponent<RectTransform>().localPosition = new Vector3(-rectTransform.rect.width / 2 + offsetX, rectTransform.rect.height / 2 - offsetY, 0);
                break;
            case (Positions.BottomRight):
                // Instantiate at bottom right
                backButton.GetComponent<RectTransform>().localPosition = new Vector3(rectTransform.rect.width / 2 - offsetX, -rectTransform.rect.height / 2 + offsetY, 0);
                break;
            case (Positions.BottomLeft):
                // Instantiate at bottom left
                backButton.GetComponent<RectTransform>().localPosition = new Vector3(-rectTransform.rect.width / 2 + offsetX, -rectTransform.rect.height / 2 + offsetY, 0);
                break;
        }

        backBtnAnimatedElement.initialWorldPosition = backButton.transform.position;
        backBtnAnimatedElement.initialLocalPosition = backButton.transform.localPosition;
    }

    private void SetGraphics()
    {
        backBtnAnimatedElement.initialColorsOfChildren = new Color[2] { imageColor, textColor };

        if (graphicType == GraphicType.Image)
        {
            img.color = imageColor;
            img.sprite = graphicSprite;
            backButton.targetGraphic = img;
            txt.gameObject.SetActive(false);
        }
        else if (graphicType == GraphicType.Text)
        {
            txt.text = buttonText;
            txt.font = font;
            txt.color = textColor;
            backButton.targetGraphic = txt;

            img.gameObject.SetActive(false);
        }
        else if (graphicType == GraphicType.Both)
        {
            img.sprite = graphicSprite;
            img.color = imageColor;
            backButton.targetGraphic = img;

            txt.text = buttonText;
            txt.font = font;
            txt.color = textColor;
            backButton.targetGraphic = txt;
        }
    }

    public enum Positions { TopRight, TopLeft, BottomRight, BottomLeft }

    public enum GraphicType { Image, Text, Both }
}