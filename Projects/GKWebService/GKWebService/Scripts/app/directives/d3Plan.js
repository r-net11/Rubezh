// Директива для тэга <d3-plan></d3-plan>
// Отрисовка плана на канве
(function ()
{
    'use strict';

    // Получение размеров окна
    function getSize()
    {
        var myWidth = 0, myHeight = 0;
        if (typeof (window.innerWidth) == 'number')
        {
            // Non-IE
            myWidth = window.innerWidth;
            myHeight = window.innerHeight;
        }
        else
        {
            if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
            {
                // IE 6+ in 'standards compliant mode'
                myWidth = document.documentElement.clientWidth;
                myHeight = document.documentElement.clientHeight;
            }
            else
            {
                if (document.body && (document.body.clientWidth || document.body.clientHeight))
                {
                    // IE 4 compatible
                    myWidth = document.body.clientWidth;
                    myHeight = document.body.clientHeight;
                }
            }
        }

        var dim = Object.create(Object.prototype, {
            width: { writable: true, configurable: true, value: myWidth },
            height: { writable: true, configurable: true, value: myHeight }
        });
        return dim;
    }

    // Создание элемента всплывающей подсказки
    function createTip()
    {
        return d3.tip()
            .attr('class', 'd3-tip')
            .offset([-10, 0])
            .html(function (d) { return d; });
    }

    // Создание канвы
    function createSvg(element)
    {
        var dimensions = getSize();
        var svg = d3.select(element)
                        .append("svg")
                        .attr("width", dimensions.width - 30).attr("height", dimensions.height - 220)
                        .call(d3.behavior.zoom().on("zoom", function () { svg.attr("transform", "translate(" + d3.event.translate + ")" + " scale(" + d3.event.scale + ")") }))
                        .append("g");
        return svg;
    }

    // Получение канвы
    function getSvg()
    {
        var svg = d3.select("svg").select("g");
        return svg;
    }

    function elementOnContextMenu(item, menuItems, scope)
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
    }

    function renderImageElement(item, i, svg, tip, menuItems, scope)
    {
        svg.append("image").attr("xlink:href", "data:image/png;base64," + item.Image)
                .attr("x", item.X)
                .attr("y", item.Y)
                .attr("width", item.Width)
                .attr("height", item.Height)
                .attr("id", function (d) { return item.Name.replace(" ", "-") + i })
                // Обработка события наведения мыши
                .on('mouseover', function (d)
                {
                    var id = document.getElementById(item.Name.replace(" ", "-"));
                    tip.show(item.Hint, id);
                })
                // Обработка события прекращения наведения мыши
                .on('mouseout', function (d) { tip.hide(); })
                // Обработка события левого клика мыши
                .on('click', function (d) { })
                // Обработка события вызова контекстного меню
                .on('contextmenu', function (d, i) { elementOnContextMenu(item, menuItems, scope); });
    }

    function renderPathElement(item, i, svg, tip, menuItems, scope)
    {
        svg.append("svg:path")
            .attr("d", item.Path)
            .style("stroke-width", item.BorderThickness)
            .style("stroke", 'rgba(' + item.Border.R + ',' + item.Border.G + ',' + item.Border.B + ',' + item.Border.A + ')')
            .style("fill", 'rgba(' + item.Fill.R + ',' + item.Fill.G + ',' + item.Fill.B + ',' + item.Fill.A + ')')
            .attr("id", function (d) { return item.Name.replace(" ", "-") })
            // Обработка события наведения мыши
            .on('mouseover', function (d)
            {
                var nodeSelection = d3.select(this).style({
                    stroke: item.BorderMouseOver,
                    fill: item.FillMouseOver
                });
                var id = document.getElementById(item.Name.replace(" ", "-"));
                tip.show(item.Hint, id);
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
            .on('click', function (d) { })
            // Обработка события вызова контекстного меню
            .on('contextmenu', function (d, i) { elementOnContextMenu(item, menuItems, scope); });
    }

    // Отрисовка одного элемента на канве
    function renderShape(item, i, svg, tip, scope)
    {

        // Создаем элементы меню
        var menuItems = ["Свойства..."];

        // ========== ДАЛЕЕ - ОТРИСОВКА В ЗАВИСИМОСТИ ОТ ТИПА ОБЪЕКТА ================

        switch (item.Type)
        {
            case "GkDevice":
                {
                    renderImageElement(item, i, svg, tip, menuItems, scope);
                    break;
                }

            case "Path":
            case "Plan":
                {
                    renderPathElement(item, i, svg, tip, menuItems, scope);
                    break;
                }
            default:
            {
                console.log("Невозможно нарисовать фигуру неизвестного типа " + item.Type + ".");
            }
        }
    }

    // Отрисовка плана на канве на основе набора данных data
    function renderSvg(data, tip, scope)
    {
        var dimensions = getSize();
        var svg = getSvg();

        svg.attr("width", dimensions.width).attr("height", dimensions.height);
        svg.selectAll("*").remove();
        if (data == null)
            return;

        // Отрисовываем каждую фигуру
        data.forEach(function (item, i, arr) { renderShape(item, i, svg, tip, scope); });
    }

    // Модуль директивы
    angular.module('canvasApp.directives').directive('d3Plan', [
        function ()
        {
            return {
                restrict: 'EA',
                link: function (scope, iElement)
                {
                    // Инициализируем всплывающую подсказку
                    var tip = createTip();

                    // Получаем размеры окна, создаем канву и инициализируем ее параметры и реакции

                    var svgNew = createSvg(iElement[0]);

                    svgNew.call(tip);

                    // Перерисовка канвы при изменении размеров окна
                    window.onresize = function () { return scope.$apply(); };
                    scope.$watch(function () { return angular.element(window)[0].innerWidth; }, function () { return scope.render(scope.d3Data); }
                    );

                    // Перерисовка канвы при изменении входных данных
                    scope.$watch('d3Data', function (newVals, oldVals) { return scope.render(newVals); }, true);

                    // Функция отрисовки канвы
                    scope.render = function (data) { renderSvg(data, tip, scope); };
                }
            };
        }
    ]);
}());