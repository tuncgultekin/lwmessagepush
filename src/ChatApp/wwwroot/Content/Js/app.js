
/**
 *   Constants
 */
var PERSON_ROW_NOTIFICATION_TEMPLATE = "<div class=\"person-notification\"><img style=\"width:20px;height:20px;\" src=\"Content/Images/new-message.png\"></div>";
var PERSON_ROW_TEMPLATE = "<div id=\"personRow_{{personName}}\" class=\"chat-person\" onclick=\"personSelected('{{personName}}',this)\"><div class=\"chat-person-icon\"><img style=\" padding:left: 10px; width:32px; height:32px;\" src=\"{{personIcon}}\" /></div><div class=\"chat-person-name\">{{personName}}</div></div>"
var PING_INTERVAL_MS = 15000;


/**
 *   Globals
 */
var selectedPerson;       // Username of actively selected person from user list
var chatContext = {};     // Context information for active conversations 
var connected = false;    // Determines current connection status
var commandClient;        // LWMessagePush client for app wise commands i.e: UpdateUserList
var chatClient;           // LWMessagePush client for messages
var currentNickName;      // Username of active user
var pingerTask;           // Pinger timer variable


/**
 *   Starts a task to send periodic ping requests for active user
 */
function startPinger() {
    pingerTask = setInterval(function () {

        $.post("api/Ping", { userName: currentNickName });

    }, PING_INTERVAL_MS);
}

/**
 *   Stops periodic ping requests
 */
function stopPinger() {
    clearInterval(pingerTask);
}

/**
 *   Creates LWMessagePush channels for message and command clients
 */
function connectDisconnect() {

    if (!connected) {

        // User list control channel (Command)
        commandClient = new LWMessagePushClient(
            function (notify) {

                // Update person list
                getPersonList();
            },
            window.location.href,
            "updateUsersReq",
            LWMessagePushConnectionMethod.Automatic,
            function (log) {
                console.log(log);
            });
        commandClient.connect();

        // Message push channel
        chatClient = new LWMessagePushClient(
            function (msg) {
                receiveMessage(msg);
            },
            window.location.href,
            currentNickName,
            LWMessagePushConnectionMethod.Automatic,
            function (log) {
                console.log(log);
            });
        chatClient.connect();

        connected = true;
    }

}

/**
 *   Selects person from user list and loads his/her chat context into MessageList
 *   
 *  @param {string} person selected username
 *  @param {domElement} domItem selected dom element
 */
function personSelected(person, domItem) {
    $(".chat-person").css("background-color", "");
    $(domItem).css("background-color", "cornflowerblue");
    selectedPerson = person;

    if (!chatContext[selectedPerson])
        chatContext[selectedPerson] = [];

    $("#messageList").empty();
    for (var i = 0; i < chatContext[selectedPerson].length; i++) {
        $("#messageList").append(chatContext[selectedPerson][i]);
    }

    enableMessageCell();

    // Remove unread message notification if there exist any
    $("#personRow_" + selectedPerson).find(".person-notification").remove();
}

/**
 *   Sends the value of "message_txt" input to selected user
 */
function sendMessage() {
    if ($("#message_txt").val() === "")
        return;

    if (!selectedPerson)
        return;

    $.post("api/SendMessageToUser", { "From": currentNickName, "To": selectedPerson, "Content": $("#message_txt").val() }, function (data) {

        var formattedMsg = getFormattedSendMessage($("#message_txt").val());

        if (!chatContext[selectedPerson])
            chatContext[selectedPerson] = [];

        chatContext[selectedPerson].push(formattedMsg);

        $("#messageList").append(formattedMsg);

        $("#message_txt").val("");

    }).fail(function (err) {
        alert("An error occured while sending a message: " + data);
    });
}

/**
 *   Appends received message into UI and chatContext
 *   @param {object} msg new message
 */
