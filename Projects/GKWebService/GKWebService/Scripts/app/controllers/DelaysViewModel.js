$(document).ready(function () {
	$("#jqGridDelays").jqGrid({
		url: '/Home/GetDelays',
		datatype: "json",
		colModel: [
			{ label: '№', name: 'Number', width:5, key: true, hidden: false, sortable: false },
			{ label: 'Задержка', name: 'Name', width:25, hidden: false, sortable: false },
			{ label: 'Логика включения', name: 'PresentationLogic', width: 75, hidden: false, sortable: false },
			{ label: 'Задержка', name: 'OnDelay', width: 10, hidden: false, sortable: false },
			{ label: 'Удержание', name: 'HoldDelay', width:10, hidden:false, sortable: false}
		],
		width: jQuery(window).width() - 242,
		height: 250,
		rowNum: 100,
		viewrecords: true
	});
});

function DelaysViewModel() {
	var self = {};

	self.No = ko.observable();
	self.Name = ko.observable();

	$('#jqGridDelays').on('jqGridSelectRow', function (event, id, selected) {

		var myGrid = $('#jqgrid');

		self.No(myGrid.jqGrid('getCell', id, 'No'));
		self.Name(myGrid.jqGrid('getCell', id, 'Name'))

	});
	return self;
}