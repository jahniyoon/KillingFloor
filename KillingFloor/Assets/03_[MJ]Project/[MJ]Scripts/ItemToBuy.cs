using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class ItemToBuy : MonoBehaviour
{
    public int coinPrice;
    public string itemName;

    public void BuyItem()
    {
        var request = new SubtractUserVirtualCurrencyRequest { VirtualCurrency = "CN", Amount = coinPrice };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractCoinsSuccess, OnError);
    }

    void OnSubtractCoinsSuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Bought Item: " + itemName);

        NetworkManager.instance.GetVirtualCurrencies();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
}
