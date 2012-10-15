using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Infrastructure.Common.Module
{
    public enum Module
    {

        [DescriptionAttribute( "Устройства, Зоны, Направления")]
        DevicesModule,

        [DescriptionAttribute("Графические планы")]
        PlansModule,

        [DescriptionAttribute("Библиотека устройств")]
        LibraryModule,

        [DescriptionAttribute("Права доступа")]
        SecurityModule,

        [DescriptionAttribute("Фильтры журнала событий")]
        FiltersModule,

        [DescriptionAttribute("Звуки")]
        SoundsModule,

        [DescriptionAttribute("Инструкции")]
        InstructionsModule,

        [DescriptionAttribute("OPC сервер")]
        OPCModule,

        [DescriptionAttribute("Диагностика")]
        DiagnosticsModule,

        [DescriptionAttribute("Видео")]
        VideoModule,
        
        [DescriptionAttribute("Групповой контроллер")]
        GKModule
    }
}
