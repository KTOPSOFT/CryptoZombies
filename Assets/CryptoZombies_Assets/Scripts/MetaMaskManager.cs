using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class MetaMaskManager : MonoBehaviour
{
    // Name of the callback method Unity will receive from JS
    [SerializeField] string callbackMethod = "OnMetaMaskResponse";

    void Start()
    {
        // optional: set GameObject name programmatically if needed
        // gameObject.name = "MetaMaskReceiver";
    }

    // Wire this to a UI Button OnClick()
    public void OnConnectButton()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestAccounts(gameObject.name, callbackMethod);
#else
        Debug.LogWarning("MetaMask integration works only in a WebGL build running in a browser with MetaMask.");
        // Simulate a response for the Editor
        OnMetaMaskResponse("{\"error\":\"not_in_webgl_editor\"}");
#endif
    }

    // This signature must match the SendMessage from the .jslib: a single string parameter
    public void OnMetaMaskResponse(string json)
    {
        Debug.Log("MetaMask response: " + json);

        try
        {
            var info = JsonUtility.FromJson<MetaMaskInfo>(json);

            if (!string.IsNullOrEmpty(info.error))
            {
                Debug.LogError("MetaMask error: " + info.error);
                // show UI error, etc.
                return;
            }

            Debug.Log("Address: " + info.address);
            Debug.Log("ChainId: " + info.chainId);
            Debug.Log("Balance (ETH): " + info.balance);

            // Do whatever you need with the info (store, display, send to server, etc.)
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse MetaMask JSON: " + e.Message);
        }
    }

    [System.Serializable]
    public class MetaMaskInfo
    {
        public string address;
        public string chainId;
        public string balance;
        public string error;
    }

    // WebGL plugin binding - make sure this matches the exported name in the jslib
    [DllImport("__Internal")]
    private static extern void RequestAccounts(string gameObject, string callbackMethod);
}