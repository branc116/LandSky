//hitMe / hitUs : sends the server the current timestamp and the number of calls to make back to the client.  hitMe: just the callign client, hitUs: all clients on the hub.
//stepOne / stepAll : increments a counter
//doneOne / doneAll : prints # of messages and total duration.

$(function () {
    var playerHub = $.connection.playerHub;

    playerHub.client.updateFrame = function (newFrame) {
        $('#myScreen').text(newFrame);
    };

    playerHub.client.nameOk = function (ok) {
        if (true === ok) {
            $("#submitForm").hide();
            $(document).keypress(function (event) {
                playerHub.server.input(event.ctrlKey, event.altKey, event.key);
            });
        } else {
            $('#inputName').css("background-color", "red");
        }
    };

    $.connection.hub.start(options, function () { console.log("Connected"); });

    $("#submitForm").submit(function (event) {
        playerHub.server.insertNew($('#inputName').val());
        return false;
    });
});