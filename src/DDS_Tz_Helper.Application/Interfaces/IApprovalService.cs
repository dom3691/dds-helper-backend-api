using DDS_Tz_Helper.Application.DTOs.Approvals;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IApprovalService
{
    Task<IReadOnlyList<ApprovalRequestDto>> GetPendingApprovalsAsync(int userId, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> ApproveAsync(ApprovalActionRequest request, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> DeclineAsync(ApprovalActionRequest request, CancellationToken cancellationToken = default);
}
