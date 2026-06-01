using DDS_Tz_Helper.Application.DTOs.ATCGrade;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IATCGradeService
{
    Task<atc_datax> DoChangeATCGradeAsync(DoChangeATCGradeRequest request, CancellationToken cancellationToken = default);
}
