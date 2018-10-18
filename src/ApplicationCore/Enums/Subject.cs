using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Subject
    {
        [Description("Edukacja wczesnoszkolna")]
        [Display(Name = "Edukacja wczesnoszkolna")]
        EarlySchoolEducation = 1,

        [Description("Wychowanie fizyczne")]
        [Display(Name = "Wychowanie fizyczne")]
        PhysicalEducation = 2,

        [Description("Religia")]
        [Display(Name = "Religia")]
        Religion = 3,

        [Description("Informatyka")]
        [Display(Name = "Informatyka")]
        ComputerScience = 4,

        [Description("Muzyka")]
        [Display(Name = "Muzyka")]
        Music = 5,

        [Description("Plastyka")]
        [Display(Name = "Plastyka")]
        ArtClasses = 6,

        [Description("J. angielski")]
        [Display(Name = "J. angielski")]
        English = 7,

        [Description("Zajęcia taneczne")]
        [Display(Name = "Zajęcia taneczne")]
        DancingClasses = 8,

        [Description("Matematyka")]
        [Display(Name = "Matematyka")]
        Maths = 9,

        [Description("J. polski")]
        [Display(Name = "J. polski")]
        Polish = 10,

        [Description("Przyroda")]
        [Display(Name = "Przyroda")]
        NaturalScience = 11,

        [Description("Fizyka")]
        [Display(Name = "Fizyka")]
        Physics = 12,

        [Description("Geografia")]
        [Display(Name = "Geografia")]
        Geography = 13,

        [Description("Biologia")]
        [Display(Name = "Biologia")]
        Biology = 14
    }
}
