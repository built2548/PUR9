using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BulletLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image bulletIcon;
    [SerializeField] private Sprite[] levelIcons; 

    // This is the method Puri_Script calls
    public void UpdateBulletUI(int currentLevel)
    {
        // Update Text
        if (levelText != null)
        {
            levelText.text = "Weapon: LVL " + (currentLevel + 1).ToString();
        }

        // Update Icon if you have sprites assigned
        if (bulletIcon != null && levelIcons != null && currentLevel < levelIcons.Length)
        {
            bulletIcon.sprite = levelIcons[currentLevel];
        }
    }
}