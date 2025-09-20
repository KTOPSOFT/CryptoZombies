mergeInto(LibraryManager.library, {
  ConnectWallet: function () {
    if (typeof window.ethereum !== 'undefined') {
      window.ethereum.request({ method: 'eth_requestAccounts' })
        .then((accounts) => {
          var account = accounts[0];
          // Send account back to Unity
          unityInstance.SendMessage("MetaMaskManager", "OnWalletConnected", account);
        })
        .catch((error) => {
          unityInstance.SendMessage("MetaMaskManager", "OnWalletError", error.message);
        });
    } else {
      alert("MetaMask not found. Please install MetaMask.");
    }
  }
});
