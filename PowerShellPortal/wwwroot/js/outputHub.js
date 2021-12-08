"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/outputHub").build();

connection.on("show", function (message) {
    var results = document.getElementById("results");
    results.innerHTML += message;
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
