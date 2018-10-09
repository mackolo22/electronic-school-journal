using System.ComponentModel;

namespace ApplicationCore.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Subject
    {
        [Description("Edukacja wczesnoszkolna")]
        EarlySchoolEducation = 1,

        [Description("Wychowanie fizyczne")]
        PhysicalEducation = 2,

        [Description("Religia")]
        Religion = 3,

        [Description("Informatyka")]
        ComputerScience = 4,

        [Description("Muzyka")]
        Music = 5,

        [Description("Plastyka")]
        ArtClasses = 6,

        [Description("J. angielski")]
        English = 7,

        [Description("Zajęcia taneczne")]
        DancingClasses = 8,

        [Description("Matematyka")]
        Maths = 9,

        [Description("J. polski")]
        Polish = 10,

        [Description("Przyroda")]
        NaturalScience = 11,

        [Description("Fizyka")]
        Physics = 12,

        [Description("Geografia")]
        Geography = 13,

        [Description("Biologia")]
        Biology = 14
    }
}
