"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/translate").build();

// this is from the server
connection.on("UpdateTranslationResults", function (result) {
       alert('updating translation results: '+result);
        console.log(connection);

        // connection id: connection.connectionId
    });
    
// this is to the server
    connection.start().then(function () {
        connection.invoke("InvokeTranslation",[{
            query: 'query',
            fromLanguageCode: '',
            toLanguageCode: ''
        }]).then(v => {
            // noop
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });

// document.getElementById("sendButton").addEventListener("click", function (event) {
//     var user = document.getElementById("userInput").value;
//     var message = document.getElementById("messageInput").value;
//     connection.invoke("SendMessage", user, message).catch(function (err) {
//         return console.error(err.toString());
//     });
//     event.preventDefault();
// });
