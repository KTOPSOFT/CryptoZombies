mergeInto(LibraryManager.library, {
  RequestAccounts: function(gameObjectPtr, callbackMethodPtr) {
    var gameObject = UTF8ToString(gameObjectPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);

    function send(obj) {
      try {
        var s = JSON.stringify(obj);
        // Send back to Unity
        SendMessage(gameObject, callbackMethod, s);
      } catch (e) {
        SendMessage(gameObject, callbackMethod, JSON.stringify({ error: String(e) }));
      }
    }

    if (typeof window === 'undefined' || !window || !window.ethereum) {
      send({ error: 'no_metamask' });
      return;
    }

    // Request accounts (this will open MetaMask prompt)
    window.ethereum.request({ method: 'eth_requestAccounts' })
      .then(function(accounts) {
        var address = (accounts && accounts.length > 0) ? accounts[0] : '';
        // Fetch chainId and balance in parallel
        Promise.all([
          window.ethereum.request({ method: 'eth_chainId' }),
          address ? window.ethereum.request({ method: 'eth_getBalance', params: [address, 'latest'] }) : Promise.resolve(null)
        ]).then(function(results) {
          var chainId = results[0] || null;
          var balanceHex = results[1] || null;
          var balance = null;
          try {
            if (balanceHex !== null) {
              // convert hex wei to decimal ETH (as a string)
              var wei = parseInt(balanceHex, 16);
              balance = (wei / Math.pow(10, 18)).toString();
            }
          } catch (e) {
            balance = balanceHex; // fallback to raw
          }
          send({ address: address, chainId: chainId, balance: balance });
        }).catch(function(err) {
          send({ address: address, error: err && err.message ? err.message : String(err) });
        });
      })
      .catch(function(err) {
        // user rejected or other error
        send({ error: err && err.message ? err.message : String(err) });
      });
  }
});
