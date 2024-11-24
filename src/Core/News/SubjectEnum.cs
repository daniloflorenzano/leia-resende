using Core.Application.Attributes;

namespace Core.News;

public enum SubjectEnum
{
    [PortugueseDescription("Indefinido")]
    Undefined = 0,
    
    [PortugueseDescription("Política")]
    Politics = 1,
    
    [PortugueseDescription("Saúde")]
    Health = 2,
    
    [PortugueseDescription("Esportes")]
    Sports = 3,
    
    [PortugueseDescription("Economia")]
    Economy = 4,
    
    [PortugueseDescription("Mulher")]
    Woman = 5,
    
    [PortugueseDescription("Cultura")]
    Culture = 6,
    
    [PortugueseDescription("Meio Ambiente")]
    Environment = 7,
    
    [PortugueseDescription("Turismo")]
    Tourism = 8,
    
    [PortugueseDescription("Educação")]
    Education = 9
}