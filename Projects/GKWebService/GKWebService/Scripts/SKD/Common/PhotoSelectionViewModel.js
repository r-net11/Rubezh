function PhotoSelectionViewModel() {
    var self = {};

    self.OpenClick = function (data, e, photo) {
        self.Photo = photo;
        $("#OpenPhoto").val(null);
        $("#OpenPhoto").trigger('click');
    };

    self.Load = function (element) {
        var file = element.files[0];
        if (file) {
            var reader = new FileReader();

            reader.onload = function (readerEvt) {
                var binaryString = readerEvt.target.result;
                self.Photo("data:image/gif;base64," + btoa(binaryString));
            };

            reader.readAsBinaryString(file);
        }
    };

    self.RemoveClick = function (data, e, photo) {
        photo(null);
    };

    return self;
}