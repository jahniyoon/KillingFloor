using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

[System.Serializable]
public class ItemToBuy : MonoBehaviour
{
    //[Mijeong] 230926 Store Test
    public string Name;
    public int Cost;
    public int GainPerSecond;

    //LEGACY:

    public void BuyItem()
    {
        var request = new SubtractUserVirtualCurrencyRequest { VirtualCurrency = "CN", Amount = Cost };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractCoinsSuccess, OnError);
    }

    void OnSubtractCoinsSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Bought Item: " + Name);

        NetworkManager.instance.GetVirtualCurrencies();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
}
