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

    public List<Weapon> weaponsToUpgrade;

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
        Debug.Log("N�vel inicial do jogador: " + currentLevel);
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

        UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        
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

        //PlayerController.Instance.activeWeapon.LevelUp();

        UIController.instance.levelUpPanel.SetActive(true);

        Time.timeScale = 0f;

        //UIController.instance.levelUpButtons[1].UpdateButtonDisplay(PlayerController.Instance.activeWeapon);
        //UIController.instance.levelUpButtons[0].UpdateButtonDisplay(PlayerController.Instance.assignedWeapons[0]);
        //UIController.instance.levelUpButtons[1].UpdateButtonDisplay(PlayerController.Instance.unassignedWeapons[0]);
        //UIController.instance.levelUpButtons[2].UpdateButtonDisplay(PlayerController.Instance.unassignedWeapons[1]);

        weaponsToUpgrade.Clear();

        List<Weapon> avaliableWeapons = new List<Weapon>();
        avaliableWeapons.AddRange(PlayerController.Instance.assignedWeapons);

        if(avaliableWeapons.Count > 0)
        {
            int selected = Random.Range(0, avaliableWeapons.Count);
            weaponsToUpgrade.Add(avaliableWeapons[selected]);
            avaliableWeapons.RemoveAt(selected);
        }

        if(PlayerController.Instance.assignedWeapons.Count + PlayerController.Instance.fullyLevelWeapons.Count < PlayerController.Instance.maxWeapons)
        {
            avaliableWeapons.AddRange(PlayerController.Instance.unassignedWeapons);
        }


        for (int i = weaponsToUpgrade.Count; i < 3; i++)
        {
            if (avaliableWeapons.Count > 0)
            {
                int selected = Random.Range(0, avaliableWeapons.Count);
                weaponsToUpgrade.Add(avaliableWeapons[selected]);
                avaliableWeapons.RemoveAt(selected);
            }
        }

        for (int i=0; i < weaponsToUpgrade.Count; i++)
        {
            UIController.instance.levelUpButtons[i].UpdateButtonDisplay(weaponsToUpgrade[i]);
        }

        for (int i = 0; i < UIController.instance.levelUpButtons.Length; i++) 
        {
            if (i < weaponsToUpgrade.Count)
            {
                UIController.instance.levelUpButtons[i].gameObject.SetActive(true);
            } else
            {
                UIController.instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }


        if (UIController.instance != null)
        {
            UIController.instance.AnimateExpBarLevelUp();
           
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
        }

        // Voc� pode adicionar mais l�gica aqui, como conceder pontos de habilidade, restaurar vida, etc.


    }
}