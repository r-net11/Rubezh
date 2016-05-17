System.register(['./data.service', './gk.service'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    function exportStar_1(m) {
        var exports = {};
        for(var n in m) {
            if (n !== "default") exports[n] = m[n];
        }
        exports_1(exports);
    }
    return {
        setters:[
            function (data_service_1_1) {
                exportStar_1(data_service_1_1);
            },
            function (gk_service_1_1) {
                exportStar_1(gk_service_1_1);
            }],
        execute: function() {
        }
    }
});
//# sourceMappingURL=index.js.map