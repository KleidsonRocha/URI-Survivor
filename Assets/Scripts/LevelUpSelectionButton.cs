using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpSelectionButton: MonoBehaviour
{
    public TMP_Text upgradeDescText, nameLevelText;
    public Image weaponIcon;

    private Weapon assignedWeapon;

    public void UpdateButtonDisplay(Weapon theWeapon)
    {
        if(theWeapon.gameObject.activeSelf == true)
        {
            upgradeDescText.text = theWeapon.stats[theWeapon.weaponLevel].UpgradeText;
            weaponIcon.sprite = theWeapon.icon;

            nameLevelText.text = theWeapon.name + " - Lvl " + theWeapon.weaponLevel;
        } else
        {
            upgradeDescText.text = "Desbloquear: " + theWeapon.name;
            weaponIcon.sprite = theWeapon.icon;

            nameLevelText.text = theWeapon.name;
        }

            assignedWeapon = theWeapon;

    }

    public void SelectUpgrade()
    {
        if (assignedWeapon != null)
        {
            if(assignedWeapon.gameObject.activeSelf == true)
            {
                assignedWeapon.LevelUp();

            } else
            {
                PlayerController.Instance.AddWeapon(assignedWeapon);
            }

                UIController.instance.levelUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
