

function HeaderIconsViewModel() {
    var self = this;
    self.isSound = ko.observable(true);
    self.pages = {
        State: ko.observable(false),
        Device: ko.observable(false)
    };

    self.SoundClick = function () {
        self.isSound(!self.isSound());
    }

    self.StatePageOpened = ko.observable(false);

    self.PageClick = function (data, e, page) {

        for (var propertyName in self.pages) {
            self.pages[propertyName](false);
        }

        self.pages[page](!self.pages[page]());
        $('li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    }

    self.MenuClick = function (data, e) {
        console.log($(e.currentTarget).parent().toggleClass("clicked"));
        
    }
}

ko.applyBindings(new HeaderIconsViewModel());