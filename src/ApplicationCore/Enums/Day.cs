using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Day
    {
        [Description("Poniedziałek")]
        [Display(Name = "pon")]
        Monday = 0,

        [Description("Wtorek")]
        [Display(Name = "wt")]
        Tuesday = 1,

        [Description("Środa")]
        [Display(Name = "śr")]
        Wednesday = 2,

        [Description("Czwartek")]
        [Display(Name = "czw")]
        Thursday = 3,

        [Description("Piątek")]
        [Display(Name = "pt")]
        Friday = 4
    }
}
