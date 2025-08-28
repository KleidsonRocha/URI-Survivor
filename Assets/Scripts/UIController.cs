using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController: MonoBehaviour 
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public Slider explvlSlider;
    public TMP_Text expLvlText;

    public void UpdateExperience(int currentExp, int levelExp, int currentlvl)
    {
        explvlSlider.maxValue = levelExp;
        explvlSlider.value = currentExp;

        expLvlText.text = "Commits: " + currentlvl;
    }
}
