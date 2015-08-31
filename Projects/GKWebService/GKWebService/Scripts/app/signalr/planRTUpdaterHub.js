$(function ()
{
    // Reference the auto-generated proxy for the hub.  
    var chat = $.connection.plansrtstatusupdaterhub;
    // Create a function that the hub can call back to display messages.
    chat.client.addTestMessage = function (name, message)
    {
        // Add the message to the page. 
        $('#message').append('<strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message));
    };
   // Start the connection.
    $.connection.hub.start().done(function ()
    {
        
    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value)
{
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
