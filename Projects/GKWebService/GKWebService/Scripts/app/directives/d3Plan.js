(function() {
    'use strict';

    // Директива для тэга <d3-plan></d3-plan>
    // Отрисовка плана на канве
    function getSize()
    {
        var myWidth = 0, myHeight = 0;
        if (typeof (window.innerWidth) == 'number')
        {
            // Non-IE
            myWidth = window.innerWidth;
            myHeight = window.innerHeight;
        }
        else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
            {
                // IE 6+ in 'standards compliant mode'
                myWidth = document.documentElement.clientWidth;
                myHeight = document.documentElement.clientHeight;
            }
            else if (document.body && (document.body.clientWidth || document.body.clientHeight))
            {
                // IE 4 compatible
                myWidth = document.body.clientWidth;
                myHeight = document.body.clientHeight;
            }

        var dim = Object.create(Object.prototype, {
            width: { writable: true, configurable: true, value: myWidth },
            height: { writable: true, configurable: true, value: myHeight }
        });
        return dim;
    }

    angular.module('canvasApp.directives').directive('d3Plan', [
        function ()
        {
            return {
                restrict: 'EA',
                link: function (scope, iElement)
                {
                    // Инициализируем всплывающую подсказку
                    var tip = d3.tip()
                        .attr('class', 'd3-tip')
                        .offset([-10, 0])
                        .html(function (d)
                        {
                            return "You're touching my " + d + "!";
                        });

                    // Получаем размеры окна, создаем канву и инициализируем ее параметры и реакции
                    var dimensions = getSize();
                    var svg = d3.select(iElement[0])
                        .append("svg")
                        .attr("width", dimensions.width - 30).attr("height", dimensions.height - 220)
                        .call(d3.behavior.zoom().on("zoom", function () { svg.attr("transform", "translate(" + d3.event.translate + ")" + " scale(" + d3.event.scale + ")") }))
                        .append("g");

                    svg.call(tip);

                    // Перерисовка канвы при изменении размеров окна
                    window.onresize = function () { return scope.$apply(); };
                    scope.$watch(function () { return angular.element(window)[0].innerWidth; }, function () { return scope.render(scope.d3Data); }
                    );

                    // Перерисовка канвы при изменении входных данных
                    scope.$watch('d3Data', function (newVals, oldVals) { return scope.render(newVals); }, true);

                    // Функция отрисовки канвы
                    scope.render = function (data)
                    {
                        var dimensions = getSize();
                        svg.attr("width", dimensions.width - 30).attr("height", dimensions.height - 80);
                        
                        svg.selectAll("*").remove();
                        if (data == null)
                            return;

                        // Отрисовываем каждую фигуру
                        data.forEach(function (item, i, arr)
                        {
                            var menuItems = ["Подробно...", item.Name, "Элемент 3", "Элемент 4"];

                            svg.append("svg:path")
                                .attr("d", item.Path)
                                .style("stroke-width", 2)
                                .style("stroke", item.Border)
                                .style("fill", item.Fill)
                                .attr("id", function (d) { return item.Name.replace(" ", "-") })
                                // Обработка события наведения мыши
                                .on('mouseover', function (d)
                                {
                                    var nodeSelection = d3.select(this).style({
                                        stroke: item.BorderMouseOver,
                                        fill: item.FillMouseOver
                                    });
                                    var id = document.getElementById(item.Name.replace(" ", "-"));
                                    tip.show(item.Name, id);
                                })
                                // Обработка события прекращения наведения мыши
                                .on('mouseout', function (d)
                                {
                                    var nodeSelection = d3.select(this).style({
                                        stroke: item.Border,
                                        fill: item.Fill
                                    });
                                    tip.hide();
                                })
                                // Обработка события левого клика мыши
                                .on('click', function (d) { alert("Вы нажали на " + item.Name); })
                                // Обработка события вызова контекстного меню
                                .on('contextmenu', function (d, i)
                                {
                                    // Создаем div, представляющий контекстное меню
                                    d3.selectAll('.context-menu').data([1])
                                        .enter()
                                        .append('div')
                                        .attr('class', 'context-menu');

                                    // Закрытие контекстного меню
                                    d3.select('body').on('click.context-menu', function () { d3.select('.context-menu').style('display', 'none'); });

                                    // Отображение контекстного меню
                                    d3.selectAll('.context-menu')
                                        .html('')
                                        .append('ul')
                                        .selectAll('li')
                                        // Список названий пунктов меню
                                        .data(menuItems).enter()
                                        .append('li')
                                        // Задаем обработку клика на элемент меню
                                        .on('click', function (d)
                                        {
                                            // Открываем модальное окно
                                            scope.ShowModal('lg', item.Name);
                                            // Закрываем контекстное меню, т.к. по нему кликнули
                                            d3.select('.context-menu').style('display', 'none');
                                            return d;
                                        })
                                        .text(function (d) { return d; });
                                    d3.select('.context-menu').style('display', 'none');

                                    // Показываем контекстное меню
                                    d3.select('.context-menu')
                                        .style('left', (d3.event.pageX - 2) + 'px')
                                        .style('top', (d3.event.pageY - 2) + 'px')
                                        .style('display', 'block');
                                    d3.event.preventDefault();
                                });


                        });

                        //create the rectangles for the bar chart
                        //svg.selectAll("rect")
                        //    .data(data)
                        //    .enter()
                        //    .append("rect")
                        //    .on("click", function(d, i) { return scope.onClick({ item: d }); })
                        //    .attr("height", 30) // height of each bar
                        //    .attr("width", 0) // initial width of 0 for transition
                        //    .attr("x", 10) // half of the 20 side margin specified above
                        //    .attr("y", function(d, i) { return i * 35; }) // height + margin between bars
                        //    .transition()
                        //    .duration(1000) // time of duration
                        //    .attr("width", function(d) { return d.score / (max / width); }); // width based on scale

                        //svg.selectAll("text")
                        //    .data(data)
                        //    .enter()
                        //    .append("text")
                        //    .attr("fill", "#fff")
                        //    .attr("y", function(d, i) { return i * 35 + 22; })
                        //    .attr("x", 15)
                        //    .text(function(d) { return d[scope.label]; });

                    };
                }
            };
        }
    ]);
}());