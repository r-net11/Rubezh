// Директива для тэга <d3-plan></d3-plan>
// Отрисовка плана на канве
(function () {
	"use strict";

	// Получение размеров окна
	function getViewPortSize() {
		var header = $('div[class="' + 'container"]');
		var sidebar = $('aside[class="' + 'left-sidebar ng-scope"]');
		var plansList = $('div[id="' + 'plansGrid"]')[0];

		var dim = Object.create(Object.prototype, {
			width: { writable: true, configurable: true, value: header.width() - sidebar.width() - plansList.offsetWidth - 10 }
		});
		return dim;
	}

	// Создание элемента всплывающей подсказки
	function createTip() {
		return d3.tip()
			.attr("class", "d3-tip")
			.offset([-10, 0])
			.html(function (d) { return d; });
	}

	function renderTipContent(hintLines) {
		var hintHtml = "<table style='width:100%;border: 0px;'>";
		hintLines.forEach(function (item, i, arr) {
			hintHtml += "<tr><td style='width: 16px;border: 0px;padding: 1px'>";
			if (item.Icon)
				hintHtml += "<img src='data:image/png;base64," + item.Icon + "' width=14 height=14></img>";
			hintHtml += "</td>";
			hintHtml += "<td style='border: 0px;padding: 1px; valign: middle'>";
			if (item.Text)
				hintHtml += "<p>" + item.Text + "</p>";
			hintHtml += "</td>";
			hintHtml += "</tr>";
		});
		hintHtml += "</table>";
		return hintHtml;
	}

	function elementOnContextMenu(item, menuItems, scope) {
		// Создаем div, представляющий контекстное меню
		d3.selectAll(".context-menu").data([1])
			.enter()
			.append("div")
			.attr("class", "context-menu");

		// Закрытие контекстного меню
		d3.select("body").on("click.context-menu", function () { d3.select(".context-menu").style("display", "none"); });

		// Отображение контекстного меню
		d3.selectAll(".context-menu")
			.html("")
			.append("ul")
			.selectAll("li")
			// Список названий пунктов меню
			.data(menuItems).enter()
			.append("li")
			// Задаем обработку клика на элемент меню
			.on("click", function (d) {
				// Открываем модальное окно
				scope.ShowModal("lg", item);
				// Закрываем контекстное меню, т.к. по нему кликнули
				d3.select(".context-menu").style("display", "none");
				return d;
			})
			.text(function (d) { return d; });
		d3.select(".context-menu").style("display", "none");

		// Показываем контекстное меню
		d3.select(".context-menu")
			.style("left", (d3.event.pageX - 2) + "px")
			.style("top", (d3.event.pageY - 2) + "px")
			.style("display", "block");
		d3.event.preventDefault();
	}

	function renderPngImageElement(item, i, svg, tip, menuItems, scope, isGroupElement) {
		if (isGroupElement === true) {
			svg.append("image").attr("xlink:href", "data:image/png;base64," + item.Image)
				.attr("x", item.X)
				.attr("y", item.Y)
				.attr("width", item.Width)
				.attr("height", item.Height)
				.attr("preserveAspectRatio", "none")
				.attr("id", function (d) { return item.Id.replace(" ", "-") });
		} else {
			svg.append("image").attr("xlink:href", "data:image/png;base64," + item.Image)
			.attr("x", item.X)
			.attr("y", item.Y)
			.attr("width", item.Width)
			.attr("height", item.Height)
			.attr("preserveAspectRatio", "none")
			.attr("id", function (d) { return item.Id.replace(" ", "-") })
			// Обработка события наведения мыши
			//.on("mouseover", function (d) {
			//	var id = document.getElementById(item.Id.replace(" ", "-"));
			//	tip.show(renderTipContent(item.Hint.StateHintLines), id);
			//})
			// Обработка события прекращения наведения мыши
			.on("mouseout", function (d) { tip.hide(); })
			// Обработка события левого клика мыши
			.on("click", function (d) { })
			// Обработка события вызова контекстного меню
			.on("contextmenu", function (d, i) { elementOnContextMenu(item, menuItems, scope); });
		}
	}

	function renderGifImageElement(item, i, svg, tip, menuItems, scope, isGroupElement) {
		if (isGroupElement === true) {
			var image = svg.append("image").attr("xlink:href", "data:image/gif;base64," + item.Image)
				.attr("x", item.X)
				.attr("y", item.Y)
				.attr("width", item.Width)
				.attr("height", item.Height)
				.attr("id", function (d) { return item.Id.replace(" ", "-") })
				.on("updateImage", function (data, e, k) {
					console.log("Updating gif on:", item.Id);
					item.Image = data.Image;
					image.attr("xlink:href", "data:image/gif;base64," + data.Image);
				});

		}
	}

	function renderPathElement(item, i, svg, tip, menuItems, scope, isGroupElement) {
		if (isGroupElement === true) {
			svg.append("svg:path")
				.attr("d", item.Path)
				.attr("shape-rendering", "geometricPrecision") // Установка качественной отрисовки принудительно
				.style("stroke-width", item.BorderThickness)
				.style("stroke", "rgba(" + item.Border.R + "," + item.Border.G + "," + item.Border.B + "," + item.Border.A + ")")
				.style("fill", "rgba(" + item.Fill.R + "," + item.Fill.G + "," + item.Fill.B + "," + item.Fill.A + ")");
		} else {
			svg.append("svg:path")
			.attr("d", item.Path)
			.attr("shape-rendering", "geometricPrecision") // Установка качественной отрисовки принудительно
			.style("stroke-width", item.BorderThickness)
			.style("stroke", "rgba(" + item.Border.R + "," + item.Border.G + "," + item.Border.B + "," + item.Border.A + ")")
			.style("fill", "rgba(" + item.Fill.R + "," + item.Fill.G + "," + item.Fill.B + "," + item.Fill.A + ")")
			.attr("id", function (d) { return item.Id.replace(" ", "-"); })
			// Обработка события наведения мыши
			.on("mouseover", function (d) {
				if (item.Type === "Plan" || !item.HasOverlay)
					return;
				d3.select(this).style("stroke", "rgba(" + item.BorderMouseOver.R + "," + item.BorderMouseOver.G + "," + item.BorderMouseOver.B + "," + item.BorderMouseOver.A + ")").style("fill", "rgba(" + item.FillMouseOver.R + "," + item.FillMouseOver.G + "," + item.FillMouseOver.B + "," + item.FillMouseOver.A + ")");
				if (item.BorderThickness === 0)
					d3.select(this).style("stroke-width", 1);
				if (!item.Hint)
					return;
				var id = document.getElementById(item.Id.replace(" ", "-"));
				tip.show(renderTipContent(item.Hint.StateHintLines), id);
			})
			// Обработка события прекращения наведения мыши
			.on("mouseout", function (d) {
				if (item.Type === "Plan" || !item.HasOverlay)
					return;
				d3.select(this).style("stroke", "rgba(" + item.Border.R + "," + item.Border.G + "," + item.Border.B + "," + item.Border.A + ")").style("fill", "rgba(" + item.Fill.R + "," + item.Fill.G + "," + item.Fill.B + "," + item.Fill.A + ")");
				if (item.BorderThickness === 0)
					d3.select(this).style("stroke-width", 0);
				tip.hide();
			})
			// Обработка события левого клика мыши
			.on("click", function (d) { })
			// Обработка события вызова контекстного меню
			.on("contextmenu", function (d, i) {
				if (item.Type === "Plan" || item.IsInGroup)
					return;
				elementOnContextMenu(item, menuItems, scope);
			});
		}
	}

	function renderGroup(item, i, svg, tip, menuItems, scope) {
		var group = svg.append("g");
		item.ChildElements.forEach(function (childItem, j, arr) { renderShape(childItem, j, group, null, scope, true); });
		group.append("svg:rect")
			.attr("width", item.Width)
			.attr("height", item.Height)
			.attr("x", item.X)
			.attr("y", item.Y)
			.attr("fill", "none")
			.attr("pointer-events", "all")
			.attr("id", function (d) { return item.Id.replace(" ", "-") + "GroupElement" })
			// Обработка события наведения мыши
			.on("mouseover", function (d) {
				if (item.HasOverlay && item.BorderMouseOver) {
					d3.select(this).style("stroke", "rgba(" + item.BorderMouseOver.R + "," + item.BorderMouseOver.G + "," + item.BorderMouseOver.B + "," + item.BorderMouseOver.A + ")");
					d3.select(this).style("stroke-width", 1);
				}
				if (!item.Hint)
					return;
				var id = document.getElementById(item.Id.replace(" ", "-") + "GroupElement");
				tip.show(renderTipContent(item.Hint.StateHintLines), id);
			})
			// Обработка события прекращения наведения мыши
			.on("mouseout", function (d) {
				d3.select(this).style("stroke-width", 0);
				tip.hide();
			})
			// Обработка события левого клика мыши
			.on("click", function (d) { })
			// Обработка события вызова контекстного меню
			.on("contextmenu", function (d, i) { elementOnContextMenu(item, menuItems, scope); })
			.on("updateHint", function (data) {
				console.log("Updating hint on:", item.Id, item.Text);
				item.Hint = data.hint;
			});
	}

	// Отрисовка одного элемента на канве
	function renderShape(item, i, svg, tip, scope, isGroupElement) {

		// Создаем элементы меню
		var menuItems = ["Свойства..."];

		// ========== ДАЛЕЕ - ОТРИСОВКА В ЗАВИСИМОСТИ ОТ ТИПА ОБЪЕКТА ================
		if (!item || !item.Type)
			console.log("Невозможно нарисовать фигуру неизвестного типа.");
		else {
			switch (item.Type) {
				case "Group":
					{
						renderGroup(item, i, svg, tip, menuItems, scope);
						break;
					}
				case "Plan":
					{
						if (item.Image === "")
							renderPathElement(item, i, svg, tip, menuItems, scope, isGroupElement);
						else
							renderPngImageElement(item, i, svg, tip, menuItems, scope, isGroupElement);
						break;
					}
				case "GkDevice":
					{
						renderGifImageElement(item, i, svg, tip, menuItems, scope, isGroupElement);
						break;
					}
				case "Path":
					{
						renderPathElement(item, i, svg, tip, menuItems, scope, isGroupElement);
						if (item.Image !== "")
							renderPngImageElement(item, i, svg, tip, menuItems, scope, isGroupElement);
						break;
					}
				default:
					{
						console.log("Невозможно нарисовать фигуру неизвестного типа " + item.Type + ".");
					}
			}
		}
	}

	// Отрисовка плана на канве на основе набора данных data
	function renderSvg(tip, scope, element) {
		var data = scope.d3Data;
		if (data == null)
			return;

		// Получаем ширину области контента, в которой будем размещать svg
		var renderWidth = Math.round(getViewPortSize().width);

		// Сравниваем с шириной плана в масштабе 100% и расчитываем коэффициент scale, который будет у нас на старте
		var calculatedScale = renderWidth / data.Width;

		// С учетом расчитанного scale, получаем стартовую высоту svg
		var renderHeight = data.Height * calculatedScale;

		d3.select("svg").remove();

		// Поведение, которое отслеживает pan и zoom
		var zoom = d3.behavior.zoom().scaleExtent([calculatedScale, Infinity]).translate([0, 0]).scale(calculatedScale);

		// В этой функции мы проверяем ограничения на pan и применяем их
		var zooming = function () {
			var translate = d3.event.translate;
			var translateX = d3.event.translate[0];
			var translateY = d3.event.translate[1];

			// Проверяем значения на ограничения pan'а

			var xMin = renderWidth - data.Width * d3.event.scale;
			if (xMin > translateX)
				translate[0] = xMin;

			var yMin = renderHeight - data.Height * d3.event.scale;
			if (yMin > translateY)
				translate[1] = yMin;

			if (translateX > 0)
				translate[0] = 0;
			if (translateY > 0)
				translate[1] = 0;

			// применяем изменение pan вместе с ограничениями к behaviour, иначе, дойдя до ограничения,
			// D3 будет продолжать двигать картинку, но отрисовываться это не будет
			// вместо этого будет все выглядеть, как будто картинка "застряла", пока мы виртуально не прокрутим до конца
			zoom.translate(translate);

			// а теперь применяем pan и zoom к  DOM элементу, чтобы все отрисовалось
			g.attr("transform", "translate(" + translate + ")" + " scale(" + d3.event.scale + ")");
		};

		var svg = d3.select(element)
			.append("svg")
			.attr("width", renderWidth)
			.attr("height", renderHeight)
			.call(zoom.scaleExtent([calculatedScale, Infinity]).on("zoom", zooming))
			.append("g")
			.attr("transform", "translate(0,0)scale(" + calculatedScale + ")");

		svg.call(tip);

		var g = d3.select("g");
		// Отрисовываем каждую фигуру
		data.Elements.forEach(function (item, i, arr) { renderShape(item, i, g, tip, scope); });
	}

	// Модуль директивы
	angular.module("gkApp.directives").directive("d3Plan", [
		function () {
			return {
				restrict: "EA",
				link: function (scope, iElement) {
					// Инициализируем всплывающую подсказку
					var tip = createTip();


					// Перерисовка канвы при изменении размеров окна
					window.onresize = function () { return scope.$apply(); };
					scope.$watch(function () {
						return angular.element(window)[0].innerWidth;
					}, function () {
						return scope.render(scope.d3Data);
					}
					);

					// Перерисовка канвы при изменении входных данных
					scope.$on("planLoad", function (event, data) { return scope.render(); });

					// Функция отрисовки канвы
					scope.render = function () { renderSvg(tip, scope, iElement[0]); };

					scope.$on('updateDeviceState', function (event, stateData) {
						var uid = stateData.Id.replace(" ", "-");
						var imageElement = d3.select($("image[id=" + uid + "]")[0]);
						if (imageElement && imageElement.length > 0 && imageElement[0][0]) {
							imageElement.on('updateImage')({
								Image: stateData.Picture
							});
						}
						var groupElementUid = stateData.Id.replace(" ", "-") + "GroupElement";
						console.log("Triggering hint update on:", groupElementUid);
						var groupElement = d3.select($("rect[id=" + groupElementUid + "]")[0]);
						if (groupElement && groupElement.length > 0 && groupElement[0][0]) {
							groupElement.on('updateHint')({
								hint: stateData.Hint
							});
						}
					});
				}
			};
		}
	]);
}());