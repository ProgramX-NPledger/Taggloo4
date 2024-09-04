"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/translate").build();

// this is from the server
connection.on("UpdateTranslationResults", function (translator,priority,result) {
    console.log(translator,priority);
    const translationResultElementId="translationResults_"+translator;
    let div = $("<div>", {id: translationResultElementId, "class": "translation-result translation-result-"+translator});
    div.append(result);

    $("#translationResults").append(div);
});
    
// build the translation request from the query string and submit to server, which will respond as and when ready 
// though UpdateTranslationResults callback
connection.start().then(function () {
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });
    let translationRequest = {
        fromLanguageCode: params.FromLanguageCode,
        toLanguageCode: params.ToLanguageCode,
        query: params.Query,
        ordinalOfFirstResult: 0,
        maximumResults: 5        
    };
    connection.invoke("InvokeTranslation",[translationRequest]).then(v => {
        // noop, ignoring the promise for now
    });
}).catch(function (err) {
    return console.error(err.toString());
});

