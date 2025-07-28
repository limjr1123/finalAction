using UnityEngine;

public class GoldSystem : MonoBehaviour, ICurrencySystem
{
    private int gold = 1000;

    public bool TrySpend(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public int GetBalance() => gold;

    public void AddCurrency(int amount)
    {
        gold += amount;
    }
}