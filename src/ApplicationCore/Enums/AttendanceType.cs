using System.ComponentModel;

namespace ApplicationCore.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AttendanceType
    {
        [Description("Nie sprawdzono")]
        None = 0,

        [Description("Obecność")]
        Presence = 1,

        [Description("Nieobecność")]
        Absence = 2,

        [Description("Nieobecność usprawiedliwiona")]
        JustifiedAbsence = 3
    }
}
