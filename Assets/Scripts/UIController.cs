using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Experience UI")]
    public Slider explvlSlider;
    [Tooltip("Arraste o componente Image do GameObject 'Fill' do seu Slider para cá. Ele é o responsável pela cor da barra de preenchimento.")]
    public Image expFillImage; 

    public TMP_Text expLvlText;

    public LevelUpSelectionButton[] levelUpButtons;

    public GameObject levelUpPanel;

    private void Awake()
    {
       
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        
    }

    public void UpdateExperience(int currentExp, int levelExp, int currentlvl)
    {
        explvlSlider.maxValue = levelExp;
        explvlSlider.value = currentExp;

        if (expLvlText != null)
        {
            expLvlText.text = "Commits: " + currentlvl;
        }

        
        if (expFillImage != null)
        {
            float fillPercentage = levelExp > 0 ? (float)currentExp / levelExp : 0f;

            // Cores baseadas nas cores de heatmap de commit do GitHub
            // Verde claro (equivalente a poucos commits)
            // Hex: #9BE9A8 -> RGB (155, 233, 168)
            Color startColor = new Color(155f / 255f, 233f / 255f, 168f / 255f);

            // Verde escuro (equivalente a muitos commits)
            // Hex: #216E39 -> RGB (33, 110, 57)
            Color endColor = new Color(33f / 255f, 110f / 255f, 57f / 255f);

            expFillImage.color = Color.Lerp(startColor, endColor, fillPercentage);

        }
        else
        {
            Debug.LogWarning("UIController: 'expFillImage' é nulo em UpdateExperience. A cor da barra não pode ser definida. Certifique-se de atribuí-lo no Inspector.");
        }
    }

    public void AnimateExpBarGain()
    {
        if (expFillImage != null)
        {
            StopAllCoroutines(); 
            StartCoroutine(FlashBarColorCoroutine(Color.green, 0.1f, 0.2f));
        }
    }

    public void AnimateExpBarLevelUp()
    {
        if (expFillImage != null)
        {
            StopAllCoroutines(); 
            // Cor dourada/amarela para o Level Up (Hex: #FFD700)
            StartCoroutine(FlashBarColorCoroutine(new Color(1f, 0.843f, 0f), 0.2f, 0.5f));
        }
    }

    // Coroutine genérica para fazer a barra piscar para uma cor e depois voltar à sua cor dinâmica
    private IEnumerator FlashBarColorCoroutine(Color flashColor, float flashDuration, float fadeBackDuration)
    {
        expFillImage.color = flashColor;

        yield return new WaitForSeconds(flashDuration);


        Color targetDynamicColor = expFillImage.color;

        float timer = 0f;
        while (timer < fadeBackDuration)
        {
            timer += Time.deltaTime;
            expFillImage.color = Color.Lerp(flashColor, targetDynamicColor, timer / fadeBackDuration);
            yield return null;
        }

        expFillImage.color = targetDynamicColor;
       
    }
}