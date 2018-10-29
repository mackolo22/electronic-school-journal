using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Day
    {
        [Description("Niedziela")]
        Sunday = 0,

        [Description("Poniedziałek")]
        [Display(Name = "pon")]
        Monday = 1,

        [Description("Wtorek")]
        [Display(Name = "wt")]
        Tuesday = 2,

        [Description("Środa")]
        [Display(Name = "śr")]
        Wednesday = 3,

        [Description("Czwartek")]
        [Display(Name = "czw")]
        Thursday = 4,

        [Description("Piątek")]
        [Display(Name = "pt")]
        Friday = 5,

        [Description("Sobota")]
        Saturday = 6,
    }
}
