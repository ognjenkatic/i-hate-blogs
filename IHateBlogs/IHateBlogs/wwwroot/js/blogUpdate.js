"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/postCompletionHub").build();

connection.on("UpdatePost", function (message) {
    console.log(message);
    $('#preview').append(message);

});

connection.on("RefreshPost", function (message) {
    location.reload();
});

connection.start().then(function () {
    connection.invoke("WatchPost", postId).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});