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
			var UID = stateData.Id.replace(" ", "-");
			$("#"+UID).attr("href", "data:image/gif;base64," + stateData.Picture);
		}

		//updateStockPrice: function (stock)
		//{
		//	var displayStock = formatStock(stock),
		//        $row = $(rowTemplate.supplant(displayStock)),
		//        $li = $(liTemplate.supplant(displayStock)),
		//        bg = stock.LastChange < 0
		//                ? '255,148,148' // red
		//                : '154,240,117'; // green

		//	$stockTableBody.find('tr[data-symbol=' + stock.Symbol + ']')
		//        .replaceWith($row);
		//	$stockTickerUl.find('li[data-symbol=' + stock.Symbol + ']')
		//        .replaceWith($li);

		//	$row.flash(bg, 1000);
		//	$li.flash(bg, 1000);
		//},

		//marketOpened: function ()
		//{
		//	$("#open").prop("disabled", true);
		//	$("#close").prop("disabled", false);
		//	$("#reset").prop("disabled", true);
		//	scrollTicker();
		//},

		//marketClosed: function ()
		//{
		//	$("#open").prop("disabled", false);
		//	$("#close").prop("disabled", true);
		//	$("#reset").prop("disabled", false);
		//	stopTicker();
		//},

		//marketReset: function ()
		//{
		//	return init();
		//}
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
