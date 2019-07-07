using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
	static IStoreController storeController = null;
	static string[] sProductIds;

	[SerializeField] Text txtCoin;
	int nCoin;

	[SerializeField] Text txtLog;

	void Awake()
	{
        if (storeController == null)
		{
			sProductIds = new string[] { "coin_1000", "coin_5000" };
			InitStore();
		}
		nCoin = 0;
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
		txtLog.text = "결제 기능 초기화 완료";
		Debug.Log(txtLog.text);
	}

	void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
	{
		txtLog.text = "OnInitializeFailed" + error;
		Debug.Log(txtLog.text);
    }

	public void OnBtnPurchaseClicked(int index)
	{
		if (storeController == null)
		{
			txtLog.text = "구매 실패 : 결제 기능 초기화 실패";
			Debug.Log(txtLog.text);
        }
		else
			storeController.InitiatePurchase(sProductIds[index]);
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
				AddCoin(1000);
			else if (e.purchasedProduct.definition.id.Equals(sProductIds[1]))
				AddCoin(5000);
		}
		else
		{
			txtLog.text = "구매 실패 : 비정상 결제";
            Debug.Log(txtLog.text);
		}

		return PurchaseProcessingResult.Complete;
	}

	void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
	{
		if (!error.Equals(PurchaseFailureReason.UserCancelled))
		{
			txtLog.text = "구매 실패 : " + error;
			Debug.Log(txtLog.text);
		}
	}

	void AddCoin(int value)
	{
		txtLog.text = "AddCoin : " + value;
		Debug.Log(txtLog.text);
		nCoin += value;
		txtCoin.text = "Coin : " + nCoin.ToString("N0");
    }
}

