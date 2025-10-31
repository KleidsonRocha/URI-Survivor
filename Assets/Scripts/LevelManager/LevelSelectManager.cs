using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class LevelSelectManager : MonoBehaviour
{
    [Tooltip("Arrastar aqui os botões de cada fase na ordem, do Level 1 ao último.")]
    public Button[] levelButtons; 

    [Tooltip("Opcional: Arrastar aqui os componentes TextMeshProUGUI (ou Text) dos botões, se necessário. Se deixado vazio, ele tentará encontrar automaticamente.")]
    public TextMeshProUGUI[] levelTexts; 
                                        

    void OnEnable() 
    {
        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager não encontrado! Certifique-se de que ele está carregado antes da cena do MainMenu.");
            return;
        }

        int currentUnlockedLevel = LevelManager.instance.GetCurrentUnlockedLevelIndex();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button button = levelButtons[i];
            TextMeshProUGUI buttonText = null; 

           
            if (levelTexts != null && levelTexts.Length > i && levelTexts[i] != null)
            {
                buttonText = levelTexts[i];
            }
            else
            {
               
                buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
              
            }

            if (buttonText == null)
            {
                Debug.LogWarning($"Botão '{button.name}' não possui um componente TextMeshProUGUI (ou Text) como filho. Não é possível atualizar o texto.");
            }

            
            button.onClick.RemoveAllListeners();

           
            string levelDisplayName = "";
            if (LevelManager.instance.levelDisplayNames != null && i < LevelManager.instance.levelDisplayNames.Length)
            {
                levelDisplayName = LevelManager.instance.levelDisplayNames[i];
            }
            else
            {
               
                levelDisplayName = $"Fase {i + 1}";
                Debug.LogWarning($"LevelManager.levelDisplayNames não possui um nome para o índice {i}. Usando fallback '{levelDisplayName}'.");
            }


            if (i <= currentUnlockedLevel)
            {
                button.interactable = true;


                if (LevelManager.instance.IsLevelCompleted(i))
                {
                    if (buttonText != null)
                    {
                        buttonText.text = $"{levelDisplayName}";
                        buttonText.color = Color.green; 
                    }
                }
                else
                {
                    if (buttonText != null)
                    {
                        buttonText.text = levelDisplayName;
                        buttonText.color = Color.white;
                    }
                }


                int levelIndexToLoad = i;
                button.onClick.AddListener(() => LevelManager.instance.LoadLevel(levelIndexToLoad));
            }
            else
            {
  
                button.interactable = false; 
                if (buttonText != null)
                {
                    buttonText.text = $"{levelDisplayName} (Bloqueada)";
                    buttonText.color = Color.gray; 
                }
            }
        }
    }

    public void OnClickResetProgress()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.ResetGameProgress();
            UpdateLevelButtons(); 
            Debug.Log("Progresso do jogo resetado.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Saindo!");
    }
}