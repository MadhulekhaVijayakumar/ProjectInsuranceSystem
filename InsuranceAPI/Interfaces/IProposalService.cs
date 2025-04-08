using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceAPI.Interfaces
{
    public interface IProposalService
    {
        Task<CreateProposalResponse> SubmitProposalWithDetails(CreateProposalRequest request);
    }
}
