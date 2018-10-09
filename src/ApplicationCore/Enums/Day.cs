using System.ComponentModel;

namespace ApplicationCore.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Day
    {
        [Description("Poniedziałek")]
        Monday = 0,

        [Description("Wtorek")]
        Tuesday = 1,

        [Description("Środa")]
        Wednesday = 2,

        [Description("Czwartek")]
        Thursday = 3,

        [Description("Piątek")]
        Friday = 4
    }
}
