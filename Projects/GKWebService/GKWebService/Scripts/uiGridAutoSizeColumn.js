(function(){
    'use strict';

    var module = angular.module('ui.grid.autoSizeColumn', ['ui.grid']);

    module.service('uiGridAutoSizeColumnService', ['gridUtil', '$q', '$timeout', 'uiGridConstants',
      function (gridUtil, $q, $timeout, uiGridConstants) {

          var service = {
              registerPublicApi: function (grid, $elm) {
                  var publicApi = {
                      methods: {
                          autoSize: {
                              fit: function (colDef) {
                                  var column = grid.getColumn(colDef.name);
                                  var colResizer = $elm[0].querySelectorAll('.' + uiGridConstants.COL_CLASS_PREFIX + column.uid + ' .ui-grid-column-resizer');
                                  angular.element(colResizer[0]).triggerHandler('dblclick');
                              }
                          }
                      }
                  };
                  grid.api.registerMethodsFromObject(publicApi.methods);
              },

              fireColumnSizeChanged: function (grid, colDef, deltaChange) {
                  $timeout(function () {
                      if (grid.api.colResizable) {
                          grid.api.colResizable.raise.columnSizeChanged(colDef, deltaChange);
                      } else {
                          gridUtil.logError("The resizeable api is not registered, this may indicate that you've included the module but not added the 'ui-grid-resize-columns' directive to your grid definition.  Cannot raise any events.");
                      }
                  });
              },

              // get either this column, or the column next to this column, to resize,
              // returns the column we're going to resize
              findTargetCol: function (col, position, rtlMultiplier) {
                  var renderContainer = col.getRenderContainer();

                  if (position === 'left') {
                      // Get the column to the left of this one
                      var colIndex = renderContainer.visibleColumnCache.indexOf(col);
                      return renderContainer.visibleColumnCache[colIndex - 1 * rtlMultiplier];
                  } else {
                      return col;
                  }
              }

          };

          return service;

      }]);

    module.directive('uiGridAutoSizeColumn', ['gridUtil', 'uiGridAutoSizeColumnService', function (gridUtil, uiGridAutoSizeColumnService) {
        return {
            replace: true,
            priority: 0,
            require: '^uiGrid',
            scope: false,
            compile: function () {
                return {
                    pre: function ($scope, $elm, $attrs, uiGridCtrl) {
                        //uiGridAutoSizeColumnService.defaultGridOptions(uiGridCtrl.grid.options);
                        //uiGridCtrl.grid.registerColumnBuilder(uiGridResizeColumnsService.colResizerColumnBuilder);
                        uiGridAutoSizeColumnService.registerPublicApi(uiGridCtrl.grid, $elm);
                    },
                    post: function ($scope, $elm, $attrs, uiGridCtrl) {
                    }
                };
            }
        };
    }]);

})();