$(function ()
{
	// Reference the auto-generated proxy for the hub.  
	var plansUpdater = $.connection.plansUpdater;

	// Add client-side hub methods that the server will call
	$.extend(plansUpdater.client, {
		recieveTestMessage: function (message)
		{
			$('.message').empty();
			$('.message').append('<strong>' + htmlEncode(message)
			 + '</strong>');
		},

		updateDeviceState: function (stateData)
		{
			var uid = stateData.Id.replace(" ", "-");
			$("#" + uid).attr("href", "data:image/gif;base64," + stateData.Picture);
		},

        updateHint: function(stateData) {

            
        }
	});

	// Start the connection
	$.connection.hub.start()
		//.then(init)
		.then(function () { return plansUpdater.server.startTestBroadcast(); });
	// Вызывается, если серверный метод возвращает значение
	//.done(function (state)
	//{

	//});
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value)
{
	var encodedValue = $('<div />').text(value).html();
	return encodedValue;
}
