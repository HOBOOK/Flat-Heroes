using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
	static IStoreController storeController = null;
	static string[] sProductIds;
	int nCoin;
    public static IAPManager instance;
    public int buyItemId;

	void Awake()
	{
        if (storeController == null)
		{
			sProductIds = new string[] { "9001", "9002","9003","9004","9005","9006","9011" };
			InitStore();
		}
        if (instance == null)
            instance = this;
		nCoin = 0;
        buyItemId = 0;

    }

	void InitStore()
	{
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct(sProductIds[0], ProductType.Consumable, new IDs { { sProductIds[0], GooglePlay.Name } });
		builder.AddProduct(sProductIds[1], ProductType.Consumable, new IDs { { sProductIds[1], GooglePlay.Name } });

		UnityPurchasing.Initialize(this, builder);
	}

	void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		storeController = controller;
		Debug.Log("결제기능 초기화");
	}

	void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializeFailed" + error);
    }

	public void OnBtnPurchaseClicked(int itemId)
	{
		if (storeController == null)
		{
			Debug.Log("구매 실패 : 결제 기능 초기화 실패");
        }
		else
        {
            buyItemId = itemId;
            int index = -1;
            for(var i = 0; i < sProductIds.Length; i++)
            {
                if(sProductIds[i].Equals(buyItemId)||sProductIds[i]==buyItemId.ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                storeController.InitiatePurchase(sProductIds[index]);
            else
                Debug.Log("구매 실패 : 결제 상품 존재하지 않음");
        }
	}

	PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
	{
		bool isSuccess = true;
#if UNITY_ANDROID && !UNITY_EDITOR
		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
		try
		{
			IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);
			for(int i = 0; i < result.Length; i++)
				Analytics.Transaction(result[i].productID, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode, result[i].transactionID, null);
		}
		catch (IAPSecurityException)
		{
			isSuccess = false;
		}
#endif
		if (isSuccess)
		{
			Debug.Log("구매 완료");
            if (e.purchasedProduct.definition.id.Equals(sProductIds[0]))
                SuccessPurchase(buyItemId);
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[1]))
                SuccessPurchase(buyItemId);
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[2]))
                SuccessPurchase(buyItemId);
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[3]))
                SuccessPurchase(buyItemId);
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[4]))
                SuccessPurchase(buyItemId);
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[5]))
                SuccessPurchase(buyItemId);
            else if (e.purchasedProduct.definition.id.Equals(sProductIds[6]))
                SuccessPurchase(buyItemId);
        }
		else
		{
            Debug.Log("구매 실패 : 비정상 결제");
		}

		return PurchaseProcessingResult.Complete;
	}

	void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
	{
		if (!error.Equals(PurchaseFailureReason.UserCancelled))
		{
			Debug.Log("구매 실패 : " + error);
		}
	}

    void SuccessPurchase(int buyItemId)
    {
        if (buyItemId > 9000)
            ItemSystem.SetObtainMoney(buyItemId);
        else
            ItemSystem.SetObtainItem(buyItemId);
        Item id = ItemSystem.GetItem(buyItemId);
        GoogleSignManager.SaveData();
        UI_Manager.instance.ShowGetAlert(id.image, string.Format("<color='yellow'>{0}</color> {1}", ItemSystem.GetItemName(id.id), LocalizationManager.GetText("alertGetMessage3")));
    }
}

