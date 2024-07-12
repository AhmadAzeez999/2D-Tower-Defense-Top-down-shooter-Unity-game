using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Levels
{
    public int levelNum;
    public Image[] boneImages;
}

public class GameData : MonoBehaviour
{
    [SerializeField] List<Levels> levels = new List<Levels>();

    int bonesEarned = 1;

    [SerializeField] Sprite boneEarnedSprite;

    [SerializeField] Sprite emptyBoneSprite;

    public bool inLevel;

    private void Start()
    {
        LoadData();
    }

    public void AddBoneEarned()
    {
        bonesEarned++;
        UpdateBonesDisplay();
    }

    public int GetBonesEarned()
    {
        return bonesEarned;
    }

    public void UpdateBonesDisplay()
    {
        foreach (Levels level in levels)
        {
            for (int i = 0; i < level.boneImages.Length; i++)
            {
                level.boneImages[i].sprite = emptyBoneSprite;
            }

            for (int i = 0; i < bonesEarned; i++)
            {
                level.boneImages[i].sprite = boneEarnedSprite;
            }
        }
    }

    public void LoadData()
    {
        bonesEarned = PlayerPrefs.GetInt("BonesEarned");

        foreach (Levels level in levels)
        {
            int allBonesEarned;
            allBonesEarned = PlayerPrefs.GetInt("Level" + level.levelNum + " bones");

            for (int i = 0; i < allBonesEarned; i++)
            {
                level.boneImages[i].sprite = boneEarnedSprite;
            }
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("BonesEarned", bonesEarned);

        foreach (Levels level in levels)
        {
            PlayerPrefs.SetInt("Level" + level.levelNum + " bones", bonesEarned);
        }
    }
}
