using System.ComponentModel;
namespace FiresecAPI.Models
{
    public enum InstructionType
    {
        [DescriptionAttribute("Общая инструкция")]
        General,

        [DescriptionAttribute("Инструкция для зон и устройств")]
        Details
    }
}
