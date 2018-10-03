using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Enums
{
    public enum Subject
    {
        [Display(Name = "brak")]
        None = 0,

        [Display(Name = "Edukacja wczesnoszkolna")]
        EarlySchoolEducation = 1,

        [Display(Name = "Wychowanie fizyczne")]
        PhysicalEducation = 2,

        [Display(Name = "Religia")]
        Religion = 3,

        [Display(Name = "Informatyka")]
        ComputerScience = 4,

        [Display(Name = "Muzyka")]
        Music = 5,

        [Display(Name = "Plastyka")]
        ArtClasses = 6,

        [Display(Name = "J. angielski")]
        English = 7,

        [Display(Name = "Zajęcia taneczne")]
        DancingClasses = 8,

        [Display(Name = "Matematyka")]
        Maths = 9,

        [Display(Name = "J. polski")]
        Polish = 10,

        [Display(Name = "Przyroda")]
        NaturalScience = 11,

        [Display(Name = "Fizyka")]
        Physics = 12,

        [Display(Name = "Geografia")]
        Geography = 13,

        [Display(Name = "Biologia")]
        Biology = 14
    }
}
