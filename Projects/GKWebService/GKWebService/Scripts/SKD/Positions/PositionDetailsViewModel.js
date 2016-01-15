function PositionDetailsViewModel() {
    var self = {};

    self.Title = ko.observable();
    self.IsNew = ko.observable(false);

    self.positionDetailsPages = {
        General: ko.observable(true),
        Photo: ko.observable(false),
    };

    $.ajax({
        dataType: "json",
        url: "Positions/GetPositionDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.Init = function (organisationUID, uid, okClick) {
        $.getJSON("/Positions/GetPositionDetails/",
            { organisationId: organisationUID, id: uid },
            function (pos) {
                ko.mapping.fromJS(pos, {}, self);
                self.OkClick = okClick;
                self.IsNew(false);
                if (uid) {
                    self.Title("Свойства должности: " + self.Position.Name());
                } else {
                    self.Title("Создание должности");
                    self.IsNew(true);
                }
                ShowBox('#position-details-box');
        })
        .fail(function (jqxhr, textStatus, error) {
            ShowError(jqxhr.responseText);
        });
    };

    self.PositionDetailsPageClick = function (data, e, page) {
        for (var propertyName in self.positionDetailsPages) {
            self.positionDetailsPages[propertyName](false);
        }

        self.positionDetailsPages[page](!self.positionDetailsPages[page]());
        $('div#position-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.PositionDetailsClose = function () {
        CloseBox();
    };

    self.SavePosition = function () {
        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: "Positions/PositionDetails",
            type: "post",
            contentType: "application/json",
            data: "{'position':" + data + ",'isNew': '" + self.IsNew() + "'}",
            success: function (error) {
                if (self.OkClick) {
                    self.OkClick();
                }
                CloseBox();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ShowError(xhr.responseText);
            }
        });
    };

    return self;
}