"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/translate").build();

// this is from the server
connection.on("UpdateTranslationResults", function (translator,priority,result) {
    const translatorPriority=+priority;
    const translationResultElementId="translationResults_"+translator;
    let div = $("<div>", {id: translationResultElementId, "class": "translation-result translation-result-"+translator, 'data-priority':priority});
    div.append(result);

    // iterate through elements inside #translationResults
    // as soon as see a priority < incoming priority, append content
    const previousResults=$('#translationResults').children('.translation-result');
    if (previousResults.length===0) {
        // no other items, so just insert here
        $("#translationResults").append(div);
    } else {
        previousResults.each(function (ordinal) {
            ordinal++;
            const previousTranslatorPriority=+($(this).attr('data-priority'));
            if (previousTranslatorPriority<=translatorPriority) { 
                $("#translationResults").append(div);
            }
        });
    }
    
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
        ordinalOfFirstResult: 0
    };
    connection.invoke("InvokeTranslation",[translationRequest]).then(v => {
        // noop, ignoring the promise for now
    });
}).catch(function (err) {
    return console.error(err.toString());
});

