using NUnit.Framework; // Remover se n�o estiver usando para testes, pode causar warning se n�o for refer�ncia real
using System.Collections.Generic;
using UnityEngine;

public class ExperienceLevelController : MonoBehaviour
{
    public static ExperienceLevelController instance;

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

    public int currentExperience;

    public ExpPickup pickup;

    public List<int> expLevels;
    public int currentLevel = 1, levelCount = 100;

    void Start()
    {
        // Certifique-se de que a lista de n�veis n�o est� vazia para evitar erros
        if (expLevels.Count == 0)
        {
            expLevels.Add(0); // N�vel 0 precisa de 0 EXP para come�ar
            expLevels.Add(10); // N�vel 1 precisa de 10 EXP (primeiro n�vel real)
        }

        while (expLevels.Count < levelCount)
        {
            // Calcula o pr�ximo n�vel baseado no anterior, com um aumento de 10%
            expLevels.Add(Mathf.CeilToInt(expLevels[expLevels.Count - 1] * 1.1f));
        }

        if (UIController.instance != null)
        {
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }
    }

    void Update()
    {

    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;


        if (UIController.instance != null)
        {
            UIController.instance.AnimateExpBarGain();
        }

        if (currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
        }

        else if (UIController.instance != null)
        {
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }
    }

    public void SpawnExp(Vector3 position, int xpValue)
    {
        if (pickup != null)
        {
            Instantiate(pickup, position, Quaternion.identity).expValue = xpValue;
        }
        else
        {
            Debug.LogWarning("ExperienceLevelController: ExpPickup prefab n�o atribu�do. N�o � poss�vel spawnar EXP.");
        }
    }

    void LevelUp()
    {
        // Remove a EXP necess�ria para o n�vel atual
        currentExperience -= expLevels[currentLevel];
        currentLevel++; 

        // Impede que o n�vel exceda o m�ximo definido
        if (currentLevel >= expLevels.Count)
        {
            currentLevel = expLevels.Count - 1;
            currentExperience = 0; 
        }


        if (UIController.instance != null)
        {
            UIController.instance.AnimateExpBarLevelUp();
           
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }

        // Voc� pode adicionar mais l�gica aqui, como conceder pontos de habilidade, restaurar vida, etc.

        Debug.Log($"Parab�ns! Voc� subiu para o n�vel {currentLevel}!");
    }
}