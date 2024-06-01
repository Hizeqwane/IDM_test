namespace IDM.Models;

public class Employee : IdFullNameBase
{
    // Табельный номер
    public string PersonnelNumber { get; set; }
    public WorkStatus Status { get; set; }

    // Должность
    public long PositionId { get; set; }

    // Подразделение
    public long UnitId { get; set; }

    // Основная или по совместительству
    public bool? IsMainJob { get; set; }

    // Дата приема на работу
    public DateTime? StartDate { get; set; }
}