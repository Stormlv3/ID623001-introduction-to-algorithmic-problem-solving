using UnityEngine;

public class MonsterSlot : MonoBehaviour
{
    public MonsterData MonsterPrefab;
    private MonsterData placedMonster = null;

    void OnMouseUp()
    {
        if (placedMonster == null)
        {
            if (CanPlaceMonster())
            {
                PlaceMonster();
            }

        }
        else if (CanUpgradeMonster())
        {
            UpgradeMonster();
        }
    }

    private bool CanUpgradeMonster()
    {
        return placedMonster != null &&
            placedMonster.GetNextLevel() != null &&
            GameManager.Instance.Gold >= placedMonster.GetNextLevel().cost;
    }

    public void UpgradeMonster()
    {
        SFXManager.Instance.PlayTowerPlaced();
        placedMonster.IncreaseLevel();
        GameManager.Instance.Gold -= placedMonster.CurrentLevel.cost;
    }

    private bool CanPlaceMonster()
    {
        return placedMonster == null && GameManager.Instance.Gold >= MonsterPrefab.levels[0].cost;
    }

    public void PlaceMonster()
    {
        SFXManager.Instance.PlayTowerPlaced();
        placedMonster = Instantiate(MonsterPrefab, transform.position, Quaternion.identity);
        GameManager.Instance.Gold -= MonsterPrefab.levels[0].cost;
    }
}