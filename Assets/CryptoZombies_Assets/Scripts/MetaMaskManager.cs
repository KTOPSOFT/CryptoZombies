using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class MetaMaskManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectWallet();

    public void OnConnectButton()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ConnectWallet();
#else
        Debug.Log("Wallet connection works only in WebGL build.");
#endif
    }

    // Called from JS on success
    public void OnWalletConnected(string account)
    {
        Debug.Log("Wallet connected: " + account);
        // Load next scene
        SceneManager.LoadScene("NextScene");
    }

    // Called from JS on error
    public void OnWalletError(string error)
    {
        Debug.LogError("Wallet error: " + error);
    }
}
