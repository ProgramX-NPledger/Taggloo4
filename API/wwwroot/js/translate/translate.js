"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/translate").build();

// this is from the server
connection.on("UpdateTranslationResults", function (result) {
    // add the results to the existing view model, which is bound to tranlsationResults div. this will allow sorting of results
    // but then how to combine with a view?
    
    console.log('UpdateTranslationResults',result);
    // create a new div for the results
    // use data binding in dynamic js view
    const translator = result.translator;
    const translationResultElementId="translationResults_"+translator;
    let div = $("<div>", {id: translationResultElementId, "class": "translation-result translation-result-"+translator});
    div.append(result);
    // don't like that this is going back to the server
    //div.load('/Translate/RenderResult',{translator: translator});

    //
        
    // TODO: dynamically load JS/HTML and bind
    // it would be ideal to be able to load some html and use the results as the viewmodel and bind to that

    $("#translationResults").append(div);
    console.log(document.getElementById(translationResultElementId));
 // 
    ko.bindingHandlers.translate = {
        'init': function() {
            console.log('bindingHandlers.init');
            return { 'controlsDescendantBindings': true }; // this line prevents parse "injected binding"
        },
        'update': function (element, valueAccessor) {
            console.log('bindingHandlers.update');
            // setHtml will unwrap the value if needed
            ko.utils.setHtml(element, valueAccessor());
        }
    };
    ko.applyBindings(result, document.getElementById(translationResultElementId));
    console.log('done');
    //
    // load a prtial view into that div
    // add the div to the wider results
    
    // $('#translationResults').load('') // https://stackoverflow.com/questions/25742315/rendering-a-partial-view-in-mvc-with-javascript
     });
    
// build the translation request from the query string and submit to server, which will respond as and when ready 
// though UpdateTranslationResults callbacj
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

