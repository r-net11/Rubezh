

function MenuViewModel() {
	var self = {};

	self.pages = {
		State: ko.observable(false),
		Journal: ko.observable(false),
		Device: ko.observable(false),
		HR: ko.observable(false),
		Archive: ko.observable(false),
		Plan: ko.observable(false),
		FireZones: ko.observable(false),
		GuardZones: ko.observable(false),
     	Directions: ko.observable(false),
     	Delays: ko.observable(false),
     	MPTs: ko.observable(false),
     	PumpStations: ko.observable(false),
	    Doors: ko.observable(false)
	};

	self.StatePageOpened = ko.observable(false);

	self.PageClick = function (data, e, page) {
		for (var propertyName in self.pages) {
			self.pages[propertyName](false);
		}

        if (page) {
            self.pages[page](!self.pages[page]());
        }
		$('ul.menu li').removeClass("active");
		$(e.currentTarget).parent().addClass("active");

		if (page === 'State') {
		    angular.element($('#pageStates')).scope().menuStateClick();
		}
	}

	self.MenuClick = function (data, e) {
		console.log($(e.currentTarget).parent().toggleClass("clicked"));

	}

	return self;
}

function HeaderIconsViewModel() {
	var self = {};
	self.isSound = ko.observable(true);
	

	self.SoundClick = function () {
		self.isSound(!self.isSound());
	}

	self.LogonClick = function () {

		var loginBox = "#login-box";

		//Fade in the Popup
		$(loginBox).fadeIn(300);

		//Set the center alignment padding + border see css style
		var popMargTop = ($(loginBox).height() + 24) / 2;
		var popMargLeft = ($(loginBox).width() + 24) / 2;

		$(loginBox).css({
			'margin-top': -popMargTop,
			'margin-left': -popMargLeft
		});

		// Add the mask to body
		$('body').append('<div id="mask"></div>');
		$('#mask').fadeIn(300);

		return false;
	}

	self.LogonClose = function () {
		$('#mask , .login-popup').fadeOut(300, function () {
			$('#mask').remove();
		});

		return false;
	}

	self.login = ko.observable();
	self.password = ko.observable();
	self.logon_error = ko.observable();

	self.Enter = function () {
		$.post("Home/Logon",
			{ login: self.login, password: self.password }, function (data) {
				console.log(data);
				if (data.Success) {
					self.logon_error("");
					self.LogonClose();
				}
				else {
					self.logon_error(data.Message);
				}

		});
	}

    self.LogonClick();

	return self;
}

var app = new function AppViewModel() {
	var self = this;
	self.Menu = MenuViewModel();

	self.Header = HeaderIconsViewModel();

	self.Header.QuestionBox = QuestionBoxViewModel();

	self.Menu.Archive = ArchiveViewModel();
	self.Menu.HR = HRViewModel(self.Menu);

	return self;
};

ko.bindingHandlers.datepicker = {
	init: function (element, valueAccessor, allBindingsAccessor) {
		//Initialize datepicker with some optional options
		var options = allBindingsAccessor().datepickerOptions || {};
		$(element).datepicker(options);
		//Handle the field changing
		ko.utils.registerEventHandler(element, "change", function () {
			var observable = valueAccessor();
			var tempdate = $(element).datepicker("getDate");
			var tempdatestr = $.datepicker.formatDate("yy-mm-dd", tempdate);
			observable(tempdatestr);
		});
		//Handle disposal (if KO removes by the template binding)
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$(element).datepicker("destroy");
		});
	},
	update: function (element, valueAccessor) {
		var value = ko.utils.unwrapObservable(valueAccessor());
		value = new Date(value);
		var current = $(element).datepicker("getDate");
		if (value - current !== 0) {
			$(element).datepicker("setDate", value);
		}
	}
};

$.datepicker.setDefaults(
  $.extend(
	{ 'dateFormat': 'dd.mm.yy' },
	$.datepicker.regional['ru']
  )
);

function QuestionBoxViewModel() {
	var self = this;
	self.Message = ko.observable();

	self.InitQuestionBox = function (msg, yesClick) {
		self.Message(msg);
		self.YesClick = yesClick;
		ShowBox("#question-box");
	};

	self.OnYesClick = function () {
		CloseBox(self.YesClick);
	};

	self.Close = function() {
		CloseBox();
	};

	return self;
};

function ShowError(responseText) {
	if (responseText && JSON.parse(responseText).errorText) {
		alert(JSON.parse(responseText).errorText);
	} else {
		alert('Ошибка');
	}
}

function ShowBox(box) {
	//Fade in the Popup
	$(box).fadeIn(300, function () {
		$(this).trigger("fadeInComplete");
	});

	//Set the center alignment padding + border see css style
	var popMargTop = ($(box).height() + 24) / 2;
	var popMargLeft = ($(box).width() + 24) / 2;

	$(box).css({
		'margin-top': -popMargTop,
		'margin-left': -popMargLeft
	});

	// Add the mask to body
	$('body').append('<div id="mask"></div>');
	$('#mask').fadeIn(300);

	return false;
};

function CloseBox(complete) {
	$('.save-cancel-popup').fadeOut(300);
	$('#mask').fadeOut(300, function () {
		$('#mask').remove();
		if (complete) {
			complete();
		}
	});

	return false;
};

ko.applyBindings(app);