function receiveMessage(msg) {
    var formattedMsg = getFormattedIncomingMessage(msg);
    var msgObj = JSON.parse(msg.Content);

    if (!chatContext[msgObj.From])
        chatContext[msgObj.From] = [];

    chatContext[msgObj.From].push(formattedMsg);

    if (msgObj.From === selectedPerson)
        $("#messageList").append(formattedMsg);
    else {
        // Add unread message notification if sender is not currenty active
        $("#personRow_" + msgObj.From).find(".person-notification").remove();
        $("#personRow_" + msgObj.From).append(PERSON_ROW_NOTIFICATION_TEMPLATE);
    }

}

/**
 *   Wraps specified string message content with a dom element for proper visualization
 *   @param {string} msg message content
 *   @returns {string} message as div element
 */
function getFormattedSendMessage(msg) {
    return "<div class=\"sent-message\">" + msg + "</div>";
}

/**
 *   Extracts the content of incoming message object and wraps the content with a dom element for proper visualization
 *   @param {object} msg incoming message
 */
function getFormattedIncomingMessage(msg) {

    var msgObj = msg;

    // Use try-catch in-case of an unformatted message
    try {
        msgObj = msg.Content
        msgObj = JSON.parse(msgObj);
        msgObj = msgObj.Content;

    } catch (e) {

    }

    return "<div class=\"incoming-message\">" + msgObj + "</div>";
}

/**
 *   Fetchs the names of active users by performing ajax call and updates user list
 */
function getPersonList() {
    // ajax call to get people 
    $.get("api/GetUserList", function (data) {

        var newMessageNotifications = {};

        $.each($($(".person-notification").parent()), function (a, b) {
            newMessageNotifications[$(b).attr("id").replace("personRow_", "")] = true;
        });

        $("#personList").empty();
        for (var i = 0; i < data.length; i++) {
            if (currentNickName == data[i].username)
                continue;

            var newRow = PERSON_ROW_TEMPLATE;
            newRow = newRow.replace(new RegExp("{{personIcon}}", 'g'), "/Content/Images/user.png");
            newRow = newRow.replace(new RegExp("{{personName}}", 'g'), data[i].username);
            $("#personList").append(newRow);

            if (newMessageNotifications[data[i].username])
                $("#personRow_" + data[i].username).append(PERSON_ROW_NOTIFICATION_TEMPLATE);
        }

    });
}

/**
 *   Appends the filtered value (special characters) of nickname input into active users pool by performing ajax call
 */
function login() {
    currentNickName = $("#nickname").val();
    if (currentNickName == "")
        return;

    // Remove special characters
    currentNickName = currentNickName.replace(/[^\w\s]/gi, '');

    if (currentNickName == "")
        return;

    $.post("api/Login", { "userName": currentNickName }, function (data) {

        if (data !== true) {
            alert("Login error: A user is active for the specified user name.");
            return;
        }

        $("#loginPanel").hide();
        $("#mainPanel").show();
        $("#header").show();

        connectDisconnect();

        getPersonList();

        $("#logout-link").text(" Logout (" + currentNickName + ") ");

        startPinger();

    }).fail(function (err) {
        alert("Login error: " + err);
    });

}

/**
 *   Removes active user name from active users pool by performing ajax call
 */
function logout() {
    $.post("api/Logout", { "userName": currentNickName }, function (data) {

        stopPinger();
        chatContext = {};
        selectedPerson = null;
        $("#personList").empty();
        $("#messageList").empty();

        $("#loginPanel").show();
        $("#mainPanel").hide();
        $("#header").hide();

        commandClient.disconnect();
        chatClient.disconnect();
        connected = false;

    });
}

/**
 *   Enables UI's message sending section
 */
function enableMessageCell() {
    $("#message_txt").removeAttr("disabled", "");
    $(".message-cell").css("pointer-events", "");
}

/**
 *   Disables UI's message sending section
 */
function disableMessageCell() {
    $("#message_txt").removeAttr("disabled", "true");
    $(".message-cell").css("pointer-events", "none");
}


disableMessageCell();

