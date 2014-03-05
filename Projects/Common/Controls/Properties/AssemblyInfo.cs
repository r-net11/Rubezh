using System.Reflection;
using System.Windows.Markup;

// Управление общими сведениями о сборке осуществляется с помощью
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("Controls")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Controls")]

//Чтобы начать построение локализованных приложений, задайте
//<UICulture>CultureYouAreCodingWith</UICulture> в файле .csproj
//внутри <PropertyGroup>.  Например, если используется английский США
//в своих исходных файлах установите <UICulture> в en-US.  Затем отмените преобразование в комментарий
//атрибута NeutralResourceLanguage ниже.  Обновите "en-US" в
//строка внизу для обеспечения соответствия настройки UICulture в файле проекта.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//	  Основной номер версии
//	  Дополнительный номер версии
//	  Номер построения
//	  Редакция
//
// Можно задать все значения или принять номер построения и номер редакции по умолчанию,
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]


[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Controls.Converters")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Controls.TreeList")]

