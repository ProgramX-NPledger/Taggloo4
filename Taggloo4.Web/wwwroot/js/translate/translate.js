"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/translate").build();

// this is from the server
connection.on("UpdateTranslationResults", function (translator,priority,result) {
    const translationResultElementId="translationResults_"+translator;
    let div = $("<div>", {id: translationResultElementId, "class": "translation-result translation-result-"+translator, 'data-priority':priority});
    div.append(result);

    // iterate through elements inside #translationResults
    // as soon as see a priority > incoming priority, prepend content

    $('#translationResults').children('.translation-result').each(function (ordinal) {
        //alert(this.value); // "this" is the current element in the loop
        console.log(translator,ordinal,$(this).attr('data-priority'));
        const nextPriority=$(this).attr('data-priority');
        console.log(nextPriority,priority,nextPriority>=priority);
        if (nextPriority>=priority) { // this isn't working
            $("#translationResults").prepend(div);
        }
    });
    
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

