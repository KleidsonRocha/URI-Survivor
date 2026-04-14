using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

[Serializable]
public class SlideEntry
{
    public string slideName = "Slide";
    public Sprite mainSprite;
    public Sprite altSprite;

    [Min(0.05f)]
    public float frameDuration = 0.35f;

    [Tooltip("Objetos opcionais deste slide, como botoes, textos e animacoes extras.")]
    public GameObject overlayRoot;
}

public class SlideSequenceController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image slideImage;

    [Header("Slides")]
    [SerializeField] private List<SlideEntry> slides = new List<SlideEntry>();

    [Tooltip("Pasta usada pelo atalho 'Populate Slides From Folder'.")]
    [SerializeField] private string slidesFolder = "Assets/URI Survivor Assets/Slides";

    [Header("Flow")]
    [SerializeField] private string sceneToLoadAtEnd = "MainMenu";
    [SerializeField] private bool allowPreviousSlide;
    [SerializeField] private bool advanceWithMouseClick = true;

    [Min(0f)]
    [SerializeField] private float inputCooldown = 0.2f;

    private int currentSlideIndex;
    private float frameTimer;
    private float lastInputTime = float.NegativeInfinity;
    private bool showingAltSprite;

    private void Start()
    {
        TryPopulateSlidesInEditor();

        if (slideImage == null)
        {
            Debug.LogError("SlideSequenceController: atribua o componente Image que vai exibir os slides.");
            enabled = false;
            return;
        }

        if (slides == null || slides.Count == 0)
        {
            Debug.LogError("SlideSequenceController: nenhum slide configurado.");
            enabled = false;
            return;
        }

        ConfigureSlideImage();
        Time.timeScale = 1f;
        ShowSlide(0);
    }

    private void Update()
    {
        UpdateSlideAnimation();
        HandleInput();
    }

    public void NextSlide()
    {
        if (currentSlideIndex >= slides.Count - 1)
        {
            FinishSequence();
            return;
        }

        ShowSlide(currentSlideIndex + 1);
    }

    public void PreviousSlide()
    {
        if (!allowPreviousSlide || currentSlideIndex <= 0)
        {
            return;
        }

        ShowSlide(currentSlideIndex - 1);
    }

    public void FinishSequence()
    {
        if (string.IsNullOrWhiteSpace(sceneToLoadAtEnd))
        {
            Debug.LogWarning("SlideSequenceController: nenhum nome de cena foi configurado para o fim da sequencia.");
            return;
        }

        SceneManager.LoadScene(sceneToLoadAtEnd);
    }

    private void HandleInput()
    {
        if (Time.unscaledTime < lastInputTime + inputCooldown)
        {
            return;
        }

        if (WasAdvancePressed())
        {
            lastInputTime = Time.unscaledTime;
            NextSlide();
            return;
        }

        if (allowPreviousSlide && WasPreviousPressed())
        {
            lastInputTime = Time.unscaledTime;
            PreviousSlide();
        }
    }

    private bool WasAdvancePressed()
    {
        Keyboard keyboard = Keyboard.current;
        bool keyboardPressed = keyboard != null &&
                              (keyboard.spaceKey.wasPressedThisFrame ||
                               keyboard.enterKey.wasPressedThisFrame ||
                               keyboard.numpadEnterKey.wasPressedThisFrame ||
                               keyboard.rightArrowKey.wasPressedThisFrame);

        bool mousePressed = advanceWithMouseClick &&
                            Mouse.current != null &&
                            Mouse.current.leftButton.wasPressedThisFrame;

        return keyboardPressed || mousePressed;
    }

    private bool WasPreviousPressed()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null &&
               (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.backspaceKey.wasPressedThisFrame);
    }

    private void ShowSlide(int slideIndex)
    {
        if (slideIndex < 0 || slideIndex >= slides.Count)
        {
            Debug.LogWarning(string.Format("SlideSequenceController: indice de slide invalido {0}.", slideIndex));
            return;
        }

        currentSlideIndex = slideIndex;
        frameTimer = 0f;
        showingAltSprite = false;

        ApplyOverlayState();
        ApplyCurrentSprite();
    }

    private void UpdateSlideAnimation()
    {
        if (slides == null || slides.Count == 0 || currentSlideIndex >= slides.Count)
        {
            return;
        }

        SlideEntry currentSlide = slides[currentSlideIndex];
        if (currentSlide.mainSprite == null || currentSlide.altSprite == null)
        {
            return;
        }

        frameTimer += Time.unscaledDeltaTime;
        float currentFrameDuration = Mathf.Max(0.05f, currentSlide.frameDuration);

        if (frameTimer < currentFrameDuration)
        {
            return;
        }

        frameTimer = 0f;
        showingAltSprite = !showingAltSprite;
        ApplyCurrentSprite();
    }

    private void ApplyCurrentSprite()
    {
        SlideEntry currentSlide = slides[currentSlideIndex];

        Sprite spriteToDisplay = currentSlide.mainSprite;
        if (showingAltSprite && currentSlide.altSprite != null)
        {
            spriteToDisplay = currentSlide.altSprite;
        }
        else if (spriteToDisplay == null)
        {
            spriteToDisplay = currentSlide.altSprite;
        }

        slideImage.sprite = spriteToDisplay;
    }

    private void ApplyOverlayState()
    {
        for (int i = 0; i < slides.Count; i++)
        {
            if (slides[i].overlayRoot != null)
            {
                slides[i].overlayRoot.SetActive(i == currentSlideIndex);
            }
        }
    }

    private void ConfigureSlideImage()
    {
        if (slideImage == null)
        {
            return;
        }

        RectTransform rectTransform = slideImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        slideImage.type = Image.Type.Simple;
        slideImage.preserveAspect = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        ConfigureSlideImage();
        TryPopulateSlidesInEditor();
    }

    private void TryPopulateSlidesInEditor()
    {
        if (slides != null && slides.Count > 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(slidesFolder) || !AssetDatabase.IsValidFolder(slidesFolder))
        {
            return;
        }

        PopulateSlidesFromFolder();
    }

    [ContextMenu("Populate Slides From Folder")]
    private void PopulateSlidesFromFolder()
    {
        if (string.IsNullOrWhiteSpace(slidesFolder) || !AssetDatabase.IsValidFolder(slidesFolder))
        {
            Debug.LogError(string.Format("SlideSequenceController: pasta invalida '{0}'.", slidesFolder));
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { slidesFolder });
        SortedDictionary<int, SlideEntry> orderedSlides = new SortedDictionary<int, SlideEntry>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            Match match = Regex.Match(fileName, @"^slide_(\d+)(?:_alt)?$", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                continue;
            }

            int slideNumber = int.Parse(match.Groups[1].Value);
            bool isAltVariant = fileName.EndsWith("_alt", StringComparison.OrdinalIgnoreCase);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite == null)
            {
                continue;
            }

            SlideEntry slideEntry;
            if (!orderedSlides.TryGetValue(slideNumber, out slideEntry))
            {
                slideEntry = new SlideEntry
                {
                    slideName = string.Format("Slide {0:00}", slideNumber)
                };

                orderedSlides.Add(slideNumber, slideEntry);
            }

            if (isAltVariant)
            {
                slideEntry.altSprite = sprite;
            }
            else
            {
                slideEntry.mainSprite = sprite;
            }
        }

        slides = orderedSlides.Values.ToList();
        EditorUtility.SetDirty(this);

        Debug.Log(string.Format("SlideSequenceController: {0} slides carregados de '{1}'.", slides.Count, slidesFolder));
    }
#else
    private void TryPopulateSlidesInEditor()
    {
    }
#endif
}
